using System;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dysnomia.Common.WebAPIWrapper.Helpers {
    public class StringToFloatConverter : JsonConverter<float> {
        public override float Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) {
            var value = reader.GetString();
            return float.Parse(value, CultureInfo.InvariantCulture);
        }

        public override bool CanConvert(Type typeToConvert) {
            return true;
        }

        public override void Write(Utf8JsonWriter writer, float value, JsonSerializerOptions options) {
            JsonSerializer.Serialize(writer, value, options);
        }
    }
}
