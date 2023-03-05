using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_A3.Models;

namespace SE1611_Group1_A3.Pages.Albums
{
    public class EditModel : PageModel
    {
        private readonly SE1611_Group1_A3.Models.MusicStoreContext _context;

        public EditModel(SE1611_Group1_A3.Models.MusicStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Album Album { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }

            var album =  await _context.Albums.FirstOrDefaultAsync(m => m.AlbumId == id);
            if (album == null)
            {
                return NotFound();
            }
            Album = album;
           ViewData["ArtistId"] = new SelectList(_context.Artists, "ArtistId", "ArtistId");
           ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "GenreId");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(Album album)
        {

            _context.Attach(album).State = EntityState.Modified;

            try
            {
                _context.Albums.Update(album);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AlbumExists(album.AlbumId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool AlbumExists(int id)
        {
          return _context.Albums.Any(e => e.AlbumId == id);
        }
    }
}
