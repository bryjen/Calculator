using CommandLine;
using Spectre.Console;

namespace calculator
{
    class Program
    {
        class Settings
        {
            [Option('t', "tests",
                Required = false, Default = null,
                HelpText = "Enter the mode you want to unit test." +
                           "Available modes so far: calc")]
            public string UnitTestName { get; set; }
        }
        
        static void Main(string[] args)
        {
            var settings = Parser.Default.ParseArguments<Settings>(args).Value;

            //executes the calculator - normal case
            if (settings.UnitTestName is null)
            {
                main.calc.Main.Enter();
                return;
            }

            switch (settings.UnitTestName)
            {
                case "calc":
                    tests.Calc.Enter();
                    return;
                default:
                    AnsiConsole.MarkupLine($"[red]   \"{settings.UnitTestName}\" is not a valid mode!\n" + 
                                           "Valid modes: calc.[/]");
                    return;
            }
        }
    }
}
/*  to-do list;
        -   Finish implementing unit tests for the calc mode.
*/