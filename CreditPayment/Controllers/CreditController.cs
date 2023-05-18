using CreditPayment.Api.Models;
using CreditPayment.Api.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace CreditPayment.Api.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CreditController : ControllerBase
	{
		private readonly CreditService loanService;

		public CreditController(CreditService loanService)
		{
			this.loanService = loanService;
		}

        [HttpPost("calculate")]
        public IActionResult CalculatePaymentSchedule([FromBody] CreditRequestModel inputModel)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			List<CreditPaymentModel> paymentSchedule = loanService.CalculatePaymentsSchedule(inputModel);
			return Ok(paymentSchedule);
		}

		
    }
}
