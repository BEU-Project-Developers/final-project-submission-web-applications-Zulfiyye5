using BookApp3.Models;
using System.ComponentModel.DataAnnotations;

namespace BookApp3.ViewModels
{
    public class ProfileEditViewModel
    {
        public List<Book> ReadBooks { get; set; }
        public int User_Id { get; set; }

        public string User_Name { get; set; }
        public string Email { get; set; }
        public string Bio { get; set; }
        public string Location { get; set; }

        public DateTime? Birthdate { get; set; }
        public IFormFile ProfilePicture { get; set; }
        public IFormFile BgPicture { get; set; }
        public string CurrentProfilePicture { get; set; }
        public string CurrentBgPicture { get; set; }

    }
}
