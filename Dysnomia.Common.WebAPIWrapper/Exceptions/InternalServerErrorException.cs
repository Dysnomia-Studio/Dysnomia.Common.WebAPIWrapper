using System;

namespace Dysnomia.Common.WebAPIWrapper.Exceptions {
	public class InternalServerErrorException : Exception {
		public InternalServerErrorException(string message) : base(message) { }
	}
}
