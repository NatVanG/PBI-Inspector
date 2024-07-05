
namespace PBIXInspectorWinLibrary
{
    public static class Constants
    {
        public const string SamplePBIPReportFolderPath = @"Files\pbip\Inventory sample - fails.Report";
        public const string SamplePBIPReportFilePath = @"Files\pbip\Inventory sample - fails.pbip";
        public const string SampleRulesFilePath = @"Files\Base-rulesV2.json";
        public const string ReportPageFieldMapFilePath = @"Files\ReportPageFieldMapV2.json";
        public const string PBIPReportJsonFileName = "report.json";
        public const string PBIPFileExtension = ".pbip";
        public const string PBIRFileExtension = ".pbir";
        public const string TestRunHTMLTemplate = @"Files\html\TestRunTemplate.html";
        public const string PBIInspectorPNG = @"Files\icon\pbiinspector.png";
        public const string TestRunHTMLFileName = "TestRun.html";
        public const string VersionPlaceholder = "%VERSION%";
        public const string JsonPlaceholder = "%JSON%";
        public const string LogoPlaceholder = "%LOGO%";
        public const string Base64ImgPrefix = @"data:image/png;base64,";
        public const string DefaultVisOpsFolder = "VisOps";
        public const string PNGOutputDir = "PBIInspectorPNG";
        public const string ADOLogIssueTemplate = "##vso[task.logissue type={0};]"; //warning|error
        public const string ADOCompleteTemplate = "##vso[task.complete result={0};]DONE";//Failed|SucceededWithIssues|Succeeded
        public const string GitHubMsgTemplate = "::{0}::";
        public const string ReadmePageUrl = "https://github.com/NatVanG/PBI-Inspector/blob/part-concept/README.md";
        public const string LatestReleasePageUrl = "https://github.com/NatVanG/PBI-Inspector/releases";
        public const string LicensePageUrl = "https://github.com/NatVanG/PBI-Inspector/blob/part-concept/LICENSE";
        public const string IssuesPageUrl = "https://github.com/NatVanG/PBI-Inspector/issues";
    }
}