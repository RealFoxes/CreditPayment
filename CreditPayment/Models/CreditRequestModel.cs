using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CreditPayment.Api.Models
{
    public class CreditRequestModel : IValidatableObject
    {

        [Range(0.01, double.MaxValue, ErrorMessage = "Credit amount should be greater than 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Start date is required.")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required.")]
        public DateTime EndDate { get; set; }

        [Range(0.01, double.MaxValue, ErrorMessage = "Interest rate should be greater than 0.")]
        public decimal InterestRate { get; set; }

        [Required(ErrorMessage = "Type is required.")]
        public PaymentType Type { get; set; }

        [Range(1, 31, ErrorMessage = "Payment day should be between 1 and 31.")]
        public int PaymentDay { get; set; } = 1;

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var errors = new List<ValidationResult>();

            if (StartDate > EndDate)
                errors.Add(new ValidationResult("StartDate should be less than the EndDate"));

            if (StartDate == EndDate)
                errors.Add(new ValidationResult("StartDate and the EndDate cannot be equal"));

            if (StartDate.Day == EndDate.Day)
                errors.Add(new ValidationResult("StartDate.Day and the EndDate.Day cannot be equal"));

            if (StartDate.Day == PaymentDay)
                errors.Add(new ValidationResult("StartDate.Day and the PaymentDay cannot be equal"));

            return errors;
        }
    }
}
