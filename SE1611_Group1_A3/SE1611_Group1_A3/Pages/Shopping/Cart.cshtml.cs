using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_A3.Models;
using SE1611_Group1_A3.Services;
using System.Text.Json;

namespace SE1611_Group1_A3.Shopping
{
    public class CartModel : PageModel
    {
        private readonly MusicStoreContext _context;
        public decimal total { get; set; }

        public CartModel(MusicStoreContext context)
        {
            _context = context;
        }
        public IList<Cart> Cart { get; set; } = default!;
        public async Task OnGetAsync()
        {
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");

            if (_context.Carts != null)
            {
                Cart = await _context.Carts
                .Where(x => x.CartId.Equals(Settings.CartId))
                .Include(a => a.Album).ToListAsync();
            }
            total = GetTotal();
            HttpContext.Session.SetInt32("Count", GetCount());          
        }
        public decimal GetTotal()
        {
            // Multiply album price by count of that album to get
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total
            decimal? total = (from cartItems in _context.Carts
                              where cartItems.CartId == Settings.CartId
                              select cartItems.Count * cartItems.Album.Price).Sum();
            return total ?? 0;
        }
        public async Task<IActionResult> OnPostRemoveFromCart(int id)
        {
            var cartItem = _context.Carts.SingleOrDefault(
                c => c.CartId == Settings.CartId
                && c.RecordId == id);

            int itemCount = 0;
            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    _context.Carts.Remove(cartItem);
                }
                // Save changes
                await _context.SaveChangesAsync();

            }

            HttpContext.Session.SetInt32("Count", GetCount());

            return RedirectToPage("/Shopping/Cart");
        }
        public async Task<IActionResult> OnPostCheckOut()
        {
            total = GetTotal();
            HttpContext.Session.SetString("Total", total.ToString());
            List<Cart> carts = _context.Carts.Where(c => c.CartId == Settings.CartId).ToList();
            List<OrderDetailDTO> orderDetailDTOs = new List<OrderDetailDTO>();
            foreach(Cart cart in carts)
            {
                OrderDetailDTO orderDetailDTO = new OrderDetailDTO();
                orderDetailDTO.AlbumId = cart.AlbumId;
                orderDetailDTO.Quantity = cart.Count;
                orderDetailDTO.UnitPrice = Decimal.Parse("8.99");
                orderDetailDTOs.Add(orderDetailDTO);
            }
            HttpContext.Session.SetString("OrderDetailList", JsonSerializer.Serialize(orderDetailDTOs));
            return RedirectToPage("/Shopping/Checkout");
        }
        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in _context.Carts
                          where cartItems.CartId == Settings.CartId
                          select (int?)cartItems.Count).Count();
            // Return 0 if all entries are null
            return count ?? 0;

        }
    }
}
