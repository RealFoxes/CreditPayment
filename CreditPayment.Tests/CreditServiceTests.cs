using CreditPayment.Api.Models;
using CreditPayment.Api.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace CreditPayment.Tests
{
    public class CreditServiceTests
    {
        [Test]
        public void CreditRequestModel_Validation_ValidModel_NoErrors()
        {
            // Arrange
            var model = new CreditRequestModel
            {
                Amount = 10000,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(12).AddDays(1),
                InterestRate = 5,
                Type = PaymentType.Annuity,
                PaymentDay = 2
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

            // Assert
            Assert.IsTrue(isValid);
            Assert.IsEmpty(validationResults);
        }

        [Test]
        public void CreditRequestModel_Validation_InvalidModel_InvalidAmount()
        {
            // Arrange
            var model = new CreditRequestModel
            {
                Amount = -10000, // Invalid amount
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(12).AddDays(1),
                InterestRate = 5,
                Type = PaymentType.Differentiated,
                PaymentDay = 2
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

            // Assert
            Assert.IsFalse(isValid);
            Assert.IsNotEmpty(validationResults);
            Assert.AreEqual("Credit amount should be greater than 0.", validationResults[0].ErrorMessage);
        }

        [Test]
        public void CreditRequestModel_Validation_InvalidModel_InvalidStartDate()
        {
            // Arrange
            var model = new CreditRequestModel
            {
                Amount = 10000,
                StartDate = DateTime.Now.AddDays(2), // Invalid start date (greater than end date)
                EndDate = DateTime.Now,
                InterestRate = 5,
                Type = PaymentType.Differentiated,
                PaymentDay = 2
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

            // Assert
            Assert.IsFalse(isValid);
            Assert.IsNotEmpty(validationResults);
            Assert.AreEqual("StartDate should be less than the EndDate", validationResults[0].ErrorMessage);
        }

        [Test]
        public void CreditRequestModel_Validation_InvalidModel_InvalidPaymentDay()
        {
            // Arrange
            var model = new CreditRequestModel
            {
                Amount = 10000,
                StartDate = DateTime.Now,
                EndDate = DateTime.Now.AddMonths(12).AddDays(1),
                InterestRate = 5,
                Type = PaymentType.Annuity,
                PaymentDay = 32 // Invalid payment day (greater than 31)
            };

            // Act
            var validationResults = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(model, new ValidationContext(model), validationResults, true);

            // Assert
            Assert.IsFalse(isValid);
            Assert.IsNotEmpty(validationResults);
            Assert.AreEqual("Payment day should be between 1 and 31.", validationResults[0].ErrorMessage);
        }

        [Test]
        public void CalculatePaymentsSchedule_AnnuityPaymentType_ReturnsExpectedPayments()
        {
            // Arrange
            var credit = new CreditRequestModel
            {
                Amount = 100000,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2024, 1, 2),
                InterestRate = 5,
                Type = PaymentType.Annuity,
                PaymentDay = 2
            };

            var service = new CreditService();

            // Act
            var payments = service.CalculatePaymentsSchedule(credit);

            // Assert
            Assert.IsNotNull(payments);
            Assert.AreEqual(12, payments.Count);

            // Check the first payment
            var firstPayment = payments.First();
            Assert.AreEqual(new DateTime(2023, 2, 1), firstPayment.Date);
            Assert.AreEqual(decimal.Round(8144.08m, 2), decimal.Round(firstPayment.BodyPayment, 2));
            Assert.AreEqual(decimal.Round(416.67m, 2), decimal.Round(firstPayment.PercentsPayment, 2));
            Assert.AreEqual(decimal.Round(91855.92m, 2), decimal.Round(firstPayment.RemainingAmount, 2));

            // Check the last payment
            var lastPayment = payments.Last();
            Assert.AreEqual(new DateTime(2024, 01, 1), lastPayment.Date);
            Assert.AreEqual(decimal.Round(8525.23m, 2), decimal.Round(lastPayment.BodyPayment, 2));
            Assert.AreEqual(decimal.Round(35.52m, 2), decimal.Round(lastPayment.PercentsPayment, 2));
            Assert.AreEqual(decimal.Round(0.00m, 2), decimal.Round(lastPayment.RemainingAmount, 2));
        }

        [Test]
        public void CalculatePaymentsSchedule_DifferentiatedPaymentType_ReturnsExpectedPayments()
        {
            // Arrange
            var credit = new CreditRequestModel
            {
                Amount = 100000,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2024, 1, 2),
                InterestRate = 5,
                Type = PaymentType.Differentiated,
                PaymentDay = 2
            };

            var service = new CreditService();

            // Act
            var payments = service.CalculatePaymentsSchedule(credit);

            // Assert
            Assert.IsNotNull(payments);
            Assert.AreEqual(12, payments.Count);

            // Check the first payment
            var firstPayment = payments.First();
            Assert.AreEqual(new DateTime(2023, 2, 2), firstPayment.Date);
            Assert.AreEqual(decimal.Round(8333.33m, 2), decimal.Round(firstPayment.BodyPayment, 2));
            Assert.AreEqual(decimal.Round(438.36m, 2), decimal.Round(firstPayment.PercentsPayment, 2));
            Assert.AreEqual(decimal.Round(91666.67m, 2), decimal.Round(firstPayment.RemainingAmount, 2));

            // Check the last payment
            var lastPayment = payments.Last();
            Assert.AreEqual(new DateTime(2024, 1, 2), lastPayment.Date);
            Assert.AreEqual(decimal.Round(8333.33m, 2), decimal.Round(lastPayment.BodyPayment, 2));
            Assert.AreEqual(decimal.Round(35.39m, 2), decimal.Round(lastPayment.PercentsPayment, 2));
            Assert.AreEqual(decimal.Round(0.00m, 2), decimal.Round(lastPayment.RemainingAmount, 2));
        }


        [Test]
        public void CalculatePaymentsSchedule_InvalidPaymentType_ThrowsNotImplementedException()
        {
            // Arrange
            var credit = new CreditRequestModel
            {
                Amount = 100000,
                StartDate = new DateTime(2023, 1, 1),
                EndDate = new DateTime(2024, 1, 2),
                InterestRate = 5,
                Type = (PaymentType)2, // Invalid payment type
                PaymentDay = 1
            };

            var service = new CreditService();

            // Act and Assert
            Assert.Throws<NotImplementedException>(() => service.CalculatePaymentsSchedule(credit));
        }
    }
}