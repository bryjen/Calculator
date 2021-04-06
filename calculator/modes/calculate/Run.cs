using System;
using System.Globalization;
using System.Text.RegularExpressions;

using calculator.modes.commands;

namespace calculator.modes.calculate
{
    public static class Run
    {
        public static void Enter()
        {
            do
            {
                var userInput = new Regex(" +").Replace(Console.ReadLine(), " ")
                    .Trim();

                if (InputIsACommand(userInput))
                {
                    Command.Execute(userInput, "calculate");
                    continue;
                }
                
                if (InputIsAnAssignment(userInput))
                {
                    Variables.Assign(userInput);
                    continue;
                }
                
                if (userInput.Equals("")) continue;

                //  by process of elimination, the userInput is an expression

                if (!Expression.IsAValidExpression(userInput))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Invalid expression");
                    Console.ResetColor();
                    continue;
                }
                var value = Expression.SolveExpression(userInput);
                
                if (value.Equals("")) continue;
                Console.WriteLine(value);


            } while (true);
        }
        
        private static bool InputIsACommand(string userInput)
        {
            return userInput.StartsWith("/");
        }

        private static bool InputIsAnAssignment(string userInput)
        {
            return userInput.Contains("=");
        }
    }
}
