using System;
using System.ComponentModel.DataAnnotations;

namespace Checkout.PaymentGateway.Api.Mvc.Validation
{
    /// <summary>
    /// Validates common value and struct types not to be their default value.
    ///
    /// Use <see cref="RequiredAttribute"/> for similar class validation
    /// </summary>
    public class NotDefaultAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            if (IsDefault(value)) return new ValidationResult($"The {validationContext.DisplayName} field must have a non-default value");

            return ValidationResult.Success;
        }

        private bool IsDefault(object value)
        {
            if (value is Guid guid && guid == default) return true;
            if (value is int i && i == default) return true;
            if (value is decimal m && m == default) return true;
            if (value is double d && d == default) return true;
            if (value is long l && l == default) return true;
            if (value is Enum && (int)value == 0) return true;

            return false;
        }
    }
}
