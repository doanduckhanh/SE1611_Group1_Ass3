﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_A3.Models;

namespace SE1611_Group1_A3.Pages.Albums
{
    public class DeleteModel : PageModel
    {
        private readonly SE1611_Group1_A3.Models.MusicStoreContext _context;

        public DeleteModel(SE1611_Group1_A3.Models.MusicStoreContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Album Album { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (HttpContext.Session.GetInt32("UserId") == null)
            {
                Response.Redirect("/Shopping/Login");
            }
            else if (HttpContext.Session.GetInt32("UserId") != null && HttpContext.Session.GetInt32("Role") != 1)
            {
                Response.Redirect("/Shopping/401");
            }
            ViewData["Role"] = HttpContext.Session.GetInt32("Role");
            ViewData["Username"] = HttpContext.Session.GetString("Username");

            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }

            var album = await _context.Albums.FirstOrDefaultAsync(m => m.AlbumId == id);
            var artis = await _context.Artists.ToListAsync();
            var genre = await _context.Genres.ToListAsync();
            if (album == null)
            {
                return NotFound();
            }
            else 
            {
                Album = album;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }
            var album = await _context.Albums.FindAsync(id);

            if (album != null)
            {
                Album = album;
                
                var carts = _context.Carts.Where(x => x.AlbumId == Album.AlbumId);
                _context.Carts.RemoveRange(carts);
                var orderDetails = _context.OrderDetails.Where(x=>x.AlbumId== Album.AlbumId);
                _context.OrderDetails.RemoveRange(orderDetails);
                _context.Albums.Remove(Album);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
