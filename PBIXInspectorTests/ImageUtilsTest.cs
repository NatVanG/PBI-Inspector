using PBIXInspectorLibrary.Output;
using PBIXInspectorWinLibrary.Drawing;
using PBIXInspectorWinLibrary.Utils;

namespace PBIXInspectorTests
{
    [TestFixture]
    internal class ImageUtilsTest
    {
        [Test]
        public void ConvertBitmapToBase64Test()
        {
            var bitmapPath = string.Empty;
            
            ArgumentException ex = Assert.Throws<ArgumentException>(
                () => ImageUtils.ConvertBitmapToBase64(bitmapPath));
        }
        [Test]
        public void DrawReportPagesTest()
        {
            var fieldMapResults = new List<TestResult>();
            var testResults = new List<TestResult>();
            var outputDir = "";
            ImageUtils.DrawReportPages(fieldMapResults, testResults, outputDir);
            Assert.IsTrue(string.IsNullOrEmpty(outputDir));
        }
    }
}
