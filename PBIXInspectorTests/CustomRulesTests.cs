using System.Text.Json.Nodes;
using PBIXInspectorLibrary.CustomRules;

namespace PBIXInspectorTests
{
    [TestFixture]
    public class CustomRulesTests
    {
#pragma warning disable CS8602
        [Test]
        public void IsNullOrEmptyRuleTest()
        {
            var json = "";
            var rule = new IsNullOrEmptyRule(json);
            var result = rule.Apply(null);
            Assert.That((bool)result.AsValue(), Is.True);
        }

        [Test]
        public void IsNullOrEmptyRuleTest2()
        {
            string json = null;
            var rule = new IsNullOrEmptyRule(json);
            var result = rule.Apply(null);
            Assert.That((bool)result.AsValue(), Is.True);
        }

        [Test]
        public void IsNullOrEmptyRuleTest3()
        {
            string json = "Hello";
            var rule = new IsNullOrEmptyRule(json);
            var result = rule.Apply(null);
            Assert.That((bool)result.AsValue(), Is.False);
        }

        [Test]
        public void ToStringTest()
        {
            var json = "{\"message\":\"Hello, world!\"}";

            var rule = new ToString(JsonNode.Parse(json));
            var result = rule.Apply(null);
            Assert.That(result?.ToString(), Is.EqualTo(json));
        }

        [Test]
        public void CountRuleTest()
        {
            var json = "[\"a\",\"b\",\"c\"]";

            var rule = new CountRule(JsonNode.Parse(json));
            var result = rule.Apply(null);
            JsonAssert.AreEquivalent(result, 3);
        }

        [Test]
        public void StringContainsMatchTest()
        {
            var searchString = "Hello, world! More text";
            var patternString = "^[a-zA-Z]+, world!";

            var rule = new StringContains(searchString, patternString);
            var result = rule.Apply(null);
            Assert.That((int)result.AsValue(), Is.GreaterThan(0));
        }

        [Test]
        public void StringContainsNoMatchTest()
        {
            var searchString = "Hello, world! More text";
            var patternString = "^[a-zA-Z]+, world!$";

            var rule = new StringContains(searchString, patternString);
            var result = rule.Apply(null);
            Assert.That((int)result.AsValue(), Is.EqualTo(0));
        }
#pragma warning restore CS8602
    }
}
