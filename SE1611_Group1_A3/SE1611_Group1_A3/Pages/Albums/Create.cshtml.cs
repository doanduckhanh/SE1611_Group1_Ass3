using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SE1611_Group1_A3.FileUploadService;
using SE1611_Group1_A3.Models;

namespace SE1611_Group1_A3.Pages.checking
{
    public class CreateModel : PageModel
    {
        private readonly SE1611_Group1_A3.Models.MusicStoreContext _context;
        private readonly ILogger<IndexModel> _logger;
        private readonly IFileUploadService fileUploadService;
        public string FilePath;
        public CreateModel(SE1611_Group1_A3.Models.MusicStoreContext context, ILogger<IndexModel> logger, IFileUploadService fileUploadService)
        {
            _context = context;
            _logger = logger;
            this.fileUploadService = fileUploadService;
        }
        public IActionResult OnGet()
        {
        ViewData["ArtistId"] = new SelectList(_context.Artists, "ArtistId", "ArtistId");
        ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "GenreId");
            return Page();
        }

        [BindProperty]
        public Album Album { get; set; }
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(Album album, IFormFile file)
        {
            if (file != null)
            {
                FilePath = await fileUploadService.UploadFileAsyn(file);
            }

             

            Album.AlbumUrl = "/"+FilePath.Substring(FilePath.IndexOf(@"\images") + 1).Replace(@"\","/");

            await _context.Albums.AddAsync(Album);
            await _context.SaveChangesAsync();
            return RedirectToPage("Index");
        }

    }
}
