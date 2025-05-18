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

    
    }
}
