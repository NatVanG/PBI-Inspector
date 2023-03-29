# VisOps with PBIX Inspector (i.e. automated visual layer testing)

***NOTE***: This is a personal side project that is not supported by Microsoft. Parsing the contents of a Power BI Desktop file (.pbix) is not supported either.

## Intro

So we've DevOps, MLOps and DataOps... but why not VisOps? In Power BI visuals are placed on a canvas and configured as desired, images such as logos may be included and theme files defined or referenced. Testing the consistency of the visuals output is typically a manual process. However because a Power BI .pbix file is packages as an archive (.zip) file it is possible to decompress and read the entries within. In particular the visuals canvas and any associated theme are in json format. Upon new releases of Power BI Desktop and Service, json schema definitions may change without warning to include new features for example. Therefore if an automated visual layer inspection or testing tool were to be created it would need to be resilient to change by being fully configurable. This is where PBIX Inspector comes in. PBIX inspector uses Greg Dennis's Json Logic .NET implementation as the Json rules engine, see https://json-everything.net/json-logic.

## Examples

Run PBIX inspector by passing in PBIX file path and Json rules filepath as shown in the following example: 
```C:\> PBIXInspectorCLI.exe -pbix "Adventure Works.pbix" -rules "Adv Works rules.json"```

If run without any parameters PBIX inspector will use sample PBIX and rules files under the "Files" folder. 

See rules examples below.

- Check that certain types of charts have both axes titles displayed:
![Rules Example 1](DocsImages/RulesExample1.png)

- Check visuals interactivity setting:
![Rules Example 2](DocsImages/RulesExample3.png)

- Check that slow data source settings are all disabled:
![Rules Example 3](DocsImages/RulesExample3.png)

- Check report theme title font attributes:
![Rules Example 4](DocsImages/RulesExample5.png)

- Check the number of report pages (could this wrap in a less than "<" test to ensure the number of pages in report are below a certain number for performance reasons for example) - showcasing the ability to express complex logic:
![Rules Example 5](DocsImages/RulesExample5.png)