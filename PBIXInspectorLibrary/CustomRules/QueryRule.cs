using Json.Logic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PBIXInspectorLibrary.CustomRules
{
    /// <summary>
    /// Handles the `query` operation.
    /// </summary>
    [Operator("query")]
    [JsonConverter(typeof(QueryJsonConverter))]
    public class QueryRule : Json.Logic.Rule
    {
        internal Json.Logic.Rule Input { get; }
        internal Json.Logic.Rule Rule { get; }

        internal QueryRule(Json.Logic.Rule input, Json.Logic.Rule rule)
        {
            Input = input;
            Rule = rule;
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
            var input = Input.Apply(data, contextData);

            return Rule.Apply(data, input);
        }
    }

    internal class QueryJsonConverter : JsonConverter<QueryRule>
    {
        public override QueryRule? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var parameters = JsonSerializer.Deserialize<Json.Logic.Rule[]>(ref reader, options);

            if (parameters is not { Length: 2 })
                throw new JsonException("The query rule needs an array with 2 parameters.");

            return new QueryRule(parameters[0], parameters[1]);
        }

        public override void Write(Utf8JsonWriter writer, QueryRule value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("query");
            writer.WriteStartArray();
            writer.WriteRule(value.Input, options);
            writer.WriteRule(value.Rule, options);
            writer.WriteEndArray();
            writer.WriteEndObject();
        }
    }

}
