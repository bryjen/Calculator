using System;
using System.Collections.Generic;
using calculator.main;
using NUnit.Framework.Internal;
using Spectre.Console;

namespace calculator.tests
{
    public class Main
    {
        public static void Enter()
        {
            List<CalcTest> tests = new List<CalcTest>();
            
            tests.Add(new CalcTest("validation1", "3 *** 5", false, null, false,
                new []{"validation"}));
            tests.Add(new CalcTest("validation2", "10-", false, null, false,
                new []{"validation"}));
            tests.Add(new CalcTest("validation3", "4 * (2 + 3", false, null, false,
                new []{"validation"}));
            tests.Add(new CalcTest("validation4", "4+3)", false, null, false,
                new []{"validation"}));
            tests.Add(new CalcTest("validation5", "123+", false, null, false,
                new []{"validation"}));
            tests.Add(new CalcTest("validation6", "1 23 456 7890", false, null, false,
                new []{"validation"}));
            tests.Add(new CalcTest("validation7", "-100", true, -100, false,
                new []{"validation"}));
            tests.Add(new CalcTest("validation8", "0.00000000000000000000", true, 0, false,
                new []{"validation", "output"}));
            tests.Add(new CalcTest("validation9", "()", false, null, false,
                new []{"validation", "parenthesis", "empty expression"}));
            tests.Add(new CalcTest("validation10", "() + 5", true, -5, false,
                new []{"validation", "parenthesis", "empty expression"}));
            tests.Add(new CalcTest("validation11", "(-4 + ()) + 5", true, 26, true,
                new []{"validation", "parenthesis", "empty expression"}));
            
            
            tests.Add(new CalcTest("add1", "3.0 + 3.0", true, 6, false,
                new []{"addition"}));
            tests.Add(new CalcTest("add2", "-3.5 +-+ 2.5", true, -6, false,
                new []{"addition"}));
            tests.Add(new CalcTest("add3", "-1987.50 + 1987", true, -0.5, false,
                new []{"addition"}));
            tests.Add(new CalcTest("add4", "10 + 9.9999", true, 19.9999, false,
                new []{"addition"}));
            tests.Add(new CalcTest("add5", "34.999 + 1.0", true, 35.999, false,
                new []{"addition"}));
            tests.Add(new CalcTest("add6", "300000000 + 900000000", true, 1200000000, true,
                new []{"addition", "large numbers"}));

            var message = "";
            main : do
            {
                Console.Clear();
                AnsiConsole.MarkupLine(message);
                message = "";
                
                foreach (var test in tests)
                {
                    test.run();
                    AnsiConsole.MarkupLine($">{test.TestName} :{(test.IsPassed() ? "[green]" : "[red]")}" +
                                           $"{(test.IsPassed() ? "Passed" : "Failed")}[/]:");
                    AnsiConsole.Write(test.AddBreak ? "\n" : "");
                }
                
                var userInput = Console.ReadLine();
                foreach (var test in tests)
                {
                    if (!userInput.Equals(test.TestName)) continue;
                    test.PrintDetails();
                    goto main;
                }

                message = "[red]Invalid Input!\nEnter the name of a test to see the test details![/]";
            } while (true);

        }
    }
}