using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace CreditPayment.Api.Models
{
    public class CreditRequestModel
    {
        //[Range(0.01, double.MaxValue, ErrorMessage = "Loan amount should be greater than 0.")]
        //public double Amount { get; set; }

        //[Required(ErrorMessage = "Start date is required.")]
        //public DateTime StartDate { get; set; }

        //[Required(ErrorMessage = "End date is required.")]
        //public DateTime EndDate { get; set; }

        //[Range(0.01, double.MaxValue, ErrorMessage = "Interest rate should be greater than 0.")]
        //public double InterestRate { get; set; }

        //[Required(ErrorMessage = "Schedule type is required.")]
        //public PaymentType ScheduleType { get; set; }

        //[Range(1, 31, ErrorMessage = "Payment day should be between 1 and 31.")]
        //public int? PaymentDay { get; set; }

        public decimal Amount { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal InterestRate { get; set; }
        public PaymentType Type { get; set; }
        public int PaymentDay { get; set; } = 1;
    }
}
