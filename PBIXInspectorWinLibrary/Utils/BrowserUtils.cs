using System.Diagnostics;

namespace PBIXInspectorWinLibrary.Utils
{
    public class BrowserUtils
    {
        public static void BrowseToPage(string url)
        {
            string request = url;
            ProcessStartInfo ps = new ProcessStartInfo
            {
                FileName = request,
                UseShellExecute = true
            };
            
            Process.Start(ps);
        }

        public static Process[] GetProcess(string name)
        {

            Process[] localByName = Process.GetProcessesByName(name);
            return localByName;
        }
    }
}
