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



        [HttpPost]
        public IActionResult RateBook(int bookId, int rating)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("SignIn", "Account");

            var existingRating = _context.Reviews
                .FirstOrDefault(r => r.Book_Id == bookId && r.User_Id == userId);

            if (existingRating != null)
            {
                existingRating.Rating = rating;
            }
            else
            {
                _context.Reviews.Add(new Review
                {
                    User_Id = userId.Value,
                    Book_Id = bookId,
                    Rating = rating,
                    Created_At = DateTime.Now
                });
            }

            _context.SaveChanges();
            return RedirectToAction("Index", new { id = bookId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ClearRating(int bookId)
        {
            var userId = GetUserId();
            if (userId == null) return RedirectToAction("SignIn", "Account");

            var ratingToRemove = _context.Reviews
                .FirstOrDefault(r => r.Book_Id == bookId && r.User_Id == userId);
            ratingToRemove.Rating=0;

            if (ratingToRemove != null)
            {
                _context.Reviews.Update(ratingToRemove);
                _context.SaveChanges();
            }

            return RedirectToAction("Index", new { id = bookId });
        }

        private int? GetUserId()
        {
            var userIdString = HttpContext.Session.GetString("UserId");
            return int.TryParse(userIdString, out var userId) ? userId : null;
        }

        [HttpPost]
        public IActionResult UpdateShelf(int bookId, string shelfType)
        {
                var userId = Int32.Parse(   HttpContext.Session.GetString("UserId"));
          
            BookShelfStatus status;
            switch (shelfType.ToLower())
            {
                case "want-to-read":
                    status = BookShelfStatus.WantToRead;
                    break;
                case "currently-reading":
                    status = BookShelfStatus.CurrentlyReading;
                    break;
                case "read":
                    status = BookShelfStatus.Read;
                    break;
                case "remove":
                    var removedEntry = _context.User_Books
                           .FirstOrDefault(ub => ub.User_Id == userId && ub.Book_Id == bookId);
                    if (removedEntry != null)
                    {
                        _context.User_Books.Remove(removedEntry);
                        _context.SaveChanges();
                    }
                    return RedirectToAction("Index", new { id = bookId });

                default:
                    return BadRequest("Invalid shelf type.");
            }

            var existingEntry = _context.User_Books
                .FirstOrDefault(ub => ub.User_Id == userId && ub.Book_Id == bookId);

            if (existingEntry != null)
            {
                existingEntry.Status = status;
                _context.User_Books.Update(existingEntry);
            }
            else
            {
                var newEntry = new UserBooks
                {
                    User_Id = userId,
                    Book_Id = bookId,
                    Status = status
                };
                _context.User_Books.Add(newEntry);
            }

            _context.SaveChanges();

            return RedirectToAction("Index", new { id = bookId });
        }

    }
}
