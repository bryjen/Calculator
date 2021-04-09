using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using calculator.main.calc;
using CommandLine;
using Spectre.Console;

namespace calculator.main
{
    public class Expression
    {
        public string InputExpression { get; }
        
        public List<string> ExpressionList { get; }
        
        public bool Valid { get; }

        public Expression(string inputExpression)
        {
            InputExpression = inputExpression.Trim();

            try
            {
                ExpressionList = ExpressionManipulation.GetExpressionInListForm(inputExpression);
                Valid = IsExpressionValid();
            }
            catch (Exception)
            {
                Valid = false;
            }
        }
        
        /** <summary>If the expression is valid, the method solves the expression. Otherwise, it will return null
         * if the method is called and the validity of the expression is false, it will return null </summary>
         *
         * <remarks>The method turns the expression (which is in infix notation) into its postfix expression using the
         * <see cref="ExpressionManipulation.InfixToPostfix(string[] infixExpression)"/> function, then solves the expression using
         * a postfix expression solving algorithm described 
         * <a href="https://runestone.academy/runestone/books/published/pythonds/BasicDS/InfixPrefixandPostfixExpressions.html">here</a>.
         * </remarks>.
         */
        public double? Solve()
        {
            if (!Valid)
            {
                AnsiConsole.MarkupLine("[red]Cannot solve expression as it is invalid![/]");
                return null;
            }
            
            //If the expression is a lone number, output it back
            if (CheckMethods.IsANumber(InputExpression)) return double.Parse(InputExpression);

            //If the expression is a lone variable, output its value back
            if (CheckMethods.IsAVariable(InputExpression)) return null; //TODO find a way to return the value of that lone variable yknow

            var postfixExpression = ExpressionManipulation.InfixToPostfix(ExpressionList.ToArray());
            Stack<double> operandStack = new Stack<double>();
            foreach (var token in postfixExpression)
            {
                if (CheckMethods.IsANumber(token))
                {
                    operandStack.Push(double.Parse(token));
                    continue;
                }
                //if its not a number, it is an operator
                var number2 = operandStack.Pop();
                var number1 = operandStack.Pop();
                operandStack.Push(Computation.Compute(number1, number2, token));
            }

            if (operandStack.Count == 1) return operandStack.Pop();
            
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("SYNTAX ERROR!");
            Console.ResetColor();
            return null;
        }
        
        /** <summary>Returns true/false depending on whether the expession is valid or not (Respectively)</summary>
         *
         * <remarks>The region "SPECIFIC_CASES" detect whether an expression is a lone variable or a lone number.
         * <example>"someVariable" for a lone variable, and "-10" for a lone number</example>
         * Furthermore, the function also checks the following:
         * <list type="bullet">
         * <item><term>If the last character of the expression is an operator</term></item>
         * <item><term>If the all the values in the expression list are numbers separated by spaces</term></item>
         * <item><term>If the parenthesis are in the right order and properly formatted</term></item>
         * </list></remarks>
         */
        private bool IsExpressionValid()
        {   //TODO Add something that checks if all variables are defined
            
            #region SPECIFIC_CASES

                //Lone number case
                if (CheckMethods.IsANumber(InputExpression)) return true;
                
                //Lone variable case
                if (CheckMethods.IsAVariable(InputExpression)) return true;
            
            #endregion
            
            //checks if the last character is an operator
            if (CheckMethods.IsAValidOperator(InputExpression[^1].ToString()))
            {
                AnsiConsole.MarkupLine("[red]Invalid expression! The expression ends with an operator![/]");
                return false;
            }
            
            //if all the values in the list are numbers separated with spaces like "1213 321 123" 
            if (ExpressionList.All(CheckMethods.IsAValidOperator))
            {
                AnsiConsole.MarkupLine("[red]Invalid expression![/]");
                return false;
            }

            if (Regex.IsMatch(InputExpression, ".*[*\\/\\^]{2,}.*"))
            {
                AnsiConsole.MarkupLine("[red]Invalid expression![/]");
                return false;
            }

            //checks if the parentheses are in the correct order and properly formatted
            Stack<string> parentheses = new Stack<string>();
            foreach (var token in ExpressionList)
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

            if (parentheses.Count != 0)
            {
                AnsiConsole.MarkupLine("[red]Invalid expression! Invalid parentheses order![/]");
                return false;
            }

            //TODO add a method there that checks the expressions inside parenthesis to make sure they are correct

            return true;
        }
    }

    internal static class Computation
    {
        internal static int GetOperatorPriorityNumber(string token)
        {
            switch (token)
            {
                case "+":
                case "-":
                    return 1;
                case "*":
                case "/":
                case "%":    
                    return 2;
                case "^":
                    return 3;
                default:
                    throw new Exception($"An invalid token was attempted to be parsed! {token}");
            }
        }
        
        internal static double Compute(double number1, double number2, string @operator)
        {
            return @operator switch
            {
                "+" => number1 + number2,
                "-" => number1 - number2,
                "*" => number1 * number2,
                "/" => number1 / number2,
                "%" => number1 % number2,
                "^" => Math.Pow(number1, number2),
                _ => throw new Exception("invalid @operator was attempted to be parsed at Computation.Compute()")
            };
        }
    }

