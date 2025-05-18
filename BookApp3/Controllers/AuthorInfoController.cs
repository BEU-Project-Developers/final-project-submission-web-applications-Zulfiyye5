
using BookApp3.Data;
using BookApp3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookApp2.Controllers
{
    public class AuthorInfoController : Controller
    {
        private readonly AppDbContext _context;

        public AuthorInfoController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(b => b.Author_id == id);

            if (author == null) return NotFound();

            author.Books ??= new List<Book>();

            return View(author);
        }
    }
}
