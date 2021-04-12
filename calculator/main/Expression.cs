using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CommandLine;
using Spectre.Console;

namespace calculator.main
{
    public class Expression
    {
        public string InputExpression { get; }
        
        /** <value>The list form of the InputExpression - from <see cref="ExpressionManipulation.GetExpressionInListForm(string)"/></value>*/
        public List<string> ExpressionList { get; }
        
        public bool IsValid { get; }

        public Expression(string inputExpression)
        {
            InputExpression = inputExpression.Trim();

            try
            {
                ExpressionList = ExpressionManipulation.GetExpressionInListForm(inputExpression);
                IsValid = IsExpressionValid();
            }
            catch (Exception)
            {
                IsValid = false;
            }
        }
        
        /** <summary>If the expression is valid, the method solves the expression. Otherwise, it will return null
         * if the method is called and the validity of the expression is false, it will return null </summary>
         *
         * <remarks>The method turns the expression (which is in infix notation) into its postfix expression using the
         * <see cref="ExpressionManipulation.InfixToPostfix(string[])"/> function, then solves the expression using
         * a postfix expression solving algorithm described 
         * <a href="https://runestone.academy/runestone/books/published/pythonds/BasicDS/InfixPrefixandPostfixExpressions.html">here</a>.
         * </remarks>.
         */
        // ReSharper disable once CognitiveComplexity
        public double? Solve()
        {
            if (!IsValid)
            {
                AnsiConsole.MarkupLine("[red]Cannot solve expression as it is invalid![/]");
                return null;
            }
            
            //If the expression is a lone number, output it back
            if (CheckMethods.IsANumber(InputExpression))
            {
                var value = double.Parse(InputExpression);
                Variables.SetAns(value.ToString());
                return value;
            }

            //If the expression is a lone variable, output its value back
            if (CheckMethods.IsAVariable(InputExpression) && ExpressionList.Count == 1)
            {
                var value = Variables.GetValue(InputExpression);
                if (value is not null)
                {
                    Variables.SetAns(value.ToString());
                    return value;
                }
                AnsiConsole.MarkupLine($"[red]The variable \"{InputExpression}\" is not initialized![/]");
                return null;
            }
            
            //Creates a copy of the list int o a string array
            var tempExpressionList = new string[ExpressionList.Count];
            for (var i = 0; i < ExpressionList.Count; i++)
            {
                tempExpressionList[i] = ExpressionList[i];
            }

            //substitutes the values of the variables in the expression. At this point, ALL variables are defined (checked).
            //A copy is used so the expression can mamintain its variables, so the value of a variable can change if the 
            //vaues in its variables (if it has any) change.
            for (var i = 0; i < tempExpressionList.Length; i++)
            {
                if (!CheckMethods.IsAVariable(tempExpressionList[i])) continue;
                tempExpressionList[i] = Variables.GetValue(tempExpressionList[i]).ToString();
            }

            var postfixExpression = ExpressionManipulation.InfixToPostfix(tempExpressionList);
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

            if (operandStack.Count == 1)
            {
                var value = operandStack.Pop();
                Variables.SetAns(value.ToString());     //sets the "ans" variable to this value
                return value;
            }
            
            AnsiConsole.MarkupLine("[red]Invalid Expression (at Solve)[/]");
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
        // ReSharper disable once CognitiveComplexity
        private bool IsExpressionValid()
        {
            #region SPECIFIC_CASES

                //Lone number case
                if (CheckMethods.IsANumber(InputExpression)) return true;

                //Lone variable case
                if (CheckMethods.IsAVariable(InputExpression) && ExpressionList.Count == 1) return true;

            #endregion
            
            //checks if the last character is an operator
            if (CheckMethods.IsAValidOperator(InputExpression[^1].ToString()) || 
                CheckMethods.IsATrigonometricFunction(ExpressionList[^1]))
            {
                AnsiConsole.MarkupLine("[red]Invalid expression! The expression ends with an operator/trigno. func![/]");
                return false;
            }
            
            //if all the values in the list are numbers separated with spaces like "1213 321 123" 
            if (ExpressionList.All(CheckMethods.IsANumber))
            {
                AnsiConsole.MarkupLine("[red]Invalid expression![/]");
                return false;
            }

            //if there are stacked operators that are neither pluses or minuses
            if (Regex.IsMatch(InputExpression, ".*[*\\/\\^]{2,}.*"))
            {
                AnsiConsole.MarkupLine("[red]Invalid expression![/]");
                return false;
            }

            
            //if the user tries to use "ans" without it being initialized
            if (ExpressionList
                .Where(token => CheckMethods.IsAVariable(token))
                .Where(token => token.Equals("ans"))
                .Any(token => Variables.GetValue("ans") is null))
            {
                AnsiConsole.MarkupLine($"[red]\"ans\" has not been initialized yet.[/]");
                return false;
            }

            //checks if each of the variables are initialized
            foreach (var token in ExpressionList
                .Where(token => CheckMethods.IsAVariable(token))
                .Where(token => !CheckMethods.IsATrigonometricFunction(token))
                .Where(token => Variables.GetValue(token) is null))
            {
                AnsiConsole.MarkupLine($"[red]The variable \"{token}\" is not initialized![/]");
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
        /** <summary>Returns the priority of an operator. Used to correctly order operators in
         * <see cref="ExpressionManipulation.InfixToPostfix(string[])"/>.</summary>
         */
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
        
        /** <summary>Returns the value of number1 @operator number2</summary>
         */
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

                //the function can increment this to when it opens a parentheses, then at certain parts in the function,
                //if this != 0, the function will decrement it then add a right ')' parentheses
                var closeParenthesis = 0;

                List<string> expressionList = new List<string>();
                StringBuilder termBuilder = new StringBuilder();
                foreach (var token in expressionArray)
                {
                    if (CheckMethods.IsAValidOperator(token) || token.Equals("(") || token.Equals(")"))
                    {
                        if (!termBuilder.ToString().Equals(""))
                            expressionList.Add(termBuilder.ToString());

                        //groups the term after the power operator, so that if it were to be split, it would still calculate correctly
                        //ex. 5^2x => 5^(2*x) WITHOUT: 5^2*x
                        if (token.Equals("^"))
                        {
                            if (closeParenthesis != 0)
                            {
                                expressionList.Add(")");
                                closeParenthesis--;
                            }
                            
                            closeParenthesis++;
                            expressionList.Add(token);
                            expressionList.Add("(");
                            
                            termBuilder.Clear();
                            continue;
                        }
                        
                        //cloeses the parenthesis
                        if (closeParenthesis != 0)
                        {
                            expressionList.Add(")");
                            closeParenthesis--;
                        }

                        expressionList.Add(token);
                        termBuilder.Clear();
                        continue;
                    }

                    if (CheckMethods.IsATrigonometricFunction(termBuilder.ToString()))
                    {
                        expressionList.Add(termBuilder.ToString());
                        expressionList.Add("(");
                        termBuilder.Clear();

                        closeParenthesis++;
                        
                        termBuilder.Append(token);
                        continue;
                    }

                    //if the term in the term builder is a number, and the token to be appended is a letter, it means that
                    //the next token will be a variable. So it appends the number and a '*' to the list then clears the
                    //termbuilder
                    if (CheckMethods.IsANumber(termBuilder.ToString()) &&
                        Regex.IsMatch(token, "[a-zA-Z]"))
                    {
                        expressionList.Add(termBuilder.ToString());
                        expressionList.Add("*");
                        termBuilder.Clear();
                    }

                    termBuilder.Append(token);
                }
                expressionList.Add(termBuilder.ToString());
                expressionList.RemoveAll(term => term.Equals(""));

                for (var i = 0; i < closeParenthesis; i++) { expressionList.Add(")"); }
            
            #endregion
            
            #region ADD_*_WHERE_MISSING
    
                //Adds '*' if a number/variable is followed by a left parenthesis 
                for (var i = 1; i < expressionList.Count; i++)
                {
                    //prevents stuff like [sin, *, (, x, )]
                    if (expressionList[i].Equals("(") && CheckMethods.IsATrigonometricFunction(expressionList[i - 1])) continue;
                    
                    if (!(expressionList[i].Equals("(") && !CheckMethods.IsAValidOperator(expressionList[i - 1])
                        && !expressionList[i-1].Equals("("))) continue;
                    expressionList.Insert(i, "*");
                }
                
                //placed into the first region

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

                //checks for '-'s, if there is a non '+' operator that preceeds it and a number that follows it, the negative
                //will group the negative sign and the number
                for (var i = 1; i < expressionList.Count; i++) 
                {
                    if (expressionList[i].Equals("-") && (!expressionList[i - 1].Equals("+") && CheckMethods.IsAValidOperator(expressionList[i - 1]))
                                                      && CheckMethods.IsANumber(expressionList[i + 1]))
                    {
                        expressionList[i] = $"-{expressionList[i + 1]}";
                        expressionList.RemoveAt(i + 1);
                    }
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

        /** <summary>Simplifies the given expression</summary>
         *
         * <remarks>The function only removes unecessary '+'s and '-'s.
         * <example>"3 +--++- 3" => "3 - 3"</example>
         * </remarks>
         */
        private static string GetSimplifiedVersion(string expression)
        {
            //turns '--' into '+'
            expression = new Regex("\\-{2}").Replace(expression, "+");
            
            //removes extra '+'
            expression = new Regex("\\++").Replace(expression, "+");
            
            //turns '-+' or '+-' into '-'
            expression = new Regex("(\\+\\-)|(\\-\\+)").Replace(expression, "-");

            if (Regex.IsMatch(expression, "(\\+\\-)|(\\-\\+)")) expression = GetSimplifiedVersion(expression);

            return expression;
        }
    }

    internal static class CheckMethods
    {
        internal static bool IsANumber(string token)
        {
            return Double.TryParse(token, out _);
        }

        /*TODO the function returns true when there is a variable in the expression, for ecample:
                x + 5 returns true
                3 + 8 * ((4 + 3) * 2 + 1) - 6 / (x + 1) returns true
         */
        internal static bool IsAVariable(string token)
        {
            return Regex.IsMatch(token, "[a-zA-Z]+");
        }

        internal static bool IsAValidOperator(string token)
        {
            var validOperators = new []
            {
                "+", "-", "*", "/", "%", "^"
            };
            return validOperators.Any(@operator => @operator.Equals(token));
        }

        internal static bool IsATrigonometricFunction(string token)
        {
            var functions = new[] { "sin", "cos", "tan", "csc", "sec", "cot", "arcsin", "arccos", "arctan" };
            return functions.Any(trignometricFunction => trignometricFunction.Equals(token));
        }
    }
}