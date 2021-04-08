using System.Collections.Generic;
using calculator.main;
using Spectre.Console;

namespace calculator.tests
{
    public class Main
    {
        public static void Enter()
        {
            List<CalcTest> tests = new List<CalcTest>();
            tests.Add(new CalcTest("test1", "3 + 8 * ((4 + 3) * 2 + 1) - 6 / (2 + 1)", true, 121));
            tests.Add(new CalcTest("test2", "8 * 3 + 12 * (4 - 2)", true, 48));
            tests.Add(new CalcTest("test3", "2 - 2 + 3", true, 3));

            foreach (var test in tests) { test.Run(); }

            foreach (var test in tests)
            {
                AnsiConsole.MarkupLine($"\n>  {test.Name}:  " + (test.GotValid == test.AssertValid && test.IsResultValid ? 
                    "[green]passed[/]" : "[red]failed[/]"));
                AnsiConsole.MarkupLine($"Expression:    \"{test.Expression}\"");
                AnsiConsole.WriteLine("Array form:    [{0}]", string.Join(", ", test.ExpressionArray));
                AnsiConsole.MarkupLine($"IsValid?       [blue]{test.AssertValid}[/]");
                AnsiConsole.MarkupLine($"got:           {(test.GotValid ? "[blue]True[/]" : "[red]alse[/]")}");
                AnsiConsole.MarkupLine($"AssertResult:  [blue]{test.AssertResult}[/]");
                AnsiConsole.MarkupLine($"got:           [{(test.IsResultValid ? "green" : "red")}]{(test.GotResult != null ? test.GotResult.Value : "null")}[/]");
            }
        }
    }
}