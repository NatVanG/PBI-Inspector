using System.Text.Json.Nodes;

namespace PBIXInspectorLibrary.Output
{
    public class TestResult
    {
        public Guid Id { get; private set; }

        public string RuleName { get; set; }

        public string? ParentName { get; set; }

        public string? ParentDisplayName { get; set; }

        public bool Pass { get; set; }

        public JsonNode? Expected { get; set; }

        public JsonNode? Actual { get; set; }

        public string Message { get; set; }

        public TestResult()
        {
            Id = System.Guid.NewGuid();
        }
    }
}