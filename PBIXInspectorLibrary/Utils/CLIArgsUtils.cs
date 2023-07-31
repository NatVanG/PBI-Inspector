using System.Diagnostics.Metrics;

namespace PBIXInspectorLibrary.Utils
{
    public class CLIArgsUtils
    {
        public static CLIArgs ParseArgs(string[] args)
        {
            const string PBIX = "-pbix", PBIP = "-pbip", RULES = "-rules", OUTPUT = "-output", VERBOSE = "-verbose";
            const string TRUE = "true";
            string[] validOptions = { PBIX, PBIP, RULES, OUTPUT, VERBOSE };
            // Test if required input arguments were supplied.
            if (args == null || !(args.Contains(PBIX) || args.Contains(PBIP)) || !(args.Contains(RULES))) { throw new ArgumentNullException("Missing arguments, ensure required -pbix or -pbip and -rules arguments are provided."); }

            int index = 0;
            int maxindex = args.Length - 2;
            var dic = new Dictionary<string, string>();
            while (index <= maxindex)
            {
                if (args[index].StartsWith("-") && validOptions.Contains(args[index]))
                {
                    dic.Add(args[index], args[index + 1]);
                    index += 2;
                }
                else
                {
                    throw new ArgumentException(string.Format("Invalid command option: \"{0}\".", args[index]));
                }
            }

            var pbiFilePath = dic.ContainsKey(PBIP) ? dic[PBIP] : (dic.ContainsKey(PBIX) ? dic[PBIX] : throw new ArgumentException(string.Format("Arguments must include either \"{0}\" or \"{1}\".", PBIP, PBIX)));
            var rulesPath = dic.ContainsKey(RULES) ? dic[RULES] : throw new ArgumentException(string.Format("Arguments must include \"{0}\"", RULES));
            var outputPath = dic.ContainsKey(OUTPUT) ? dic[OUTPUT] : string.Empty;
            var verboseString = dic.ContainsKey(VERBOSE) ? dic[VERBOSE] : TRUE;

            return new CLIArgs { PBIFilePath = pbiFilePath, RulesFilePath = rulesPath, OutputPath = outputPath, VerboseString = verboseString};
        }

        
    }
}
