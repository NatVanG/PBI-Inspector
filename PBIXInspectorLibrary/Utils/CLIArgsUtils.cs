namespace PBIXInspectorLibrary.Utils
{
    public class CLIArgsUtils
    {
        //TODO: additional error handling required.
        public static CLIArgs ParseArgs(string[] args)
        {
            const string PBIX = "-pbix", RULES = "-rules", VERBOSE = "-verbose";
            string[] validOptions = { PBIX, RULES, VERBOSE };
            // Test if input arguments were supplied.
            if (args == null || args.Length < 4) { throw new ArgumentNullException("Missing arguments, ensure both -pbix and -rules are provided."); }

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

            return new CLIArgs { PBIXFilePath = dic[PBIX], RulesFilePath = dic[RULES], VerboseString = dic[VERBOSE] };
        }
    }
}
