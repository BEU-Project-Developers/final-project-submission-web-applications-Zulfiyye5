using BookApp3.Controllers;
using BookApp3.Data;
using BookApp3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookApp2.Controllers
{
    public class BookDetailsController : Controller
    {

        private readonly AppDbContext _context;
        public BookDetailsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index(int id)
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
            var userId = HttpContext.Session.GetString("UserId");
            if (userId!=null)
            {
                var userReview = await _context.Reviews
          .FirstOrDefaultAsync(r => r.Book_Id == id && r.User_Id == int.Parse(userId));


                ViewData["UserRating"] = userReview?.Rating;

            }

            return View(book);

        }


        [HttpPost]
        public IActionResult RateBook(int bookId, int rating)
        {
            return RedirectToAction("Index", "Home");
        }


    }
}
