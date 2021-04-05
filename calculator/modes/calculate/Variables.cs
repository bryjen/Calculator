using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text.RegularExpressions;

namespace calculator.modes.calculate
{
    public static class Variables
    {
        //stores the key and an expression
        private static Dictionary<string, string> variables = new Dictionary<string, string>();
        public static void Assign(string assignExpression)
        {
            /*
                Assigning a variable consists of the following syntax: identifier = value. An assignment command is
            valid if and only if:
                -   The variable name is a single word
                -   The variable name consists of only letters // no numbers must be present
                
                -   Other variables and expressions can be set as the value if and only if the expression is valid,
                which means that:
                    -   it must be in a correct format
                    -   all variables used are defined variables
                    -   etc. (goto Expression.IsAValidExpression()
                
                Notes:
                -   When assigned values, the expression is not solved to allow values to change dynamically
                ex: > a = 10
                    > b = a + 5
                    > b
                    15
                    > a = 5
                    > b
                    10
             */

            assignExpression = assignExpression.Replace(" ", "");
            
            if (!IsAValidAssignment(assignExpression))
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Invalid assignment:" +
                                  "\n   - The identifier can only contain letters with no spaces" +
                                  "\n   - The value must be a valid expression (where all variables are defined)");
                Console.ResetColor();
                return;
            }
            
            var array = assignExpression.Split("=");
            if (variables.ContainsKey(array[0]))
            {
                variables[array[0]] = array[1];
                return;
            }
            variables.Add(array[0], array[1]);

        }

        public static string GetValueOfVariable(string variableName)
        {
            try
            {
                return variables[variableName];
            }
            catch (Exception)
            {
                return null;
            }
        }

        private static bool IsAValidAssignment(string assignExpression)
        {
            /*
             Check Variables.Assign(string assignExpression) to see what makes an assignment valid
             */
            
            var array = assignExpression.Split("=");
            var identifier = array[0];
            var value = array[1];

            if (array.Length != 2) return false;

            if (Regex.IsMatch(identifier, ".*\\d.*")) return false;   //if the identifier contains any numbers

            if (!Expression.IsAValidExpression(value)) return false;
            
            if (Expression.IsALoneVariable(value.Split(new char[]{'+', '-'})) &&
                !variables.ContainsKey(value)) return false;
            

            return true;
        }
    }
}