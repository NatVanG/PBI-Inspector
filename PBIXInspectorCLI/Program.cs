using PBIXInspectorLibrary;
using PBIXInspectorLibrary.Output;
using PBIXInspectorWinLibrary;
using PBIXInspectorWinLibrary.Drawing;
using PBIXInspectorWinLibrary.Utils;

internal partial class Program
{
    private static CLIArgs _parsedArgs = null;

    private static void Main(string[] args)
    { 
#if DEBUG
        Console.WriteLine("Attach debugger to process? Press any key to continue.");
        Console.ReadLine();
#endif

        try
        {
            _parsedArgs = CLIArgsUtils.ParseArgs(args);

            Welcome();
            
            PBIXInspectorWinLibrary.Main.WinMessageIssued += Main_MessageIssued;
            PBIXInspectorWinLibrary.Main.Run(_parsedArgs);

            Console.ResetColor();
            Console.WriteLine("\nPress any key to quit application.");
            Console.ReadLine();
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
        }
        finally
        {
            PBIXInspectorWinLibrary.Main.WinMessageIssued -= Main_MessageIssued;
            PBIXInspectorWinLibrary.Main.CleanUp();
        }
    }

    private static void Main_MessageIssued(object? sender, PBIXInspectorLibrary.MessageIssuedEventArgs e)
    {
        if (e.MessageType == MessageTypeEnum.Dialog)
        {
            if (_parsedArgs.ADOOutput)
            {
                //Running in non-interactive mode on Azure DevOps.
                e.DialogOKResponse = true;
            }
            else
            {
                Console.WriteLine(string.Concat(e.Message, " Y/N"));
                var a = Console.ReadLine();
                e.DialogOKResponse = !string.IsNullOrEmpty(a) && a.Equals("Y", StringComparison.OrdinalIgnoreCase);
            }
        }
        else
        {
            if (!_parsedArgs.ADOOutput || (_parsedArgs.ADOOutput && (e.MessageType == MessageTypeEnum.Error || e.MessageType == MessageTypeEnum.Warning)))
            {
                Console.WriteLine(FormatConsoleMessage(e.MessageType, e.Message));
            }
        }
    }

    private static String FormatConsoleMessage(MessageTypeEnum messageType, string message)
    {
        string template = _parsedArgs.ADOOutput ? Constants.ADOMsgTemplate : "{0}";
        string msgType = _parsedArgs.ADOOutput ? messageType.ToString().ToLower() : messageType.ToString();
        string msgSeparator = _parsedArgs.ADOOutput ? "" : ": ";
        string messageTypeFormat = string.Format(template,msgType);

        return string.Concat(messageTypeFormat, msgSeparator, message);
    }

    private static void Welcome()
    {
        if (!_parsedArgs.CONSOLEOutput) return;
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(AppUtils.About());
        Console.ResetColor();
    }
}