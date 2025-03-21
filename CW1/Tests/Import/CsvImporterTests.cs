using CsvHelper;
using HSE_BANK.Import;

namespace Tests.Import;

public class CsvImporterTests
{
    private class TestEntity
    {
        public string Name { get; set; }
        public int Age { get; set; }
    }

    [Fact]
    public void ParseData_ShouldReturnObjects_WhenCsvIsValid()
    {
        // Arrange
        string csvData = "Name,Age\nJohn,30\nAlice,25";
        var importer = new CsvImporter<TestEntity>();

        // Act
        var result = importer.ImportData(CreateTempFile(csvData));

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Count);
        Assert.Equal("John", result[0].Name);
        Assert.Equal(30, result[0].Age);
        Assert.Equal("Alice", result[1].Name);
        Assert.Equal(25, result[1].Age);
    }

    [Fact]
    public void ParseData_ShouldReturnEmptyList_WhenCsvIsEmpty()
    {
        // Arrange
        string csvData = "";
        var importer = new CsvImporter<TestEntity>();

        // Act
        var result = importer.ImportData(CreateTempFile(csvData));

        // Assert
        Assert.Null(result);
    }

    [Fact]
    public void ParseData_ShouldThrowException_WhenCsvIsInvalid()
    {
        // Arrange
        string csvData = "InvalidHeader\nDataWithoutSeparator";
        var importer = new CsvImporter<TestEntity>();

        // Act & Assert
        Assert.Throws<HeaderValidationException>(() => importer.ImportData(CreateTempFile(csvData)));
    }

    [Fact]
    public void ImportData_ShouldCallParseData()
    {
        // Arrange
        string csvData = "Name,Age\nEve,40";
        var mockImporter = new CsvImporter<TestEntity>();

        // Act
        var result = mockImporter.ImportData(CreateTempFile(csvData));

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