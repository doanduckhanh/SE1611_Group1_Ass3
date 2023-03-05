using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_A3.Models;

namespace SE1611_Group1_A3.Pages.Genres
{
    public class DetailsModel : PageModel
    {
        private readonly SE1611_Group1_A3.Models.MusicStoreContext _context;

        public DetailsModel(SE1611_Group1_A3.Models.MusicStoreContext context)
        {
            _context = context;
        }

      public Genre Genre { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Genres == null)
            {
                return NotFound();
            }

            var genre = await _context.Genres.FirstOrDefaultAsync(m => m.GenreId == id);
            if (genre == null)
            {
                return NotFound();
            }
            else 
            {
                Genre = genre;
            }
            return Page();
        }
    }
}
