using PBIXInspectorLibrary.CustomRules;
using System.Text.Json;

namespace PBIXInspectorLibrary
{
    public class InspectorBase
    {
        public InspectorBase()
        {

        }

        public InspectorBase(string pbiFilePath, InspectionRules inspectionRules)
        {
            if (string.IsNullOrEmpty(pbiFilePath)) throw new ArgumentNullException(nameof(pbiFilePath));
            if (!File.Exists(pbiFilePath)) throw new FileNotFoundException();
        }

        public InspectorBase(string pbiFilePath, string rulesFilePath)
        {
            if (string.IsNullOrEmpty(pbiFilePath)) throw new ArgumentNullException(nameof(pbiFilePath));
        }

        public T? DeserialiseRulesFromFilePath<T>(string rulesFilePath)
        {
            rulesFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, rulesFilePath);
            
            if (!File.Exists(rulesFilePath)) throw new FileNotFoundException(string.Format("File with path \"{0}\" was not found", rulesFilePath));

            string jsonString = File.ReadAllText(rulesFilePath);

            return DeserialiseRules<T>(jsonString);

        }

        public static T? DeserialiseRules<T>(string jsonString)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<T>(jsonString, options);
        }

        public static T? DeserialiseRules<T>(Stream jsonStream)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<T>(jsonStream, options);
        }
    }
}
