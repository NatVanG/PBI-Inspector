using System.Drawing;
using System.Text.Json.Nodes;
using PBIXInspectorLibrary;
using PBIXInspectorLibrary.Output;

namespace PBIXInspectorWinLibrary.Drawing
{
    public class ImageUtils
    {
        public static string ConvertBitmapToBase64(string bitmapPath)
        {
            using (Image image = Image.FromFile(bitmapPath))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }

        public static void DrawReportPages(IEnumerable<TestResult> fieldMapResults, IEnumerable<TestResult> testResults, string outputDir)
        {
            foreach (TestResult testResult in testResults.Where(_ => !string.IsNullOrEmpty(_.ParentName)))
            {
                var testResultId = testResult.Id;

                foreach (TestResult fields in fieldMapResults.Where(_ => _.ParentName.Equals(testResult.ParentName)))
                {
                    var pageName = fields.ParentName;
                    var pageDisplayName = fields.ParentDisplayName;
                    //TODO: page size is currently hardcoded to 1280x720 (i.e. 16x9 aspect ratio). 
                    var pageSize = new ReportPage.PageSize { Height = 720, Width = 1280 };
                    List<PBIXInspectorWinLibrary.Drawing.ReportPage.VisualContainer> visuals = new List<ReportPage.VisualContainer>();
                    foreach (var f in fields.Actual.AsArray())
                    {
                        var name = f["name"].ToString();
                        var visualType = f["visualType"].ToString();
                        var x = (int)Math.Round(f["x"].GetValue<decimal>());
                        var y = (int)Math.Round(f["y"].GetValue<decimal>());
                        var height = (int)Math.Round(f["height"].GetValue<decimal>());
                        var width = (int)Math.Round(f["width"].GetValue<decimal>());
                        var visible = f["visible"].GetValue<bool>();

                        var pass = !(testResult.Actual != null && testResult.Actual is JsonArray
                                                                && testResult.Actual.AsArray().Any(_ => _ != null
                                                                    && _ is JsonValue && _.AsValue().ToString().Equals(name)));

                        visuals.Add(new ReportPage.VisualContainer { Name = name.ToString(), VisualType = visualType.ToString(), X = x, Y = y, Height = height, Width = width, Pass = pass, Visible = visible });
                    }

                    var rp = new ReportPage(pageName, pageDisplayName, pageSize, visuals);
                    rp.Draw();
                    var filename = string.Concat(testResultId, ".png");
                    var filepath = Path.Combine(outputDir, filename);
                    rp.Save(filepath);
                }
            }
        }
    }
}
