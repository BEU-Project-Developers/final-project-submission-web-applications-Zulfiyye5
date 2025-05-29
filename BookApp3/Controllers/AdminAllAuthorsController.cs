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
    }
}
