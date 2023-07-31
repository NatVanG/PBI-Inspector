namespace PBIXInspectorLibrary.Output
{
    public class TestRun
    {
        public Guid Id { get; private set; }
        public string TestedFilePath { get; set; }
        public string RulesFilePath { get; set; }
        public DateTime CompletionTime { get; set; }
        public bool Verbose { get; set; }
        public IEnumerable<TestResult> Results { get; set; }

        public TestRun()
        {
            Id = System.Guid.NewGuid();
        }

    }
}