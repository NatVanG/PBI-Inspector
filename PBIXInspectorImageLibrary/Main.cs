using PBIXInspectorImageLibrary.Drawing;
using PBIXInspectorImageLibrary.Utils;
using PBIXInspectorLibrary;
using PBIXInspectorLibrary.Output;
using System.Text.Json;

namespace PBIXInspectorImageLibrary
{
    public class Main
    {
        public static event EventHandler<MessageIssuedEventArgs>? WinMessageIssued;
        private static Inspector? _insp = null;
        private static Args? _args = null;
        private static int _errorCount = 0;
        private static int _warningCount = 0;


        public static int ErrorCount
        {
            get
            {
                return _errorCount;
            }
            private set
            {
                _errorCount = value;
            }
        }

        public static int WarningCount
        {
            get
            {
                return _warningCount;
            }
            private set
            {
                _warningCount = value;
            }
        }

        public static void Run(string pbiFilePath, string rulesFilePath, string outputPath, bool verbose, bool jsonOutput, bool htmlOutput)
        {
            var formatsString = string.Concat(jsonOutput ? "JSON" : string.Empty, ",", htmlOutput ? "HTML" : string.Empty);
            var verboseString = verbose.ToString();

            string resolvedPbiFilePath = string.Empty;

            try
            {
                resolvedPbiFilePath = ArgsUtils.ResolvePbiFilePathInput(pbiFilePath);
            }
            catch (ArgumentException e)
            {
                OnMessageIssued(MessageTypeEnum.Error, e.Message);
            }

            var args = new Args { PBIFilePath = resolvedPbiFilePath, RulesFilePath = rulesFilePath, OutputPath = outputPath, FormatsString = formatsString, VerboseString = verboseString };

            Run(args);
        }

