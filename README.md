# VisOps with PBI Inspector (i.e. automated visual layer testing for Microsoft Power BI)

<img src="DocsImages/pbiinspector.png" alt="PBI Inspector logo" height="150"/>

## NOTE :pencil:

This is a community project that is not supported by Microsoft. 

Please also note that while PBI Inspector can read Power BI Desktop files with the ".pbix" extension as well as files in the PBIP format, the former is not supported by Microsoft.

## Thanks :pray:

Thanks to [Michael Kovalsky](https://github.com/m-kovalsky) and [Rui Romano](https://github.com/ruiromano) for their feedback on this project. Thanks also to [Luke Young](https://www.linkedin.com/in/luke-young-2301/) for creating the PBI Inspector logo.

## Bugs :beetle:

Please report issues [here](https://github.com/NatVanG/PBI-Inspector/issues).

## <a name="contents"></a>Contents

- [Intro](#intro)
- [Base rules](#baserulesoverview)
- [Graphical user interface](#gui)
- [Command line](#cli)
- [Azure DevOps integration](#ado)
- [Interpreting results](#results)
- [Known issues](#knownissues)
- [Custom rules examples](#customrulesexamples)
- [Custom rules guide](#customrulesguide)
- [Contributing ideas and discussions](#contributing)
- [Report an issue](#reportanissue)


## <a id="intro"></a>Intro

So we've DevOps, MLOps and DataOps... but why not VisOps? How can we ensure that business intelligence charts and other visuals within report pages are published in a consistent, performance optimised and accessible state? For example, are local report settings set in a consistent manner for a consistent user experience? Are visuals deviating from the specified theme by, say, using custom colours? Are visuals kept lean so they render quickly? Are charts axes titles displayed? etc.

With Microsoft Power BI, visuals are placed on a canvas and formatted as desired, images may be included and theme files referenced. Testing the consistency of the visuals output has thus far typically been a manual process. Recently, a [new Power BI file format (.pbip) was introduced](https://powerbi.microsoft.com/en-us/blog/deep-dive-into-power-bi-desktop-developer-mode-preview/) to enable pro developer application lifecycle management and source control. In particular, the report's layout definition and any associated theme are in json format and therefore readable by both machines and humans. However upon new releases of Power BI, the json structure may introduce changes without warning to include new features for example. Therefore an automated visual layout testing tool should be resilient to such changes while providing a powerful rule logic creation framework. PBI Inspector provides the ability to define fully configurable testing rules (themselves written in json) powered by Greg Dennis's Json Logic .NET implementation, see https://json-everything.net/json-logic. 

## <a id="baserulesoverview"></a>Base rules

While PBI Inspector supports custom rules, it also includes the following base rules defined at https://raw.githubusercontent.com/NatVanG/PBI-Inspector/main/Rules/Base-rules.json, some rules allow for user parameters:

1. Remove custom visuals which are not used in the report (no user parameters)
2. Reduce the number of visible visuals on the page (set parameter ```paramMaxVisualsPerPage``` to the maximum number of allowed visible visuals on the page)
3. Reduce the number of objects within visuals (override hardcoded ```4``` parameter value the the maximum number of allowed objects per visuals)
4. Reduce usage of TopN filtering visuals by page (set ```paramMaxTopNFilteringPerPage```)
5. Reduce usage of Advanced filtering visuals by page (set ```paramMaxAdvancedFilteringVisualsPerPage```)
6. Reduce number of pages per report (set ```paramMaxNumberOfPagesPerReport```)
7. Avoid setting ‘Show items with no data’ on columns (no user parameters)
8. Tooltip and Drillthrough pages should be hidden (no user parameters)

To modify parameters, save a local copy of the Base-rules.json file at https://raw.githubusercontent.com/NatVanG/PBI-Inspector/main/Rules/Base-rules.json and point PBI Inspector to the new file.

To disable a rule, edit the rule's json to specify ```"disabled": true```. At runtime PBI Inspector will ignore any disabled rule.

Currently these changes need to be made directly in the rules file json, however the plan is to provide a more intuitive user interface in upcoming releases of PBI Inspector.

## <a id="gui"></a>Run from the graphical user interface (GUI)

***Binaries***: 
- The self-contained (.NET 6.0 runtime included) Windows Forms application is available at: https://github.com/NatVanG/PBI-Inspector/releases/tag/v1.9.3-WinForm.

Running ```PBIXInspectorWinForm.exe``` presents the user with the following interface: 

![WinForm 1](DocsImages/WinForm1.png)

1. Browse to your local PBI Desktop File, either the PBIP "report.json" file or the PBIX file i.e. "*.pbix". Alternatively to try out the tool, select "Use sample".
2. Either use the base rules file included in the application or select your own.
3. Use the "Browse" button to select an output directory to which the results will be written. Alternatively, select the "Use temp files" check box to write the resuls to a temporary folder that will be deleted upon exiting the application.
4. Select output formats, either JSON or HTML or both. To simply view the test results in a formatted page select the HTML output.
5. Select "Verbose" to output both test passes and fails, if left unselected then only failed test results will be reported.  
6. Select "Run". The test run log messages are displayed at the bottom of the window. If "Use temp files" is selected (or the Output directory field is left blank) along with the HTML output check box, then the browser will open to display the HTML results.
7. Any test run information, warnings or errors are displayed in the console output textbox.

## <a id="cli"></a>Run from the command line  

***Binaries***: The command line interface application is available at: https://github.com/NatVanG/PBI-Inspector/releases/latest
(.NET 6.0 dependency not included).

All command line parameters are as follows:

```-pbip filepath```: Deprecated. Please use -pbipreport argument instead.

```-pbipreport folderpath```: Required (or specify -pbix). The path to the PBIP's "*.Report" folder.

```-pbix filepath```: Required (or specify -pbipreport). The filepath of the PBIX Power BI Desktop file to be inspected. 

```-rules filepath```: Required. The filepath to the rules file. Save a local copy of the base rules file at https://raw.githubusercontent.com/NatVanG/PBI-Inspector/main/Rules/Base-rules.json and modify as required.

```-verbose true|false```: Optional, false by default. If false then only rule violations will be shown otherwise all results will be listed.

```-output directorypath```: Optional. If -formats is set to either JSON, HTML or PNG, writes results to the specified directory, any existing files will be overwritten. If not supplied then a temporary directory will be created in the user's temporary files folder. 

```-formats CONSOLE,JSON,HTML,PNG,ADO```: Optional. Comma-separated list of output formats. 
- **CONSOLE** (default) writes results to the console output. If "-formats" is not specified then "CONSOLE" will be used by default.
- **JSON** writes results to a Json file.
- **HTML** writes results to a formatted Html page. If no output directory is specified and the HTML format is specified, then a browser page will be opened to display the HTML results. When specifying "HTML" format, report page wireframe images will be created so there is no need to also include the "PNG" format. 
- **PNG** draws report pages wireframes clearly showing any failing visuals. 
- **ADO** outputs Azure DevOps compatible task commands for use in a deployment pipeline. Task commands issued are "task.logissue" and "task.complete", see https://learn.microsoft.com/en-us/azure/devops/pipelines/scripts/logging-commands?view=azure-devops&tabs=bash#task-commands. PBI Inspector rules definition can be given a "logType" attribute of either "warning" or "error" which will be passed to the Azure DevOps task command as follows: ```##vso[task.logissue type=warning|error]```. When specifying "ADO" all other output format types will be ignored.

**Commmand line examples:**

- Run "Base-rules.json" rule definitions against PBI report file at "Sales.Report and return results in Json and HTML formats:

``` PBIXInspectorCLI.exe -pbipreport "C:\Files\Sales.Report" -rules ".\Files\Base-rules.json" -output "C:\Files\TestRun" -formats "JSON,HTML"```

- Run "Base-rules.json" rule definitions against PBI report file at "Sales.Report and return results to the console only:

``` PBIXInspectorCLI.exe -pbipreport "C:\Files\Sales.Report" -rules ".\Files\Base-rules.json" -output "C:\Files\TestRun" -formats "Console"```

- Run "Base-rules.json" rule definitions against PBI report file at "Sales.Report and return results as Azure DevOps compatible log and tasks commands (see https://learn.microsoft.com/en-us/azure/devops/pipelines/scripts/logging-commands?view=azure-devops&tabs=bash#task-commands):

``` PBIXInspectorCLI.exe -pbipreport "C:\Files\Sales.Report" -rules ".\Files\Base-rules.json"  -formats "ADO"```

## <a id="ado"></a>Azure DevOps integration

For an example on how to run PBI Inspector as part of an Azure DevOps pipeline job (alongside Tabular Editor's BPA rules), see Rui Romano's repo at https://github.com/RuiRomano/powerbi-devmode-pipelines and this YAML file in particular: https://github.com/RuiRomano/powerbi-devmode-pipelines/blob/main/azure-pipelines-build.yml.

## <a id="results"></a>Interpreting results

 If a verbose output was requested, then results for both test passes and failures will be reported. The JSON output is intended to be consumed by a subsequent process, for example a Power BI report may be created that uses the JSON file as a data source and visualises the PBI Inspector test results. The HTML page is a more readable format for humans which also includes report page wireframe images when tests are at the report page level. These images are intended to help the user identify visuals that have failed the test such as the example image below. The PBI Inspector logo is also displayed at the centre of each failing visuals as an additional identification aid when the wireframe is busy. 

![Wireframe with failures](DocsImages/WireframeWithFailures.png)

Visuals with a dotted border are visuals hidden by default as the following example:

![Wireframe with hidden visual](DocsImages/WireframeWithHiddenVisual.png)

## <a id="knownissues"></a>Known issues

-  Currently page wireframes are only created in a 16:9 aspect ratio so custom report page sizes including tooltip pages may not render as expected as shown in the following tooltip page example. See tooltip page example below:
 
 ![Tooltip page with incorrect aspect ratio](DocsImages/TooltipPageWithIncorrectAspectRatio.png)

 - Currently page wireframes do not faithfully represents the report page layout when visual groups are present.
 
 All issues should be logged at https://github.com/NatVanG/PBI-Inspector/issues.

## <a id="customrulesexamples"></a>Custom Rules Examples

*Please note that this section is not a guide to creating custom rules, just a very high-level overview and some examples. I'm currently writing a guide to rule creation in the project's wiki, see **[Anatomy of a rules file](https://github.com/NatVanG/PBI-Inspector/wiki/Anatomy-of-a-rules-file)**.*

A PBI Inspector test is written in json and is in in three parts:
1. The [JSONLogic](https://json-everything.net/json-logic) rule
2. Some data mapping logic
3. The expected result

As an added benefit, a rule can be written in such a way as to return a result more useful than just true or false. For example, an array of visual IDs or names failing the test can be returned and plotted on a wireframe diagram for ease of identification, for an illustration of this, see the second rule example below.

Besides the base rules defined at https://raw.githubusercontent.com/NatVanG/PBI-Inspector/main/Rules/Base-rules.json, see other rules examples below (these are available at [Example rules.json](https://raw.githubusercontent.com/NatVanG/PBI-Inspector/main/DocsExamples/Example-rules.json)).

- Check that certain types of charts have both axes titles displayed:

```
 {
                    "name": "Show visual axes title",
                    "description": "Check that certain charts have both axes title showing.",
                    "disabled": false,
                    "logType": "warning",
                    "forEachPath": "$.sections[*]",
                    "forEachPathName": "$.name",
                    "forEachPathDisplayName": "$.displayName",
                    "path": "$.visualContainers[*].config",
                    "pathErrorWhenNoMatch": false,
                    "test": [
                        {
                            "map": [
                                {
                                    "filter": [
                                        {
                                            "var": "visualsConfigArray"
                                        },
                                        {
                                            "and": [
                                                {
                                                    "in": [
                                                        {
                                                            "var": "singleVisual.visualType"
                                                        },
                                                        [
                                                            "lineChart",
                                                            "barChart",
                                                            "columnChart",
                                                            "clusteredBarChart",
                                                            "stackedBarChart"
                                                        ]
                                                    ]
                                                },
                                                {
                                                    "or": [
                                                        {
                                                            "==": [
                                                                {
                                                                    "var": "singleVisual.objects.categoryAxis.0.properties.showAxisTitle.expr.Literal.Value"
                                                                },
                                                                "false"
                                                            ]
                                                        },
                                                        {
                                                            "==": [
                                                                {
                                                                    "var": "singleVisual.objects.valueAxis.0.properties.showAxisTitle.expr.Literal.Value"
                                                                },
                                                                "false"
                                                            ]
                                                        }
                                                    ]
                                                }
                                            ]
                                        }
                                    ]
                                },
                                {
                                    "var": "name"
                                }
                            ]
                        },
                        {
                            "visualsConfigArray": "."
                        },
                        []
                    ]
                }
```

- Check that visuals are wider than they are tall (for fun or seriously):

```
{
                    "name": "Charts wider than tall",
                    "description": "Want to check that your charts are wider than tall?",
                    "disabled": false,
                    "logType": "warning",
                    "forEachPath": "$.sections[*]",
                    "forEachPathName": "$.name",
                    "forEachPathDisplayName": "$.displayName",
                    "path": "$.visualContainers[*].config",
                    "pathErrorWhenNoMatch": false,
                    "test": [
                        {
                            "map": [
                                {
                                    "filter": [
                                        {
                                            "var": "visualsConfigArray"
                                        },
                                        {
                                            "<=": [
                                                {
                                                    "var": "layouts.0.position.width"
                                                },
                                                {
                                                    "var": "layouts.0.position.height"
                                                }
                                            ]
                                        }
                                    ]
                                },
                                {
                                    "var": "name"
                                }
                            ]
                        },
                        {
                            "visualsConfigArray": "."
                        },
                        []
                    ]
}
```

Example wireframe output highlighting two visuals that failed the test because they are taller than they are wide:
![Charts wider than tall test output](DocsImages/WireframeChartsWiderThanTall.png)

- For a consistent user experience over import mode or a fast direct query source, check that slow data source settings are all disabled:

```
{
                    "name": "Sample - ReportSlowDatasourceSettings",
                    "disabled": false,
                    "logType": "warning",
                    "description": "Check that report slow data source settings are all disabled.",
                    "path": "$.config",
                    "pathErrorWhenNoMatch": true,
                    "test": [
                        {
                            "!": [
                                {
                                    "or": [
                                        {
                                            "var": "isCrossHighlightingDisabled"
                                        },
                                        {
                                            "var": "isSlicerSelectionsButtonEnabled"
                                        },
                                        {
                                            "var": "isFilterSelectionsButtonEnabled"
                                        },
                                        {
                                            "var": "isFieldWellButtonEnabled"
                                        },
                                        {
                                            "var": "isApplyAllButtonEnabled"
                                        }
                                    ]
                                }
                            ]
                        },
                        {
                            "isCrossHighlightingDisabled": "/slowDataSourceSettings/isCrossHighlightingDisabled",
                            "isSlicerSelectionsButtonEnabled": "/slowDataSourceSettings/isSlicerSelectionsButtonEnabled",
                            "isFilterSelectionsButtonEnabled": "/slowDataSourceSettings/isFilterSelectionsButtonEnabled",
                            "isFieldWellButtonEnabled": "/slowDataSourceSettings/isFieldWellButtonEnabled",
                            "isApplyAllButtonEnabled": "/slowDataSourceSettings/isApplyAllButtonEnabled"
                        },
                        true
                    ]
}
```

- Check that the ratio of visuals across the report using custom colours does not exceed 10%  while excluding textbox visuals from the analysis). 

```
 {
          "name": "Percentage of charts across the report using custom colours is not greater than 10%",
          "description": "Check that charts avoid custom colours and use theme colours instead.",
          "disabled": false,
          "logType": "warning",
          "path": "$.sections[*].visualContainers[*].config",
          "pathErrorWhenNoMatch": true,
          "test": [
            {
              "<=": [
                {
                  "/": [
                    {
                      "count": [
                        {
                          "filter": [
                            {
                              "var": "visualConfigArray"
                            },
                            {
                              "and": [
                                {
                                  "!": [
                                    {
                                      "in": [
                                        {
                                          "var": "singleVisual.visualType"
                                        },
                                        [
                                          "textbox"
                                        ]
                                      ]
                                    }
                                  ]
                                },
                                {
                                  "strcontains": [
                                    {
                                      "tostring": [
                                        {
                                          "var": ""
                                        }
                                      ]
                                    },
                                    "#([A-Fa-f0-9]{6}|[A-Fa-f0-9]{3})"
                                  ]
                                }
                              ]
                            }
                          ]
                        }
                      ]
                    },
                    {
                      "count": [
                        {
                          "filter": [
                            {
                              "var": "visualConfigArray"
                            },
                            {
                              "!": [
                                {
                                  "in": [
                                    {
                                      "var": "singleVisual.visualType"
                                    },
                                    [
                                      "textbox"
                                    ]
                                  ]
                                }
                              ]
                            }
                          ]
                        }
                      ]
                    }
                  ]
                },
                { "var": "paramMaxAllowedRatio" }
              ]
            },
            {
              "visualConfigArray": ".",
              "paramMaxAllowedRatio": 0.1
            },
            true
          ]
        }
```

## <a id="customrulesguide"></a>Custom rules guide

I've started writing this guide to rule creation in the project's wiki: **[Anatomy of a rules file](https://github.com/NatVanG/PBI-Inspector/wiki/Anatomy-of-a-rules-file)**. I'll be adding more content to this guide over time so do check back in.

## <a id="contributing"></a>Contributing ideas and discussions

Please contribute to ideas (for example ideas for new rules) and discussions at https://github.com/NatVanG/PBI-Inspector/discussions.

## <a id="reportanissue"></a>Report an issue

Please report issues at https://github.com/NatVanG/PBI-Inspector/issues.