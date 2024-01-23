
namespace PBIXInspectorWinLibrary.Utils
{
    public class ArgsUtils
    {
        public static Args ParseArgs(string[] args)
        {
            const string PBIX = "-pbix", PBIP = "-pbip", PBIPREPORT = "-pbipreport", RULES = "-rules", OUTPUT = "-output", FORMATS = "-formats", VERBOSE = "-verbose";
            const string TRUE = "true";
            const string FALSE = "false";
            string[] validOptions = { PBIX, PBIP, PBIPREPORT, RULES, OUTPUT, FORMATS, VERBOSE };

            int index = 0;
            int maxindex = args.Length - 2;
            var dic = new Dictionary<string, string>();
            while (index <= maxindex)
            {
                if (args[index].StartsWith("-") && validOptions.Contains(args[index], StringComparer.OrdinalIgnoreCase))
                {
                    var argName = args[index].ToLower();
                    var argValue = args[index + 1];
                    dic.Add(argName.ToLower(), argValue.ToLower());
                    index += 2;
                }
                else
                {
                    throw new ArgumentException(string.Format("Invalid command option: \"{0}\".", args[index]));
                }
            }

            if (dic.ContainsKey(PBIP)) { throw new ArgumentException(string.Format("-pbip argument is deprecated, please use -pbipreport instead.")); }

            if (!dic.ContainsKey(PBIPREPORT) && !dic.ContainsKey(PBIX)) { throw new ArgumentNullException("-pbipreport or -pbix must be defined."); }

            if (!dic.ContainsKey(RULES)) { throw new ArgumentNullException("-rules must be defined"); }

            var pbiFilePath = dic.ContainsKey(PBIPREPORT) ? dic[PBIPREPORT] : dic[PBIX];
            pbiFilePath = ResolvePbiFilePathInput(pbiFilePath);
            var rulesPath = dic[RULES];
            var outputPath = dic.ContainsKey(OUTPUT) ? dic[OUTPUT] : string.Empty;
            var verboseString = dic.ContainsKey(VERBOSE) ? dic[VERBOSE] : FALSE;
            var formatsString = dic.ContainsKey(FORMATS) ? dic[FORMATS] : string.Empty;

            return new Args { PBIFilePath = pbiFilePath, RulesFilePath = rulesPath, OutputPath = outputPath, FormatsString = formatsString, VerboseString = verboseString };
        }

        public static string? ResolvePbiFilePathInput(string pbiFilePath)
        {
            var resolvedPath = pbiFilePath;

            if (!string.IsNullOrEmpty(pbiFilePath) && (pbiFilePath.ToLower().EndsWith(Constants.PBIPReportJsonFileName)))
            {
                resolvedPath = Path.GetDirectoryName(pbiFilePath);
            }

            //TODO: support PBIP file path. Need to parse the json and retrieve the report folder path.
            if (!string.IsNullOrEmpty(pbiFilePath) && (pbiFilePath.ToLower().EndsWith(Constants.PBIPFileExtension) 
                                                    || pbiFilePath.ToLower().EndsWith(Constants.PBIRFileExtension)))
            {
                throw new ArgumentException(string.Format("PBIP and PBIR file types are not yet supported. Please specify a report.json file path instead."));
            }
            
            return resolvedPath;
        }
    }
}