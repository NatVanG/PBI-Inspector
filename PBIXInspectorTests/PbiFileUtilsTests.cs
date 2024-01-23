#pragma warning disable CS8602 

using PBIXInspectorLibrary;

namespace PBIXInspectorTests
{
    public class PbiFileUtilsTests
    {
        [Test]
        public void PbiFileUtilsThrows()
        {
            PbiFile pbiFile = null;
            string pbiFilePath = null;
            PBIXInspectorException ex = Assert.Throws<PBIXInspectorException>(
            () => pbiFile = PbiFileUtils.InitPbiFile(pbiFilePath));
        }

        [Test]
        public void PbiFileUtilsThrows2()
        {
            PbiFile pbiFile = null;
            string pbiFilePath = string.Empty;
            PBIXInspectorException ex = Assert.Throws<PBIXInspectorException>(
            () => pbiFile = PbiFileUtils.InitPbiFile(pbiFilePath));
        }

        [Test]
        public void PbiFileUtilsThrows3()
        {
            PbiFile pbiFile = null;
            string pbiFilePath = "myreport.xxxx";
            PBIXInspectorException ex = Assert.Throws<PBIXInspectorException>(
            () => pbiFile = PbiFileUtils.InitPbiFile(pbiFilePath));
        }
    }
}
