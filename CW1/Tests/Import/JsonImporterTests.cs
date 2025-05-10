using HSE_BANK.Import;
using Newtonsoft.Json;

namespace Tests.Import;

public class JsonImporterTests
{
    private class TestEntity
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    [Fact]
    public void ParseData_ShouldReturnObjects_WhenJsonIsValid()
    {
        // Arrange
        string jsonData = "[{\"Name\":\"John\",\"Age\":30},{\"Name\":\"Alice\",\"Age\":25}]";
        var importer = new JsonImporter<TestEntity>();

        // Act
        var result = importer.ParseData(jsonData);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("John", result[0].Name);
        Assert.Equal(30, result[0].Age);
        Assert.Equal("Alice", result[1].Name);
        Assert.Equal(25, result[1].Age);
    }

    [Fact]
    public void ParseData_ShouldReturnNull_WhenJsonIsEmpty()
    {
        // Arrange
        string jsonData = "";
        var importer = new JsonImporter<TestEntity>();

        // Act
        var result = importer.ParseData(jsonData);

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ParseData_ShouldThrowException_WhenJsonIsInvalid()
    {
        // Arrange
        string jsonData = "{InvalidJson:}";
        var importer = new JsonImporter<TestEntity>();

        // Act & Assert
        Assert.Throws<JsonSerializationException>(() => importer.ParseData(jsonData));
    }

    [Fact]
    public void ImportData_ShouldCallParseData()
    {
        // Arrange
        string jsonData = "[{\"Name\":\"Eve\",\"Age\":40}]";
        var importer = new JsonImporter<TestEntity>();

        // Act
        var result = importer.ImportData(CreateTempFile(jsonData));

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Eve", result[0].Name);
        Assert.Equal(40, result[0].Age);
    }

    private string CreateTempFile(string content)
    {
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, content);
        return tempFile;
    }
}