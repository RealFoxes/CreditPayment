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
			List<CreditPaymentModel> payments;
			switch (credit.Type)
			{
				case PaymentType.Annuity:
					// Очень долго голову ломал, но так и не понял как хотят в тз видеть аннуететный по дням, сделал месячный расчёт без учета дня оплаты
					payments = GetAnnuityPayments(credit);
					break;
				case PaymentType.Differentiated:
					payments = GetDifferentiatedPayments(credit);
					break;
				default:
					throw new NotImplementedException();
			}

			return payments;
		}
		private List<CreditPaymentModel> GetAnnuityPayments(CreditRequestModel credit)
		{
			var payments = new List<CreditPaymentModel>();
			var currentDate = credit.StartDate.AddMonths(1);
			var currentAmount = credit.Amount;

			int totalMonths = GetTotalMonths(credit.StartDate, credit.EndDate);
			var annuityPayment = CalculateAnnuityMonthPayment(credit.InterestRate, credit.Amount, totalMonths);

			while (currentDate < credit.EndDate)
			{
				decimal percents = currentAmount * credit.InterestRate / 100 / 12;
				currentAmount -= annuityPayment - percents;

				payments.Add(new CreditPaymentModel
				{
					Date = currentDate,
					BodyPayment = (annuityPayment - percents),
					PercentsPayment = percents,
					RemainingAmount = currentAmount
				});


				currentDate = currentDate.AddMonths(1);
			}

			return payments;
		}
		private decimal CalculateAnnuityMonthPayment(decimal interestRate, decimal amount, int durationInMonths)
		{
			decimal mountPercent = interestRate / (100 * 12);
			var annuityPayment = 
				(amount * mountPercent /
				(decimal)(1 - Math.Pow((double)(1 + mountPercent), -durationInMonths)));

			return annuityPayment;
		}

		private List<CreditPaymentModel> GetDifferentiatedPayments(CreditRequestModel credit)
		{
			var payments = new List<CreditPaymentModel>();
			int totalMonths = GetTotalMonths(credit.StartDate, credit.EndDate);
			var currentDate = credit.StartDate;

			var currentAmount = credit.Amount;
			var bodyPayment = credit.Amount / totalMonths;

			decimal percentsPayment = 0;

			while (currentDate <= credit.EndDate)
			{
				var daysYear = DateTime.IsLeapYear(currentDate.Year) ? 366 : 365;
				var daysInMonth = DateTime.DaysInMonth(currentDate.Year, currentDate.Month);

				var paymentDay = credit.PaymentDay > daysInMonth ? daysInMonth : credit.PaymentDay;
				var paymentDate = currentDate.AddDays(paymentDay - currentDate.Day);

				bool isStartCreditDate = currentDate.Month == credit.StartDate.Month && currentDate.Year == credit.StartDate.Year;
				//Платёж каждый месяц
				if (currentDate.Day == paymentDate.Day && !isStartCreditDate)
				{
					currentAmount -= bodyPayment;

					payments.Add(new CreditPaymentModel
					{
						Date = currentDate,
						BodyPayment = bodyPayment,
						PercentsPayment = percentsPayment,
						RemainingAmount = currentAmount
					});

					percentsPayment = 0;
				}

				percentsPayment += CalculateDifferentiatedDayPayment(currentAmount, credit.InterestRate, daysYear);

				currentDate = currentDate.AddDays(1);
			}

			return payments;
		}

		private static int GetTotalMonths(DateTime startDate, DateTime endDate)
		{
			return ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;
		}

		private decimal CalculateDifferentiatedDayPayment(decimal creditAmount, decimal interestRate, int daysYear)
		{
			return creditAmount * interestRate / 100 / daysYear;
		}

	}
}
