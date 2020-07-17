using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using GroupUp.Models;
using Newtonsoft.Json;

namespace GroupUp.ViewModels
{
    public class GroupRatingViewModel
    {
        [RatingsAreValid]
        public Dictionary<string, int> UserRatings { get; set; }

        [Required]
        public int GroupId { get; set; }
    }

    public class RatingsAreValid : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var dict = (Dictionary<string, int>) validationContext.ObjectInstance;
            foreach (var kvp in dict)
            {
                if (kvp.Value < 0 || kvp.Value > 10)
                {
                    return new ValidationResult($"The rating for {kvp.Key} must be between 0 and 10.");
                }
            }
            return ValidationResult.Success;
        }
    }

}