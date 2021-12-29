using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dysnomia.Common.WebAPIWrapper.Helpers {
	public class WhateverToStringConverter : JsonConverter<string> {
		public override string Read(ref Utf8JsonReader reader, Type type, JsonSerializerOptions options) {
			if (reader.TokenType == JsonTokenType.Number) {
				return reader.GetUInt64().ToString();
			}

			if (reader.TokenType == JsonTokenType.String) {
				return reader.GetString();
			}

			if (reader.TokenType == JsonTokenType.False) {
				return "false";
			}

			if (reader.TokenType == JsonTokenType.True) {
				return "true";
			}

			if (reader.TokenType == JsonTokenType.Null) {
				return null;
			}

			using (JsonDocument document = JsonDocument.ParseValue(ref reader)) {
				throw new Exception($"unable to parse {document.RootElement.ToString()} to string");
			}
		}

		public override bool CanConvert(Type typeToConvert) {
			return true;
		}

		public override void Write(Utf8JsonWriter writer, string value, JsonSerializerOptions options) {
			JsonSerializer.Serialize(writer, value, options);
		}
	}
}
