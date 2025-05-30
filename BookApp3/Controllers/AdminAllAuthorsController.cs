using BookApp3.Data;
using BookApp3.Models;
using BookApp3.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookApp3.Controllers
{
    public class AdminAllAuthorsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminAllAuthorsController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            var authors = _context.Authors.Include(a => a.Books).ToList();
            return View("Index", authors);
        }

        public IActionResult Add()
        {
            return View(new AuthorViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Add(AuthorViewModel model)
        {
            string profilePictureUrl = null;

            if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
            {
                profilePictureUrl = await SaveFile(model.ProfileImageFile, "author-profiles");
            }
            else if (!string.IsNullOrEmpty(model.Profile_Picture))
            {
                profilePictureUrl = model.Profile_Picture;
            }

            var author = new Author
            {
                Name = model.Name,
                Born_Loc = model.Born_Loc,
                Born_Date = model.Born_Date,
                Bio = model.Bio,
                Profile_Picture = profilePictureUrl
            };

            _context.Authors.Add(author);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Author added successfully!";
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var author = await _context.Authors.FindAsync(id);
            if (author == null)
            {
                return NotFound();
            }

            var model = new AuthorViewModel
            {
                Author_id = author.Author_id,
                Name = author.Name,
                Born_Loc = author.Born_Loc,
                Born_Date = author.Born_Date,
                Bio = author.Bio,
                Profile_Picture = author.Profile_Picture
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AuthorViewModel model)
        {
            if (id != model.Author_id)
            {
                return NotFound();
            }


            try
            {
                var author = await _context.Authors.FindAsync(id);
                if (author == null)
                {
                    return NotFound();
                }

               
                if (model.ProfileImageFile != null && model.ProfileImageFile.Length > 0)
                {
                 
                    var newProfilePictureUrl = await SaveFile(model.ProfileImageFile, "author-profiles");

                  
                    if (!string.IsNullOrEmpty(author.Profile_Picture) &&
                        !author.Profile_Picture.StartsWith("/images/default-"))
                    {
                        DeleteFile(author.Profile_Picture);
                    }

                    author.Profile_Picture = newProfilePictureUrl;
                }
                else if (!string.IsNullOrEmpty(model.Profile_Picture) &&
                         model.Profile_Picture != author.Profile_Picture)
                {
                    
                    author.Profile_Picture = model.Profile_Picture;
                }

              
                author.Name = model.Name;
                author.Born_Loc = model.Born_Loc;
                author.Born_Date = model.Born_Date;
                author.Bio = model.Bio;

                _context.Update(author);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Author updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AuthorExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.Author_id == id);

            if (author == null)
            {
                return NotFound();
            }

          
            if (author.Books != null && author.Books.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete author because they have associated books. Please delete the books first.";
                return RedirectToAction(nameof(Index));
            }

           
            if (!string.IsNullOrEmpty(author.Profile_Picture) &&
                !author.Profile_Picture.StartsWith("/images/default-"))
            {
                DeleteFile(author.Profile_Picture);
            }

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Author deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

        private bool AuthorExists(int id)
        {
            return _context.Authors.Any(e => e.Author_id == id);
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

        private void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath))
            {
                return;
            }

            var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, filePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }
    }
}