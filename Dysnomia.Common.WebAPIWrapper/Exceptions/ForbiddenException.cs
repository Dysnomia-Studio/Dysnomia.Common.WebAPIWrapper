using System;

namespace Dysnomia.Common.WebAPIWrapper.Exceptions {
	public class ForbiddenException : Exception {
		public ForbiddenException(string message) : base(message) { }
	}
}
