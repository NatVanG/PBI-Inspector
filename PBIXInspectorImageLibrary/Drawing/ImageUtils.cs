using PBIXInspectorLibrary.Output;
using SkiaSharp;
using System.Text.Json.Nodes;

namespace PBIXInspectorImageLibrary.Drawing
{
    public class ImageUtils
    {

        public static string ConvertBitmapToBase64(string bitmapPath)
        {
            bitmapPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, bitmapPath);
            using var bitmap = SKBitmap.Decode(bitmapPath);
            var skData = bitmap.Encode(SKEncodedImageFormat.Png, 100);

            using MemoryStream m = new();

            skData.SaveTo(m);

            byte[] imageBytes = m.ToArray();

            // Convert byte[] to Base64 String
            string base64String = Convert.ToBase64String(imageBytes);
            return base64String;
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
                    List<ReportPage.VisualContainer> visuals = new List<ReportPage.VisualContainer>();
                    foreach (var f in fields.Actual.AsArray())
                    {
                        var name = f["name"].ToString();
                        var visualType = f["visualType"].ToString();
                        var x = (int)Math.Round(f["x"].GetValue<decimal>());
                        var y = (int)Math.Round(f["y"].GetValue<decimal>());
                        var height = (int)Math.Round(f["height"].GetValue<decimal>());
                        var width = (int)Math.Round(f["width"].GetValue<decimal>());
                        var visible = f["visible"].GetValue<bool>();

                        //If a visual name is returned in the test actual array then highlight it as a test failure in the page wireframe
                        //A visual name can be returned either as a JsonValue or a named JsonObject (i.e. {"name": "VisualName"}) hence the "or else" operator below (i.e. "||")
                        var visualNameInTestActualArray = (testResult.Actual != null && testResult.Actual is JsonArray
                                                                && testResult.Actual.AsArray().Any(_ => _ != null
                                                                    && _ is JsonValue && _.AsValue().ToString().Equals(name)))
                                                          || (testResult.Actual != null && testResult.Actual is JsonArray
                                                        && testResult.Actual.AsArray().Any(_ => _ != null && _ is JsonObject && _ is not JsonValue
                                                            && _["name"] != null && _["name"] is JsonValue && _["name"].AsValue().ToString().Equals(name)));


                        bool visualPass = !visualNameInTestActualArray;
                        visuals.Add(new ReportPage.VisualContainer { Name = name.ToString(), VisualType = visualType.ToString(), X = x, Y = y, Height = height, Width = width, Pass = visualPass, Visible = visible });

                    }
                    using var rp = new ReportPage(pageName, pageDisplayName, pageSize, visuals);
                    rp.Draw();
                    var filename = string.Concat(testResultId, ".png");
                    var filepath = Path.Combine(outputDir, filename);
                    rp.Save(filepath);
                }
            }
        }
    }
}
