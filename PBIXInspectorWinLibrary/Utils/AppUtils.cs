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

        public static string About()
        {
            var version = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
            var about = string.Format("VisOps with PBI Inspector v{0}", version);
            return about;
        }

    }
}
