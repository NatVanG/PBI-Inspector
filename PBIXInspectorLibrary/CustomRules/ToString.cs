using Json.Logic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PBIXInspectorLibrary.CustomRules
{
    /// <summary>
    /// Handles the `tostring` operation. Converts a JsonNode to a string.
    /// </summary>
    [Operator("tostring")]
    [JsonConverter(typeof(ToStringJsonConverter))]
    public class ToString : Json.Logic.Rule
    {
        internal Json.Logic.Rule InputString { get; }

        internal ToString(Json.Logic.Rule inputString)
        {
            InputString = inputString;
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
            var inputString = InputString.Apply(data, contextData);

            //if (inputString is not JsonValue inputStringValue || !inputStringValue.TryGetValue(out string? stringInputString))
            //    throw new JsonLogicException($"Cannot ToString a non-string searchString.");

            return inputString?.ToJsonString();
        }
    }

    internal class ToStringJsonConverter : JsonConverter<ToString>
    {
        public override ToString? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var parameters = JsonSerializer.Deserialize<Json.Logic.Rule[]>(ref reader, options);

            if (parameters is not { Length: 1 })
                throw new JsonException("The ToString rule needs an array with 1 parameters.");

            if (parameters.Length == 1) return new ToString(parameters[0]);

            return new ToString(parameters[0]);
        }

        public override void Write(Utf8JsonWriter writer, ToString value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("tostring");
            writer.WriteStartArray();
            writer.WriteRule(value.InputString, options);
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }
}
