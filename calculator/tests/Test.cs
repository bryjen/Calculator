using System;
using System.Collections.Generic;
using calculator.main;
using NUnit.Framework;
using Spectre.Console;
using static calculator.main.Expression;

namespace calculator.tests
{

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

        [Test]
        public void run()
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