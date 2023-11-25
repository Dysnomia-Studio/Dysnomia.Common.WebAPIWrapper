using System;

namespace Dysnomia.Common.WebAPIWrapper.Exceptions {
	public class BadRequestException : Exception {
		public BadRequestException() { }
		public BadRequestException(string message) : base(message) { }
		public BadRequestException(string message, Exception innerException) : base(message, innerException) { }
	}
}
