using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.Domain.CustomAnnotations
{
    public class GreaterThanAttribute : ValidationAttribute
    {
        private readonly string _anotherProperty;
        private DateTime? startDate;
        public GreaterThanAttribute(string AnotherProperty)
        {
            _anotherProperty = AnotherProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            GetStartDateValue(validationContext);
            DateTime? endDate = value as DateTime?;
            DateTime? StartDate = startDate;
            DateTime? EndDate = endDate;
            if (StartDate!= null && EndDate != null)
            {
                if (EndDate.Value.Date < StartDate.Value.Date)
                {
                    var errorMessage = FormatErrorMessage(validationContext.DisplayName);
                    return new ValidationResult(errorMessage);
                }
            }

            return ValidationResult.Success;
        }

        private void GetStartDateValue(ValidationContext validationContext)
        {
            var property = validationContext.ObjectType.GetProperty(_anotherProperty);
            startDate = (DateTime?)property.GetValue(validationContext.ObjectInstance, null);
        }

    }
}