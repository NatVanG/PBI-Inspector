using PBIXInspectorLibrary.Utils;

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
            string[] args = "-pbip pbipPath -rules rulesPath -verbose true".Split(" ");
            var parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbipPath") && parsedArgs.RulesFilePath.Equals("rulesPath") && parsedArgs.Verbose);
        }

        [Test]
        public void TestCLIArgsUtilsSuccess_VerboseOptionMissing()
        {
            string[] args = "-pbip pbipPath -rules rulesPath".Split(" ");
            var parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbipPath") && parsedArgs.RulesFilePath.Equals("rulesPath") && parsedArgs.Verbose);
        }

        [Test]
        public void TestCLIArgsUtilsSuccess_VerboseOptionFalse()
        {
            string[] args = "-pbip pbipPath -rules rulesPath -verbose false".Split(" ");
            var parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbipPath") && parsedArgs.RulesFilePath.Equals("rulesPath") && !parsedArgs.Verbose);
        }

        [Test]
        public void TestCLIArgsUtilsSuccess_VerboseOptionUnparseable()
        {
            string[] args = "-pbip pbipPath -rules rulesPath -verbose XYZ".Split(" ");
            var parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbipPath") && parsedArgs.RulesFilePath.Equals("rulesPath") && !parsedArgs.Verbose);
        }

        [Test]
        public void TestCLIArgsUtilsSuccess_FavourPBIPOption()
        {
            string[] args = "-pbix pbixPath -pbip pbipPath -rules rulesPath -verbose true".Split(" ");
            var parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIFilePath.Equals("pbipPath") && parsedArgs.RulesFilePath.Equals("rulesPath"));
        }

        [Test]
        public void TestCLIArgsUtilsThrows()
        {
            string[] args = "-pbix pbixPath".Split(" ");
            CLIArgs? parsedArgs = null;

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
            () => parsedArgs = CLIArgsUtils.ParseArgs(args));
        }

        [Test]
        public void TestCLIArgsUtilsThrows2()
        {
            string[] args = "-rules rulesPath".Split(" ");
            CLIArgs? parsedArgs = null;

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
            () => parsedArgs = CLIArgsUtils.ParseArgs(args));
        }

        [Test]
        public void TestCLIArgsUtilsThrows3()
        {
            string[] args = "-other other".Split(" ");
            CLIArgs? parsedArgs = null;

            ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
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


    }
}