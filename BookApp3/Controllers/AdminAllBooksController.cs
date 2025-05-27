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
        public async Task<IActionResult> Edit(int id)
        {
            var book = await _context.Books
                .Include(b => b.Author)
                .Include(b => b.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(b => b.Book_Id == id);

            if (book == null)
            {
                return NotFound();
            }

            var userIdString = HttpContext.Session.GetString("UserId");

            if (!string.IsNullOrEmpty(userIdString) && int.TryParse(userIdString, out int userId))
            {
                var userReview = await _context.Reviews
                    .FirstOrDefaultAsync(r => r.Book_Id == id && r.User_Id == userId);
                ViewData["UserRating"] = userReview?.Rating;

                var bookStatus = await _context.User_Books
                    .FirstOrDefaultAsync(b => b.Book_Id == id && b.User_Id == userId);


                ViewData["BookStatus"] = bookStatus?.Status.ToString();

            }

            return View(book);
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
