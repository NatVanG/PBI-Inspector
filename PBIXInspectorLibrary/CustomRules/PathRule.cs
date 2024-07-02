using Json.Logic;
using Json.More;
using Json.Path;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PBIXInspectorLibrary.CustomRules
{
    /// <summary>
    /// Handles the `path` operation.
    /// </summary>
    [Operator("path")]
    [JsonConverter(typeof(PathRuleJsonConverter))]
    public class PathRule : Json.Logic.Rule
    {
        internal Json.Logic.Rule? Path { get; }
        internal Json.Logic.Rule? DefaultValue { get; }

        internal PathRule()
        {
        }
        internal PathRule(Json.Logic.Rule path)
        {
            Path = path;
        }

        //internal PathRule(Rule path, Rule defaultValue)
        //{
        //    Path = path;
        //    DefaultValue = defaultValue;
        //}

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
            if (Path == null) return data;

            var path = Path.Apply(data, contextData);
            var pathString = path.Stringify()!;
            if (pathString == string.Empty) return contextData ?? data;

            var jpath = JsonPath.Parse(pathString);
         
            var result = jpath.Evaluate(contextData ?? data); 

            return ToJsonArray(result.Matches);

            //return DefaultValue?.Apply(data, contextData) ?? null;
        }

        private JsonArray ToJsonArray(NodeList? nodes)
        {
            if (nodes == null || nodes.Count == 0) return new JsonArray();
            return new JsonArray(nodes.Select(_ => _.Value.Copy()).ToArray());
        }
    }

    internal class PathRuleJsonConverter : JsonConverter<PathRule>
    {
        public override PathRule? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var node = JsonSerializer.Deserialize<JsonNode?>(ref reader, options);

            var parameters = node is JsonArray
                ? node.Deserialize<Json.Logic.Rule[]>()
                : new[] { node.Deserialize<Json.Logic.Rule>()! };

            if (parameters is not ({ Length: 1 }))
                throw new JsonException("The path rule needs an array with 1 parameter.");

            return new PathRule(parameters[0]);
        }

        public override void Write(Utf8JsonWriter writer, PathRule value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WritePropertyName("path");
            if (value.DefaultValue != null)
            {
                writer.WriteStartArray();
                writer.WriteRule(value.Path, options);
                writer.WriteRule(value.DefaultValue, options);
                writer.WriteEndArray();
            }
            else
                writer.WriteRule(value.Path, options);

            writer.WriteEndObject();
        }
    }
}
