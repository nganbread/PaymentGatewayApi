using System;
using System.ComponentModel.DataAnnotations;

namespace Checkout.PaymentGateway.Api.Mvc.Validation
{
    /// <summary>
    /// Validates that an enum is one of it's defined values. Does not work for flags enums 
    /// </summary>
    public class EnumIsDefinedAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (value is Enum && !Enum.IsDefined(value.GetType(), value))
            {
               return new ValidationResult($"The {validationContext.DisplayName} field is not a valid enum value");
            }

            return ValidationResult.Success;
        }
    }
}