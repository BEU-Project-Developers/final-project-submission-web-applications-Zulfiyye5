using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace BookApp3.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int User_Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string User_Name { get; set; }
        public string? Profile_Picture { get; set; }
        public string? Bg_Picture { get; set; }

        public string? Bio { get; set; }

        public DateTime? Birthdate { get; set; }

        public int? offSetY { get;set;}

        public DateTime? JoinedIn { get;set;}

        public ICollection<Review> Reviews { get; set; }
        public ICollection<UserBooks> UserBooks { get; set; }
        public User()
        {
            Reviews = new List<Review>();
            UserBooks = new List<UserBooks>();
        }
    }

}
