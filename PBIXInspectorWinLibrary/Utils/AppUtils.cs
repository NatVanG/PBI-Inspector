using PBIXInspectorLibrary;
using System.Diagnostics;

namespace PBIXInspectorWinLibrary.Utils
{
    public class AppUtils
    {
        public static void WinOpen(string url)
        {
            string request = url;
            ProcessStartInfo ps = new()
            {
                FileName = request,
                UseShellExecute = true
            };

            try
            {
                Process.Start(ps);
            }
            catch
            {
                throw new PBIXInspectorException(string.Format("Could not launch browser or Windows Exployer for location \"{0}\".", url));
            }
        }
    }
}
