using PBIXInspectorLibrary;
using PBIXInspectorLibrary.Output;
using PBIXInspectorWinLibrary;
using PBIXInspectorWinLibrary.Drawing;
using PBIXInspectorWinLibrary.Utils;
using System.Reflection;
using System.Text.Json;

internal partial class Program
{
    private static void Main(string[] args)
    {
        const string PNGOutputDir = "PBIInspectorPNG";
        Inspector? _insp = null;
        IEnumerable<TestResult> _testResults = null;
        string _jsonTestRun = string.Empty;

        Inspector? _fieldMapInsp = null;
        IEnumerable<TestResult> _fieldMapResults = null;

        CLIArgs _parsedArgs = null;

        Welcome();

#if DEBUG
        Console.WriteLine("Attach debugger to process? Press any key to continue.");
        Console.ReadLine();
#endif

        try
        {
            _parsedArgs = CLIArgsUtils.ParseArgs(args);
            _insp = new Inspector(_parsedArgs.PBIFilePath, _parsedArgs.RulesFilePath);
            _insp.MessageIssued += Insp_MessageIssued;
            _testResults = _insp.Inspect().Where(_ => (!_parsedArgs.Verbose && !_.Pass) || (_parsedArgs.Verbose));
            

            if (_parsedArgs.CONSOLEOutput)
            {
                foreach (var result in _testResults)
                {
                    Console.ForegroundColor = result.Pass ? ConsoleColor.Green : ConsoleColor.Red;
                    Console.WriteLine(result.Message);
                }

                Console.ResetColor();
            }

            //Ensure output dir exists
            if (_parsedArgs.JSONOutput || _parsedArgs.HTMLOutput || _parsedArgs.PNGOutput)
            {
                if (!Directory.Exists(_parsedArgs.OutputDirPath))
                {
                    Directory.CreateDirectory(_parsedArgs.OutputDirPath);
                }
            }

            if (_parsedArgs.JSONOutput || _parsedArgs.HTMLOutput)
            {
                var outputFilePath = string.Empty;
                var pbiFileNameWOextension = Path.GetFileNameWithoutExtension(_parsedArgs.PBIFilePath);

                if (!string.IsNullOrEmpty(_parsedArgs.OutputDirPath))
                {
                   outputFilePath = Path.Combine(_parsedArgs.OutputDirPath, string.Concat("TestRun_", pbiFileNameWOextension , ".json"));
                }
                else
                {
                    throw new ArgumentException("Directory with path \"{0}\" does not exist",  _parsedArgs.OutputDirPath);
                }

                var testRun = new TestRun() { CompletionTime = DateTime.Now, TestedFilePath = _parsedArgs.PBIFilePath, RulesFilePath = _parsedArgs.RulesFilePath, Verbose = _parsedArgs.Verbose, Results = _testResults };
                _jsonTestRun = JsonSerializer.Serialize(testRun);
                if (_parsedArgs.JSONOutput)
                {
                    Console.WriteLine("Writing JSON to file.");
                    File.WriteAllText(outputFilePath, _jsonTestRun, System.Text.Encoding.UTF8);
                }
            }

            if (_parsedArgs.PNGOutput || _parsedArgs.HTMLOutput)
            {
                _fieldMapInsp = new Inspector(_parsedArgs.PBIFilePath, Constants.ReportPageFieldMapFilePath);
                _fieldMapResults = _fieldMapInsp.Inspect();
                var outputPNGDirPath = Path.Combine(_parsedArgs.OutputDirPath, PNGOutputDir);

                Console.WriteLine("Writing report page wireframe images to files.");
                if (Directory.Exists(outputPNGDirPath)) Directory.Delete(outputPNGDirPath, true);
                Directory.CreateDirectory(outputPNGDirPath);
                ImageUtils.DrawReportPages(_fieldMapResults, _testResults, outputPNGDirPath);
            }

            if (_parsedArgs.HTMLOutput)
            {
                string pbiinspectorlogobase64 = string.Concat(Constants.Base64ImgPrefix, ImageUtils.ConvertBitmapToBase64(Constants.PBIInspectorPNG));
                //string nowireframebase64 = string.Concat(Base64ImgPrefix, ImageUtils.ConvertBitmapToBase64(@"Files\png\nowireframe.png"));
                string template = File.ReadAllText(Constants.TestRunHTMLTemplate);
                string html = template.Replace(Constants.LogoPlaceholder, pbiinspectorlogobase64);
                html = html.Replace(Constants.VersionPlaceholder, About());
                html = html.Replace(Constants.JsonPlaceholder, _jsonTestRun);

                var outputHTMLFilePath = Path.Combine(_parsedArgs.OutputDirPath, Constants.TestRunHTMLFileName);

                Console.WriteLine("Writing HTML output to file.");
                File.WriteAllText(outputHTMLFilePath, html);

                //Results have been written to a temporary directory so show output to user automatically.
                if (_parsedArgs.DeleteOutputDirOnExit)
                {
                    AppUtils.WinOpen(outputHTMLFilePath);
                }
            }

            Console.ResetColor();
            Console.WriteLine("\nPress any key to quit application.");
            Console.ReadLine();

            //TODO: add VSTS option like Tabular Editor BPA to communicate errors or warnings to the Azure DevOps step, see https://learn.microsoft.com/en-us/azure/devops/pipelines/scripts/logging-commands?view=azure-devops&tabs=bash#formatting-commands
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            if (_insp != null)
            {
                _insp.MessageIssued -= Insp_MessageIssued;
            }

            if (_parsedArgs.DeleteOutputDirOnExit)
            {
                if (Directory.Exists(_parsedArgs.OutputDirPath)) Directory.Delete(_parsedArgs.OutputDirPath, true);
            }
        }
    }

    private static void Welcome()
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine("PBIX Inspector v{0}", Assembly.GetExecutingAssembly().GetName().Version.ToString());
        Console.ResetColor();
    }

    private static void Insp_MessageIssued(object? sender, MessageIssuedEventArgs e)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine();
        Console.WriteLine("{0}: {1}", e.MessageType.ToString(), e.Message);
    }

    public static string About()
    {
        var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        var about = string.Format("VisOps with PBI Inspector v{0}", version);
        return about;
    }
}