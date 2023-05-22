using System;

namespace Dysnomia.Common.WebAPIWrapper.Exceptions {
	public class BadRequestException : Exception {
		public BadRequestException(string message) : base(message) { }
	}
}
