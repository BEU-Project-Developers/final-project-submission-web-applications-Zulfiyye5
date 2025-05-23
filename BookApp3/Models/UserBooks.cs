using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookApp3.Models
{
    public class UserBooks
    {
        public int User_Id { get; set; }
        public int Book_Id { get; set; }

        public BookShelfStatus Status { get; set; }

        public User? User { get; set; }
        public Book? Book { get; set; }
    }

    public enum BookShelfStatus
    {
        WantToRead = 0,
        CurrentlyReading = 1,
        Read = 2
    }
}
