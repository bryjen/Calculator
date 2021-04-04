using System;

namespace calculator.modes.commands
{
    
    public class Command
    {

        public static void Execute(string command, string fromWhichMode)
        {
            command = command.Replace("/", "")
                .ToLower();

            switch (command)
            {
                case "help":
                    Help(fromWhichMode);
                    break;
                case "exit":
                    Exit();
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Unknown command!");
                    Console.ResetColor();
                    break;
            }
        }

        private static void Help(string fromWhichMode)
        {
            switch (fromWhichMode)
            {
                case "calculate":
                    Console.WriteLine("The program calculates the sum/subtraction of numbers");
                    break;
                default:
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("An error has occurred calculator.modes.commands.Help at \"default\"\n" +
                                      "A request was sent from an unknown mode");
                    Console.ResetColor();
                    break;
            }
        }

        private static void Exit()
        {
            Console.WriteLine(new Random().Next(2) == 0 ? "Bye!" : "GoodBye!");
            Environment.Exit(0);
        }
    }
}