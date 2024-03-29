﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using SE1611_Group1_A3.FileUploadService;
using SE1611_Group1_A3.Models;

namespace SE1611_Group1_A3.Pages.Albums
{
    public class CreateModel : PageModel
    {
        private readonly SE1611_Group1_A3.Models.MusicStoreContext _context;
        private readonly ILogger<IndexModel> _logger;
        public readonly IFileUploadService fileUploadService;
        public string filePath;
        public CreateModel(SE1611_Group1_A3.Models.MusicStoreContext context,ILogger<IndexModel> logger,IFileUploadService fileUploadService)
        {
            _context = context;
            _logger = logger;
            this.fileUploadService= fileUploadService;
        }

        public IActionResult OnGet()
        {
            if (HttpContext.Session.GetInt32("UserId") == null )
            {
                Response.Redirect("/Shopping/Login");
            }else if (HttpContext.Session.GetInt32("UserId") != null && HttpContext.Session.GetInt32("Role") != 1)
            {
                Response.Redirect("/Shopping/401");
            }
            ViewData["ArtistId"] = new SelectList(_context.Artists, "ArtistId", "Name");
            ViewData["GenreId"] = new SelectList(_context.Genres, "GenreId", "Name");
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");

            return Page();
        }

        [BindProperty]
        public Album Album { get; set; }

        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync(Album album,IFormFile file)
        {
            if (file == null)
            {
                return RedirectToPage("./Create");
            }

            Album = album;
            filePath= await fileUploadService.UploadFileAsync(file);
            Album.AlbumUrl = filePath.Substring(filePath.IndexOf(@"\images")).Replace(@"\","/");
            _context.Albums.Add(Album);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
