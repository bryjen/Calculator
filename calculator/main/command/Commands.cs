using System;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace calculator.main.command
{
    public class Commands
    {
        public static void Execute(string command, string fromWhere)
        {
            var mainCommand = Regex.Match(command, "/[a-zA-Z]+").Value.Trim();
            var tokens = new Regex("/[a-zA-Z]+").Replace(command, "").Trim();

            switch (mainCommand)
            {
                case "/exit":
                    AnsiConsole.MarkupLine("[blue]Bye![/]");
                    Environment.Exit(0);
                    return;
                case "/help":
                    ExecuteHelp(tokens, fromWhere);
                    break;
                case "/clear":
                    if (!new Regex("/clear").Replace(command, "").Equals(""))
                    {
                        AnsiConsole.MarkupLine("[red]Unknown token after \"/clear\"![/]");
                        return;
                    }
                    Console.Clear();
                    return;
                case "/angle":
                    ChangeAngle(tokens);
                    break;
                default:
                    AnsiConsole.MarkupLine("[red]Unknown Command![/]");
                    break;
            }
        }

        private static void ExecuteHelp(string token, string fromWhere)
        {
            switch (token)
            {
                case "":
                    break;
                default:
                    AnsiConsole.MarkupLine($"[red]Unknown token \"{token}\"[/]");
                    return;
            }
            
            switch (fromWhere)
            {
                case "calc":
                    AnsiConsole.MarkupLine("            CALCULATE:" +
                                           "    \n  [blue]Evaluating Expressions[/] :  The program functions like a normal" +
                                           "calculator that follows the standard order of operations, with an" +
                                           "addition of % (modulus) having the same priority as '*' and '/'." +
                                           "\n" +
                                           "\n  [blue]Variables[/]  :   To declare variables, simply type the name of the" +
                                           "variable with an '=' then the expression you want to store in the variable. It " +
                                           "can be just numbers, or it can include other variables as well. If so, the" +
                                           "variables must be already defined beforehand. In addition to that, the expression " +
                                           "must also be valid - i.e. it must be in a valid format. Variables are also dynamic," +
                                           "meaning that if a variable contains another variable, the value of the upper " +
                                           "variable will change if the lover variable changes as well." +
                                           "\n  An example of a valid variable declaration: " +
                                           "\n a = 3 + 8 * ((4 + 3) * 2 + 1) - 6 / (2 + 1)");
                    return;
                default:
                    AnsiConsole.MarkupLine($"[red]AN ERROR HAS OCCURRED, UNKNOWN FROMWHERE \"{fromWhere}\"[/]");
                    return;
            }
        }

        private static void ChangeAngle(string token)
        {
            switch (token)
            {
                case "deg":
                case "degree":
                case "degrees":
                    Program.ProgramSettings.Angle = "deg";
                    AnsiConsole.MarkupLine("[blue]Angle mode set to degrees/deg[/]");
                    return;
                case "rad":
                case "radian":
                case "radians":
                    Program.ProgramSettings.Angle = "rad";
                    AnsiConsole.MarkupLine("[blue]Angle mode set to radians/rad[/]");
                    return;
                default:
                    AnsiConsole.MarkupLine("[red]Unknown angle mode: \"{ProgramSettings.Angle}\". Valid modes are:\n" +
                                           "degree(s) // deg, radian(s) // rad[/]");
                    return;
            }
        }
    }
}