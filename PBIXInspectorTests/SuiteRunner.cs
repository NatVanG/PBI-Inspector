//MIT License

//Copyright (c) 2022 Greg Dennis

//Permission is hereby granted, free of charge, to any person obtaining a copy
//of this software and associated documentation files (the "Software"), to deal
//in the Software without restriction, including without limitation the rights
//to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//copies of the Software, and to permit persons to whom the Software is
//furnished to do so, subject to the following conditions:

//The above copyright notice and this permission notice shall be included in all
//copies or substantial portions of the Software.

//THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//SOFTWARE.

using PBIXInspectorLibrary;
using PBIXInspectorLibrary.Output;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace PBIXInspectorTests;

/// <summary>
/// The code in this SuiteRunner is adapted from Greg Dennis's Json Everything test suite (see https://github.com/gregsdennis/json-everything) to ensure that we're not breaking core JsonLogic functionality.
/// </summary>
public class SuiteRunner
{
    #region PbixTestSuite
    public static IEnumerable<TestCaseData> PbixTestSuite()
    {
        string PBIXFilePath = @"Files\Inventory test.pbix";
        string RulesFilePath = @"Files\Inventory rules test.json";

        Console.WriteLine("Running test suite...");
        return Suite(PBIXFilePath, RulesFilePath);
    }

    [TestCaseSource(nameof(PbixTestSuite))]
    public void RunPbixTest(TestResult testResult)
    {
        Assert.True(testResult.Pass, testResult.Message);
    }
    #endregion

    #region PbipTestSuite
    public static IEnumerable<TestCaseData> PbipTestSuite()
    {
        string PBIPFilePath = @"Files\pbip\Inventory test.Report";
        string RulesFilePath = @"Files\Inventory rules test.json";

        Console.WriteLine("Running test suite...");
        return Suite(PBIPFilePath, RulesFilePath);
    }

    [TestCaseSource(nameof(PbipTestSuite))]
    public void RunPbipTest(TestResult testResult)
    {
        Assert.True(testResult.Pass, testResult.Message);
    }
    #endregion

    #region BasePassSuite
    public static IEnumerable<TestCaseData> BasePassPBIXSuite()
    {
        string PBIXFilePath = @"Files\Base-rules-passes.pbix";
        string RulesFilePath = @"Files\Base-rules.json";

        Console.WriteLine("Running base pass PBIX suite...");
        return Suite(PBIXFilePath, RulesFilePath);
    }

    [TestCaseSource(nameof(BasePassPBIXSuite))]
    public void RunBasePassPBIX(TestResult testResult)
    {
        Assert.True(testResult.Pass, testResult.Message);
    }
    #endregion

    #region BaseFailSuite 
    public static IEnumerable<TestCaseData> BaseFailPBIXSuite()
    {
        string PBIXFilePath = @"Files\Base-rules-fails.pbix";
        string RulesFilePath = @"Files\Base-rules.json";

        Console.WriteLine("Running base fail PBIX suite...");
        return Suite(PBIXFilePath, RulesFilePath);
    }

    [TestCaseSource(nameof(BaseFailPBIXSuite))]
    public void RunBaseFailPBIX(TestResult testResult)
    {
        RunBaseFail(testResult);
    }

    public static IEnumerable<TestCaseData> BaseFailPBIPSuite()
    {
        string PBIPFilePath = @"Files\pbip\Inventory sample - fails.Report";
        string RulesFilePath = @"Files\Base-rules.json";

        Console.WriteLine("Running base fail PBIP suite...");
        return Suite(PBIPFilePath, RulesFilePath);
    }

    [TestCaseSource(nameof(BaseFailPBIPSuite))]
    public void RunBaseFailPBIP(TestResult testResult)
    {
        RunBaseFail(testResult);
    }

