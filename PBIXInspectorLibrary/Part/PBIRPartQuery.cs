using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Text.Json.Nodes;
using System.ComponentModel;
using PBIXInspectorLibrary.Part;
using System.Reflection;
using System.IO;
using System.Runtime.CompilerServices;
using Json.More;

namespace PBIXInspectorLibrary.Part
{
    internal class PBIRPartQuery : BasePartQuery, IPBIPartQuery
    {
        private const string NAMEPOINTER = "/name";
        private const string DISPLAYNAMEPOINTER = "/displayName";

        //TODO: harden logic to extract path value here.
        private const string REPORTFOLDERPOINTER = "/artifacts/0/report/path";
        private const string PBIPEXT = ".pbip";
        private const string PBIXEXT = ".pbix";

        public PBIRPartQuery(string path) : base(path)
        {
            if (path == null || path.Length == 0) throw new ArgumentNullException(nameof(path));
            if (!File.Exists(path) && path.EndsWith(PBIPEXT)) throw new ArgumentException($"PBI Desktop file {path} does not exist");
            if (path.ToLower().EndsWith(PBIXEXT)) throw new ArgumentException($"PBIX files are not currently supported, please specify a PBIP");
            if (File.Exists(path) && !path.ToLower().EndsWith(PBIPEXT)) throw new ArgumentException($"PBI Desktop file {path} must have .pbip extension");
            if (!File.Exists(path) && !Directory.Exists(path)) throw new ArgumentException($"{path} does not exist");

            string? reportFolderPath = null;

            if (File.Exists(path) && path.EndsWith(PBIPEXT))
            {
                var pbip = new Part("pbip", path);
                reportFolderPath = ReportPath(pbip);
            }
            else if (Directory.Exists(path))
            {
                reportFolderPath = path;
            }

            this.RootPart = new Part("root", reportFolderPath!);
            SetParts(this.RootPart);
        }

        //TODO: add support for pbir or folder.
        public string ReportPath(Part context)
        {
            var node = ToJsonNode(context);
            var val = TryGetJsonNodeStringValue(node, REPORTFOLDERPOINTER);

            val = Path.Combine(Path.GetDirectoryName(context.Path), val);

            return val;
        }

        public string PartName(Part context)
        {
            var node = ToJsonNode(context);
            var val = TryGetJsonNodeStringValue(node, NAMEPOINTER);

            return val;
        }

        public string PartDisplayName(Part context)
        {
            var node = ToJsonNode(context);
            var val = TryGetJsonNodeStringValue(node, DISPLAYNAMEPOINTER);

            return val;
        }

        public Part Report(Part context)
        {
            IEnumerable<Part> q = from p in Part.Flatten(TopParent(context))
                                          where PartType(p) == PartTypeEnum.File && p.Name.EndsWith("report.json")
                                          select p;

            return q.Single();
        }


        public Part ReportExtensions(Part context)
        {
            IEnumerable<Part> q = from p in Part.Flatten(TopParent(context))
                                  where PartType(p) == PartTypeEnum.File && p.Name.EndsWith("reportExtensions.json")
                                  select p;

            return q.Single();
        }

        public Part Version(Part context)
        {
            IEnumerable<Part> q = from p in Part.Flatten(TopParent(context))
                                          where PartType(p) == PartTypeEnum.File && p.Name.EndsWith("version.json")
                                          select p;

            return q.Single();
        }

        public Part PagesHeader(Part context)
        {
            IEnumerable<Part> q = from p in Part.Flatten(TopParent(context))
                                          where PartType(p) == PartTypeEnum.File && p.Name.EndsWith("pages.json")
                                          select p;

            return q.Single();
        }

        public List<Part> Pages(Part context)
        {
            IEnumerable<Part> q = from p in Part.Flatten(PartType(context) == PartTypeEnum.File ? context.Parent : context)
                                          where PartType(p) == PartTypeEnum.File && p.Name.EndsWith("page.json")
                                          select p;

            return q.ToList();
        }

