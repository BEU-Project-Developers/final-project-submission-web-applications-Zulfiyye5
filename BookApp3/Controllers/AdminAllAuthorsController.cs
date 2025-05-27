using BookApp3.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BookApp3.Controllers
{
    public class AdminAllAuthorsController : Controller
    {
        private readonly AppDbContext _context;
        public AdminAllAuthorsController(AppDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var authors = _context.Authors.Include(a => a.Books).ToList();
            return View("Index", authors);
        }
    }
}
