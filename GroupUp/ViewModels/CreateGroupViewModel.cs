using System.ComponentModel.DataAnnotations;

namespace GroupUp.ViewModels
{
    
    public class CreateGroupViewModel
    {
        [Required]
        public int GroupId { get; set; }
        [Required]
        public string Title { get; set; }

        [Required]
        [Display(Name="Description")]
        public string DetailedDescription { get; set; }

        [Required]
        [Range(2, 50)]
        [Display(Name="Maximum number of users")]
        public int MaxUserCapacity { get; set; }

        [Required]
        [LocationVerification]
        public string City { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        public string Continent { get; set; }

    }

    public class LocationVerification : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var viewModel = (CreateGroupViewModel) validationContext.ObjectInstance;
            var city = viewModel.City;
            var country = viewModel.Country;
            var continent = viewModel.Continent;

            // if country is "-" but city is not:
            if (country == "-" && city != "-")
            {
                return new ValidationResult("Country can not be \"-\" when city is not.");
            }

            if (continent == "-" && (city != "-" || country != "-"))
            {
                return new ValidationResult("Continent can not be \"-\" when both city and country are not.");
            }

            else
            {
                return ValidationResult.Success;
            }
        }
    }
}