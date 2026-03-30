using GoodHamburger.Api.Models;
using System.Net.Http.Json;

namespace GoodHamburger.Client.Services
{
    public class OrderState
    {
        private readonly HttpClient _http;

        public OrderState(HttpClient http)
        {
            _http = http;
        }

        
        public Order? CurrentOrder { get; private set; }
        public string? ErrorMessage { get; private set; }

        
        public event Action? OnChange;

        private void NotifyStateChanged() => OnChange?.Invoke();

        public async Task AddItem(MenuItem item)
        {
            ErrorMessage = null;
            try
            {
                
                if (item.Type == TypeItem.Hamburger &&
                    CurrentOrder?.OrderItems.Any(oi => oi.Type == TypeItem.Hamburger) == true)
                {
                    ErrorMessage = "You can only have one burger per order. Try adding extras!";
                    NotifyStateChanged();
                    return;
                }

                
                if (CurrentOrder == null)
                {
                    var responseOrder = await _http.PostAsync("api/orders", null);
                    if (responseOrder.IsSuccessStatusCode)
                    {
                        CurrentOrder = await responseOrder.Content.ReadFromJsonAsync<Order>();
                    }
                }

                
                if (CurrentOrder != null)
                {
                    var request = new { MenuItemId = item.MenuItemId };
                    var responseItem = await _http.PostAsJsonAsync($"api/orders/{CurrentOrder.OrderId}/items", request);

                    if (responseItem.IsSuccessStatusCode)
                    {
                        
                        CurrentOrder = await responseItem.Content.ReadFromJsonAsync<Order>();
                    }
                    else
                    {
                        ErrorMessage = await responseItem.Content.ReadAsStringAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = "Connection failure while adding item.";
                Console.WriteLine($"Erro: {ex.Message}");
            }

            NotifyStateChanged();
        }

        public async Task RemoveItem(OrderItem item)
        {
            if (CurrentOrder == null) return;

            try
            {
                
                var updatedIds = CurrentOrder.OrderItems
                    .Where(oi => oi.MenuItemId != item.MenuItemId)
                    .Select(oi => oi.MenuItemId)
                    .ToList();

                var request = new UpdateOrderRequest(updatedIds);
                var response = await _http.PutAsJsonAsync($"api/orders/{CurrentOrder.OrderId}", request);

                if (response.IsSuccessStatusCode)
                {
                    CurrentOrder = await response.Content.ReadFromJsonAsync<Order>();
                }
                else
                {
                    ErrorMessage = "Could not remove the item.";
                }
            }
            catch (Exception)
            {
                ErrorMessage = "Error communicating with the server.";
            }

            NotifyStateChanged();
        }

        public async Task CancelOrder()
        {
            if (CurrentOrder == null) return;

            try
            {
                var response = await _http.DeleteAsync($"api/orders/{CurrentOrder.OrderId}");
                if (response.IsSuccessStatusCode)
                {
                    CurrentOrder = null;
                    ErrorMessage = null;
                }
            }
            catch (Exception)
            {
                ErrorMessage = "Failed to cancel order.";
            }

            NotifyStateChanged();
        }
        public void ClearError()
        {
            ErrorMessage = null;
            NotifyStateChanged();
        }


        public bool IsItemInOrder(Guid id)
            => CurrentOrder?.OrderItems.Any(oi => oi.MenuItemId == id) ?? false;

        
        public record UpdateOrderRequest(List<Guid> MenuItemIds);
    }
}