using BookApp3.Data;
using BookApp3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookApp3.Controllers
{
    public class AddReviewController : Controller
    {
        private readonly AppDbContext _context;
        public AddReviewController(AppDbContext context)
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
            if (userId != null)
            {
                var userReview = await _context.Reviews
          .FirstOrDefaultAsync(r => r.Book_Id == id && r.User_Id == int.Parse(userId));


                ViewData["UserRating"] = userReview?.Rating;

            }

            return View(book);

        }

        [HttpPost]
        [HttpPost]
        public IActionResult AddReview(int BookId, string Content)
        {
            var userId = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrWhiteSpace(Content) || string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "Review cannot be empty or user not logged in.");
                return RedirectToAction("Index", "AddReview", new { id = BookId });
            }

            var review = new Review
            {
                Book_Id = BookId,
                User_Id = int.Parse(userId),
                Review_Text = Content,
                Created_At = DateTime.Now
            };

            _context.Reviews.Add(review);
            _context.SaveChanges();

            return RedirectToAction("Index", "BookDetails", new { id = BookId });
        }

    }
}
