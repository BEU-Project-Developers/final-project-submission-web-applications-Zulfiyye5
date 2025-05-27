using System.ComponentModel.DataAnnotations;
namespace BookApp3.Models
{
    public class Review
    {
        [Key]
        public int Review_Id { get; set; }
        public int User_Id { get; set; }
        public int Book_Id { get; set; }
        public int Rating { get; set; }=0;
        public string? Review_Text { get; set; }
        public DateTime Created_At { get; set; }

        public User User { get; set; }
        public Book Book { get; set; }
    }

}
