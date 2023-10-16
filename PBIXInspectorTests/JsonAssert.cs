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

using System.Text.Json.Nodes;
using Json.More;

namespace PBIXInspectorTests
{
    public static class JsonAssert
    {
        public static void AreEquivalent(JsonNode? expected, JsonNode? actual)
        {
            if (!expected.IsEquivalentTo(actual))
                Assert.Fail($"Expected: {expected.AsJsonString()}\nActual: {actual.AsJsonString()}");
        }

        public static void IsNull(JsonNode? actual)
        {
            if (actual != null)
                Assert.Fail($"Expected: null\nActual: {actual.AsJsonString()}");
        }

        public static void IsTrue(JsonNode? actual)
        {
            if (actual is not JsonValue value || !value.TryGetValue(out bool b) || !b)
                Assert.Fail($"Expected: true\nActual: {actual.AsJsonString()}");
        }

        public static void IsFalse(JsonNode? actual)
        {
            if (actual is not JsonValue value || !value.TryGetValue(out bool b) || b)
                Assert.Fail($"Expected: true\nActual: {actual.AsJsonString()}");
        }
    }
}
