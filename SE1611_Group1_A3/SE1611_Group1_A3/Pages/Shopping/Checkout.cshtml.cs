using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SE1611_Group1_A3.Models;
using SE1611_Group1_A3.Services;
using System.Text.Json;

namespace SE1611_Group1_A3.Pages.Shopping
{
    public class CheckoutModel : PageModel
    {
        MusicStoreContext storeDB = new MusicStoreContext();
        public static MusicStoreContext context = new MusicStoreContext();
        [BindProperty]
        public string? FirstName { get; set; }
        [BindProperty]
        public string? LastName { get; set; }
        [BindProperty]
        public string? Address { get; set; }
        [BindProperty]
        public string? City { get; set; }
        [BindProperty]
        public string? State { get; set; }
        [BindProperty]
        public string? Country { get; set; }
        [BindProperty]
        public string? Phone { get; set; }
        [BindProperty]
        public string? Email { get; set; }
        string total { get; set; }
        public void OnGet()
        {
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            int id = (int)HttpContext.Session.GetInt32("UserId");
            User user = context.Users.FirstOrDefault(x => x.Id == id);
            ViewData["user"] = user;
            ViewData["Total"] = HttpContext.Session.GetString("Total");
        }
        public async Task<IActionResult> OnPostSave()
        {
            int id = (int)HttpContext.Session.GetInt32("UserId");
            User user = context.Users.FirstOrDefault(x => x.Id == id);
            List<OrderDetailDTO> orderDetailDTOs = JsonSerializer.Deserialize<List<OrderDetailDTO>>(HttpContext.Session.GetString("OrderDetailList"));
            Order order = new Order();
            order.OrderDate = DateTime.Now;
            order.PromoCode = null;
            order.UserName = HttpContext.Session.GetString("Username");
            order.FirstName = user.FirstName;
            order.LastName = user.LastName;
            order.Address = user.Address;
            order.City = user.City;
            order.State = user.State;
            order.Country = user.Country;
            order.Phone = user.Phone;
            order.Email = user.Email;
            total = HttpContext.Session.GetString("Total");
            order.Total = Decimal.Parse(total);
            CreateOrder(order, orderDetailDTOs);
            return RedirectToPage("/Shopping/Index");
        }
        public int CreateOrder(Order order, List<OrderDetailDTO> orderDetailDTOs)
        {
            decimal orderTotal = 0;
            // Set the order's total to the orderTotal count
            order.Total = orderTotal;
            // Save the order
            try
            {
                storeDB.Orders.Add(order);
                storeDB.SaveChanges();
            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
                return -1;
            }
            int orderID = storeDB.Orders.Select(o => o.OrderId).Max();
            // Iterate over the items in the cart, adding the order details for each
            foreach (OrderDetailDTO item in orderDetailDTOs)
            {
                var orderDetail = new OrderDetail
                {
                    AlbumId = item.AlbumId,
                    OrderId = orderID,
                    UnitPrice = item.UnitPrice,
                    Quantity = item.Quantity
                };
                try
                {
                    storeDB.OrderDetails.Add(orderDetail);
                    storeDB.SaveChanges();
                }
                catch (Exception ex)
                {
                    //MessageBox.Show(ex.Message);
                    return -1;
                }
            }
            // Empty the shopping cart
            EmptyCart();
            // Return the OrderId as the confirmation number
            return orderID;
        }

        public void EmptyCart()
        {
            var cartItems = storeDB.Carts
                .Where(cart => cart.CartId == Settings.CartId);

            foreach (var cartItem in cartItems)
            {
                storeDB.Carts.Remove(cartItem);
            }
            // Save changes
            storeDB.SaveChanges();
        }
    }
}
