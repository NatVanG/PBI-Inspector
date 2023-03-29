using System.Text.Json;
using System.Text.Json.Serialization;


namespace PBIXInspectorTests
{
    [JsonConverter(typeof(JsonLogicTestSuiteConverter))]
    public class JsonLogicTestSuite
    {
#pragma warning disable CS8618
        public List<JsonLogicTest> Tests { get; set; }
#pragma warning restore CS8618
    }

    public class JsonLogicTestSuiteConverter : JsonConverter<JsonLogicTestSuite?>
    {
        public override JsonLogicTestSuite? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType != JsonTokenType.StartArray)
                throw new JsonException("Test suite must be an array of tests.");

            var tests = JsonSerializer.Deserialize<List<JsonLogicTest>>(ref reader, options)!
                .Where(t => t != null)
                .ToList();

            return new JsonLogicTestSuite { Tests = tests };
        }

        public override void Write(Utf8JsonWriter writer, JsonLogicTestSuite? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
