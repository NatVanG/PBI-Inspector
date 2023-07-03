using Json.Logic;
using Json.Logic.Rules;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PBIXInspectorLibrary.CustomRules
{
    /// <summary>
    /// Handles the `count` operation.
    /// </summary>
    [Operator("isnullorempty")]
    [JsonConverter(typeof(IsNullOrEmptyJsonConverter))]
    public class IsNullOrEmptyRule : Json.Logic.Rule
    {
        internal Json.Logic.Rule Value { get; }

        internal IsNullOrEmptyRule(Json.Logic.Rule value)
        {
            Value = value;
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
            var value = Value.Apply(data, contextData);

            return !value.IsTruthy();
        }
    }


    internal class IsNullOrEmptyJsonConverter : JsonConverter<IsNullOrEmptyRule>
    {
        public override IsNullOrEmptyRule? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var node = JsonSerializer.Deserialize<JsonNode?>(ref reader, options);

            var parameters = node is JsonArray
                ? node.Deserialize<Json.Logic.Rule[]>()
                : new[] { node.Deserialize<Json.Logic.Rule>()! };

            if (parameters is not { Length: 1 })
                throw new JsonException("The isnullorempty rule needs an array with a single parameter.");

            return new IsNullOrEmptyRule(parameters[0]);
        }

        public override void Write(Utf8JsonWriter writer, IsNullOrEmptyRule value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("isnullorempty");
            writer.WriteRule(value.Value, options);
            writer.WriteEndObject();
        }
    }
}