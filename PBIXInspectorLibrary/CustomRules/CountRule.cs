using Json.Logic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PBIXInspectorLibrary.CustomRules
{
    /// <summary>
    /// Handles the `count` operation.
    /// </summary>
    [Operator("count")]
    [JsonConverter(typeof(CountJsonConverter))]
    public class CountRule : Json.Logic.Rule
    {
        private const string ARRAY_PARAM_NAME = "array";

        internal List<Json.Logic.Rule> Items { get; }

        internal CountRule(Json.Logic.Rule a, params Json.Logic.Rule[] more)
        {
            Items = new List<Json.Logic.Rule> { a };
            Items.AddRange(more);
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
            int result = 0;

            if (data != null)
            {
                if (data[ARRAY_PARAM_NAME] is JsonArray)
                {
                    result = data[ARRAY_PARAM_NAME] != null ? data[ARRAY_PARAM_NAME].AsArray().Count : 0;
                }
            }

            return result;
        }
    }


    internal class CountJsonConverter : JsonConverter<CountRule>
    {
        public override CountRule? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var node = JsonSerializer.Deserialize<JsonNode?>(ref reader, options);

            var parameters = node is JsonArray
                ? node.Deserialize<Json.Logic.Rule[]>()
                : new[] { node.Deserialize<Json.Logic.Rule>()! };

            if (parameters == null || parameters.Length == 0)
                throw new JsonException("The count rule needs an array of parameters.");

            return new CountRule(parameters[0], parameters.Skip(1).ToArray());
        }

        public override void Write(Utf8JsonWriter writer, CountRule value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
