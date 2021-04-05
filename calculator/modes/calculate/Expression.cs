using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace calculator.modes.calculate
{
    public static class Expression
    {

        private static string[] validOperators = new string[]
        {
            "+", "-"
        };
        
        public static bool IsALoneVariable(string[] expressionArray)
        {
            return expressionArray.Length == 1 && !IsANumber(expressionArray[0]);
        }
        
        public static bool IsAValidExpression(string expression)
        {
            /*
                Parameters of a valid Expression:
              -   All variables in the expression must be already defined/assigned
              -   The expression must end with a non-operator
             
                Additional Remarks:
              -   The function will return true if the expression is a lone variable
             ex: "variableName", where variableName was previously assinged a value;
             
                
                    First the function splits and trims the expression into its terms
                ex: "123 + abc" => ["123", "abc"].
                    If the last part of the expression ends in either a '+' or a '+', then the array would end with an
                empty string
                ex: "abc + 123+" or "abc + 123-" =>  ["abc", "123", ""]
                If the last term is an empty string, then the expression is invalid (Returns false)
                
                    Then the function checks if the expression is a lone variable - ex: "variableName". This is a
                special case as the function does not need to check if the variable is defined or not.
                
                    After so, the function loops through all terms and checks if any variables are undefined. If so,
                the expression is invalid (Returns false)
             */
            
            var terms = expression.Split(new char[] {'+', '-'});
            terms = terms.Select(email => email.Trim()).ToArray();
            
            if (terms[^1].Equals("")) return false;
            
            if (terms.Length == 1 && !Regex.IsMatch(expression, ".*\\d.*")) return true;
            
            return terms.Where(term => !term.Equals(""))
                .Where(term => !IsANumber(term))
                .All(term => Variables.GetValueOfVariable(term) is not null);
        }

        public static string SolveExpression(string expression)
        {
            /*
                How the function "solves" the equation:
                
                    The function first simplifies the equation. For example, "9 +++ 10 -- 8" becomes "9 + 10 + *" 
                (check Expression.Simplify() for more details).
                    Then the function checks if the expression is a lone variable - ex. "variableName", where variableName
                is defined -. If so, if the lone variable has a value, it will return the root value (a number), otherwise, 
                an error will be thrown
                
                    After that, all the variables in the expression are solved (recursively). For example, lets say a = 5,
                and b = a + 5. The expression b + 20 will be solved as follows:
                
                b (10)   <=  a (5)   <= 5
                +            +       
                20           5
                =            =
                30           10
             */
            
            expression = Simplify(expression);
            
            var expressionArray = GetExpressionInArrayForm(expression);
            
            if (IsALoneVariable(expressionArray))return GetVariableValue(expression);
            
            for (int i = 0; i < expressionArray.Length; i++)
            {
                if (!IsAVariable(expressionArray[i])) continue;
                expressionArray[i] = SolveExpression(expressionArray[i]);
                i = -1;
            }

            double result = 0;
            result = double.Parse(expressionArray[0]);
            for (int i = 1; i < expressionArray.Length; i+= 2)
            {
                try
                {
                    if (!IsAValidOOperator(expressionArray[i])) throw new Exception();  //to goto the catch block
                    result = Compute(result, Double.Parse(expressionArray[i + 1]), expressionArray[i]);
                }
                catch (Exception)
                {
                    Console.WriteLine($"Invalid format! : \"{expression}\"");
                    return "";
                }
            }
            
            return result.ToString(CultureInfo.InvariantCulture);
        }

        private static string Simplify(string expression)
        {
            //turns '--' into '+'
            expression = new Regex("\\-{2}").Replace(expression, "+");
            
            //removes extra '+'
            expression = new Regex("\\++").Replace(expression, "+");
            
            //turns '-+' or '+-' into '-'
            expression = new Regex("(\\+\\-)|(\\-\\+)").Replace(expression, "-");

            return expression;
        }
        
        private static string[] GetExpressionInArrayForm(string expression)
        { 
            /*      Works almost the same way as string.ToCharArray(), but numbers and variables are grouped and the
                array is of string type, not char. For example:
                "abc + 123", where abc is defined => ["abc", "+", "123"]
            */
            
            
            //gets the expression into a string array of each of its individual letters/symbols
            var tempCharArray = new Regex(" +").Replace(expression, "")
                .ToCharArray();
            var expressionLetterArray = new string[tempCharArray.Length];
            for (var i = 0; i < tempCharArray.Length; i++)
            {
                expressionLetterArray[i] = tempCharArray[i].ToString();
            }

            //groups non-operators together 
            //ex: [a, b, c, +, 1, 2, 3] => [abc, +, 123]
            List<string> terms = new List<string>();
            StringBuilder termBuilder = new StringBuilder();
            foreach (var letter in expressionLetterArray)
            {
                if (IsAValidOOperator(letter))
                {
                    terms.Add(termBuilder.ToString().Equals("") ? "0" : termBuilder.ToString());
                    terms.Add(letter);

                    termBuilder.Clear();
                    continue;
                }

                termBuilder.Append(letter);
            }
            terms.Add(termBuilder.ToString());
            
            return  terms.ToArray();
        }

        private static double Compute(double number1, double number2, string operand)
        {
            switch (operand)
            {
                case "+" :
                    return number1 + number2;
                case "-" :
                    return number1 - number2;
                default:
                    throw new Exception("Error at Expresssion.Compute() - An unknown operand was parsed");
            }
        }

        private static string GetVariableValue(string variableName)
        {
            /*
                If the variable is not defined, casting "Variables.GetValueOfVariable" would return null. Hence, if
            the returned value is null, the function will throw an error.
            
                If the variable returns another variable, the function will recursively get to the root variable and 
            will return a number
             */
            
            var value = Variables.GetValueOfVariable(variableName);
            
            if (value is not null)
            {
                if (IsANumber(value)) return value;

                if (IsALoneVariable(validOperators)) return GetVariableValue(value);

                return SolveExpression(value);

                //return IsAVariable(value) ? GetVariableValue(value) : value;
            }
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Unknown variable!");
            Console.ResetColor();
            return "";
        }

        

        

        private static bool IsANumber(string term)
        {
            return double.TryParse(term, out _);
        }

        private static bool IsAValidOOperator(string term)
        {
            return validOperators.Any(term.Equals);
        }
        
        private static bool IsAVariable(string term)
        {
            return !IsANumber(term) && !IsAValidOOperator(term);
        }
    }
}