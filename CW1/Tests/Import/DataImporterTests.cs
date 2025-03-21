using HSE_BANK.Import;
using Moq;

namespace Tests.Import;

public class DataImporterTests
{
    private class TestDataImporter : DataImporter<string>
    {
        public override List<string> ParseData(string fileContent)
        {
            return new List<string> { fileContent };
        }
    }

    [Fact]
    public void ImportData_ShouldReturnData_WhenFileIsNotEmpty()
    {
        // Arrange
        var importer = new TestDataImporter();
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "Test Data");

        // Act
        var result = importer.ImportData(tempFile);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Test Data", result[0]);

        // Cleanup
        File.Delete(tempFile);
    }

    [Fact]
    public void ImportData_ShouldReturnNull_WhenFileIsEmpty()
    {
        // Arrange
        var importer = new TestDataImporter();
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "");

        // Act
        var result = importer.ImportData(tempFile);

        // Assert
        Assert.Null(result);

        // Cleanup
        File.Delete(tempFile);
    }

    [Fact]
    public void ImportData_ShouldThrowException_WhenFileDoesNotExist()
    {
        // Arrange
        var importer = new TestDataImporter();
        var invalidPath = "non_existent_file.txt";

        // Act & Assert
        Assert.Throws<FileNotFoundException>(() => importer.ImportData(invalidPath));
    }

    [Fact]
    public void ReadFile_ShouldReturnFileContent()
    {
        // Arrange
        var importer = new TestDataImporter();
        var tempFile = Path.GetTempFileName();
        File.WriteAllText(tempFile, "Sample Content");

        // Act
        var result = importer.ImportData(tempFile);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Sample Content", result[0]);

        // Cleanup
        File.Delete(tempFile);
    }

    [Fact]
    public void ImportData_ShouldCallParseData()
    {
        // Arrange
        var mockImporter = new Mock<DataImporter<string>>();
        mockImporter.Setup(i => i.ReadFile(It.IsAny<string>())).Returns("Mocked Content");
        mockImporter.Setup(i => i.ParseData(It.IsAny<string>())).Returns(new List<string> { "Parsed Data" });

        // Act
        var result = mockImporter.Object.ImportData("dummy_path");

        // Assert
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Equal("Parsed Data", result[0]);
        mockImporter.Verify(i => i.ParseData("Mocked Content"), Times.Once);
    }
}