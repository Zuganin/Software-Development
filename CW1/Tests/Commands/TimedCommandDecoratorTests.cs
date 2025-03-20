using Xunit;
using Moq;
using HSE_BANK.Commands;
using HSE_BANK.Interfaces.Command;
using System;
using System.IO;

public class TimedCommandDecoratorTests
{
    [Fact]
    public void Execute_ShouldMeasureExecutionTime()
    {
        // Arrange
        var commandMock = new Mock<ICommands>();
        var decorator = new TimedCommandDecorator();
        decorator.SetCommand(commandMock.Object);

        using (var consoleOutput = new StringWriter())
        {
            Console.SetOut(consoleOutput);

            // Act
            decorator.Execute();

            // Assert
            commandMock.Verify(c => c.Execute(), Times.Once);
            var output = consoleOutput.ToString();
            Assert.Contains("Время выполнения команды", output);
        }
    }
}