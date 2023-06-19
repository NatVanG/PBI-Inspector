namespace PBIXInspectorLibrary.Utils
{
    public class CLIArgs
    {
        public string PBIFilePath { get; set; }

        public string RulesFilePath { get; set; }

        public string VerboseString { get; set; }

        public bool Verbose { get
            {
                var verbose = false;
                _ = bool.TryParse(VerboseString, out verbose);
                return verbose;
            }
        }
    }
}