using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models
{
    public class User
    {
        [Key]
        public int UserId {get;set;}

        [Display(Name="First Name")]
        [Required]
        [MinLength(2)]
        public string FirstName {get;set;}

        [Display(Name="Last Name")]
        [Required]
        [MinLength(2)]
        public string LastName {get;set;}

        [Display(Name="Email")]
        [Required]
        [EmailAddress]
        public string Email {get;set;}

        [Display(Name="Password")]
        [Required]
        [MinLength(8)]
        [DataType(DataType.Password)]
        public string Password {get;set;}
        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        [NotMapped]
        [Display(Name="Confirm Password")]
        [Required]
        [MinLength(8)]
        [Compare("Password")]
        [DataType(DataType.Password)]
        public string ConfirmPassword {get;set;}

        public List<Association> Weddings {get;set;}
    }
}
