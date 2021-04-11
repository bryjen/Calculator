using System;

using System.Collections.Generic;
using System.Linq;
using Spectre.Console;

using calculator.main;
using NUnit.Framework;

namespace calculator.tests
{
    
    public class Calc
    {
        /** <summary>Runs unit tests on the mode calc. To see how the tests work, see <see cref="CalcTest"/>.</summary>
         *
         * <remarks>The function runs two types of tests, normal tests and variables tests (organized into their
         * respectively named regions. Normal tests are normal computations that do not include any variables. Normal 
         * tests are run in a loop later down the function. On the other hand, variable tests are run when they are declared.
         * These tests test that variables and constants are working (like throwing an error if the variable has not been
         * defined yet). Variables cannot be freely controlled in loops, so the execution of variable tests occur outside
         * them.</remarks>
         */
        // ReSharper disable once CognitiveComplexity
        public static void Enter()
        {
            #region NORMAL_TESTS

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
    
                tests.Add(new CalcTest("test1", "3.0 + 3.0", true, 6, false,
                    new []{"addition"}));
                tests.Add(new CalcTest("test2", "-3.5 +-+ 2.5", true, -6, false,
                    new []{"addition", "decimals"}));
                tests.Add(new CalcTest("test3", "-1987.50 + 1987", true, -0.5, false,
                    new []{"addition", "decimals"}));
                tests.Add(new CalcTest("test4", "10 + 9.9999", true, 19.9999, false,
                    new []{"addition", "decimals"}));
                tests.Add(new CalcTest("test5", "34.999 + 1.0", true, 35.999, false,
                    new []{"addition", "decimals"}));
                tests.Add(new CalcTest("test6", "300000000 + 900000000", true, 1200000000, true,
                    new []{"addition", "large numbers"}));
                
                tests.Add(new CalcTest("test7", " 7.12345678 - 2.21098765", true, 4.91246913, false,
                    new []{"subtraction", "decimals"}));
                tests.Add(new CalcTest("test8", " -500 -+-- 12.3456789", true, -512.3456789, false,
                    new []{"subtraction", "decimals"}));
                
                tests.Add(new CalcTest("test9", "1.23456789 *               -2.10987654", true, -2.6047858281483007, true,
                    new []{"multiplication", "decimals"}));
                tests.Add(new CalcTest("test10", "123456789 * 210987654", true, 26047858281483010, false,
                    new []{"multiplication", "large numbers"}));
                tests.Add(new CalcTest("test11", "-      500    * 123.456789", true, -61728.3945, true,
                    new []{"multiplication", "decimals"}));
                
                tests.Add(new CalcTest("test12", "3 + 8 * ((4 + 3) * 2 + 1) - 6 / (2 + 1)", true, 121, false,
                    new []{"addition", "subtraction", "multiplication", "division"}));
                tests.Add(new CalcTest("test13", "-+3+8((4+3)*2+1)-6/(2+1)", true, 115, false,
                    new []{"addition", "subtraction", "multiplication", "division"}));
                tests.Add(new CalcTest("test14", "+-2(-3+--4)/2", true, -1, false,
                    new []{"addition", "multiplication", "division"}));
                tests.Add(new CalcTest("test15", "8 * 3 + 12 * (4 - 2)", true, 48, true,
                    new []{"addition", "multiplication", "division"}));

            #endregion

            /* Variable tests can be executed with different circumstances. Like for example, a variable has not been defined yet,
             a first test executes and returns an error (since the expression is invalid), then the variable is defined,
             then the test passes.
             */
            #region VARIABLE_TESTS

                List<CalcTest> variableTests = new List<CalcTest>();
                
                Variables.Assign("someVariable = 10");
                var test16 = new CalcTest("test16", "someVariable", true, 10, false,
                    new []{"variables"});
                test16.Run();
                
                var test17 = new CalcTest("test17", "pi * e * tau", true, Math.PI * Math.E * Math.Tau, false,
                    new []{"constants"});
                test17.Run();
                
                Variables.Assign("a = 4");
                Variables.Assign("b = 5");
                var test18 = new CalcTest("test18", "a*2+b*3+c*(2+3)", false, null, false,
                    new []{"addition", "multiplication", "division", "validation", "variables"});
                test18.Run();
                Variables.Assign("c = 6");
                var test19 = new CalcTest("test19", "a*2+b*3+c*(2+3)", true, 53, false,
                    new []{"addition", "multiplication", "division", "variables"});
                test19.Run();
                Variables.Clear();
                
                Variables.Assign("a=1");
                Variables.Assign("b = a + 1");
                Variables.Assign("c = b + 1");
                var test20 = new CalcTest("test20", "c", true, 3, false,
                    new []{"addition", "variables"});
                test20.Run();
                Variables.Assign("a = 2");
                Variables.Assign("b = a + 2");
                var test21 = new CalcTest("test21", "c", true, 5, false,
                    new []{"addition", "dynamic variables"});
                test21.Run();
                Variables.Clear();
                
                variableTests.Add(test16);
                variableTests.Add(test17);
                variableTests.Add(test18);
                variableTests.Add(test19);
                variableTests.Add(test20);
                variableTests.Add(test21);
    
            #endregion
            
            

            var message = "";
            main : do
            {
                Console.Clear();
                AnsiConsole.MarkupLine(message);
                message = "";
                
                foreach (var test in tests)
                {
                    test.Run();
                    AnsiConsole.MarkupLine($">{test.TestName} :{(test.IsPassed() ? "[green]" : "[red]")}" +
                                           $"{(test.IsPassed() ? "Passed" : "Failed")}[/]:");
                    AnsiConsole.Write(test.AddBreak ? "\n" : "");
                }
                foreach (var test in variableTests)
                {
                    AnsiConsole.MarkupLine($">{test.TestName} :{(test.IsPassed() ? "[green]" : "[red]")}" +
                                           $"{(test.IsPassed() ? "Passed" : "Failed")}[/]:");
                    AnsiConsole.Write(test.AddBreak ? "\n" : "");
                }
                
                var userInput = Console.ReadLine();
                foreach (var test in tests
                    .Where(test => userInput.Equals(test.TestName)))
                {
                    test.PrintDetails();
                    goto main;
                }
                foreach (var test in variableTests
                    .Where(test => userInput.Equals(test.TestName)))
                {
                    test.PrintDetails();
                    goto main;
                }

                message = "[red]Invalid Input!\nEnter the name of a test to see the test details![/]";
            } while (true);
        }
    }
    
