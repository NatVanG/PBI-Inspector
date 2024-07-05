# VisOps with PBI Inspector V2 (i.e. automated visual layer testing for Microsoft Power BI)

<img src="DocsImages/pbiinspector.png" alt="PBI Inspector logo" height="150"/>

## NOTE :pencil:

This is a community project that is not supported by Microsoft. 

:exclamation: This version of PBI Inspector only supports the new enhanced metadata file format (PBIR), see https://learn.microsoft.com/en-gb/power-bi/developer/projects/projects-report. For the older PBIR-legacy file format, please use the previous version of PBI Inspector available at https://github.com/NatVanG/PBI-Inspector. 

## Breaking changes :boom:
To support the new enhanced report format (PBIR), a new "part" custom command has been introduced which helps to navigate to or iterate over the new metadata file format's parts such as "Pages", "Visuals", "Bookmarks" etc. Rules defined against the new format are not backward compatible with the older PBIR-legacy format and vice versa.

## Thanks :pray:

Thanks to [Michael Kovalsky](https://github.com/m-kovalsky) and [Rui Romano](https://github.com/ruiromano) for their feedback on this project. Thanks also to [Luke Young](https://www.linkedin.com/in/luke-young-2301/) for creating the PBI Inspector logo.

## Bugs :beetle:

Please report issues [here](https://github.com/NatVanG/PBI-Inspector/issues).

## <a name="contents"></a>Contents

- [Intro](#intro)
- [Base rules](#baserulesoverview)
- [Run from the Graphical user interface](#gui)
- [Run from the Command line](#cli)
- [Interpreting results](#results)
- [Running reports on reports](#reporting)
- [Custom rules guide](#customerruleguide)
- [Examples](#customrulesexamples)
- [Known issues](#knownissues)
- [Report an issue](#reportanissue)

## <a id="intro"></a>Intro

So we've DevOps, MLOps and DataOps... but why not VisOps? How can we ensure that business intelligence charts and other visuals within report pages are published in a consistent, performance optimised and accessible state? For example, are local report settings set in a consistent manner for a consistent user experience? Are visuals deviating from the specified theme by, say, using custom colours? Are visuals kept lean so they render quickly? Are charts axes titles displayed? etc.

With Microsoft Power BI, visuals are placed on a canvas and formatted as desired, images may be included and theme files referenced. Testing the consistency of the visuals output has thus far typically been a manual process. The [Power BI file format (.pbip) was introduced](https://powerbi.microsoft.com/en-us/blog/deep-dive-into-power-bi-desktop-developer-mode-preview/) then recently [enhanced](https://learn.microsoft.com/en-gb/power-bi/developer/projects/projects-report) to enable pro developer application lifecycle management and source control.  PBI Inspector provides the ability to define fully configurable testing rules powered by Greg Dennis's Json Logic .NET implementation, see https://json-everything.net/json-logic. 

## <a id="baserulesoverview"></a>Base rules

While PBI Inspector supports custom rules, it also includes the following base rules defined at https://github.com/NatVanG/PBI-Inspector/blob/part-concept/Rules/Base-rulesV2.json, some rules allow for user parameters:

1. Remove custom visuals which are not used in the report (no user parameters)
2. Reduce the number of visible visuals on the page (set parameter ```paramMaxVisualsPerPage``` to the maximum number of allowed visible visuals on the page)
3. Reduce the number of objects within visuals (override hardcoded ```4``` parameter value the the maximum number of allowed objects per visuals)
4. Reduce usage of TopN filtering visuals by page (set ```paramMaxTopNFilteringPerPage```)
5. Reduce usage of Advanced filtering visuals by page (set ```paramMaxAdvancedFilteringVisualsPerPage```)
6. Reduce number of pages per report (set ```paramMaxNumberOfPagesPerReport```)
7. Avoid setting ‘Show items with no data’ on columns (no user parameters)
8. Tooltip and Drillthrough pages should be hidden (no user parameters)
9. Ensure charts use theme colours (no user parameters)
10. Ensure pages do not scroll vertically (no user parameters)
11. Ensure alternativeText has been defined for all visuals (disabled by default, no user parameters)

To modify parameters, save a local copy of the Base-rulesV2.json file at https://github.com/NatVanG/PBI-Inspector/blob/part-concept/Rules/Base-rulesV2.json and point PBI Inspector to the new file.

To disable a rule, edit the rule's json to specify ```"disabled": true```. At runtime PBI Inspector will ignore any disabled rule.

## <a id="gui"></a>Run from the graphical user interface (GUI)

Running ```PBIXInspectorWinForm.exe``` presents the user with the following interface: 

![WinForm 1](DocsImages/WinForm1.png)

1. Browse to your local PBI Desktop File, either the *.pbip file or its parent folder. 
2. Either use the base rules file included in the application or select your own.
3. Use the "Browse" button to select an output directory to which the results will be written. Alternatively, select the "Use temp files" check box to write the resuls to a temporary folder that will be deleted upon exiting the application.
4. Select output formats, either JSON or HTML or both. To simply view the test results in a formatted page select the HTML output.
5. Select "Verbose" to output both test passes and fails, if left unselected then only failed test results will be reported.  
6. Select "Run". The test run log messages are displayed at the bottom of the window. If "Use temp files" is selected (or the Output directory field is left blank) along with the HTML output check box, then the browser will open to display the HTML results.
7. Any test run information, warnings or errors are displayed in the console output textbox.

## <a id="cli"></a>Run from the command line interface (CLI)

All command line parameters are as follows:

```-pbip filepath```: Required. The path to the *.pbip file.

```-pbix filepath```: Not currently supported. 

```-rules filepath```: Required. The filepath to the rules file. Save a local copy of the base rules file at https://raw.githubusercontent.com/NatVanG/PBI-Inspector/main/Rules/Base-rulesV2.json and modify as required.

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

``` PBIXInspectorCLI.exe -pbip "C:\Files\Sales.pbip" -rules ".\Files\Base-rulesV2.json" -output "C:\Files\TestRun" -formats "JSON,HTML"```

- Run "Base-rules.json" rule definitions against PBI report file at "Sales.Report and return results to the console only:

``` PBIXInspectorCLI.exe -pbip "C:\Files\Sales.pbip" -rules ".\Files\Base-rulesV2.json" -output "C:\Files\TestRun" -formats "Console"```

- Run "Base-rules.json" rule definitions against PBI report file at "Sales.Report and return results as Azure DevOps compatible log and tasks commands (see https://learn.microsoft.com/en-us/azure/devops/pipelines/scripts/logging-commands?view=azure-devops&tabs=bash#task-commands):

``` PBIXInspectorCLI.exe -pbip "C:\Files\Sales.pbip" -rules ".\Files\Base-rulesV2.json"  -formats "ADO"```

## <a id="results"></a>Interpreting results

 If a verbose output was requested, then results for both test passes and failures will be reported. The JSON output is intended to be consumed by a subsequent process, for example a Power BI report may be created that uses the JSON file as a data source and visualises the PBI Inspector test results. The HTML page is a more readable format for humans which also includes report page wireframe images when tests are at the report page level. These images are intended to help the user identify visuals that have failed the test such as the example image below. The PBI Inspector logo is also displayed at the centre of each failing visuals as an additional identification aid when the wireframe is busy. 

![Wireframe with failures](DocsImages/WireframeWithFailures.png)

Visuals with a dotted border are visuals hidden by default as the following example:

![Wireframe with hidden visual](DocsImages/WireframeWithHiddenVisual.png)

## <a id="customerruleguide"></a>Custom Rules Guide

Custom rules are defined in a JSON file. The JSON file should be an array of rule objects as follows:

```json
{
  "rules": [
  ...
  ]
}
```

Each rule object has the following properties:

```json
{
    "id": "A unique ID",
    "name": "A name that is shown in HTML results with wireframe images.",
    "description": "Details to help you and others understand what this rule does",
    "disabled": true|false(default),
    "part": "Optional. One of Report|Pages|AllPages|Visuals|AllVisuals|Bookmarks|AllBookmarks. If the part defined, such as Pages, is an array with multiple items it will be iterated over and the rule will apply to each item.",
    "test": [
    //test logic
    ,
    //optional data variables
    ,
    //expected result
    ],
    "patch": 
    [
    //optional patch logic
    ]
}
```

For example (without patch logic):

```json
{
      "id": "SHOW_AXES_TITLES",
      "name": "Show visual axes titles",
      "description": "Check that certain charts have both axes title showing, returns an array of visual names that fail the test.",
      "part": "Pages",
      "disabled": false,
      "test": [
        {
          "map": [
            {
              "filter": [
                {
                  "part": "Visuals"
                },
                {
                  "and": [
                    {
                      "in": [
                        {
                          "var": "visual.visualType"
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
                              "var": "visual.objects.categoryAxis.0.properties.showAxisTitle.expr.Literal.Value"
                            },
                            "false"
                          ]
                        },
                        {
                          "==": [
                            {
                              "var": "visual.objects.valueAxis.0.properties.showAxisTitle.expr.Literal.Value"
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
        },
        []
      ]
    }
 ```

Optionally a rule can now also define a *patch* to fix items (e.g. visuals) failing the test.  For example a patch for the test above is as follows. The patch iterates through the failing visual names returned and fixes the "Visuals" part of the report definition by setting the "showAxisTitle" property to "true" for both the category and value axes:

```json
"patch": [
        "Visuals",
        [
          {
            "op": "replace",
            "path": "/visual/objects/categoryAxis/0/properties/showAxisTitle/expr/Literal/Value",
            "value": "true"
          },
          {
            "op": "replace",
            "path": "/visual/objects/valueAxis/0/properties/showAxisTitle/expr/Literal/Value",
            "value": "true"
          }
        ]
      ]
```

A patch definition has the following structure:

```json
"patch": [
        "One of Report|Pages|AllPages|Visuals|AllVisuals|Bookmarks|AllBookmarks",
        [patch logic operator array]
      ]
```

The patch logic operator array uses the .NET implementation of JSON Patch, for syntax examples see https://docs.json-everything.net/patch/basics/.

Therefore the full rule example including the patch is as follows:

```json
    {
      "id": "SHOW_AXES_TITLES",
      "name": "Show visual axes titles",
      "description": "Check that certain charts have both axes title showing.",
      "part": "Pages",
      "disabled": true,
      "applyPatch": true,
      "test": [
        {
          "map": [
            {
              "filter": [
                {
                  "part": "Visuals"
                },
                {
                  "and": [
                    {
                      "in": [
                        {
                          "var": "visual.visualType"
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
                              "var": "visual.objects.categoryAxis.0.properties.showAxisTitle.expr.Literal.Value"
                            },
                            "false"
                          ]
                        },
                        {
                          "==": [
                            {
                              "var": "visual.objects.valueAxis.0.properties.showAxisTitle.expr.Literal.Value"
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
        },
        []
      ],
      "patch": [
        "Visuals",
        [
          {
            "op": "replace",
            "path": "/visual/objects/categoryAxis/0/properties/showAxisTitle/expr/Literal/Value",
            "value": "true"
          },
          {
            "op": "replace",
            "path": "/visual/objects/valueAxis/0/properties/showAxisTitle/expr/Literal/Value",
            "value": "true"
          }
        ]
      ]
    }
```

## <a id="customrulesexamples"></a>Rule File Examples

For full rule file examples see:
- [Base Rules](Rules/Base-rulesV2.json)
- [Example Rules](DocsExamples/Example-rulesV2.json)
- [Example Rules with Patch](DocsExamples/Example-patches.json)

## <a id="knownissues"></a>Known issues

-  Currently page wireframes are only created in a 16:9 aspect ratio so custom report page sizes including tooltip pages may not render as expected as shown in the following tooltip page example. See tooltip page example below:
 
 ![Tooltip page with incorrect aspect ratio](DocsImages/TooltipPageWithIncorrectAspectRatio.png)

 - Currently page wireframes do not faithfully represents the report page layout when visual groups are present.
 
 All issues should be logged at https://github.com/NatVanG/PBI-Inspector/issues.

## <a id="reportanissue"></a>Report an issue

Please report issues at https://github.com/NatVanG/PBI-Inspector/issues.