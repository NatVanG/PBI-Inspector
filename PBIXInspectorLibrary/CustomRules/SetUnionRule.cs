using Json.Logic;
using Json.More;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;


namespace PBIXInspectorLibrary.CustomRules;

/// <summary>
/// Handles the `union` operation.
/// </summary>
[Operator("union")]
[JsonConverter(typeof(SetUnionRuleJsonConverter))]
public class SetUnionRule : Json.Logic.Rule
{
    internal Json.Logic.Rule Set1 { get; }
    internal Json.Logic.Rule Set2 { get; }

    public SetUnionRule(Json.Logic.Rule set1, Json.Logic.Rule set2)
    {
        Set1 = set1;
        Set2 = set2;
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
        var set1 = Set1.Apply(data, contextData);
        var set2 = Set2.Apply(data, contextData);

        if (set1 is not JsonArray || set2 is not JsonArray)
            return new JsonArray();

        var arr1 = (JsonArray)set1;
        var arr2 = (JsonArray)set2;

        var union = new JsonArray();

        foreach (var item in arr1)
        {
            var copy = item.Copy();
            if (!union.Any(x => x.IsEquivalentTo(copy)))
            {
                union.Add(copy);
            }
        }

        foreach (var item in arr2)
        {
            var copy = item.Copy();
            if (!union.Any(x => x.IsEquivalentTo(copy)))
            {
                union.Add(copy);
            }
        }

        return union;
    }
}

internal class SetUnionRuleJsonConverter : JsonConverter<SetUnionRule>
{
    public override SetUnionRule? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var parameters = JsonSerializer.Deserialize<Json.Logic.Rule[]>(ref reader, options);

        if (parameters is not { Length: 2 })
            throw new JsonException("The union rule needs an array with 2 parameters.");

        return new SetUnionRule(parameters[0], parameters[1]);
    }

    public override void Write(Utf8JsonWriter writer, SetUnionRule value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("union");
        writer.WriteStartArray();
        writer.WriteRule(value.Set1, options);
        writer.WriteRule(value.Set2, options);
        writer.WriteEndArray();
        writer.WriteEndObject();
    }
}
