using System;
using System.Text.RegularExpressions;

using calculator.modes.commands;

namespace calculator.modes.calculate
{
    public class Run
    {
        public static void Enter()
        {
            mainLoop :
            do
            {
                var userInput = new Regex(" +").Replace(Console.ReadLine(), " ")
                    .Trim();

                if (InputIsACommand(userInput))
                {
                    Command.Execute(userInput, "calculate");
                    continue;
                }
                
            } while (true);
        }


        private static void ExecuteExpression(string expression)
        {
            
        }

        private static bool IsExpressionValid(string expression)
        {
            return false;
        }
        

        private static bool InputIsACommand(string userInput)
        {
            return userInput.StartsWith("/");
        }
    }
}

/*
var userInput = Console.ReadLine();
userInput = new Regex("\\-{2}").Replace(userInput, "+");  //two minuses make a plus
userInput = new Regex("\\++").Replace(userInput, "+");  //removes extra "+"
userInput = new Regex("(\\+\\-)|(\\-\\+)").Replace(userInput, "-"); //a plus and a minus make a minus
var numbers = new Regex(" +").Replace(userInput, " ")   //removes redundant spaces 
    .Trim()
    .Split(" ");

if (userInput.Equals("")) continue;
if (userInput.StartsWith("/"))
{
    ExecuteCommand(userInput);
    continue;
}
if (numbers.Length == 1)
{
    Console.WriteLine(userInput.Trim());
    continue;
}

var sum = int.Parse(numbers[0]);
for (var i = 1; i < numbers.Length; i += 2)
{
    try
    {
        if (numbers[i].Equals("+") || numbers[i].Equals("-"))
        {
            sum = Compute(sum, int.Parse(numbers[i + 1]), numbers[i]);
        }
        else
        {
            throw new Exception();  //goto to the catch block
        }
    }
    catch (Exception)
    {
        Console.WriteLine($"Invalid format! : \"{userInput}\"");
        goto main;
    }
}
Console.WriteLine(sum);
} while (true);
*/