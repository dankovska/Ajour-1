using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace AjourBT.CustomAnnotations
{
    public class GreaterThanAttribute : ValidationAttribute, IClientValidatable
    {
        private readonly string _anotherProperty;
        private string startDate;
        public GreaterThanAttribute(string AnotherProperty)
        {
            _anotherProperty = AnotherProperty;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            GetStartDateValue(validationContext);
            string endDate = (string)value;
            DateTime StartDate = DateTime.ParseExact(startDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            DateTime EndDate = DateTime.ParseExact(endDate, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            if (EndDate != null && StartDate != null)
            {
                if (EndDate.Date < StartDate.Date)
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
            startDate = (string)property.GetValue(validationContext.ObjectInstance, null);
        }


        public IEnumerable<ModelClientValidationRule> GetClientValidationRules(ModelMetadata metadata, ControllerContext context)
        {

            ModelClientValidationRule mcvrDate = new ModelClientValidationRule();
            mcvrDate.ValidationType = "checkdates";
            mcvrDate.ErrorMessage = FormatErrorMessage(metadata.DisplayName);
            mcvrDate.ValidationParameters.Add("startdate", _anotherProperty);
            yield return mcvrDate;
        }


    }
}