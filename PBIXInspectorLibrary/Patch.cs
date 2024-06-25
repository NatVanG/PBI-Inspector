using Json.Patch;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace PBIXInspectorLibrary;

[JsonConverter(typeof(PatchConverter))]
public class Patch
{
#pragma warning disable CS8618
    public string PartName { get; set; }
    public JsonPatch? Ops { get; set; }
#pragma warning restore CS8618
}

public class PatchConverter : JsonConverter<Patch?>
{
    public override Patch? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var node = JsonSerializer.Deserialize<JsonNode?>(ref reader, options);
        if (node is not JsonArray arr) return null;
        if (arr.Count < 2) throw new InvalidOperationException("ERROR: Patch should be defined as a two member array for part name and ops array");

        var part = arr[0].ToString();//JsonSerializer.Serialize(arr[0], new JsonSerializerOptions { Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });
        var ops = JsonSerializer.Deserialize<JsonPatch>(arr[1]); //TODO: handle serialization exception.

        return new Patch
        {
            PartName = part,
            Ops = ops
        };
    }

    public override void Write(Utf8JsonWriter writer, Patch? value, JsonSerializerOptions options)
    {
        throw new NotImplementedException();
    }
}