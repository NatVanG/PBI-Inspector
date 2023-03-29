//using Newtonsoft.Json;
using System.Text.Json;

namespace PBIXInspectorLibrary
{
    public class InspectorBase
    {
        public InspectorBase(string pbixFilePath, InspectionRules inspectionRules)
        {

        }

        public InspectorBase(string pbixFilePath, string rulesFilePath)
        {

        }

        public T? DeserialiseRules<T>(string rulesFilePath)
        {
            if (!File.Exists(rulesFilePath)) throw new FileNotFoundException();

            string jsonString = File.ReadAllText(rulesFilePath);
            //return JsonConvert.DeserializeObject<T>(jsonString);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };
            return JsonSerializer.Deserialize<T>(jsonString, options);

        }

    }
}
