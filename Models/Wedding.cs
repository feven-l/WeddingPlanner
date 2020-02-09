using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WeddingPlanner.Models
{
    public class FutureDateAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(Object value, ValidationContext validationContext)
        {
            var dt = value as DateTime? ?? new DateTime();
            if(dt < DateTime.Now){
                return new ValidationResult("Date cannot be in the past.");
            }
            return ValidationResult.Success;
        }
    }
    public class Wedding
    {
        [Key]
        public int WeddingId {get;set;}
        [Required]
        [Display(Name="Wedder One")]

        public int CreatorId {get;set;} = 0;
        public string WedderOne {get;set;}

        [Display(Name="Wedder Two")]
        [Required]
        public string WedderTwo {get;set;}

        [Display(Name="Wedding Date")]
        [Required]
        [FutureDate]
        [DataType(DataType.Date)]
        public DateTime WeddingDate {get;set;}

        [Required]
        [Display(Name="Wedding Address")]
        public string Address {get;set;}

        public DateTime CreatedAt {get;set;} = DateTime.Now;
        public DateTime UpdatedAt {get;set;} = DateTime.Now;

        public List<Association> guests {get;set;}

    }
}