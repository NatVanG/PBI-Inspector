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
            var parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbixPath") && parsedArgs.RulesFilePath.Equals("rulesPath") && parsedArgs.Verbose);
        }

        [Test]
        public void TestCLIArgsUtilsSwappedParamsSuccess()
        {
            string[] args = "-rules rulesPath -pbix pbixPath -verbose true".Split(" ");
            var parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbixPath") && parsedArgs.RulesFilePath.Equals("rulesPath") && parsedArgs.Verbose);
        }

        [Test]
        public void TestCLIArgsUtilsSuccess_PBIPOption()
        {
            string[] args = "-pbipreport pbipPath -rules rulesPath -verbose true".Split(" ");
            var parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbipPath") && parsedArgs.RulesFilePath.Equals("rulesPath") && parsedArgs.Verbose);
        }

        [Test]
        public void TestCLIArgsUtilsSuccess_VerboseOptionMissing()
        {
            string[] args = "-pbipreport pbipPath -rules rulesPath".Split(" ");
            var parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbipPath") && parsedArgs.RulesFilePath.Equals("rulesPath") && parsedArgs.Verbose);
        }

        [Test]
        public void TestCLIArgsUtilsSuccess_VerboseOptionFalse()
        {
            string[] args = "-pbipreport pbipPath -rules rulesPath -verbose false".Split(" ");
            var parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbipPath") && parsedArgs.RulesFilePath.Equals("rulesPath") && !parsedArgs.Verbose);
        }

        [Test]
        public void TestCLIArgsUtilsSuccess_VerboseOptionUnparseable()
        {
            string[] args = "-pbipreport pbipPath -rules rulesPath -verbose XYZ".Split(" ");
            var parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbipPath") && parsedArgs.RulesFilePath.Equals("rulesPath") && !parsedArgs.Verbose);
        }

        [Test]
        public void TestCLIArgsUtilsSuccess_FavourPBIPReportOption()
        {
            string[] args = "-pbix pbixPath -pbipreport pbipPath -rules rulesPath -verbose true".Split(" ");
            var parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbipPath") && parsedArgs.RulesFilePath.Equals("rulesPath"));
        }

        [Test]
        public void TestCLIArgsUtilsPBIX()
        {
            string[] args = "-pbix pbixPath".Split(" ");
            CLIArgs? parsedArgs = null;

            parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.IsTrue(parsedArgs.PBIFilePath.Equals("pbixPath", StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void TestCLIArgsUtilsRules()
        {
            string[] args = "-rules rulesPath".Split(" ");
            CLIArgs? parsedArgs = null;

            parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.IsTrue(parsedArgs.RulesFilePath.Equals("rulesPath", StringComparison.OrdinalIgnoreCase));
        }

        [Test]
        public void TestCLIArgsUtilsFormats()
        {
            string[] args = "-formats CONSOLE,HTML,PNG,JSON".Split(" ");
            CLIArgs? parsedArgs = null;

            parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.IsTrue(parsedArgs.CONSOLEOutput && parsedArgs.HTMLOutput && parsedArgs.PNGOutput && parsedArgs.JSONOutput);
        }

        [Test]
        public void TestCLIArgsUtilsDefaults()
        {
            string[] args = "".Split(" ");
            CLIArgs? parsedArgs = null;

            parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.IsTrue(parsedArgs.CONSOLEOutput 
                && parsedArgs.Verbose 
                && parsedArgs.DeleteOutputDirOnExit 
                && !string.IsNullOrEmpty(parsedArgs.OutputDirPath)
                && !string.IsNullOrEmpty(parsedArgs.PBIFilePath)
                && !string.IsNullOrEmpty(parsedArgs.RulesFilePath)
                && !parsedArgs.HTMLOutput 
                && !parsedArgs.JSONOutput 
                && !parsedArgs.PNGOutput);
        }

        [Test]
        public void TestCLIArgsUtilsThrows3()
        {
            string[] args = "-other other".Split(" ");
            CLIArgs? parsedArgs = null;

            ArgumentException ex = Assert.Throws<ArgumentException>(
            () => parsedArgs = CLIArgsUtils.ParseArgs(args));
        }

        [Test]
        public void TestCLIArgsUtilsThrows4()
        {
            string[] args = "-rules -pbix pbixPath -other".Split(" ");
            CLIArgs? parsedArgs = null;

            ArgumentException ex = Assert.Throws<ArgumentException>(
            () => parsedArgs = CLIArgsUtils.ParseArgs(args));
        }

        [Test]
        public void TestCLIArgsUtilsThrows5()
        {
            string[] args = "-rules rulesPath -pbix pbixPath -other stuff".Split(" ");
            CLIArgs? parsedArgs = null;

            ArgumentException ex = Assert.Throws<ArgumentException>(
            () => parsedArgs = CLIArgsUtils.ParseArgs(args));
        }

        [Test]
        public void TestCLIArgsUtilsThrows6()
        {
            string[] args = "-rules rulesPath -pbip pbipPath".Split(" ");
            CLIArgs? parsedArgs = null;

            ArgumentException ex = Assert.Throws<ArgumentException>(
            () => parsedArgs = CLIArgsUtils.ParseArgs(args));
        }


    }
}
#pragma warning restore CS8602 