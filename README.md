# Good Hamburger Developer Test
Good Hamburger is a full-stack web application built with **Blazor WebAssembly** and **.NET API**. The web app features a side menu, real-time cart management and an discount engine to calculate the final order value for the user.

## Tech Stack
* Frontend: Blazor WebAssembly (.NET 8)
* Backend: ASP.NET Core Minimal APIs
* Database: SQLite with Entity Framework Core
* Shared Library: Models and business logic shared between Client and Server projects.
* UX/UI: Bootstrap 5

## The Structure

### GoodHamburger.Api:
The API layer handles data persistence using **Entity Framework Core** and **SQLite**.

### GoodHamburger.Client:
The Frontend layer was made using **Blazor WebAssembly** and **Bootstrap 5**.

### GoodHamburger.Shared:
This is a Class Library created to optimize the structure of the app and to prevent the need of the "Models" folder in both Api and Client projects.

## Technical Highlights

### State Container Pattern
I implemented the State Container pattern using the OrderState service. This ensures the shopping cart remains persistent as the user navigates between pages.

### Shared Business Logic
The core logic for total calculations and discounts (up to 20% off for combos) is encapsulated within the Order class in the Shared project. This "Single Source of Truth" ensures that both the Frontend and the Backend use identical rules, preventing price discrepancies.

### Optimistic Concurrency Handling
I addressed Optimistic Concurrency challenges by using AsNoTracking() in the API and ensuring state synchronization. This prevents the "0 rows affected" error by making sure the client always works with the most recent version of the data returned by the server.

## Business Rules

### Item Limits
Restriction to only one "hamburger", one "fries" and one "Soft Drink" per order. The UI was designed to make it easier to understand.

### Combo Discounts
* **20% Discount:** Sandwich + Fries + Soft Drink.

* **15% Discount:** Sandwich + Soft Drink.

* **10% Discount:** Sandwich + Fries.

```
public void UpdateTotals()
{
    decimal subtotal = OrderItems.Sum(oi => oi.Price);
    this.Price = subtotal;

    
    bool hasHamburger = OrderItems.Any(oi => oi.Type == TypeItem.Hamburger);
    bool hasFries = OrderItems.Any(oi => oi.Name.Contains("Fries", StringComparison.OrdinalIgnoreCase));
    bool hasSoftDrink = OrderItems.Any(oi => oi.Name.Contains("Soft Drink", StringComparison.OrdinalIgnoreCase));

    decimal discount = 0;
    if (hasHamburger && hasFries && hasSoftDrink) discount = 0.20m;
    else if (hasHamburger && hasSoftDrink) discount = 0.15m;
    else if (hasHamburger && hasFries) discount = 0.10m;

    this.TotalPrice = subtotal * (1 - discount);
}
```
## How To Run

1. Clone the repository.
2. Ensure you have the .NET 8 SDK installed.
3. Open the solution in Visual Studio.
4. Right-click the Solution and select "Configure Startup Projects...".
5. Set both GoodHamburger.Api and GoodHamburger.Client to Start.
5. Run the application (F5).