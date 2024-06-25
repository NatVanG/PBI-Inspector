using System.Runtime;

namespace PBIXInspectorLibrary
{
    /// <summary>
    /// Deserialises inspection rules from json 
    /// </summary>
    public class InspectionRules : IInspectionRules
    {
        public List<Rule> Rules { get; set; }
    }

    public class Rule
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public bool Disabled { get; set; }

        public string LogType { get; set; }

        public string Part { get; set; }

        public Test Test { get; set; }

        public bool ApplyPatch { get; set; }

        public Patch? Patch { get; set; }

        public bool PathErrorWhenNoMatch { get; set; }
    }
}
