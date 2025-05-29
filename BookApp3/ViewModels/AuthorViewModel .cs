using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookApp3.ViewModels
{
    public class AuthorViewModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Author_id { get; set; }

        [Required(ErrorMessage = "Author name is required")]
        [Display(Name = "Author Name")]
        public string Name { get; set; }

        [Display(Name = "Birth Location")]
        public string Born_Loc { get; set; }

        [Display(Name = "Birth Date")]
        [DataType(DataType.Date)]
        public string Born_Date { get; set; }

        [Display(Name = "Biography")]
        public string Bio { get; set; }

        [Display(Name = "Profile Picture URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string Profile_Picture { get; set; }

        [Display(Name = "Upload Profile Picture")]
        public IFormFile ProfileImageFile { get; set; }
    }
}