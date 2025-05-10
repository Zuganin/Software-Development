using HSE_BANK.Domain_Models;
using HSE_BANK.Domain_Models.Enums;
using HSE_BANK.Export;
using HSE_BANK.Interfaces.Export;
using Moq;

namespace Tests.Export;

public class ExporterTests
{
    [Fact]
    public void ExportAll_ShouldCallAcceptAndClose()
    {
        // Arrange
        var visitorMock = new Mock<IExportVisitor>();
        Exporter exporter = new();
        exporter.SetVisitor(visitorMock.Object);

        var accounts = new List<BankAccount> { new BankAccount("Основной", 1000) };
        var categories = new List<Category> { new Category("Еда", CategoryType.Supermarkets) };
        var operations = new List<Operation> { new Operation(OperationType.Expenses, accounts[0].Id, 500, 
            System.DateTime.Now, "Покупка", categories[0]) };

        // Act
        Exporter.ExportAll(accounts, categories, operations);

        // Assert
        visitorMock.Verify(v => v.Visit(It.IsAny<BankAccount>()), Times.Once);
        visitorMock.Verify(v => v.Visit(It.IsAny<Category>()), Times.Once);
        visitorMock.Verify(v => v.Visit(It.IsAny<Operation>()), Times.Once);
        visitorMock.Verify(v => v.Close(), Times.Once);
    }
}