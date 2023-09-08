namespace PBIXInspectorLibrary
{
    /// <summary>
    /// Deserialises inspection rules from json 
    /// </summary>
    public class InspectionRules : IInspectionRules
    {
        public List<PbiEntry> PbiEntries { get; set; }
    }

    public class PbiEntry
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string PbixEntryPath { get; set; }

        public string PbipEntryPath { get; set; }

        //TODO: use string enum converter deserialiser
        public string ContentType { get; set; }

        public int CodePage { get; set; }

        public IEnumerable<Rule> Rules { get; set; }

        public IEnumerable<Rule> EnabledRules
        {
            get
            {
                return Rules.Where(_ => !_.Disabled);
            }
        }
    }

    public class Rule
    {
        public string Name { get; set; }

        public bool Disabled { get; set; }

        public string ForEachPath { get; set; }

        public string ForEachPathName { get; set; }

        public string ForEachPathDisplayName { get; set; }

        public string Path { get; set; }

        public Test Test { get; set; }

        public bool PathErrorWhenNoMatch { get; set; }

        public static implicit operator Rule(Json.Logic.Rule v)
        {
            throw new NotImplementedException();
        }
    }
}
