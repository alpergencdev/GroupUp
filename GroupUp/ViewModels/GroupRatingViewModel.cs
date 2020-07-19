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

            var thisViewModel = (GroupRatingViewModel) validationContext.ObjectInstance;
            foreach (var kvp in thisViewModel.UserRatings)
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