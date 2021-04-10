using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Spectre.Console;

namespace calculator.main
{
    public static class Variables
    {
        private static readonly Dictionary<string, Expression> variables = new Dictionary<string, Expression>();
        private static readonly Dictionary<string, Expression> constants = new Dictionary<string, Expression>
        {
            { "pi", new Expression($"{Math.PI}") },
            { "e", new Expression($"{Math.E}") },
            { "tau", new Expression($"{Math.Tau}") }
        };

        /**<summary>Assigns a value (string + Expression()) into variables (dictionary). Outputs "Invalid Assignment!"
         * if assignment is invalid and exits/returns the function</summary>
         *
         * <remarks>The assignment is invalid if fails to meet any one of the following criteria:
         * <list type="bullet">
         * <item><term>If the expression contains more than one "=" characters</term></item>
         * <item><term>If the variables name contains no spaces and no numbers</term></item>
         * <item><term>If the expression is valid, see <see cref="Expression.IsExpressionValid()"/></term></item>
         * </list>
         * </remarks>
         */
        public static void Assign(string assignmentExpression)
        {
            #region CHECK_IF_THE_ASSIGNMENT_IS_CORRECT

                var variableAndExpression = assignmentExpression.Split("=");
    
                //the assignment expression can only have one equal
                if (variableAndExpression.Length != 2)
                {
                    AnsiConsole.MarkupLine("[red]Invalid Assignment![/]");
                    return;
                }
    
                var variable = variableAndExpression[0].Trim();
                var expression = new Expression(variableAndExpression[1].Trim());
    
                //checks if the variable's name is valid (no spaces, no numbers)
                variable = new Regex(" +").Replace(variable, " ");
                if (Regex.IsMatch(variable, "\\d+") || variable.Contains(" "))
                {
                    AnsiConsole.MarkupLine("[red]Invalid Assignment![/]");
                    return;
                }

                //checks if the expression is valid
                if (!expression.IsValid)
                {
                    AnsiConsole.MarkupLine("[red]Invalid Assignment![/]");
                    return;
                }
                
                //checks if the variable is already defined as a constant
                if (constants.ContainsKey(variable.Trim().ToLower()))
                {
                    AnsiConsole.MarkupLine($"[red]\"{variable}\" is already defined as a constant![/]");
                    return;
                }

            #endregion
            
            variables[variable] = expression;
            AnsiConsole.MarkupLine("[blue]Assigned![/]");
        }

        public static double? GetValue(string variable)
        {
            if (variables.ContainsKey(variable))
            {
                var value = variables[variable];
                return Double.TryParse(value.InputExpression, out _) ? Double.Parse(value.InputExpression) : value.Solve();
            }
            
            variable = variable.ToLower();     //constant names will be case insensitive
            if (constants.ContainsKey(variable))
            {
                var value = constants[variable];
                return Double.TryParse(value.InputExpression, out _) ? Double.Parse(value.InputExpression) : value.Solve();
            }

            return null;
        }
    }
}