using Newtonsoft.Json;

namespace HSE_BANK.Import;

public class JsonImporter<T> : DataImporter<T>
{
    protected override List<T> ParseData(string fileContent)
    {
        return JsonConvert.DeserializeObject<List<T>>(fileContent);
    }
}