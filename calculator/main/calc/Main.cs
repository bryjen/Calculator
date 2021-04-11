using System;
using Spectre.Console;

namespace calculator.main.calc
{
    public class Main
    {
        public static void Enter()
        {
            do
            {
                AnsiConsole.Write("> ");
                var userInput = Console.ReadLine() 
                    .Trim();

                //if the input is a command
                if (userInput.StartsWith("/"))
                {
                    command.Commands.Execute(userInput, "calc");
                    continue;
                }

                //if the input is an assignment
                if (userInput.Contains("="))
                {
                    Variables.Assign(userInput);
                    continue;
                }

                var expression = new Expression(userInput);
                if (expression.IsValid)
                {
                    var value = expression.Solve();
                    if (value is null) continue;
                    AnsiConsole.MarkupLine($"[blue]{value}[/]");
                }
            } while (true);
        }
    }
}