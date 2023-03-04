using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SE1611_Group1_A3.Services;

namespace SE1611_Group1_A3.Shopping
{
    public class LogoutModel : PageModel
    {
        public IActionResult OnGetLogout()
        {
            HttpContext.Session.Clear();
            Settings.CartId = "";
            Settings.UserName = "";
            return RedirectToPage("/Shopping/Index");
        }
    }
}
