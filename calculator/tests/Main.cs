using System;
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
            
            tests.Add(new CalcTest("a1", "+10", true, 10, 
                null));
            tests.Add(new CalcTest("a2", "-22", true, -22,  
                null));
            tests.Add(new CalcTest("a3", "3 + 2 * 4", true, 11, 
                null));
            tests.Add(new CalcTest("a4", "2 * (3 + 4) + 1", true, 15,   
                null));
            tests.Add(new CalcTest("a5", "8 * 3 + 12 * (4 - 2)", true, 48,  
                null));
            tests.Add(new CalcTest("a6", "2 - 2 + 3", true, 3,  
                null));
            tests.Add(new CalcTest("a7", "2*2^3", true, 16, 
                null));
            tests.Add(new CalcTest("a8", "8 + 7 - 4", true, 11, 
                null));
            tests.Add(new CalcTest("a9", "3 + 8 * ((4 + 3) * 2 + 1) - 6 / (2 + 1)", true, 121,  
                null));
            
            tests.Add(new CalcTest("b1", "3 *** 5", false, null,
                null));
            tests.Add(new CalcTest("b2", "10-", false, null,
                null));
            tests.Add(new CalcTest("b3", "4 * (2 + 3", false, null,
                null));
            tests.Add(new CalcTest("b4", "4+3)", false, null,
                null));
            tests.Add(new CalcTest("b5", "123+", false, null,
                null));
            tests.Add(new CalcTest("b6", "-22", true, -22,
                null));
            tests.Add(new CalcTest("(postponed)b7", "()", false, null,
                null));
            tests.Add(new CalcTest("(postponed)b8", "() + 5", false, null,
                null));
            tests.Add(new CalcTest("b9", "(-5 * 2 ) + 5", true, -5,
                null));
            tests.Add(new CalcTest("b10", "(-4 + 5( 4+ 1)) + 5", true, 26,
                null));
            tests.Add(new CalcTest("(postponed)b11", "(-4 + ()) + 5", false, null,
                null));

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