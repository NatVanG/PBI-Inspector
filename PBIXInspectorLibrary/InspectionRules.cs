namespace PBIXInspectorLibrary
{
    /// <summary>
    /// Deserialises inspection rules from json 
    /// </summary>
    public class InspectionRules : IInspectionRules
    {
        public List<PbixEntry> PbixEntries { get; set; }
    }

    public class PbixEntry
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public string Path { get; set; }

        //TODO: use string enum converter deserialiser
        public string ContentType { get; set; }

        public int CodePage { get; set; }

        public IEnumerable<Rule> Rules { get; set; }
    }

    public class Rule
    {
        public string Name { get; set; }

        public string Path { get; set; }

        public Test Test { get; set; }

        public bool PathErrorWhenNoMatch { get; set; }
    }
}
