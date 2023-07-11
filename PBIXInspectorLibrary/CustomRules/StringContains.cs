using Json.Logic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace PBIXInspectorLibrary.CustomRules
{
    /// <summary>
    /// Handles the `strcontains` operation.
    /// </summary>
    [Operator("strcontains")]
    [JsonConverter(typeof(StringContainsJsonConverter))]
    public class StringContains : Json.Logic.Rule
    {
        internal Json.Logic.Rule SearchString { get; }
        internal Json.Logic.Rule ContainsString { get; }

        internal StringContains(Json.Logic.Rule searchString, Json.Logic.Rule containsString)
        {
            SearchString = searchString;
            ContainsString = containsString;
        }

        /// <summary>
        /// Applies the rule to the input data.
        /// </summary>
        /// <param name="data">The input data.</param>
        /// <param name="contextData">
        ///     Optional secondary data.  Used by a few operators to pass a secondary
        ///     data context to inner operators.
        /// </param>
        /// <returns>The result of the rule.</returns>
        public override JsonNode? Apply(JsonNode? data, JsonNode? contextData = null)
        {
            var searchString = SearchString.Apply(data, contextData);
            var containsString = ContainsString.Apply(data, contextData);

            if (searchString is not JsonValue searchStringValue || !searchStringValue.TryGetValue(out string? stringSearchString))
                throw new JsonLogicException($"Cannot stringcontains a non-string searchString.");

            if (containsString is not JsonValue containsStringValue || !containsStringValue.TryGetValue(out string? stringContainsString))
                throw new JsonLogicException($"Cannot stringcontains a non-string containsString.");

            return Regex.Matches(stringSearchString, stringContainsString).Count;
        }
    }

    internal class StringContainsJsonConverter : JsonConverter<StringContains>
    {
        public override StringContains? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var parameters = JsonSerializer.Deserialize<Json.Logic.Rule[]>(ref reader, options);

            if (parameters is not { Length: 2 })
                throw new JsonException("The stringcontains rule needs an array with 2 parameters.");

            if (parameters.Length == 2) return new StringContains(parameters[0], parameters[1]);

            return new StringContains(parameters[0], parameters[1]);
        }

        public override void Write(Utf8JsonWriter writer, StringContains value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("strcontains");
            writer.WriteStartArray();
            writer.WriteRule(value.SearchString, options);
            writer.WriteRule(value.ContainsString, options);

            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}