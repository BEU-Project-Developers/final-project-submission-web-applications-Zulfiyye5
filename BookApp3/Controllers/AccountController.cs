using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using System.Security.Claims;
using System.Text;
using BookApp3.Data;
using BookApp3.Models;
using BookApp3.ViewModels;


namespace BookApp3.Controllers
{
    public class AccountController : Controller
    {
        private readonly AppDbContext _context;

        public AccountController(AppDbContext context)
        {
            _context = context;
        }


        public ActionResult SignUp()
        {
            
            return View();
        }

        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignUp(SignUpViewModel model)
        {
            if (ModelState.IsValid)
            {
                
                if (_context.Users.Any(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already in use.");
                    return View(model);
                }

                var user = new User
                {
                    User_Name = model.User_Name,
                    Email = model.Email,
                    Password = model.Password 
                };

                _context.Users.Add(user);
                _context.SaveChanges();

                return RedirectToAction("Index","Home");
            }

            return View(model);
        }

        public ActionResult SignIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult SaveBgOffset(int userId, int offsetY)
        {
            var user = _context.Users.FirstOrDefault(u => u.User_Id == userId);
            if (user != null)
            {
                user.offSetY = offsetY; 
                _context.SaveChanges(); 
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "User not found" });
        }


        public ActionResult GetBgOffset(int userId)
        {
            var user = _context.Users.FirstOrDefault(u => u.User_Id == userId);
            if (user != null)
            {
                return Json(new { offsetY = user.offSetY ?? 0 });
            }
            return Json(new { offsetY = 0 });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SignIn(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email && u.Password == password);

            
            if (user != null)
            {

                HttpContext.Session.SetString("UserId", user.User_Id.ToString());
                HttpContext.Session.SetString("UserName", user.User_Name);
                if(user.User_Name == "admin")
                {
                    return RedirectToRoute("admin", new { controller = "AdminAllBooks", action = "Index" });

                }

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Message = "Invalid email or password";
            return View();
        }

        public ActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Signin", "Account");
        }

    }
}