    /** <summary>A class that represents a test for the mode calc.
     *  See <see cref="CalcTest"/></summary>
     */
    public class CalcTest
    {
        public string TestName { get; private set; }
        
        public Expression Expression { get; private set; }
        
        public bool ExpectedValidity { get; private set; }
        
        public bool GotValidity { get; private set; }
        
        public double? ExpectedResult { get; private set; }
        
        public double? GotResult { get; private set; }
        
        public bool IsResultCorrect { get; private set; }
        
        public string[] TestsWhat { get; private set; }
        
        public bool AddBreak { get; private set; }

        /** <param name="testName">Defines the name of the test. This will be used to access its details.
         * If you try to access tests with the same name, the one that is placed earlier in the list will be picked.</param>
         * <param name="expression">Defines the expression of the test. It can be valid or invalid, depending on what
         * is being tested</param>
         * <param name="expectedValidity">Defines the expected validity of the expression. The expression's validity
         * must match the expected validity to have a proper test.</param>
         * <param name="expectedResult">Defines the expected result upon solving the equation. If the expression is invalid
         * (i.e. expectedValidity = false),the expected result should be null</param>
         * <param name="addBreak">Defines whether or not an extra line will be placed after all the tests are displated
         * in list form. It is optional and purely aesthetic and has nothing to do with the performance of the test.</param>
         * <param name="testsWhat">Defines what "tags" or what "parts" the test is supposed to be testing. It is
         * optional and purely aesthetic and has nothing to do with the performance of the test.</param>
         */
        public CalcTest(string testName, string expression, bool expectedValidity, double? expectedResult,
            bool addBreak, string[] testsWhat)
        {
            TestName = testName;
            Expression = new Expression(expression.Trim());
            ExpectedValidity = expectedValidity;
            ExpectedResult = expectedResult;
            TestsWhat = testsWhat;
            AddBreak = addBreak;
        }

        /** <summary>First checks if the expression's validity matches the expected validity. If they do not match,
         * then the test fails and the expression is not computed. If the expected and got validities are false,
         * the test passes and returns. If the expected and got validities are true, the result is computed.
         * If the expected and got results are equal, then test passes; otherwise, the test will fail.</summary>
         */
        [Test]
        public void Run()
        {
            //Assert the validity of the expression
            GotValidity = Expression.IsValid;
            if (GotValidity != ExpectedValidity)
            {
                GotResult = null;
                if (ExpectedResult is null)
                {
                    IsResultCorrect = true;
                    return;
                }

                IsResultCorrect = false;
            }
            
            if (ExpectedResult is null)
            {
                GotResult = null;
                IsResultCorrect = true;
                return;
            }
            
            //Assert the result of the expression, if and only if the expression is valid
            try
            {
                GotResult = Expression.Solve();
                Assert.AreEqual(ExpectedResult, GotResult);
                IsResultCorrect = true;
            }
            catch (Exception)
            {
                IsResultCorrect = false;
            }
        }

        public bool IsPassed()
        {
            return ExpectedValidity == GotValidity && IsResultCorrect;
        }

        public void PrintDetails()
        {
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.WriteLine("[Press enter to go back to the main menu]");
            Console.ResetColor();
            AnsiConsole.MarkupLine($">  {TestName} :{(IsPassed() ? "[green]" : "[red]")}" +
                                   $"{(IsPassed() ? "Passed" : "Failed")}[/]:\n");
            AnsiConsole.MarkupLine($"    Expression(string):      {Expression.InputExpression}");
            Console.WriteLine("    Expression(string[]):\n    [{0}]", string.Join(", ", Expression.ExpressionList.ToArray()));
            
            AnsiConsole.MarkupLine($"\n    Expected Validity:       {ExpectedValidity}");
            AnsiConsole.MarkupLine($"    Got Validity:            {(GotValidity == ExpectedValidity ? "[green]": "[red]")}{GotValidity}[/]");
            
            AnsiConsole.MarkupLine($"\n    Expected Result:         {ExpectedResult}");
            AnsiConsole.MarkupLine($"    Got Result:              {(IsResultCorrect ? "[green]" : "[red]")}{GotResult}[/]");
            
            AnsiConsole.WriteLine("\n[\"{0}\"]", string.Join("\", \"", TestsWhat));

            Console.ReadLine();
        }
    }
}