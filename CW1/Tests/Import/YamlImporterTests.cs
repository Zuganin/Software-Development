using HSE_BANK.Import;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace Tests.Import;

public class YamlImporterTests
{
    private class TestEntity
    {
        [YamlMember(Alias = "Name")]
        public string Name { get; set; }
        [YamlMember(Alias = "Age")]
        public int Age { get; set; }

    }


    [Fact]
    public void ParseData_ShouldReturnNull_WhenYamlIsEmpty()
    {
        // Arrange
        string yamlData = "";
        var importer = new YamlImporter<TestEntity>();

        // Act
        var result = importer.ParseData(yamlData);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ParseData_ShouldThrowException_WhenYamlIsInvalid()
    {
        // Arrange
        string yamlData = "{InvalidYaml:}";
        var importer = new YamlImporter<TestEntity>();

        // Act & Assert
        Assert.Throws<YamlException>(() => importer.ParseData(yamlData));
    }
    

    private string CreateTempFile(string content)
    {
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, content);
        return tempFile;
    }
}