using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using calculator.main.calc;
using CommandLine;

namespace calculator.main
{
    public class Expression
    {
        public string InputExpression { get; private set; }
        
        public List<string> ExpressionList { get; private set; }
        
        public bool Valid { get; private set; }

        public Expression(string inputExpression)
        {
            InputExpression = inputExpression;

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

        public double Solve()
        {
            return 48;
        }

        private bool IsExpressionValid()
        {
            #region SPECIFIC_CASES
            
            

            #endregion

            return true;
        }
    }

    internal static class ExpressionManipulation
    {
        internal static List<string> GetExpressionInListForm(string expression)     //Can thrown an exception. If it does, expression is invalid
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

            //Main.PrintArray(expressionList.ToArray());
            return expressionList;
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