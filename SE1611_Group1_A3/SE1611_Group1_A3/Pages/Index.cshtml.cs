using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_A3.Models;
using SE1611_Group1_A3.Services;

namespace SE1611_Group1_A3.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SE1611_Group1_A3.Models.MusicStoreContext _context;

        public IndexModel(ILogger<IndexModel> logger, Models.MusicStoreContext context)
        {
            _logger = logger;
            _context = context;
        }

        public void OnGet()
        {
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");

            

            
            
        }


    }
}