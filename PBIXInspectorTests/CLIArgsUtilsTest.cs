using PBIXInspectorLibrary.Utils;

namespace PBIXInspectorTests
{
    public class CLIArgsUtilsTest
    {
        [Test]
        public void TestCLIArgsUtilsSuccess()
        {
            string[] args = "-pbix pbixPath -rules rulesPath".Split(" ");
            var parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIXFilePath.Equals("pbixPath") && parsedArgs.RulesFilePath.Equals("rulesPath"));
        }

        [Test]
        public void TestCLIArgsUtilsSwappedParamsSuccess()
        {
            string[] args = "-rules rulesPath -pbix pbixPath".Split(" ");
            var parsedArgs = CLIArgsUtils.ParseArgs(args);

            Assert.True(parsedArgs.PBIXFilePath.Equals("pbixPath") && parsedArgs.RulesFilePath.Equals("rulesPath"));
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