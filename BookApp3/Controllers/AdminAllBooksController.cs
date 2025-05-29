using BookApp3.Data;
using BookApp3.Models;
using BookApp3.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace BookApp3.Controllers
{
    public class AdminAllBooksController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminAllBooksController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
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
            ViewBag.Authors = _context.Authors
      .Select(a => new SelectListItem
      {
          Value = a.Author_id.ToString(),  
          Text = a.Name                   
      })
      .ToList();
            var bookViewModel = new BookViewModel
            {
                Book_Id = book.Book_Id,
                Title = book.Title,
                Description = book.Description,
                Author_Id = book.Author_Id,
                Page_Count = book.Page_Count,
                First_Publish = book.First_Publish,
                Current_Cover_Url = book.Cover_Url
            };

            return View(bookViewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(BookViewModel model, IFormFile coverFile)
        {
            var book = await _context.Books.FindAsync(model.Book_Id);
            if (book == null)
            {
                return NotFound();
            }

            // Update basic properties
            book.Title = model.Title;
            book.Description = model.Description;
            book.Author_Id = model.Author_Id;
            book.Page_Count = model.Page_Count;
            book.First_Publish = model.First_Publish;

            // Handle cover image update
            if (coverFile != null && coverFile.Length > 0)
            {
                // Save the new file and update the URL
                book.Cover_Url = await SaveFile(coverFile, "book-covers");
            }
            else if (!string.IsNullOrEmpty(model.Cover_Url))
            {
                // Use the URL from the hidden field (could be from URL input or reset)
                book.Cover_Url = model.Cover_Url;
            }

            try
            {
                _context.Update(book);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BookExists(model.Book_Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Book_Id == id);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            
            var book = await _context.Books
                .Include(b => b.Reviews) 
                .FirstOrDefaultAsync(b => b.Book_Id == id);
            var userBooks = await _context.User_Books
            .Where(ub => ub.Book_Id == id)  
            .ToListAsync();

            if (book.Reviews != null && book.Reviews.Any())
            {
                _context.Reviews.RemoveRange(book.Reviews);
            }

            if (userBooks.Any())
            {
                _context.User_Books.RemoveRange(userBooks);
            }

           
            _context.Books.Remove(book);

            await _context.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        private async Task<string> SaveFile(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0)
            {
                return null;
            }


            var uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "uploads", folderName);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }


            var uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(file.FileName);
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/uploads/{folderName}/{uniqueFileName}";
        }
        public IActionResult Add()
        {
            ViewBag.Authors = _context.Authors
                .Select(a => new SelectListItem
                {
                    Value = a.Author_id.ToString(),
                    Text = a.Name
                })
                .ToList();

            return View(new BookViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(BookViewModel model, IFormFile coverImage)
        {
            // Repopulate authors dropdown if validation fails
            ViewBag.Authors = _context.Authors
                .Select(a => new SelectListItem
                {
                    Value = a.Author_id.ToString(),
                    Text = a.Name
                })
                .ToList();


      
            string coverUrl = null;

            if (coverImage != null && coverImage.Length > 0)
            {
               
                coverUrl = await SaveFile(coverImage, "book-covers");
            }
            else if (!string.IsNullOrEmpty(model.Cover_Url))
            {
                
                coverUrl = model.Cover_Url;
            }

            var newBook = new Book
            {
                Title = model.Title,
                Description = model.Description,
                Author_Id = model.Author_Id, 
                Page_Count = model.Page_Count,
                First_Publish = model.First_Publish,
                Cover_Url = coverUrl
            };

            try
            {
                _context.Books.Add(newBook);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Book added successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An error occurred while saving: " + ex.Message);
                return View(model);
            }
        }
    }
    
}
