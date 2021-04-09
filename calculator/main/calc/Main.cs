using System;
using Spectre.Console;

namespace calculator.main.calc
{
    public class Main
    {
        public static void Enter()
        {
            var userInput = Console.ReadLine();
            var start = DateTime.Now;
            var expression = new Expression(userInput);
            var duration = DateTime.Now - start;
            AnsiConsole.MarkupLine($"[blue]{duration.TotalMilliseconds}ms[/]");
            
            AnsiConsole.WriteLine("[{0}]", string.Join(", ", expression.ExpressionList));
            AnsiConsole.WriteLine(expression.Valid);

            var value = expression.Solve();
            AnsiConsole.MarkupLine(value is null ? "[red]An error has occurred in computation[/]" : $"{value}");
        }
    }
}