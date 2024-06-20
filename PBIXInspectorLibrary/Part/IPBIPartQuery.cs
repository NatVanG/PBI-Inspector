using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace PBIXInspectorLibrary.Part
{
    public interface IPBIPartQuery
    {
        public abstract Part RootPart { get; set; }

        public abstract object? Invoke(string query, Part context);

        public abstract string PartName(Part context);

        public abstract string PartDisplayName(Part context);
        
        public abstract Part Report(Part context);

        public abstract Part? ReportExtensions(Part context);

        public abstract Part Version(Part context);

        public abstract Part PagesHeader(Part context);

        public abstract List<Part> Pages(Part context);

        public abstract List<Part> AllVisuals(Part context);

        public abstract List<Part> Visuals(Part context);

        public abstract List<Part> AllMobileVisuals(Part context);

        public abstract List<Part> MobileVisuals(Part context);

        public abstract Part BookmarksHeader(Part context);

        public abstract List<Part> AllBookmarks(Part context);

        public abstract List<Part> Bookmarks(Part context);

        public abstract Part? UniquePart(string query, Part context);

        public abstract JsonNode? ToJsonNode(Part context);

        public abstract JsonNode? ToJsonNode(object? value);

        public JsonArray ToJsonArray(List<Part> parts);
    }
}
