using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_A3.Models;

namespace SE1611_Group1_A3.Pages
{
    public class CartModel : PageModel
    {
        private readonly SE1611_Group1_A3.Models.MusicStoreContext _context;
        public decimal total { get; set; }

        public CartModel(MusicStoreContext context)
        {
            _context = context;
        }
        public IList<Cart> Cart { get; set; } = default!;
        public async Task OnGetAsync()
        {
            if (_context.Carts != null)
            {
                Cart = await _context.Carts
                .Where(x => x.CartId.Equals("user"))
                .Include(a => a.Album).ToListAsync();
            }
            this.total = GetTotal();
        }
        public decimal GetTotal()
        {
            // Multiply album price by count of that album to get
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total
            decimal? total = (from cartItems in _context.Carts
                              where cartItems.CartId == "user"
                              select (int?)cartItems.Count * cartItems.Album.Price).Sum();
            return total ?? 0;
        }
        public async Task<IActionResult> OnPostRemoveFromCart(int id)
        {           
            var cartItem = _context.Carts.SingleOrDefault(
                c => c.CartId ==  "user"  //Settings.Default["CartId"].ToString()
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



            return RedirectToPage("/Cart");
        }
    }
}
