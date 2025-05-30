using BookApp3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookApp3.Controllers
{
    public class AdminAllUsersController : Controller
    {
        private readonly AppDbContext _context;
        public AdminAllUsersController(AppDbContext context)
        {
            _context = context;
        }
        public async Task<IActionResult> Index()
        {
            var users = await _context.Users.ToListAsync();
            return View(users);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            var reviews = _context.Reviews.Where(r => r.User_Id == id);
            _context.Reviews.RemoveRange(reviews);
     
            var userBooks = _context.User_Books.Where(ub => ub.User_Id == id);
            _context.User_Books.RemoveRange(userBooks);
        
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "User deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