        public static void Run(Args args)
        {
            _args = args;

            IEnumerable<TestResult> _testResults = null;
            string _jsonTestRun = string.Empty;

            Inspector? _fieldMapInsp = null;
            IEnumerable<TestResult> _fieldMapResults = null;

            OnMessageIssued(MessageTypeEnum.Information, string.Concat("Test run started at (UTC): ", DateTime.Now.ToUniversalTime()));

            try
            {
                _insp = new Inspector(Main._args.PBIFilePath, Main._args.RulesFilePath);

                _insp.MessageIssued += Insp_MessageIssued;

                var inspectResult = _insp.Inspect();
                _testResults = inspectResult.Where(_ => !Main._args.Verbose && _?.Pass == false || Main._args.Verbose);


                if (Main._args.CONSOLEOutput || Main._args.ADOOutput)
                {
                    foreach (var result in _testResults)
                    {
                        //TODO: use Test log type json property instead
                        var msgType = result.Pass ? MessageTypeEnum.Information : result.LogType;
                        OnMessageIssued(msgType, result.Message);
                    }
                }

                //Ensure output dir exists
                if (!Main._args.ADOOutput && (Main._args.JSONOutput || Main._args.HTMLOutput || Main._args.PNGOutput))
                {
                    if (!Directory.Exists(Main._args.OutputDirPath))
                    {
                        Directory.CreateDirectory(Main._args.OutputDirPath);
                    }
                }

                if (!Main._args.ADOOutput && (Main._args.JSONOutput || Main._args.HTMLOutput))
                {
                    var outputFilePath = string.Empty;
                    var pbiFileNameWOextension = Path.GetFileNameWithoutExtension(Main._args.PBIFilePath);

                    if (!string.IsNullOrEmpty(Main._args.OutputDirPath))
                    {
                        outputFilePath = Path.Combine(Main._args.OutputDirPath, string.Concat("TestRun_", pbiFileNameWOextension, ".json"));
                    }
                    else
                    {
                        throw new ArgumentException("Directory with path \"{0}\" does not exist", Main._args.OutputDirPath);
                    }

                    var testRun = new TestRun() { CompletionTime = DateTime.Now, TestedFilePath = Main._args.PBIFilePath, RulesFilePath = Main._args.RulesFilePath, Verbose = Main._args.Verbose, Results = _testResults };
                    _jsonTestRun = JsonSerializer.Serialize(testRun);
                    if (Main._args.JSONOutput)
                    {
                        OnMessageIssued(MessageTypeEnum.Information, string.Format("Writing JSON output to file at \"{0}\".", outputFilePath));
                        File.WriteAllText(outputFilePath, _jsonTestRun, System.Text.Encoding.UTF8);
                    }
                }

                if (!Main._args.ADOOutput && (Main._args.PNGOutput || Main._args.HTMLOutput))
                {
                    _fieldMapInsp = new Inspector(Main._args.PBIFilePath, Constants.ReportPageFieldMapFilePath);
                    _fieldMapResults = _fieldMapInsp.Inspect();

                    var outputPNGDirPath = Path.Combine(Main._args.OutputDirPath, Constants.PNGOutputDir);

                    if (Directory.Exists(outputPNGDirPath))
                    {
                        var eventArgs = RaiseWinMessage(MessageTypeEnum.Dialog, string.Format("Delete all existing directory content at \"{0}\"?", outputPNGDirPath));
                        if (eventArgs.DialogOKResponse)
                        {
                            Directory.Delete(outputPNGDirPath, true);
                        }
                    }
                    Directory.CreateDirectory(outputPNGDirPath);
                    OnMessageIssued(MessageTypeEnum.Information, string.Format("Writing report page wireframe images to files at \"{0}\".", outputPNGDirPath));

                    ImageUtils.DrawReportPages(_fieldMapResults, _testResults, outputPNGDirPath);
                }

                if (!Main._args.ADOOutput && Main._args.HTMLOutput)
                {
                    string pbiinspectorlogobase64 = string.Concat(Constants.Base64ImgPrefix, ImageUtils.ConvertBitmapToBase64(Constants.PBIInspectorPNG));

                    string template = File.ReadAllText(Constants.TestRunHTMLTemplate);
                    string html = template.Replace(Constants.LogoPlaceholder, pbiinspectorlogobase64, StringComparison.OrdinalIgnoreCase);
                    html = html.Replace(Constants.VersionPlaceholder, AppUtils.About(), StringComparison.OrdinalIgnoreCase);
                    html = html.Replace(Constants.JsonPlaceholder, _jsonTestRun, StringComparison.OrdinalIgnoreCase);

                    var outputHTMLFilePath = Path.Combine(Main._args.OutputDirPath, Constants.TestRunHTMLFileName);

                    OnMessageIssued(MessageTypeEnum.Information, string.Format("Writing HTML output to file at \"{0}\".", outputHTMLFilePath));
                    File.WriteAllText(outputHTMLFilePath, html);

                    //Results have been written to a temporary directory so show output to user automatically.
                    if (Main._args.DeleteOutputDirOnExit)
                    {
                        AppUtils.WinOpen(outputHTMLFilePath);
                    }
                }
            }
            catch (PBIXInspectorException e)
            {
                OnMessageIssued(MessageTypeEnum.Error, e.Message);
            }
            catch (ArgumentException e)
            {
                OnMessageIssued(MessageTypeEnum.Error, e.Message);
            }
            catch (Exception e)
            {
                OnMessageIssued(MessageTypeEnum.Error, e.Message);
            }
            finally
            {
                OnMessageIssued(MessageTypeEnum.Complete, string.Concat("Test run completed at (UTC): ", DateTime.Now.ToUniversalTime()));
            }
        }

        public static void CleanUp()
        {
            if (_insp != null)
            {
                _insp.MessageIssued -= Insp_MessageIssued;
            }

            if (Main._args != null && Main._args.DeleteOutputDirOnExit && Directory.Exists(Main._args.OutputDirPath))
            {
                Directory.Delete(Main._args.OutputDirPath, true);
            }
        }

        private static void Insp_MessageIssued(object? sender, MessageIssuedEventArgs e)
        {
            MessageIssued(e);
        }

        private static MessageIssuedEventArgs RaiseWinMessage(MessageTypeEnum messageType, string message)
        {
            var args = new MessageIssuedEventArgs(message, messageType);
            WinMessageIssued?.Invoke(null, args);
            return args;
        }

        private static void OnMessageIssued(MessageTypeEnum messageType, string message)
        {
            var e = new MessageIssuedEventArgs(message, messageType);
            MessageIssued(e);
        }

        private static void MessageIssued(MessageIssuedEventArgs e)
        {
            if (Main._args != null && Main._args.ADOOutput)
            {
                if (e.MessageType == MessageTypeEnum.Error) ErrorCount++;
                if (e.MessageType == MessageTypeEnum.Warning) WarningCount++;
            }

            EventHandler<MessageIssuedEventArgs>? handler = WinMessageIssued;
            if (handler != null)
            {
                handler(null, e);
            }
        }
    }
}
