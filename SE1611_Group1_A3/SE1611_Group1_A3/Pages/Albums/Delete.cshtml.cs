﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SE1611_Group1_A3.Models;

namespace SE1611_Group1_A3.Pages.checking
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
            if (id == null || _context.Albums == null)
            {
                return NotFound();
            }

            var album = await _context.Albums.FirstOrDefaultAsync(m => m.AlbumId == id);

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
                var c = _context.Carts.Where(x => x.AlbumId == album.AlbumId);
                var o = _context.OrderDetails.Where(x => x.AlbumId == album.AlbumId);
                _context.Carts.RemoveRange(c);
                _context.OrderDetails.RemoveRange(o);
                var a = _context.Albums.FirstOrDefault(x => x.AlbumId == album.AlbumId);
                _context.Albums.Remove(a);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