    private void RunBaseFail(TestResult testResult)
    {
        string expected = "[]";
        switch (testResult.RuleId)
        {
            case "REMOVE_UNUSED_CUSTOM_VISUALS":
                expected = "[\"Aquarium1442671919391\"]";
                JsonAssert.AreEquivalent(testResult.Actual, JsonNode.Parse(expected));
                break;
            case "REDUCE_VISUALS_ON_PAGE":
                if (testResult.ParentName == "ReportSectionfb0835fa991786b43a3f")
                {
                    Assert.False(testResult.Pass, testResult.Message);
                }
                else
                {
                    Assert.True(testResult.Pass, testResult.Message);
                }
                break;
            case "REDUCE_OBJECTS_WITHIN_VISUALS":
                if (testResult.ParentName == "ReportSection4602098ba1ff5a3805a9")
                {
                    Assert.False(testResult.Pass, testResult.Message);
                }
                else
                {
                    Assert.True(testResult.Pass, testResult.Message);
                }
                break;
            case "REDUCE_TOPN_FILTERS":
                if (testResult.ParentName == "ReportSection3440cc1dc4ec63ca3d06")
                {
                    Assert.False(testResult.Pass, testResult.Message);
                }
                else
                {
                    Assert.True(testResult.Pass, testResult.Message);
                }
                break;
            case "REDUCE_ADVANCED_FILTERS":
                if (testResult.ParentName == "ReportSectiond7d52b137add50d28b88")
                {
                    Assert.False(testResult.Pass, testResult.Message);
                }
                else
                {
                    Assert.True(testResult.Pass, testResult.Message);
                }
                break;
            case "REDUCE_PAGES":
                Assert.True(testResult.Pass, testResult.Message);
                break;
            case "AVOID_SHOW_ITEMS_WITH_NO_DATA":
                expected = "[\"797168e1f1e7658ceae6\",\"97ad01a2b8fbfca3220c\"]";
                if (testResult.ParentName == "ReportSection5f326c8a8185db501ad9")
                {
                    JsonAssert.AreEquivalent(testResult.Actual, JsonNode.Parse(expected));
                }
                else
                {
                    Assert.True(testResult.Pass, testResult.Message);
                }
                break;
            case "HIDE_TOOLTIP_DRILLTROUGH_PAGES":
                if (testResult.ParentName == "ReportSectionadc267c0d12e40458799"
                        || testResult.ParentName == "ReportSection8952e5fd70dcea579d3b")
                {
                    Assert.False(testResult.Pass, testResult.Message);
                }
                else
                {
                    Assert.True(testResult.Pass, testResult.Message);
                }
                break;
            case "ENSURE_THEME_COLOURS":
                if (testResult.ParentName == "ReportSection6c3c3f97279fafdeeb57")
                {
                    expected = "[\"1a67964cf02b6170c3b8\"]";
                    JsonAssert.AreEquivalent(testResult.Actual, JsonNode.Parse(expected));
                }
                else
                {
                    Assert.True(testResult.Pass, testResult.Message);
                }
                break;
            case "ENSURE_PAGES_DO_NOT_SCROLL_VERTICALLY":
                expected = "[\"Scrolling page\"]";
                JsonAssert.AreEquivalent(testResult.Actual, JsonNode.Parse(expected));
                break;
            case "ENSURE_ALTTEXT":
                Assert.False(testResult.Pass, testResult.Message);
                break;
            default:
                Assert.True(testResult.Pass, testResult.Message);
                break;
        }
    }
    #endregion

    #region ExampleFailSuite

    public static IEnumerable<TestCaseData> ExampleFailPBIXSuite()
    {
        string PBIXFilePath = @"Files\Example-rules-fails.pbix";
        string RulesFilePath = @"Files\Example-rules.json";

        Console.WriteLine("Running example fail PBIX suite...");
        return Suite(PBIXFilePath, RulesFilePath);
    }

    [TestCaseSource(nameof(ExampleFailPBIXSuite))]
    public void RunExampleFailPBIX(TestResult testResult)
    {
        RunExampleFail(testResult);
    }

