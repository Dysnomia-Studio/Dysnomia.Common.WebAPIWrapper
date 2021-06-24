using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dysnomia.Common.WebAPIWrapper.Helpers {
	public class NumberToBooleanConverter : JsonConverter<bool> {
		public override bool Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) {
			if (reader.TokenType == JsonTokenType.Number) {
				return reader.GetInt64() == 1;
			}

			return reader.GetBoolean();
		}
		public override bool CanConvert(Type typeToConvert) {
			return true;
		}

		public override void Write(Utf8JsonWriter writer, bool value, JsonSerializerOptions options) {
			JsonSerializer.Serialize(writer, value, options);
		}
	}
}
