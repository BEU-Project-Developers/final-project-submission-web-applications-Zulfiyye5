using BookApp3.Models;
using System.ComponentModel.DataAnnotations;

namespace BookApp3.ViewModels
{
    public class ProfileViewModel
    {
        public User User { get; set; }
        public List<Book> WantToReadBooks { get; set; }
        public List<Book> CurrentlyReadingBooks { get; set; }
        public List<Book> ReadBooks { get; set; }

    }
}