    private void RunExampleFail(TestResult testResult)
    {
        string expected = "[]";
        switch (testResult.RuleId)
        {
            case "CHARTS_WIDER_THAN_TALL":
                if (testResult.ParentDisplayName == testResult.RuleId)
                {
                    expected = "[\"3f7d302598c1e81e7e78\", \"5094f3ff553da63e610e\"]";
                    JsonAssert.AreEquivalent(JsonNode.Parse(expected), testResult.Actual);
                    Assert.False(testResult.Pass, testResult.Message);
                }
                else
                {
                    Assert.True(testResult.Pass, testResult.Message);
                }
                break;
            case "DISABLE_SLOW_DATASOURCE_SETTINGS":
                Assert.False(testResult.Pass, testResult.Message);
                break;
            case "LOCAL_REPORT_SETTINGS":
                Assert.False(testResult.Pass, testResult.Message);
                break;
            case "SHOW_AXES_TITLES":
                if (testResult.ParentDisplayName == testResult.RuleId)
                {
                    expected = "[\"a9243890e8b7ec111322\", \"d65c53d5b679c4cacba0\", \"8a0d8392a2400e899bcc\"]";
                    JsonAssert.AreEquivalent(JsonNode.Parse(expected), testResult.Actual);
                    Assert.False(testResult.Pass, testResult.Message);
                }
                else
                {
                    Assert.True(testResult.Pass, testResult.Message);
                }
                break;
            case "PERCENTAGE_OF_CHARTS_USING_CUSTOM_COLOURS":
                //if (testResult.ParentName == "ReportSectiond7d52b137add50d28b88")
                //{
                //    Assert.False(testResult.Pass, testResult.Message);
                //}
                //else
                //{
                //    Assert.True(testResult.Pass, testResult.Message);
                //}
                break;
            case "ENSURE_ALT_TEXT_DEFINED_FOR_VISUALS":
                expected = "[\"9032ab70a7e060d08574\",\"eca6ff83ecb390801c3a\"]";
                if (testResult.ParentDisplayName == testResult.RuleId)
                {
                    JsonAssert.AreEquivalent(JsonNode.Parse(expected), testResult.Actual);
                    Assert.False(testResult.Pass, testResult.Message);
                }

                break;
            case "DISABLE_DROP_SHADOWS_ON_VISUALS":
                expected = "[\"bdb3c2666ac0e67947aa\",\"5d4868734a72096e0ada\"]";
                if (testResult.ParentDisplayName == testResult.RuleId)
                {
                    JsonAssert.AreEquivalent(JsonNode.Parse(expected), testResult.Actual);
                    Assert.False(testResult.Pass, testResult.Message);
                }
                else
                {
                    Assert.True(testResult.Pass, testResult.Message);
                }
                break;
            case "GIVE_VISIBLE_PAGES_MEANINGFUL_NAMES":
                if (testResult.ParentDisplayName == "Page 1")
                {
                    Assert.False(testResult.Pass, testResult.Message);
                }
                else
                {
                    Assert.False(testResult.Pass, testResult.Message);
                }
                break;
            case "DENEB_CHARTS_PROPERTIES":
                //TODO: complete this test
                //Assert.False(testResult.Pass, testResult.Message);
                break;
            case "CHECK_FOR_VISUALS_OVERLAP":
                expected = "[\"2beb787442a6d0432b4d\",\"11f540db1a90abb52cda\",\"93e80741178005eb0ab4\",\"dead16c359819062e164\"]";
                if (testResult.ParentDisplayName == testResult.RuleId)
                {
                    JsonAssert.AreEquivalent(JsonNode.Parse(expected), testResult.Actual);
                    Assert.False(testResult.Pass, testResult.Message);
                }
                break;
            case "CHECK_FOR_LOCAL_MEASURES":
                //TODO: complete this test
                //Assert.False(testResult.Pass, testResult.Message);
                break;
            case "REPORT_THEME_NAME":
                Assert.False(testResult.Pass, testResult.Message);
                break;
            case "REPORT_THEME_TITLE_FONT":
                Assert.False(testResult.Pass, testResult.Message);
                break;
            default:
                Assert.True(testResult.Pass, testResult.Message);
                break;
        }
    }
    #endregion

