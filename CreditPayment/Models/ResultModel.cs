using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CreditPayment.Api.Models
{
	public class ResultModel
	{
		public ResultModel()
		{
		}

		public ResultModel(string errorMessage)
		{
			if (string.IsNullOrEmpty(errorMessage))
			{
				throw new ArgumentException("Message cannot be empty.", nameof(errorMessage));
			}

			ErrorMessage = errorMessage;
		}

		public bool IsSuccess => ErrorMessage == null;

		public string ErrorMessage { get; set; }
	}

	public class ResultModel<T> : ResultModel
	{
		public ResultModel()
		{
		}

		public ResultModel(T content)
		{
			Content = content;
		}

		public ResultModel(string errorMessage)
			: base(errorMessage)
		{
		}
		public T Content { get; set; }

	}
}
