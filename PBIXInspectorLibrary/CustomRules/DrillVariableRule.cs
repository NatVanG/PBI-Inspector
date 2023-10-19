using Json.Logic;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Json.Pointer;
using System.Net.Http.Headers;

namespace PBIXInspectorLibrary.CustomRules;

/// <summary>
/// Handles the `drillvar` operation.
/// </summary>
[Operator("drillvar")]
[JsonConverter(typeof(DrillVariableRuleJsonConverter))]
public class DrillVariableRule : Json.Logic.Rule
{
    internal const char DRILLCHAR = '>';

    internal Json.Logic.Rule? Path { get; }
    internal Json.Logic.Rule? DefaultValue { get; }

    internal DrillVariableRule()
    {
    }
    internal DrillVariableRule(Json.Logic.Rule path)
    {
        Path = path;
    }
    internal DrillVariableRule(Json.Logic.Rule path, Json.Logic.Rule defaultValue)
    {
        Path = path;
        DefaultValue = defaultValue;
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
        if (Path == null) return data;

        var path = Path.Apply(data, contextData);
        var pathString = path.Stringify()!;
        if (pathString == string.Empty) return contextData ?? data;

        return EvalPath(pathString, data, contextData);
    }

    internal JsonNode? EvalPath(string pathString, JsonNode? data, JsonNode? contextData = null)
    {
        if (pathString.Contains(DRILLCHAR))
        {
            var leftString = pathString.Substring(0, pathString.IndexOf(DRILLCHAR));
            var rightString = pathString.Substring(pathString.IndexOf(DRILLCHAR) + 1);

            var pointer = JsonPointer.Parse(leftString == string.Empty ? "" : $"/{leftString.Replace('.', '/')}");
            if (pointer.TryEvaluate(contextData ?? data, out var pathEval) ||
                pointer.TryEvaluate(data, out pathEval))
            {
                if (pathEval is JsonValue val)
                {
                    //remove single quotes from beginning and end of string if any.
                    string strVal;
                    if  (val.ToString()!.StartsWith("'") && val.ToString()!.EndsWith("'"))
                    {
                        strVal = val.ToString()!.Substring(1, val.ToString()!.Length - 2);
                    }
                    else
                    {
                        strVal = val.ToString()!;
                    }

                    var pathEvalNode = JsonNode.Parse(strVal);
                    return EvalPath(rightString, data, pathEvalNode);
                }
                else
                {
                    return EvalPath(rightString, data, pathEval);
                }
            }
        }
        else
        {
            var pointer = JsonPointer.Parse(pathString == string.Empty ? "" : $"/{pathString.Replace('.', '/')}");
            if (pointer.TryEvaluate(contextData ?? data, out var pathEval) ||
                pointer.TryEvaluate(data, out pathEval))
                return pathEval;
        }

        return DefaultValue?.Apply(data, contextData) ?? null;
    }
}

internal class DrillVariableRuleJsonConverter : JsonConverter<DrillVariableRule>
{
    public override DrillVariableRule? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var node = JsonSerializer.Deserialize<JsonNode?>(ref reader, options);

        var parameters = node is JsonArray
            ? node.Deserialize<Json.Logic.Rule[]>()
            : new[] { node.Deserialize<Json.Logic.Rule>()! };

        if (parameters is not ({ Length: 0 } or { Length: 1 } or { Length: 2 }))
            throw new JsonException("The var rule needs an array with 0, 1, or 2 parameters.");

        return parameters.Length switch
        {
            0 => new DrillVariableRule(),
            1 => new DrillVariableRule(parameters[0]),
            _ => new DrillVariableRule(parameters[0], parameters[1])
        };
    }

    public override void Write(Utf8JsonWriter writer, DrillVariableRule value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WritePropertyName("drillvar");
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
