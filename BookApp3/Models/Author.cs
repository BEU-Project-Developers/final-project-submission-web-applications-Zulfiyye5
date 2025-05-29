using System.ComponentModel.DataAnnotations;
namespace BookApp3.Models
{
    public class Author
    {
        [Key]
        public int Author_id { get; set; }
        public string Born_Loc { get; set; }
        public string Born_Date { get; set; }
        public string Name { get; set; }
        public string Bio { get; set; }
        public string Profile_Picture { get; set; }

        public ICollection<Book> Books { get; set; }
    }

}
