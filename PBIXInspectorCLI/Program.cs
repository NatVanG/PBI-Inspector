using PBIXInspectorLibrary;
using PBIXInspectorLibrary.Utils;
using System.Reflection;

internal partial class Program
{
    private static void Main(string[] args)
    {
        const string SamplePBIXFilePath = @"Files\Inventory sample.pbix";
        const string SampleRulesFilePath = @"Files\Inventory rules sample.json";

        Inspector? insp = null;

        Welcome();

        try
        {
            var parsedArgs = CLIArgsUtils.ParseArgs(args);
            insp = RunInspector(parsedArgs.PBIXFilePath, parsedArgs.RulesFilePath);
        }
        catch (ArgumentNullException)
        {
            //Console.WriteLine(e.Message);
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("\nRunning with sample files for demo purposes.\nSample .pbix file is at \"{0}\".\nSample inspection rules json file is at \"{1}\".\n", Path.Combine(AppContext.BaseDirectory, SamplePBIXFilePath), Path.Combine(AppContext.BaseDirectory, SampleRulesFilePath));
            Console.ResetColor();
            insp = RunInspector(SamplePBIXFilePath, SampleRulesFilePath);
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

    private static Inspector RunInspector(string PBIXFilePath, string RulesFilePath)
    {
        Inspector insp = new Inspector(PBIXFilePath, RulesFilePath);
        insp.MessageIssued += Insp_MessageIssued;

        try
        {
            var testResults = insp.Inspect();

            foreach (var result in testResults)
            {
                Console.WriteLine();
                Console.ForegroundColor = result.Result ? ConsoleColor.Green : ConsoleColor.Red;
                Console.WriteLine(result.ResultMessage);
            }
        }
        catch (PBIXInspectorException e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            Console.ResetColor();
        }

        return insp;
    }

    private static void Insp_MessageIssued(object? sender, MessageIssuedEventArgs e)
    {
        Console.WriteLine("{0}: {1}", e.MessageType.ToString(), e.Message);
    }
}