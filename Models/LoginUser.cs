using System.ComponentModel.DataAnnotations;

namespace WeddingPlanner.Models
{
    public class LoginUser
    {
        [Display(Name="Email")]
        [Required]
        [EmailAddress]
        public string Email {get;set;}

        [Display(Name="Password")]
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password {get;set;}
    }
}