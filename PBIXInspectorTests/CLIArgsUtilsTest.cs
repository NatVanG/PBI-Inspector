using PBIXInspectorWinLibrary.Utils;

#pragma warning disable CS8602 
namespace PBIXInspectorTests
{
    public class CLIArgsUtilsTest
    {
        [Test]
        public void TestCLIArgsUtilsSuccess()
        {
            string[] args = "-pbix pbixPath -rules rulesPath -verbose true".Split(" ");
            var parsedArgs = ArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbixPath", StringComparison.OrdinalIgnoreCase) && parsedArgs.RulesFilePath.Equals("rulesPath", StringComparison.OrdinalIgnoreCase) && parsedArgs.Verbose);
        }

        [Test]
        public void TestCLIArgsUtilsSwappedParamsSuccess()
        {
            string[] args = "-rules rulesPath -pbix pbixPath -verbose true".Split(" ");
            var parsedArgs = ArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbixPath", StringComparison.OrdinalIgnoreCase) && parsedArgs.RulesFilePath.Equals("rulesPath", StringComparison.OrdinalIgnoreCase) && parsedArgs.Verbose);
        }

        [Test]
        public void TestCLIArgsUtilsSuccess_PBIPOption()
        {
            string[] args = "-pbipreport pbipPath -rules rulesPath -verbose true".Split(" ");
            var parsedArgs = ArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbipPath", StringComparison.OrdinalIgnoreCase) && parsedArgs.RulesFilePath.Equals("rulesPath", StringComparison.OrdinalIgnoreCase) && parsedArgs.Verbose);
        }

        [Test]
        public void TestCLIArgsUtilsSuccess_VerboseOptionMissing()
        {
            string[] args = "-pbipreport pbipPath -rules rulesPath".Split(" ");
            var parsedArgs = ArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbipPath", StringComparison.OrdinalIgnoreCase) && parsedArgs.RulesFilePath.Equals("rulesPath", StringComparison.OrdinalIgnoreCase) && !parsedArgs.Verbose);
        }

        [Test]
        public void TestCLIArgsUtilsSuccess_VerboseOptionFalse()
        {
            string[] args = "-pbipreport pbipPath -rules rulesPath -verbose false".Split(" ");
            var parsedArgs = ArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbipPath", StringComparison.OrdinalIgnoreCase) && parsedArgs.RulesFilePath.Equals("rulesPath", StringComparison.OrdinalIgnoreCase) && !parsedArgs.Verbose);
        }

        [Test]
        public void TestCLIArgsUtilsSuccess_VerboseOptionUnparseable()
        {
            string[] args = "-pbipreport pbipPath -rules rulesPath -verbose XYZ".Split(" ");
            var parsedArgs = ArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbipPath", StringComparison.OrdinalIgnoreCase) && parsedArgs.RulesFilePath.Equals("rulesPath", StringComparison.OrdinalIgnoreCase) && !parsedArgs.Verbose);
        }

        [Test]
        public void TestCLIArgsUtilsSuccess_FavourPBIPReportOption()
        {
            string[] args = "-pbix pbixPath -pbipreport pbipPath -rules rulesPath -verbose true".Split(" ");
            var parsedArgs = ArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbipPath", StringComparison.OrdinalIgnoreCase) && parsedArgs.RulesFilePath.Equals("rulesPath", StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void TestCLIArgsUtilsPBIX()
        {
            string[] args = "-pbix pbixPath".Split(" ");
            Args? parsedArgs = null;

            parsedArgs = ArgsUtils.ParseArgs(args);

            Assert.IsTrue(parsedArgs.PBIFilePath.Equals("pbixPath", StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void TestCLIArgsUtilsRules()
        {
            string[] args = "-pbipreport path -rules rulesPath".Split(" ");
            Args? parsedArgs = null;

            parsedArgs = ArgsUtils.ParseArgs(args);

            Assert.IsTrue(parsedArgs.RulesFilePath.Equals("rulesPath", StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void TestCLIArgsUtilsFormats()
        {
            string[] args = "-pbipreport path -formats CONSOLE,HTML,PNG,JSON".Split(" ");
            Args? parsedArgs = null;

            parsedArgs = ArgsUtils.ParseArgs(args);

            Assert.IsTrue(parsedArgs.CONSOLEOutput && parsedArgs.HTMLOutput && parsedArgs.PNGOutput && parsedArgs.JSONOutput);
        }

        [Test]
        public void TestCLIArgsUtilsDefaults()
        {
            string[] args = "-pbipreport path".Split(" ");
            Args? parsedArgs = null;

            parsedArgs = ArgsUtils.ParseArgs(args);

            Assert.IsTrue(parsedArgs.CONSOLEOutput 
                && !parsedArgs.Verbose 
                && parsedArgs.DeleteOutputDirOnExit 
                && !string.IsNullOrEmpty(parsedArgs.OutputDirPath)
                && !string.IsNullOrEmpty(parsedArgs.RulesFilePath)
                && !parsedArgs.HTMLOutput 
                && !parsedArgs.JSONOutput 
                && !parsedArgs.PNGOutput);
        }

        [Test]
        public void TestCLIArgsUtilsThrows2()
        {
            string[] args = "-rules rulesPath".Split(" ");
            Args? parsedArgs = null;

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
            () => parsedArgs = ArgsUtils.ParseArgs(args));
        }

        [Test]
        public void TestCLIArgsUtilsThrows3()
        {
            string[] args = "-other other".Split(" ");
            Args? parsedArgs = null;

            ArgumentException ex = Assert.Throws<ArgumentException>(
            () => parsedArgs = ArgsUtils.ParseArgs(args));
        }

        [Test]
        public void TestCLIArgsUtilsThrows4()
        {
            string[] args = "-rules -pbix pbixPath -other".Split(" ");
            Args? parsedArgs = null;

            ArgumentException ex = Assert.Throws<ArgumentException>(
            () => parsedArgs = ArgsUtils.ParseArgs(args));
        }

        [Test]
        public void TestCLIArgsUtilsThrows5()
        {
            string[] args = "-rules rulesPath -pbix pbixPath -other stuff".Split(" ");
            Args? parsedArgs = null;

            ArgumentException ex = Assert.Throws<ArgumentException>(
            () => parsedArgs = ArgsUtils.ParseArgs(args));
        }

        [Test]
        public void TestCLIArgsUtilsThrows6()
        {
            string[] args = "-rules rulesPath -pbip pbipPath".Split(" ");
            Args? parsedArgs = null;

            ArgumentException ex = Assert.Throws<ArgumentException>(
            () => parsedArgs = ArgsUtils.ParseArgs(args));
        }

        [Test]
        public void TestCLIArgsUtilsResolvePbiFilePathInput1()
        {
            string inputPath = @"C:\TEMP\VisOps\Sales - custom colours.Report";
            string resolvedPath = ArgsUtils.ResolvePbiFilePathInput(inputPath);

            Assert.IsTrue(resolvedPath == inputPath);
        }

        [Test]
        public void TestCLIArgsUtilsResolvePbiFilePathInput2()
        {
            string inputPath = @"C:\TEMP\VisOps\Sales - custom colours.Report\report.json";
            string expectedPath = @"C:\TEMP\VisOps\Sales - custom colours.Report";
            string resolvedPath = ArgsUtils.ResolvePbiFilePathInput(inputPath);

            Assert.IsTrue(resolvedPath == expectedPath);
        }

    }
}
#pragma warning restore CS8602 