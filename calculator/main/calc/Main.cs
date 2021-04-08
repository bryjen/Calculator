using System;
using Spectre.Console;

namespace calculator.main.calc
{
    public class Main
    {
        public static void Enter()
        {
            var start = DateTime.Now;
            ExpressionManipulation.GetExpressionInListForm("-3(-5a + someVariable - 400(-5 + 99a)) + -(5-2)");
            var duration = DateTime.Now - start;
            AnsiConsole.WriteLine($"{duration.TotalMilliseconds}ms");
        }
        //test cases:
        //-3(-5a + someVariable - 400(-5 + 99a))
        //3 + 8  ((4 + 3) * 2 + 1) - 6 / (2 + 1)

        public static void PrintArray(string[] array)
        {
            AnsiConsole.WriteLine("[{0}]", string.Join(", ", array));
        }
    }
}