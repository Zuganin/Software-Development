using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HSE_BANK.Import;

public class YamlImporter<T> : DataImporter<T>
{
    public override List<T> ParseData(string fileContent)
    {
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        return deserializer.Deserialize<List<T>>(fileContent);
    }
}