    internal static class ExpressionManipulation
    {
        /** <summary>Turns an expression into a string list of all its components (which includes numbers, variables, and
         * operators)</summary>
         * 
         * <remarks>The function also adds changes the expression where necessary. For example, adding multiplication operators
         * if it preceeds a left parenthesis, splitting a term into its coefficient and variable, proper joining of negative
         * numbers, etc. </remarks>
         *
         * <exception cref="Exception">To be clear, right now the the exact exceptions are not documented, but, if the function
         * ever throwns an exeption, the validity of the expression is set to false - as parsing a regular equation should
         * not throw an exception</exception>
         */
        // ReSharper disable once CognitiveComplexity
        internal static List<string> GetExpressionInListForm(string expression)
        {
            #region GET_EXPRESSION_IN_LIST_FORM
            
                expression = new Regex(" +").Replace(expression, "");
                expression = GetSimplifiedVersion(expression);
                var expressionArray = expression.ToCharArray().Select(c => c.ToString().Trim()).ToArray();

                List<string> expressionList = new List<string>();
                StringBuilder termBuilder = new StringBuilder();
                foreach (var token in expressionArray)
                {
                    if (CheckMethods.IsAValidOperator(token) || token.Equals("(") || token.Equals(")"))
                    {
                        if (!termBuilder.ToString().Equals(""))
                            expressionList.Add(termBuilder.ToString());
                        expressionList.Add(token);

                        termBuilder.Clear();
                        continue;
                    }

                    termBuilder.Append(token);
                }
                expressionList.Add(termBuilder.ToString());
                expressionList.RemoveAll(term => term.Equals(""));
            
            #endregion

            #region ADD_*_WHERE_MISSING
    
                //Adds '*' if a number/variable is followed by a left parenthesis 
                for (var i = 1; i < expressionList.Count; i++)
                {
                    if (!(expressionList[i].Equals("(") && !CheckMethods.IsAValidOperator(expressionList[i - 1])
                        && !expressionList[i-1].Equals("("))) continue;
                    expressionList.Insert(i, "*");
                }
                
                //Splits a variable and its coefficient, then adds a '*" between them
                for (var i = 0; i < expressionList.Count; i++)
                {
                    if (!Regex.IsMatch(expressionList[i], "\\d+[a-zA-Z]+")) continue;
                    var coefficient = Regex.Match(expressionList[i], "\\d+").Value;
                    var variable = new Regex("\\d+").Replace(expressionList[i], "");
                    expressionList.Insert(i, "*");
                    expressionList.Insert(i, coefficient);
                    expressionList[i + 2] = variable;
                }

            #endregion
            
            #region FIX_NEGATIVE_SIGNS_WHERE_NEEDED

                //Groups negative signs if a number is preceded by a left parenthesis or if it starts the expression
                if (expressionList[0].Equals("-") && (CheckMethods.IsANumber(expressionList[1]) || 
                    CheckMethods.IsAVariable(expressionList[1])))
                {
                    expressionList.RemoveAt(0);
                    expressionList[0] = $"-{expressionList[0]}";
                }
                for (var i = 0; i < expressionList.Count; i++)
                {
                    if (!expressionList[i].Equals("(")) continue;

                    if (!(expressionList[i + 1].Equals("-") && !CheckMethods.IsAValidOperator(expressionList[i + 2]))) continue;
                    expressionList[i + 1] = $"-{expressionList[i + 2]}";
                    expressionList.RemoveAt(i + 2);
                }

            #endregion

            if (expressionList[0].Equals("+")) { expressionList.RemoveAt(0); }

            //Main.PrintArray(expressionList.ToArray());
            return expressionList;
        }
        
        /** <summary>Turns an infix string[] expression to a postfix string list expression</summary>
         *
         * <remarks>The function uses a standard infix to postfix conversion algorithm described
         * <a href="https://runestone.academy/runestone/books/published/pythonds/BasicDS/InfixPrefixandPostfixExpressions.html">here</a>.
         * </remarks>.
         */
        // ReSharper disable once CognitiveComplexity
        internal static List<string> InfixToPostfix(string[] infixExpression)
        {
            Stack<string> opstack = new Stack<string>();
            List<string> postfixExpression = new List<string>();

            foreach (var token in infixExpression)
            {
                //if the token is a number, add it to the list
                if (CheckMethods.IsANumber(token))
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
                if (CheckMethods.IsAValidOperator(token))
                {
                    var priority = Computation.GetOperatorPriorityNumber(token);
                    do
                    {
                        if (opstack.Count == 0 ||opstack.Peek().Equals("(") || opstack.Peek().Equals(")"))
                        {
                            opstack.Push(token);
                            break;
                        }
                        
                        if (Computation.GetOperatorPriorityNumber(opstack.Peek()) >= priority)
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

            return postfixExpression;
        }

        private static string GetSimplifiedVersion(string expression)
        {
            //turns '--' into '+'
            expression = new Regex("\\-{2}").Replace(expression, "+");
            
            //removes extra '+'
            expression = new Regex("\\++").Replace(expression, "+");
            
            //turns '-+' or '+-' into '-'
            expression = new Regex("(\\+\\-)|(\\-\\+)").Replace(expression, "-");

            return expression;
        }
    }

    internal static class CheckMethods
    {
        internal static bool IsANumber(string token)
        {
            return Double.TryParse(token, out _);
        }

        internal static bool IsAVariable(string token)
        {
            return Regex.IsMatch(token, "[a-zA-Z]+");
        }

        internal static bool IsAValidOperator(string token)
        {
            var validOperators = new string[]
            {
                "+", "-", "*", "/", "%", "^"
            };
            return validOperators.Any(@operator => @operator.Equals(token));
        }
    }
}