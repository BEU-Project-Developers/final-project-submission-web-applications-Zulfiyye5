using BookApp3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookApp3.Controllers
{
    public class AdminAllBooksController : Controller
    {
        private readonly AppDbContext _context;
        public AdminAllBooksController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var books = await _context.Books.Include(b => b.Author).ToListAsync();
            return View(books);
        }
        public IActionResult Delete(int id)
        {
          
            var book = _context.Books.FirstOrDefault(b => b.Book_Id == id);

            if (book == null)
                return NotFound();

          
            _context.Books.Remove(book);
            _context.SaveChanges();


            return RedirectToAction("Index");
        }



    }
}