        public List<Part> AllVisuals(Part context)
        {
            return Visuals(TopParent(context));
        }

        public List<Part> Visuals(Part context)
        {
            IEnumerable<Part> q = from p in Part.Flatten(PartType(context) == PartTypeEnum.File ? context.Parent : context)
                                            where PartType(p) == PartTypeEnum.File && p.Name.EndsWith("visual.json")
                                            select p;

            return q.ToList();
        }

        public List<Part> AllMobileVisuals(Part context)
        {
            return MobileVisuals(TopParent(context));
        }

        public List<Part> MobileVisuals(Part context)
        {
            IEnumerable<Part> q = from p in Part.Flatten(PartType(context) == PartTypeEnum.File ? context.Parent : context)
                                            where PartType(p) == PartTypeEnum.File && p.Name.EndsWith("mobile.json")
                                            select p;

            return q.ToList();
        }

        public Part BookmarksHeader(Part context)
        {
            IEnumerable<Part> q = from p in Part.Flatten(TopParent(context))
                                            where PartType(p) == PartTypeEnum.File && p.Name.EndsWith("bookmarks.json")
                                            select p;

            return q.Single();
        }


        public List<Part> AllBookmarks(Part context)
        {
            return Bookmarks(TopParent(context));
        }

        public List<Part> Bookmarks(Part context)
        {
            IEnumerable<Part> q = from p in Part.Flatten(PartType(context) == PartTypeEnum.File ? context.Parent : context)
                                            where PartType(p) == PartTypeEnum.File && p.Name.EndsWith("bookmark.json")
                                            select p;

            return q.ToList();
        }

        public Part UniquePart(string query, Part context)
        {
            IEnumerable<Part> q = from p in Part.Flatten(TopParent(context))
                                            where p.Name.EndsWith(query, StringComparison.InvariantCultureIgnoreCase)
                                            select p;

            return q.Single();
        }

        public JsonNode ToJsonNode(object? value)
        {
            if (value == null) return null;

            if (value is List<Part>)
            {
                return ToJsonArray(value as List<Part>);
            }
            else
            {
                return PartType((Part)value) == PartTypeEnum.File ? ToJsonNode((Part)value) : null;
            }
        }

        public JsonArray ToJsonArray(List<Part> parts)
        {
            if (parts == null || parts.Count == 0) return new JsonArray();
            return new JsonArray(parts.Select(_ => ToJsonNode(_).Copy()).ToArray());
        }

        //returns null if the file does not exist or is not a json file or if it's a directory
        public JsonNode? ToJsonNode(Part context)
        {
            if (context == null) return null;
            if (context.Content != null) return context.Content;

            JsonNode? node = null;

            if (File.Exists(context.Path))
            {
                try
                {
                    node = JsonNode.Parse(File.ReadAllText(context.Path));
                    context.Content = node;
                }
                catch (System.Text.Json.JsonException ex)
                {
                    throw new Exception($"Error parsing {context.Path}", ex);
                }
            }

            return node;
        }

        private static PartTypeEnum PartType(Part context)
        {
            if (File.Exists(context.Path))
            {
                return PartTypeEnum.File;
            }
            else if (Directory.Exists(context.Path))
            {
                return PartTypeEnum.Folder;
            }
            else
            {
                throw new Exception($"Path {context.Path} does not exist");
            }
        }

        private void SetParts(Part context)
        {
            if (!Directory.Exists(context.Path)) return;

            context.Parts = new List<Part>();

            foreach (string filePath in Directory.GetFiles(context.Path))
            {
                FileInfo fileInfo = new FileInfo(filePath);
                Part filePart = new Part(fileInfo.Name, fileInfo.FullName, context);
                context.Parts.Add(filePart);
            }

            foreach (string dirPath in Directory.GetDirectories(context.Path))
            {
                DirectoryInfo dirInfo = new DirectoryInfo(dirPath);
                Part dirPart = new Part(dirInfo.Name, dirInfo.FullName, context);
                context.Parts.Add(dirPart);
                SetParts(dirPart);
            }
        }
    }
}