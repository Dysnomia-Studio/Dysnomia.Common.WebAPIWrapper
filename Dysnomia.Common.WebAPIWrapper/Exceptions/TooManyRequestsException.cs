using System;

namespace Dysnomia.Common.WebAPIWrapper.Exceptions {
	public class TooManyRequestsException : Exception {
		public TooManyRequestsException(string message) : base(message) { }
	}
}
