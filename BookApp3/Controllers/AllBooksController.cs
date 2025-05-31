using System.Diagnostics;
using BookApp3.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace BookApp3.Controllers
{
    
    public class AllBooksController : Controller
    {
       
        private readonly AppDbContext _context;
        public AllBooksController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.Include(b => b.Author).ToListAsync();
            return View(books);
        }
        public async Task<IActionResult> Search(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return RedirectToAction(nameof(Index));
            }

            var searchTermLower = searchTerm.ToLower();

            var books = await _context.Books
                .Include(b => b.Author)
                .Where(b =>
                    (b.Title != null && b.Title.ToLower().Contains(searchTermLower)) ||
                    (b.Description != null && b.Description.ToLower().Contains(searchTermLower)) ||
                    (b.Author != null && b.Author.Name != null && b.Author.Name.ToLower().Contains(searchTermLower)))
                .OrderBy(b => b.Title) 
                .ToListAsync();

            ViewBag.SearchTerm = searchTerm;
            return View(nameof(Index), books);
        }
    }
}
