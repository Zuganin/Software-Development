using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace HSE_BANK.Import;

public class YamlImporter<T> : DataImporter<T>
{
    protected override List<T> ParseData(string fileContent)
    {
        // Используем YamlDotNet для десериализации YAML
        var deserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
        return deserializer.Deserialize<List<T>>(fileContent);
    }
}