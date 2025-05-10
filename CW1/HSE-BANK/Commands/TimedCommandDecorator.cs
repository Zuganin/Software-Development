using System.Diagnostics;
using HSE_BANK.Interfaces.Command;

namespace HSE_BANK.Commands;

public class TimedCommandDecorator : ICommands
{
    private ICommands _command;

    public void SetCommand(ICommands command)
    {
        _command = command;
    }

    public void Execute()
    {
        var stopwatch = Stopwatch.StartNew();
        _command.Execute();
        stopwatch.Stop();
        Console.WriteLine($"Время выполнения команды: {stopwatch.ElapsedMilliseconds} мс.");
    }
}