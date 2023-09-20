using Json.Logic;
using System.Drawing;
using System.Text.Json.Nodes;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace PBIXInspectorLibrary.CustomRules
{
    internal class RectangleOverlap
    {
        /// <summary>
        /// Handles the `count` operation.
        /// </summary>
        [Operator("rectoverlap")]
        [JsonConverter(typeof(RectOverlapJsonConverter))]
        public class RectOverlapRule : Json.Logic.Rule
        {
            internal Json.Logic.Rule Input { get; }


            internal RectOverlapRule(Json.Logic.Rule input)
            {
                throw new NotImplementedException("The rectoverlap custom rule has not yet been implemented.");
                Input = input;
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
                throw new NotImplementedException("The rectoverlap custom rule has not yet been implemented.");
                JsonArray? result = null;
                var input = Input.Apply(data, contextData);

                if (input is null) return result;
                if (input is not JsonArray arr)
                    throw new JsonLogicException($"Cannot run rectoverlap rule over non-JsonArray object.");


                //run logic here
                //Create input namedrectangle list
                var namedRects = new List<NamedRectangle>();
                var resultNameRects = CheckRectangleOverlaps(namedRects); 
                result = ToJsonArray(resultNameRects);

                return result;
            }

            private List<NamedRectangle> CheckRectangleOverlaps(List<NamedRectangle> namedRects)
            {
                var result = new List<NamedRectangle>();
                
                //foreach (var namedRect in namedRects)
                //{
                //    List<NamedRectangle> namedRectMinusOne = namedRects.CopyTo()
                //}

                //Rectangle rectangle1 = new Rectangle(50, 50, 200, 100);
                //Rectangle rectangle2 = new Rectangle(70, 20, 100, 200);

                //if (rectangle1.IntersectsWith(rectangle2))
                //{

                //}

                return result;
            }

            private JsonArray? ToJsonArray(List<NamedRectangle> namedRects)
            {
                return new JsonArray();
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
                    throw new JsonException("The count rule needs an array of parameters.");

                return new RectOverlapRule(parameters[0]);
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
}
