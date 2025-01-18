using Dysnomia.Common.WebAPIWrapper.Helpers;

using System.Text.Json;
using System.Text.Json.Serialization;

namespace Dysnomia.Common.WebAPIWrapper.Test {
    public class JsonTests {
        public class Entity {
            [JsonConverter(typeof(NumberToBooleanConverter))]
            public bool NumberToBool { get; set; }
            [JsonConverter(typeof(StringToBooleanConverter))]
            public bool StringToBool { get; set; }
            [JsonConverter(typeof(EmptyArrayToObjectConverter<SubEntity>))]
            public SubEntity SubEntities { get; set; } = null!;
            [JsonConverter(typeof(StringToNumberConverter<int>))]
            public int StringToNumber { get; set; }
            [JsonConverter(typeof(WhateverToStringConverter))]
            public string NumberToString { get; set; } = null!;
            [JsonConverter(typeof(WhateverToStringConverter))]
            public string StringToString { get; set; } = null!;
            [JsonConverter(typeof(WhateverToStringConverter))]
            public string BoolToString { get; set; } = null!;
            [JsonConverter(typeof(WhateverToStringConverter))]
            public string NullToString { get; set; } = null!;
        }

        public class SubEntity {
            public string Foo { get; set; } = null!;
        }

        [Fact]
        public void JsonTest_Converters_Test1() {
            var input = @"{""SubEntities"":[],""NumberToBool"":0,""StringToBool"":""false"",""StringToNumber"":""10"",""NumberToString"":10,""StringToString"":""test"",""BoolToString"":false,""NullToString"":null}";
            var expected = @"{""NumberToBool"":false,""StringToBool"":false,""SubEntities"":{""Foo"":null},""StringToNumber"":10,""NumberToString"":""10"",""StringToString"":""test"",""BoolToString"":""false"",""NullToString"":null}";

            var entity = JsonSerializer.Deserialize<Entity>(input);
            var output = JsonSerializer.Serialize(entity);

            Assert.Equal(expected, output);
        }

        [Fact]
        public void JsonTest_Converters_Test2() {
            var input = @"{""SubEntities"":{""Foo"":""bar""},""NumberToBool"":1,""StringToBool"":""true"",""StringToNumber"":""42"",""NumberToString"":42,""StringToString"":""foo"",""BoolToString"":true,""NullToString"":null}";
            var expected = @"{""NumberToBool"":true,""StringToBool"":true,""SubEntities"":{""Foo"":""bar""},""StringToNumber"":42,""NumberToString"":""42"",""StringToString"":""foo"",""BoolToString"":""true"",""NullToString"":null}";

            var entity = JsonSerializer.Deserialize<Entity>(input);
            var output = JsonSerializer.Serialize(entity);

            Assert.Equal(expected, output);
        }
    }
}