using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace calculator.modes.calculate
{
    public static class Expression
    {

        public static bool IsAValidExpression(string expression)
        {
            /*
                Parameters of a valid Expression:
              -   All variables in the expression must be already defined/assigned
              -   The expression must end with a non-operator
              -   There must be an equal number or left and right side parentheses, and they must close properly
              
             
                Additional Remarks:
              -   The function will return true if the expression is a lone variable
             ex: "variableName", where variableName was previously assinged a value;
             */
            expression = Simplify(expression);
            var expressionArray = GetExpressionInArrayForm(expression);

            //if the expression is a lone variable, return true (whether it is defined or not)
            if (expressionArray.Length == 1 && !Regex.IsMatch(expression, ".*\\d.*")
                && !expression.Trim().Contains(" ")) return true;

            if (IsANumber(expression, true)) return true;

            if (expressionArray.Where(token => !IsANumber(token, false) && !IsAValidOperator(token) && !token.Equals(""))
                .Any(token => Variables.GetValueOfVariable(token) is null))
                return false;

            if (!Regex.IsMatch(expressionArray[^1], "[()]") && !IsANumber(expressionArray[^1], false)) return false;

            //if operators are stacked, ex. "3+/* 2"
            if (Regex.IsMatch(expression, ".*[+\\-*/]{2}.*")) return false;

            //returns false to cases like "243 31 32 1"
            if (IsANumber(new Regex(" +").Replace(expression, ""), true)) return false;

            //if the expression is just a number, and there are signs that trail/end it, return false. ex: "1-"
            if (Regex.IsMatch(expression.Trim(), "^\\d+[+\\-*/]+$")) return false;
            
            Stack<string> parentheses = new Stack<string>();
            foreach (var token in expressionArray)
            {
                switch (token)
                {
                    case "(":
                        parentheses.Push(token);
                        continue;
                    case ")" when !parentheses.TryPeek(out _):
                        return false;
                    case ")":
                        parentheses.Pop();
                        break;
                }
            }

            return parentheses.Count == 0;
            
            //TODO Add ways to check expressions inside parentheses in the future to further improve accuracy
        }


        public static string SolveExpression(string expression)
        {
            //[CASE] if the entire expression is a single number
            var temp = new Regex(" +").Replace(expression, " ");
            if (IsANumber(temp.Trim().Replace(" ", ""), true)) return expression;

            expression = Simplify(expression);
            var expressionArray = GetExpressionInArrayForm(expression);

            //[CASE] if the entire expression is a lone variable
            if (expressionArray.Length == 1 && !Regex.IsMatch(expressionArray[0], "^[0-9]*$"))
            {
                var value = GetVariableValue(expression);
                
                
                return value;
            }
            
            //substitute all the values of variables (if any)
            expressionArray = Substitute(expressionArray);

            expressionArray = InfixToPostfix(expressionArray);

            Stack<double> operandStack = new Stack<double>();
            foreach (var token in expressionArray)
            {
                if (IsANumber(token, false))
                {
                    operandStack.Push(double.Parse(token));
                    continue;
                }
                //if its not a number, it is an operator
                var number2 = operandStack.Pop();
                var number1 = operandStack.Pop();
                operandStack.Push(Compute(number1, number2, token));
            }

            if (operandStack.Count == 1) return operandStack.Pop().ToString();
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("SYNTAX ERROR!");
            Console.ResetColor();
            return "";
        }

        
        #region Expression Manipulation

        private static string[] GetExpressionInArrayForm(string expression)
        {

            var tempCharArray = new Regex(" +").Replace(expression, "")
                .ToCharArray();
            var expressionLetterArray = new string[tempCharArray.Length];
            for (var i = 0; i < tempCharArray.Length; i++)
            {
                expressionLetterArray[i] = tempCharArray[i].ToString();
            }
            
            List<string> terms = new List<string>();
            StringBuilder termBuilder = new StringBuilder();
            foreach (var letter in expressionLetterArray)
            {
                if (IsAValidOperator(letter))
                {
                    //this means that the previous and current terms are operators "... + ( ..."
                    if (!termBuilder.ToString().Equals("")) 
                        terms.Add(termBuilder.ToString());
                    terms.Add(letter);

                    termBuilder.Clear();
                    continue;
                }

                termBuilder.Append(letter);
            }
            terms.Add(termBuilder.ToString());
            terms.RemoveAll(term => term.Equals(""));
            
            return terms.ToArray();
        }
        
        //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //
        
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
        
        //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //  //

        private static string[] Substitute(string[] expression)
        {
            for (var i = 0; i < expression.Length; i++)
            {
                if (!IsANumber(expression[i], true) && !IsAValidOperator(expression[i]))  //i.e. if it is a variable
                {
                    expression[i] = GetVariableValue(expression[i]);
                    i = -1;
                }
            }

            return expression;
        }
        
        #endregion

        #region Infix to Postfix Conversion
        
        private static int GetOperatorPriorityNumber(string token)
        {
            switch (token)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                    return 2;
                case "^":
                    return 3;
                default:
                    throw new Exception($"An invalid token was attempted to be parsed! {token}");
            }
        }

        // ReSharper disable once CognitiveComplexity
        private static string[] InfixToPostfix(string[] infixExpression)
        {
            Stack<string> opstack = new Stack<string>();
            List<string> postfixExpression = new List<string>();

            foreach (var token in infixExpression)
            {
                //if the token is a number, add it to the list
                if (IsANumber(token, false))
                {
                    postfixExpression.Add(token);
                    continue;
                }

                //if the token is a left parenthesis, add it to the opstack
                if (token.Equals("("))
                {
                    opstack.Push(token);
                    continue;
                }

                //if the token is a right parenthesis, pop the opstack until the corresponding left parenthesis is reached
                if (token.Equals(")"))
                {
                    do
                    {
                        
                        var value = opstack.Pop();
                        if (value.Equals("(")) break;
                        postfixExpression.Add(value);
                    } while (true);
                    continue;
                }

                /*
                 If the token is an operator, check if there are any operators in the stack that have a higher or equal
                 priority level (defined by the GetOperatorPriorityNumber() function). If so append them to the list.
                 After doing so, push the operator into the list
                 */
                if (IsAValidOperator(token))
                {
                    var priority = GetOperatorPriorityNumber(token);
                    do
                    {
                        if (opstack.Count == 0 ||opstack.Peek().Equals("(") || opstack.Peek().Equals(")"))
                        {
                            opstack.Push(token);
                            break;
                        }
                        
                        if (GetOperatorPriorityNumber(opstack.Peek()) >= priority)
                        {
                            postfixExpression.Add(opstack.Pop());
                            continue;
                        }
                        opstack.Push(token);
                        
                        break;
                    } while (true);
                }
            }

            postfixExpression.AddRange(opstack);    //adds the remaining operators into the postfix expression

            return postfixExpression.ToArray();
        }
        
        #endregion


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
                if (IsANumber(value, true)) return value;

                var expressionArray = GetExpressionInArrayForm(value);
                if (expressionArray.Length == 1 &&
                    !Regex.IsMatch(expressionArray[0], "^[0-9]*$"))
                    return GetVariableValue(value);

                return SolveExpression(value);
            }
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Unknown variable!");
            Console.ResetColor();
            return "";
        }   //TODO REFACTOR

        private static double Compute(double number1, double number2, string @operator)
        {
            switch (@operator)
            {
                case "+":
                    return number1 + number2;
                case "-":
                    return number1 - number2;
                case "*":
                    return number1 * number2;
                case "/":
                    return number1 / number2;
                case "^":
                    return Math.Pow(number1, number2);
                default:
                    throw new Exception("invalid @operator was attempted to be parsed at Expression.Compute()");
            }
        }

        private static bool IsAValidOperator(string term)
        {
            var validOperators = new string[] {"+", "-", "*", "/", "^", "(", ")"};

            return validOperators.Any(@operator => @operator.Equals(term));
        }
        
        //TODO CLARIFY WHAT "SIGN" DOES
        private static bool IsANumber(string token, bool signSensitive)
        {
            return Regex.IsMatch(token, signSensitive ? "^[+-]?[0-9]*$" : "^[0-9]*$");
        }
        
    }
}