    #region JsonLogicSuite 
    /// <summary>
    /// Test PBIX using the base JsonLogicTest file to make sure we didn't break JsonLogic
    /// </summary>
    /// <returns></returns>
    public static IEnumerable<TestCaseData> JsonLogicSuite()
    {
        string PBIXFilePath = @"Files\Inventory test.pbix";
        var testsPath = Path.Combine(TestContext.CurrentContext.WorkDirectory, @"Files\JsonLogicTests.json");

        return Task.Run(async () =>
        {
            string content = null!;
            try
            {
                using var client = new HttpClient();
                using var request = new HttpRequestMessage(HttpMethod.Get, "https://jsonlogic.com/tests.json");
                using var response = await client.SendAsync(request);

                content = await response.Content.ReadAsStringAsync();

                await File.WriteAllTextAsync(testsPath, content);
            }
            catch (Exception e)
            {
                content ??= await File.ReadAllTextAsync(testsPath);

                Console.WriteLine(e);
            }

            var testSuite = JsonSerializer.Deserialize<JsonLogicTestSuite>(content);
            var inspectionRules = new InspectionRules();
            var rules = testSuite!.Tests.Select(t => new PBIXInspectorLibrary.Rule() { Name = t.Logic, Path = "$", PathErrorWhenNoMatch = false, Test = new PBIXInspectorLibrary.Test() { Logic = t.Logic!, Data = t.Data!, Expected = t.Expected! } });
            inspectionRules.Rules = rules.ToList();

            return Suite(PBIXFilePath, inspectionRules);
        }).Result;
    }

    [TestCaseSource(nameof(JsonLogicSuite))]
    public void RunJsonLogicTest(TestResult testResult)
    {
        Assert.True(testResult.Pass, testResult.Message);
    }
    #endregion

    #region SampleSuite 
    public static IEnumerable<TestCaseData> SampleSuite()
    {
        string PBIXFilePath = @"Files\Inventory sample.pbix";
        string RulesFilePath = @"Files\Inventory rules sample.json";

        Console.WriteLine("Running sample suite...");
        return Suite(PBIXFilePath, RulesFilePath);
    }

    [TestCaseSource(nameof(SampleSuite))]
    public void RunSample(TestResult testResult)
    {
        Assert.True(testResult.Pass, testResult.Message);
    }
    #endregion

    #region BaseSuite 
    public static IEnumerable<TestCaseData> BaseSuite()
    {
        string PBIXFilePath = @"Files\Inventory sample.pbix";
        string RulesFilePath = @"Files\Base-rules.json";

        Console.WriteLine("Running base suite...");
        return Suite(PBIXFilePath, RulesFilePath);
    }

    [TestCaseSource(nameof(BaseSuite))]
    public void RunBase(TestResult testResult)
    {
        Assert.True(testResult.Pass, testResult.Message);
    }
    #endregion

    public static IEnumerable<TestCaseData> Suite(string PBIXFilePath, string RulesFilePath)
    {
        try
        {
            Inspector insp = new Inspector(PBIXFilePath, RulesFilePath);

            var testResults = insp.Inspect();

            return testResults.Select(t => new TestCaseData(t) { TestName = t.RuleName });
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (PBIXInspectorException e)
        {
            Console.WriteLine(e.Message);
        }

        return null;
    }

    public static IEnumerable<TestCaseData> Suite(string PBIXFilePath, InspectionRules inspectionRules)
    {
        try
        {
            Inspector insp = new Inspector(PBIXFilePath, inspectionRules);

            var testResults = insp.Inspect();

            return testResults.Select(t => new TestCaseData(t) { TestName = t.RuleName });
        }
        catch (ArgumentNullException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (ArgumentException e)
        {
            Console.WriteLine(e.Message);
        }
        catch (PBIXInspectorException e)
        {
            Console.WriteLine(e.Message);
        }

        return null;
    }
}