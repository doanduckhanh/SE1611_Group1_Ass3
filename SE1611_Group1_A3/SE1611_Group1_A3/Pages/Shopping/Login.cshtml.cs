using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SE1611_Group1_A3.Models;
using System.ComponentModel.DataAnnotations;

namespace SE1611_Group1_A3.Shopping
{
    public class LoginModel : PageModel
    {
        MusicStoreContext _context = new MusicStoreContext();
        [BindProperty]
        [Required]
        public string Username { get; set; }

        [BindProperty]
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string Role { get; set; }

        public string Msg { get; set; }


        public void OnGet()
        {

        }

        public IActionResult OnPost()
        {

            var user = _context.Users.FirstOrDefault(u => u.UserName == Username && u.Password == Password);

            if (user != null)
            {
                if (Username.Equals(user.UserName, StringComparison.Ordinal) && Password.Equals(user.Password, StringComparison.Ordinal))
                {
                    HttpContext.Session.SetInt32("Role", user.Role);
                    HttpContext.Session.SetString("Username", user.UserName);
                    HttpContext.Session.SetInt32("UserId", user.Id);
                    ViewData["Role"] = user.Role;
                    return RedirectToPage("/Shopping/Index");
                }
                else
                {
                    Msg = "Invalid email or password.";
                    return Page();
                }
            }
            else
            {
                Msg = "Invalid email or password.";
                return Page();
            }
        }
    }
}

