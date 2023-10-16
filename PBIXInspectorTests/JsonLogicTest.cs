//MIT License

//Copyright (c) 2022 Greg Dennis

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PBIXInspectorTests
{

    [JsonConverter(typeof(TestConverter))]
    public class JsonLogicTest
    {
#pragma warning disable CS8618
        public string Logic { get; set; }
        public JsonNode? Data { get; set; }
        public JsonNode? Expected { get; set; }
#pragma warning restore CS8618
    }

    public class TestConverter : JsonConverter<JsonLogicTest?>
    {
        public override JsonLogicTest? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var node = JsonSerializer.Deserialize<JsonNode?>(ref reader, options);
            if (node is not JsonArray arr) return null;

            var logic = JsonSerializer.Serialize(arr[0], new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
            var data = arr[1];
            var expected = arr[2];

            return new JsonLogicTest
            {
                Logic = logic,
                Data = data,
                Expected = expected
            };
        }

        public override void Write(Utf8JsonWriter writer, JsonLogicTest? value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
