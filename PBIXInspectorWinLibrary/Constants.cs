
namespace PBIXInspectorWinLibrary
{
    public static class Constants
    {
        public const string SamplePBIXFilePath = @"Files\Inventory sample.pbix";
        public const string SamplePBIPReportFilePath = @"Files\pbip\Inventory sample\Inventory sample.Report";
        public const string SampleRulesFilePath = @"Files\Base rules.json";
        public const string ReportPageFieldMapFilePath = @"Files\ReportPageFieldMap.json";
        public const string TestRunHTMLTemplate = @"Files\html\TestRunTemplate.html";
        public const string PBIInspectorPNG = @"Files\icon\pbiinspector.png";
        public const string TestRunHTMLFileName = "TestRun.html";
        public const string VersionPlaceholder = "%VERSION%";
        public const string JsonPlaceholder = "%JSON%";
        public const string LogoPlaceholder = "%LOGO%";
        public const string Base64ImgPrefix = @"data:image/png;base64,";
        public const string DefaultVisOpsFolder = "VisOps";
        public const string PNGOutputDir = "PBIInspectorPNG";
        public const string ADOMsgTemplate = "##vso[task.logissue type={0};]";
        public const string GitHubMsgTemplate = "::{0}::";

        public const string ReadmePageUrl = "https://github.com/NatVanG/PBIXInspector/blob/main/README.md";
        public const string LicensePageUrl = "https://github.com/NatVanG/PBIXInspector/blob/main/LICENSE";
    }
}
