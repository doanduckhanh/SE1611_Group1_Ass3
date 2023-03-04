using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_A3.Models;
using SE1611_Group1_A3.Services;

namespace SE1611_Group1_A3.Shopping
{
    public class IndexModel : PageModel
    {
        public static MusicStoreContext context = new MusicStoreContext();

        /*ShoppingCart shopping = ShoppingCart.GetCart();*/

        PaginatedList<Album> albums = new PaginatedList<Album>(context.Albums.ToList(), context.Albums.Count(), 1, 3);

        [BindProperty(SupportsGet = true)]
        public string? SearchString { get; set; }

        public SelectList? Genres { get; set; }

        [BindProperty(SupportsGet = true)]
        public int GenreId { get; set; } = 0;

        [BindProperty(SupportsGet = true)]
        public int IndexPaging { get; set; } = 1;

        [BindProperty(Name = "id", SupportsGet = true)]
        public int Id { get; set; }

        public int TotalPage { get; set; }
        public void OnGet(int genreId, string searchString, int indexPaging)
        {
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");
            if (genreId != 0)
            {
                var listAlbums = String.IsNullOrEmpty(searchString) ? context.Albums.Where(x => x.GenreId == genreId).ToList() : context.Albums.Where(x => x.GenreId == genreId && x.Title.Contains(searchString)).ToList();
                albums = new PaginatedList<Album>(listAlbums, listAlbums.Count, 1, 3);
            }
            else
            {
                var listAlbums = String.IsNullOrEmpty(searchString) ? context.Albums.ToList() : context.Albums.Where(x => x.Title.Contains(searchString)).ToList();
                albums = new PaginatedList<Album>(listAlbums, listAlbums.Count, 1, 3);
            }
            TotalPage = albums.TotalPages;
            ViewData["genreList"] = context.Genres.Distinct().ToList();
            ViewData["Product"] = PaginatedList<Album>.Create(albums.AsQueryable<Album>(), indexPaging, 3);
        }
        public IActionResult OnPostAddToCart(int id)
        {
            var cartId = GetCartId();
            AddToCart(id,cartId);
            HttpContext.Session.SetInt32("Count", new CartModel(context).GetCount());
            return RedirectToPage("/Shopping/Cart");
        }

 
        public static string GetCartId()
        {
            if (string.IsNullOrEmpty(Settings.CartId))
            {
                if (!string.IsNullOrEmpty(Settings.UserName))
                {
                    Settings.CartId = Settings.UserName;
                }

                else
                {
                    Guid tempCartId = Guid.NewGuid();
                    Settings.CartId = tempCartId.ToString();
                }
            }
            return Settings.CartId;
        }

        public void AddToCart(int albumId, String ShoppingCartId)
        {
            // Get the matching cart and album instances
            var cartItem = context.Carts.SingleOrDefault(
                c => c.CartId == ShoppingCartId
                && c.AlbumId == albumId);

            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new Cart
                {
                    AlbumId = albumId,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };

                context.Carts.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, then add one to the quantity
                cartItem.Count++;
            }
            // Save changes
            context.SaveChanges();
        }
    }
}
