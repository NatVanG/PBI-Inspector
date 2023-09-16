using PBIXInspectorLibrary.Output;
using PBIXInspectorLibrary;
using PBIXInspectorWinLibrary.Drawing;
using PBIXInspectorWinLibrary.Utils;
using System.Text.Json;

namespace PBIXInspectorWinLibrary
{
    public class Main
    {
        public static event EventHandler<MessageIssuedEventArgs>? WinMessageIssued;
        private static Inspector? _insp = null;
        private static CLIArgs? _args = null;
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


        public static void Run(CLIArgs args)
        {
            _args = args;

            IEnumerable<TestResult> _testResults = null;
            string _jsonTestRun = string.Empty;

            Inspector? _fieldMapInsp = null;
            IEnumerable<TestResult> _fieldMapResults = null;

            OnMessageIssued(MessageTypeEnum.Information, string.Concat("Test run started at (UTC): ", DateTime.Now.ToUniversalTime()));

            try
            {
                _insp = new Inspector(_args.PBIFilePath, _args.RulesFilePath);
                _insp.MessageIssued += Insp_MessageIssued;

                _testResults = _insp.Inspect().Where(_ => (!_args.Verbose && !_.Pass) || (_args.Verbose));

                if (_args.CONSOLEOutput || _args.ADOOutput)
                {
                    foreach (var result in _testResults)
                    {
                        //TODO: use Test log type json property instead
                        var msgType = result.Pass ? MessageTypeEnum.Information : result.LogType;
                        OnMessageIssued(msgType, result.Message);
                    }
                }

                //Ensure output dir exists
                if (!_args.ADOOutput && (_args.JSONOutput || _args.HTMLOutput || _args.PNGOutput))
                {
                    if (!Directory.Exists(_args.OutputDirPath))
                    {
                        Directory.CreateDirectory(_args.OutputDirPath);
                    }
                }

                if (!_args.ADOOutput && (_args.JSONOutput || _args.HTMLOutput))
                {
                    var outputFilePath = string.Empty;
                    var pbiFileNameWOextension = Path.GetFileNameWithoutExtension(_args.PBIFilePath);

                    if (!string.IsNullOrEmpty(_args.OutputDirPath))
                    {
                        outputFilePath = Path.Combine(_args.OutputDirPath, string.Concat("TestRun_", pbiFileNameWOextension, ".json"));
                    }
                    else
                    {
                        throw new ArgumentException("Directory with path \"{0}\" does not exist", _args.OutputDirPath);
                    }

                    var testRun = new TestRun() { CompletionTime = DateTime.Now, TestedFilePath = _args.PBIFilePath, RulesFilePath = _args.RulesFilePath, Verbose = _args.Verbose, Results = _testResults };
                    _jsonTestRun = JsonSerializer.Serialize(testRun);
                    if (_args.JSONOutput)
                    {
                        OnMessageIssued(MessageTypeEnum.Information, "Writing JSON to file.");
                        File.WriteAllText(outputFilePath, _jsonTestRun, System.Text.Encoding.UTF8);
                    }
                }

                if (!_args.ADOOutput && (_args.PNGOutput || _args.HTMLOutput))
                {
                    _fieldMapInsp = new Inspector(_args.PBIFilePath, Constants.ReportPageFieldMapFilePath);
                     _fieldMapResults = _fieldMapInsp.Inspect();
 
                    var outputPNGDirPath = Path.Combine(_args.OutputDirPath, Constants.PNGOutputDir);

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

                if (!_args.ADOOutput && _args.HTMLOutput)
                {
                    string pbiinspectorlogobase64 = string.Concat(Constants.Base64ImgPrefix, ImageUtils.ConvertBitmapToBase64(Constants.PBIInspectorPNG));
                    //string nowireframebase64 = string.Concat(Base64ImgPrefix, ImageUtils.ConvertBitmapToBase64(@"Files\png\nowireframe.png"));
                    string template = File.ReadAllText(Constants.TestRunHTMLTemplate);
                    string html = template.Replace(Constants.LogoPlaceholder, pbiinspectorlogobase64, StringComparison.OrdinalIgnoreCase);
                    html = html.Replace(Constants.VersionPlaceholder, AppUtils.About(), StringComparison.OrdinalIgnoreCase);
                    html = html.Replace(Constants.JsonPlaceholder, _jsonTestRun, StringComparison.OrdinalIgnoreCase);

                    var outputHTMLFilePath = Path.Combine(_args.OutputDirPath, Constants.TestRunHTMLFileName);

                    OnMessageIssued(MessageTypeEnum.Information, string.Format("Writing HTML output to file at \"{0}\".", outputHTMLFilePath));
                    File.WriteAllText(outputHTMLFilePath, html);

                    //Results have been written to a temporary directory so show output to user automatically.
                    if (_args.DeleteOutputDirOnExit)
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

            if (_args != null && _args.DeleteOutputDirOnExit && Directory.Exists(_args.OutputDirPath))
            {
                Directory.Delete(_args.OutputDirPath, true);
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
            if (e.MessageType == MessageTypeEnum.Error) ErrorCount++;
            if (e.MessageType == MessageTypeEnum.Warning) WarningCount++;

            EventHandler<MessageIssuedEventArgs>? handler = WinMessageIssued;
            if (handler != null)
            {
                handler(null, e);
            }
        }
    }
}
