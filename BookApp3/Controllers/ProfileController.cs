using BookApp3.Data;
using BookApp3.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookApp2.Controllers
{
    public class ProfileController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ProfileController(AppDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<IActionResult> Index()
        {
   
            int id;
            if (HttpContext.Session.GetString("UserId") != null)
            {
                id = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            }
            else
            {
                id = 1;
            }

            var user = await _context.Users
                .Include(u => u.Reviews) 
                .FirstOrDefaultAsync(u => u.User_Id == id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }



        [HttpGet]
        public async Task<IActionResult> Edit()
        {
            int id;
            if (HttpContext.Session.GetString("UserId") != null)
            {
                id = Convert.ToInt32(HttpContext.Session.GetString("UserId"));
            }
            else
            {
                id = 1; 
            }

            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }

            var model = new ProfileEditViewModel
            {
                User_Id = user.User_Id,
                User_Name = user.User_Name,
                Email = user.Email,
                Bio = user.Bio,
               
                Birthdate = user.Birthdate,
                CurrentProfilePicture = user.Profile_Picture,
                CurrentBgPicture = user.Bg_Picture
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProfileEditViewModel model)
        {
    


            {
                Console.WriteLine("Model is valid");
                var user = await _context.Users.FindAsync(model.User_Id);
                if (user == null)
                {
                    Console.WriteLine("User not found");
                    return NotFound();
                    
                }

               
                user.User_Name = model.User_Name;
                user.Email = model.Email;
                user.Bio = model.Bio;
               
               
                user.Birthdate = model.Birthdate;


                if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
                {
                    user.Profile_Picture = await SaveFile(model.ProfilePicture, "profile-pictures");
                }


                if (model.BgPicture != null && model.BgPicture.Length > 0)
                {
                    user.Bg_Picture = await SaveFile(model.BgPicture, "background-pictures");
                }

                _context.Update(user);
                await _context.SaveChangesAsync();
                
                return RedirectToAction(nameof(Index));
            }


         
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