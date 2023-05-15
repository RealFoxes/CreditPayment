using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreditPayment.Api.Models
{
	public class CreditPaymentModel
	{
        public DateTime Date { get; set; }
        //public decimal BodyPayment { get; set; }
        //public decimal PercentsPayment { get; set; }
        //public decimal RemainingAmount { get; set; }
        public string BodyPayment { get; set; }
        public string PercentsPayment { get; set; }
        public string RemainingAmount { get; set; }
    }
}
