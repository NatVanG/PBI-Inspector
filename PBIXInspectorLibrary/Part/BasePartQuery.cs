using Json.Pointer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PBIXInspectorLibrary.Part
{
    public class BasePartQuery
    {
        private const string UNIQUEPARTMETHODNAME = "UniquePart";

        public BasePartQuery(string filePath)
        {

        }

        public Part RootPart { get; set; }

        public object? Invoke(string query, Part context)
        {
            object? result = null;
            var type = this.GetType();
            System.Reflection.MethodInfo? mi = type.GetMethod(query);
            if (mi != null)
            {
                result = mi.Invoke(this, new object?[] { context });
            }
            else
            {
                mi = type.GetMethod(UNIQUEPARTMETHODNAME);
                if (mi != null)
                {
                    result = mi.Invoke(this, new object?[] { query, context });
                }
            }

            return result;
        }

        public object? Invoke(string query)
        {
            return Invoke(query, ContextService.GetInstance().Part);
        }

        public static Part Parent(Part part)
        {
            return part.Parent;
        }

        public static Part TopParent(Part part)
        {
            if (part.Parent == null) return part;
            return TopParent(part.Parent);
        }

        protected static string TryGetJsonNodeStringValue(JsonNode node, string query)
        {
            JsonPointer pt = JsonPointer.Parse(query);

            if (pt.TryEvaluate(node, out var result))
            {
                if (result is JsonValue val)
                {
                    return val.ToString();
                }
            }

            return null;
        }
    }
}
