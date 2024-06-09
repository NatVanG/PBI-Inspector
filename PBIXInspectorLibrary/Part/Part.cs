﻿using System.Text.Json.Nodes;
using System.IO;
using System.Reflection.Metadata.Ecma335;
using System.Text.Json;

namespace PBIXInspectorLibrary.Part
{
    public class Part
    {
        public Part Parent { get; private set; }

        public string Name { get; set; }

        public string Path { get; private set; }

        public JsonNode? Content { get; set; }

        public Part(string name, string path, Part parent = null)
        {
            Parent = parent;
            Name = name;
            Path = path;
        }

        public List<Part> Parts { get; set; }

        public static IEnumerable<Part> Flatten(Part part)
        {
            yield return part;

            if (part.Parts != null && part.Parts.Count > 0)
            {
                foreach (var p in part.Parts)
                {
                    foreach (var pp in Flatten(p))
                    {
                        yield return pp;
                    }
                }
            }
        }
    }


}