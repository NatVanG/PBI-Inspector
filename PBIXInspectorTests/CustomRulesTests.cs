using Json.More;
using PBIXInspectorLibrary.CustomRules;
using System.Text.Json.Nodes;

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

        [Test]
        public void SetIntersectionTest1()
        {
            var arr1 = "[\"a\",\"b\"]";
            var arr2 = "[\"b\",\"c\"]";

            var expected = "[\"b\"]";

            var rule = new SetIntersectionRule(JsonNode.Parse(arr1), JsonNode.Parse(arr2));
            var result = rule.Apply(null);
            Assert.That(result.IsEquivalentTo(JsonNode.Parse(expected)));
        }

        [Test]
        public void SetIntersectionTest2()
        {
            var arr1 = "[\"a\",\"b\",\"c\"]";
            var arr2 = "[\"b\",\"c\",\"d\"]";

            var expected = "[\"b\", \"c\"]";

            var rule = new SetIntersectionRule(JsonNode.Parse(arr1), JsonNode.Parse(arr2));
            var result = rule.Apply(null);
            Assert.That(result.IsEquivalentTo(JsonNode.Parse(expected)));
        }

        [Test]
        public void SetIntersectionTest3()
        {
            var arr1 = "[\"a\",\"b\",\"c\"]";
            var arr2 = "[\"d\",\"e\",\"f\"]";

            var expected = "[]";

            var rule = new SetIntersectionRule(JsonNode.Parse(arr1), JsonNode.Parse(arr2));
            var result = rule.Apply(null);
            Assert.That(result.IsEquivalentTo(JsonNode.Parse(expected)));
        }

        [Test]
        public void SetIntersectionTest4()
        {
            var arr1 = "[\"d\",\"e\",\"f\"]";
            var arr2 = "[\"a\",\"b\",\"c\"]";

            var expected = "[]";

            var rule = new SetIntersectionRule(JsonNode.Parse(arr1), JsonNode.Parse(arr2));
            var result = rule.Apply(null);
            Assert.That(result.IsEquivalentTo(JsonNode.Parse(expected)));
        }

        [Test]
        public void SetUnionTest1()
        {
            var arr1 = "[\"a\",\"b\",\"c\"]";
            var arr2 = "[\"d\",\"e\",\"f\"]";

            var expected = "[\"a\",\"b\",\"c\",\"d\",\"e\",\"f\"]";

            var rule = new SetUnionRule(JsonNode.Parse(arr1), JsonNode.Parse(arr2));
            var result = rule.Apply(null);
            Assert.That(result.IsEquivalentTo(JsonNode.Parse(expected)));
        }

        [Test]
        public void SetUnionTest2()
        {
            var arr1 = "[]";
            var arr2 = "[\"d\",\"e\",\"f\"]";

            var expected = "[\"d\",\"e\",\"f\"]";

            var rule = new SetUnionRule(JsonNode.Parse(arr1), JsonNode.Parse(arr2));
            var result = rule.Apply(null);
            Assert.That(result.IsEquivalentTo(JsonNode.Parse(expected)));
        }

        [Test]
        public void SetUnionTest3()
        {
            var arr1 = "[\"a\",\"b\",\"c\"]";
            var arr2 = "[]";

            var expected = "[\"a\",\"b\",\"c\"]";

            var rule = new SetUnionRule(JsonNode.Parse(arr1), JsonNode.Parse(arr2));
            var result = rule.Apply(null);
            Assert.That(result.IsEquivalentTo(JsonNode.Parse(expected)));
        }

        [Test]
        public void SetUnionTest4()
        {
            var arr1 = "[\"a\",\"b\",\"c\",\"a\"]";
            var arr2 = "[]";

            var expected = "[\"a\",\"b\",\"c\"]";

            var rule = new SetUnionRule(JsonNode.Parse(arr1), JsonNode.Parse(arr2));
            var result = rule.Apply(null);
            Assert.That(result.IsEquivalentTo(JsonNode.Parse(expected)));
        }

        [Test]
        public void SetUnionTest5()
        {
            var arr1 = "[\"a\",\"b\",\"c\",\"d\"]";
            var arr2 = "[\"d\",\"e\",\"f\"]";

            var expected = "[\"a\",\"b\",\"c\",\"d\",\"e\",\"f\"]";

            var rule = new SetUnionRule(JsonNode.Parse(arr1), JsonNode.Parse(arr2));
            var result = rule.Apply(null);
            Assert.That(result.IsEquivalentTo(JsonNode.Parse(expected)));
        }

        [Test]
        public void SetDifferenceTest1()
        {
            var arr1 = "[\"a\",\"b\",\"c\"]";
            var arr2 = "[\"c\",\"d\",\"e\"]";

            var expected = "[\"a\",\"b\"]";

            var rule = new SetDifferenceRule(JsonNode.Parse(arr1), JsonNode.Parse(arr2));
            var result = rule.Apply(null);
            Assert.That(result.IsEquivalentTo(JsonNode.Parse(expected)));
        }

        [Test]
        public void SetSymmetricDifferenceTest1()
        {
            var arr1 = "[\"a\",\"b\",\"c\"]";
            var arr2 = "[\"c\",\"d\",\"e\"]";

            var expected = "[\"a\",\"b\",\"d\",\"e\"]";

            var rule = new SetSymmetricDifferenceRule(JsonNode.Parse(arr1), JsonNode.Parse(arr2));
            var result = rule.Apply(null);
            Assert.That(result.IsEquivalentTo(JsonNode.Parse(expected)));
        }

        [Test]
        public void SetEqualTest1()
        {
            var arr1 = "[\"a\",\"b\",\"c\"]";
            var arr2 = "[\"c\",\"d\",\"e\"]";

            var rule = new SetEqualRule(JsonNode.Parse(arr1), JsonNode.Parse(arr2));
            var result = rule.Apply(null);
            Assert.That((bool)result.AsValue(), Is.False);
        }

        [Test]
        public void SetEqualTest2()
        {
            var arr1 = "[\"a\",\"b\",\"c\"]";
            var arr2 = "[\"a\",\"b\",\"c\"]";

            var rule = new SetEqualRule(JsonNode.Parse(arr1), JsonNode.Parse(arr2));
            var result = rule.Apply(null);
            Assert.That((bool)result.AsValue(), Is.True);
        }

        [Test]
        public void SetEqualTest3()
        {
            var arr1 = "[\"a\",\"b\",\"c\"]";
            var arr2 = "[\"c\",\"b\",\"a\"]";

            var rule = new SetEqualRule(JsonNode.Parse(arr1), JsonNode.Parse(arr2));
            var result = rule.Apply(null);
            Assert.That((bool)result.AsValue(), Is.True);
        }
#pragma warning restore CS8602
    }
}
