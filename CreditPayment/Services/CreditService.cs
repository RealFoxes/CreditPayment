using CreditPayment.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreditPayment.Api.Services
{
	public class CreditService
	{

		public CreditService()
		{

		}

		public List<CreditPaymentModel> CalculatePaymentsSchedule(CreditRequestModel credit)
		{
			var payments = new List<CreditPaymentModel>();

			var currentDate = credit.StartDate;

			// 16.05.2023 | 16.05.2024 => 12 months 
			var totalMonths = ((credit.EndDate.Year - credit.StartDate.Year) * 12) + credit.EndDate.Month - credit.StartDate.Month;
			if (credit.PaymentDay < credit.StartDate.Day)
				totalMonths--;

			var currentAmount = credit.Amount;
			var bodyPayment = credit.Amount / totalMonths;

			decimal percentsPayment = 0;
			while (currentDate <= credit.EndDate)
			{
				// credit.PaymentDay 31 | currentDate.Day = 25 | currentDate.Month = February | max 28-29 days
				// credit.PaymentDay 31 | currentDate.Day = 29 | currentDate.Month = June | max 30 days

				// creadit.PaymentDay 10 | currentDate.Day = 15

				var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

				var creditPaymentDay = credit.PaymentDay;
				if (creditPaymentDay > daysInMonth)
					creditPaymentDay = daysInMonth;

				var paymentDate = currentDate.AddDays(creditPaymentDay - currentDate.Day);

				//Платёж каждый месяц
				if (currentDate.Day == paymentDate.Day)
				{
					currentAmount = currentAmount - bodyPayment;

					payments.Add(new CreditPaymentModel
					{
						Date = currentDate,
						BodyPayment = bodyPayment.ToString("F"),
						PercentsPayment = percentsPayment.ToString("F"),
						RemainingAmount = currentAmount.ToString("F")
					});

					percentsPayment = 0;
				}

				var daysYear = DateTime.IsLeapYear(currentDate.Year) ? 366 : 365;
				percentsPayment = percentsPayment + CalculateAmountPercents(currentAmount, credit.InterestRate, daysYear);


				currentDate = currentDate.AddDays(1);
			}

			return payments;
		}

		private decimal CalculateAmountPercents(decimal creditAmount, decimal interestRate, int daysYear)
		{
			return creditAmount * interestRate / 100 / daysYear;
		}
		private decimal CalculateRemaningAmountPercents(decimal creditAmount, decimal interestRate, int daysYear, int totalDays)
		{
			return creditAmount * interestRate / 100 / daysYear * totalDays;
		}
		private decimal CalculateDifferentiatedPayment(decimal loanAmount, decimal interestRate, int numberOfDays, int paymentNumber)
		{
            decimal monthlyInterestRate = interestRate / 12 / 100;
            decimal principalPayment = loanAmount / numberOfDays * (paymentNumber - 1);
            decimal interestPayment = loanAmount * monthlyInterestRate;
            decimal differentiatedPayment = principalPayment + interestPayment;
			return differentiatedPayment;
		}
		private decimal CalculateAnnuityPayment(decimal interestRate, decimal amount, int durationInMonths)
		{
			decimal mountPercent = interestRate / (100 * 12);
			var annuityPayment = amount *
				(mountPercent /
				(decimal)(1 - Math.Pow((double)(1 + mountPercent), -durationInMonths)));
			// остаточные месяцы!!!

			return annuityPayment;
		}
	}
}
