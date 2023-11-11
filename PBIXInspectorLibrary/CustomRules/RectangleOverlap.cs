using Json.Logic;
using System.Drawing;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PBIXInspectorLibrary.CustomRules
{

    /// <summary>
    /// Handles the `rectoverlap` operation.
    /// </summary>
    [Operator("rectoverlap")]
    [JsonConverter(typeof(RectOverlapJsonConverter))]
    public class RectOverlapRule : Json.Logic.Rule
    {
        internal Json.Logic.Rule Input { get; }
        internal Json.Logic.Rule? Margin { get; }

        internal RectOverlapRule(Json.Logic.Rule input, Json.Logic.Rule? margin)
        {
            Input = input;
            Margin = margin;
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
            JsonArray result = new JsonArray();
            var input = Input.Apply(data, contextData);
            var margin = Margin != null ? Margin.Apply(data, contextData).Numberify() : 0;

            if (input is null) return result;
            if (input is not JsonArray arr)
                throw new JsonLogicException($"The RectOverlapRule expects JsonArray as input.");

            var namedRects = new List<NamedRectangle>();
            foreach (var item in arr)
            {
                var name = item["name"].Stringify();
                var x = item["x"].Numberify();
                var y = item["y"].Numberify();
                var width = item["width"].Numberify();
                var height = item["height"].Numberify();

                var rect = new Rectangle((int)x, (int)y, (int)width, (int)height);
                rect.Inflate((int)margin, (int)margin);
                namedRects.Add(new NamedRectangle(name, rect));
            }

            var overlaps = CheckRectangleOverlaps(namedRects);

            foreach (var r in overlaps)
            {
                result.Add(r.Name);
            }

            return result;
        }

        private List<NamedRectangle> CheckRectangleOverlaps(List<NamedRectangle> namedRects)
        {
            var result = new List<NamedRectangle>();

            foreach (var namedRect in namedRects)
            {
                foreach (var otherRect in namedRects)
                {
                    if (namedRect.Name == otherRect.Name) continue;

                    if (namedRect.Rectangle.IntersectsWith(otherRect.Rectangle))
                    {
                        if (!result.Contains(namedRect)) result.Add(namedRect);
                        if (!result.Contains(otherRect)) result.Add(otherRect);
                    }
                }
            }

            return result;
        }
    }


    internal class RectOverlapJsonConverter : JsonConverter<RectOverlapRule>
    {
        public override RectOverlapRule? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var node = JsonSerializer.Deserialize<JsonNode?>(ref reader, options);

            var parameters = node is JsonArray
                ? node.Deserialize<Json.Logic.Rule[]>()
                : new[] { node.Deserialize<Json.Logic.Rule>()! };

            if (parameters == null || parameters.Length == 0)
                throw new JsonException("The count rule needs an array of at least one parameter.");

            return new RectOverlapRule(parameters[0], parameters.Length == 2 ? parameters[1] : null);
        }

        public override void Write(Utf8JsonWriter writer, RectOverlapRule value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }

    internal class NamedRectangle
    {
        internal NamedRectangle(string name, Rectangle rect)
        {
            this.Name = name;
            this.Rectangle = rect;
        }

        public string Name { get; set; }
        public Rectangle Rectangle { get; set; }
    }
}