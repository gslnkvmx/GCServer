// Services/ConsoleLogger.cs
using System;

public class ConsoleLogger
{
  private static readonly object _lock = new object();

  public void Log(string action, string message, bool isNoError = true)
  {
    lock (_lock)
    {
      var time = DateTime.Now.ToString("HH:mm:ss");
      var symbol = !isNoError ? "[✖]" : "[✓]";
      var color = !isNoError ? ConsoleColor.Red : ConsoleColor.Green;

      Console.ForegroundColor = ConsoleColor.DarkGray;
      Console.Write($"[{time}] ");

      Console.ForegroundColor = color;
      Console.Write($"{symbol} {action}: ");

      Console.ResetColor();
      Console.WriteLine(message);
    }
  }
}