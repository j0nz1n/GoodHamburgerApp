using GoodHamburger.Api.Data;
using GoodHamburger.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
       
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddCors(options => {
    options.AddDefaultPolicy(policy => {
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.CustomSchemaIds(type => type.FullName);
});


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=order.sqlite"));

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthorization();
app.MapControllers();


app.MapPost("/api/orders", async (AppDbContext db) =>
{
    var order = new Order();
    db.Orders.Add(order);
    await db.SaveChangesAsync();
    return Results.Created($"/api/orders/{order.OrderId}", order);
});


app.MapPost("/api/orders/{orderId:guid}/items", async (Guid orderId, AddItemRequest request, AppDbContext db) =>
{
    
    var order = await db.Orders
        .Include(o => o.OrderItems)
        .AsNoTracking()
        .FirstOrDefaultAsync(o => o.OrderId == orderId);

    if (order == null) return Results.NotFound("Pedido não encontrado.");

    var menuItem = await db.MenuItems.AsNoTracking()
        .FirstOrDefaultAsync(m => m.MenuItemId == request.MenuItemId);

    if (menuItem == null) return Results.NotFound("Item do menu não encontrado.");

    try
    {
        
        var newItem = new OrderItem
        {
            Id = Guid.NewGuid(), 
            OrderId = order.OrderId,
            MenuItemId = menuItem.MenuItemId,
            Name = menuItem.Name,
            Price = menuItem.Price,
            Type = menuItem.Type
        };

        
        order.OrderItems.Add(newItem);
        order.UpdateTotals();

        
        db.OrderItems.Add(newItem);
        db.Orders.Update(order);

        
        await db.SaveChangesAsync();

        return Results.Ok(order);
    }
    catch (Exception ex)
    {
        return Results.BadRequest($"Erro ao salvar: {ex.Message}");
    }
});

app.MapGet("/api/menu", async (AppDbContext db) =>
{
    var items = await db.MenuItems.ToListAsync();
    return Results.Ok(items);
});

app.MapGet("/api/orders/{orderId:guid}", async (Guid orderId, AppDbContext db) =>
{
    var order = await db.Orders
        .Include(o => o.OrderItems)
        .FirstOrDefaultAsync(o => o.OrderId == orderId);

    return order is not null
        ? Results.Ok(order)
        : Results.NotFound($"Order with ID {orderId} not found.");
});


app.MapGet("/api/orders", async (AppDbContext db) =>
{
    var orders = await db.Orders
        .Include(o => o.OrderItems)
        .ToListAsync();

    return Results.Ok(orders);
});

app.MapPut("/api/orders/{orderId:guid}", async (Guid orderId, UpdateOrderRequest request, AppDbContext db) =>
{
    
    var order = await db.Orders
        .Include(o => o.OrderItems)
        .FirstOrDefaultAsync(o => o.OrderId == orderId);

    if (order is null) return Results.NotFound("Pedido não encontrado.");

    
    order.OrderItems.Clear();


    foreach (var itemId in request.MenuItemIds)
    {
        var menuItem = await db.MenuItems.FindAsync(itemId);
        if (menuItem is null) return Results.BadRequest($"Item do menu {itemId} não existe.");

        
        var orderItem = new OrderItem
        {
            OrderId = order.OrderId,
            MenuItemId = menuItem.MenuItemId,
            Name = menuItem.Name,
            Price = menuItem.Price,
            Type = menuItem.Type
        };

        order.OrderItems.Add(orderItem);
    }

    
    order.UpdateTotals();
    await db.SaveChangesAsync();
    return Results.Ok(order);
});

app.MapDelete("/api/orders/{orderId:guid}", async (Guid orderId, AppDbContext db) =>
{
    var order = await db.Orders.FindAsync(orderId);
    if (order == null) return Results.NotFound($"Order with ID {orderId} not found.");

    db.Orders.Remove(order);
    await db.SaveChangesAsync();
    return Results.NoContent();
});

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.Run();


public record AddItemRequest(Guid MenuItemId);
public record UpdateOrderRequest(List<Guid> MenuItemIds);