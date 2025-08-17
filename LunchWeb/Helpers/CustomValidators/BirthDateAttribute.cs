using System.ComponentModel.DataAnnotations;

namespace LunchWeb.Helpers.CustomValidators
{
    public class BirthDateAttribute : ValidationAttribute
    {
        private readonly DateTime _minDate = new DateTime(1930, 1, 1);

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            if (value is DateTime date)
            {
                var maxDate = DateTime.Today.AddYears(-5);
                if (date < _minDate || date > maxDate)
                {
                    return new ValidationResult($"Datum narození musí být mezi {_minDate:dd.MM.yyyy} a {maxDate:dd.MM.yyyy}.");
                }
            }

            return ValidationResult.Success!;
        }
    }
}
