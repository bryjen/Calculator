using CommandLine;
using Spectre.Console;

namespace calculator
{
    class Program
    {
        internal class Settings
        {
            [Option('t', "tests",
                Required = false, Default = null,
                HelpText = "Enter the mode you want to unit test." +
                           "Available modes so far: calc")]
            public string UnitTestName { get; set; }
            
            [Option('a', "angle",
                Required = false, Default = "deg",
                HelpText = "The angle modes. Available: \"DEGrees\", \"RADians\"")]
            public string Angle { get; set; }
        }

        public static Settings ProgramSettings { get; set; }
        
        static void Main(string[] args)
        {
            ProgramSettings = Parser.Default.ParseArguments<Settings>(args).Value;

            switch (ProgramSettings.Angle)
            {
                case "degree":
                case "degrees":    
                    ProgramSettings.Angle = "deg";
                    break;
                case "radian":
                case "radians":
                    ProgramSettings.Angle = "rad";
                    break;
                case "rad":
                case "deg":
                    break;
                default:
                    AnsiConsole.MarkupLine($"[red]Unknown param for angle mode: \"{ProgramSettings.Angle}\". Valid modes are:\n" +
                                           "degree(s) // deg, radian(s) // rad\n" +
                                           "Angle mode set to \"deg\" by default[/]");
                    ProgramSettings.Angle = "deg";
                    break;
            }
            
            //executes the calculator - normal case
            if (ProgramSettings.UnitTestName is null)
            {
                main.calc.Main.Enter();
                return;
            }

            switch (ProgramSettings.UnitTestName)
            {
                case "calc":
                    tests.Calc.Enter();
                    return;
                default:
                    AnsiConsole.MarkupLine($"[red]   \"{ProgramSettings.UnitTestName}\" is not a valid mode!\n" + 
                                           "Valid modes: calc.[/]");
                    return;
            }
        }
    }
}