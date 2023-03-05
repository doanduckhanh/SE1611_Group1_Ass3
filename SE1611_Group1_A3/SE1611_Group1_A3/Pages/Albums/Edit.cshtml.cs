using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_A3.FileUploadService;
using SE1611_Group1_A3.Models;

namespace SE1611_Group1_A3.Pages.Albums
{
    public class EditModel : PageModel
    {
        private readonly SE1611_Group1_A3.Models.MusicStoreContext _context;
        private readonly ILogger<IndexModel> _logger;
        public readonly IFileUploadService fileUploadService;
        public string filePath;
        public EditModel(SE1611_Group1_A3.Models.MusicStoreContext context, ILogger<IndexModel> logger, IFileUploadService fileUploadService)
        {
            _context = context;
            _logger = logger;
            this.fileUploadService = fileUploadService;
        }

        [BindProperty]
        public Album Album { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                Response.Redirect("/Shopping/Login");
            }
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");

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
           ViewData["ArtistId"] = new SelectList(_context.Artists, "ArtistId", "Name");
           ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(Album album, IFormFile file)
        {
            _context.Attach(album).State = EntityState.Modified;
            if (file != null)
            {
                filePath = await fileUploadService.UploadFileAsync(file);
                album.AlbumUrl = filePath.Substring(filePath.IndexOf(@"\images")).Replace(@"\", "/");
            }
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
