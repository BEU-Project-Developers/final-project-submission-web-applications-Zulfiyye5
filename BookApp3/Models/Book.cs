using System.ComponentModel.DataAnnotations;

namespace BookApp3.Models
{
    public class Book
    {
        [Key]
        public int Book_Id { get; set; }
        public string Cover_Url { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Author_Id { get; set; }
        public int Page_Count { get; set; }
        public string First_Publish { get; set; }
 
        public Author Author { get; set; }

        public ICollection<Review> Reviews { get; set; }
    }

}
