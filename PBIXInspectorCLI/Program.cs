using PBIXInspectorLibrary;
using PBIXInspectorLibrary.Output;
using PBIXInspectorLibrary.Utils;
using System.Reflection;
using System.Text.Json;

internal partial class Program
{
    private static void Main(string[] args)
    {
        const string SamplePBIXFilePath = @"Files\Inventory sample.pbix";
        const string SamplePBIPFilePath = @"Files\pbip\Inventory sample.pbip";
        const string SampleRulesFilePath = @"Files\Base rules.json";
        const bool Verbose = true;

        Inspector? insp = null;

        Welcome();

#if DEBUG
        Console.WriteLine("Attach debugger to process? Press any key to continue.");
        Console.ReadLine();
#endif

        try
        {
            var parsedArgs = CLIArgsUtils.ParseArgs(args);

            insp = RunInspector(parsedArgs.PBIFilePath, parsedArgs.RulesFilePath, parsedArgs.Verbose, parsedArgs.OutputPath);
        }
        catch (ArgumentNullException)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nRunning with sample files for demo purposes.\nSample .pbix file is at \"{0}\".\nSample inspection rules json file is at \"{1}\".\n", Path.Combine(AppContext.BaseDirectory, SamplePBIXFilePath), Path.Combine(AppContext.BaseDirectory, SampleRulesFilePath));
            Console.ResetColor();

            //TODO: use appSettings property to switch file mode i.e. PBIX vs. PBIP. 
            //insp = RunInspector(SamplePBIXFilePath, SampleRulesFilePath, Verbose);
            insp = RunInspector(SamplePBIPFilePath, SampleRulesFilePath, Verbose);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            if (insp != null)
            {
                insp.MessageIssued -= Insp_MessageIssued;
            }
            Console.ResetColor();
            Console.WriteLine("\nPress any key to quit application.");
            Console.ReadLine();
        }
    }

    private static void Welcome()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("PBIX Inspector v{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
        Console.ResetColor();
    }

    private static Inspector RunInspector(string PBIFilePath, string RulesFilePath, bool Verbose, string OutputPath = "")
    {
        Inspector insp = new Inspector(PBIFilePath, RulesFilePath);
        insp.MessageIssued += Insp_MessageIssued;

        try
        {
            var testResults = insp.Inspect();

            if (!Verbose) {
                Console.WriteLine("Verbose param is set to false so only listing test failures.");
                testResults = from result in testResults where !result.Pass select result;
            }
           
            foreach (var result in testResults)
            {
                Console.ForegroundColor = result.Pass ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine(result.Message);
            }

            Console.ResetColor ();

            //Write results to file?
            if (!string.IsNullOrEmpty(OutputPath))
            {
                FileAttributes attr = File.GetAttributes(OutputPath);

                //TODO: referencing testResults var currently causes another test run through yield statements. 
                insp.MessageIssued -= Insp_MessageIssued;
                var testRun = new TestRun() {  CompletionTime = DateTime.Now, TestedFilePath = PBIFilePath, RulesFilePath = RulesFilePath, Verbose = Verbose, Results = testResults};

                string outputFilePath = OutputPath;
                //detect whether path is a directory
                if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
                {
                    outputFilePath = Path.Combine(OutputPath, string.Concat("TestRun-", testRun.Id.ToString(), ".json"));
                }
                else
                {
                    if (!File.Exists(outputFilePath)) { throw new FileNotFoundException(outputFilePath); }
                }

                string jsonTestRun = JsonSerializer.Serialize(testRun);
                
                File.WriteAllText(outputFilePath, jsonTestRun, System.Text.Encoding.UTF8);
            }

            //TODO: Explore https://json2html.com for html output.
        }
        catch (PBIXInspectorException e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            insp.MessageIssued -= Insp_MessageIssued;
            Console.ResetColor();
        }

        return insp;
    }

    private static void Insp_MessageIssued(object? sender, MessageIssuedEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine();
        Console.WriteLine("{0}: {1}", e.MessageType.ToString(), e.Message);
    }
}