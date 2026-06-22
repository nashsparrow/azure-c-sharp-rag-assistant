using System.ComponentModel.DataAnnotations;

namespace AzureCSharpRAGAssistant.Api.Validators
{
    public class NotWhiteSpaceAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            if (value is string s && !string.IsNullOrWhiteSpace(s))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult(ErrorMessage ?? $"{validationContext.MemberName} cannot be empty or whitespace.");
        }
    }
}