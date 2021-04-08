using CommandLine;

namespace calculator
{
    class Program
    {
        class Settings
        {
            [Option('t', "tests",
                Required = false, Default = false,
                HelpText = "Enables/Disables Unit Testing mode")]
            public bool UnitTestMode { get; set; }
            
            
        }
        
        static void Main(string[] args)
        {
            var settings = Parser.Default.ParseArguments<Settings>(args).Value;
            if (settings.UnitTestMode)
            {
                tests.Main.Enter();
                return;
            }
            main.calc.Main.Enter();
        }
    }
}

/*
 DO (IN ORDER)
 -  IMPLEMENT NON-VARIABLE Expression.IsExpressionValid() and Expression.Solve()
 -  POLISH UP/REDO UNIT TESTING THINGY TO MAKE IT LOOK BETTER + BE ABLE TO BE MORE CONCISE + DISPLAY MORE INFO
 -  ADD VARIABLE SUPPORT IN DIRECTORY "variables". Make the code here cleaner and more readable + make it so that fetching values
    (which includes solving the expressions within them) in this class, instead of in the expression (is subject change)
 -  ADD VARIABLE SUPPORT TO Expression.IsExpressionValid() and Expression.Solve()
 
 Total time for these ^ around 3 days
 
 Then add the new features
    
*/

/*
v1.0
REWORK
    -   Using Spectre.Console to display output instead of the standard Console.Write(Line)() method.
    
    -   Using NUnit to create unit tests for the program
    (remarks: (1) the tests in the "tests" directory are only temporary and will be improved later. For now, it only serves
    as a test. (2) There is problem trying to use Rider's way of implementing unit tests, so I have to figure how to make them
    by myself)
    
    -   Using CommandLineParser to parse command line arguments. Right now, there are only one program argument, that being 
    "--t" or "--tests" (default: false, required: false) which will enable unit testing mode if set to true.
    Future arguments may include the ability to turn colors on/off, debug mode (which displays extra info like time taken for calculatrion,
    the list form of the equation, etc.), etc.
    
    -   Added Expression.GetExpressionInListForm(), an improved version of last version's Expression.GetExpressionInArrayForm()
        -   Multiplication signs are now automatically added when there is a left parenthesis following it:
            ("a(b)" => "a * (b)")
        -   Negative signs are now grouped if it is resenting negation (follows nothing/a left parenthesis)
            -3 + 2 used to give [-,3, +, 2], now [-3, +, 2]
        -   If a term consists of a variable and a (visible) coefficient, the function will split them
            5a => [5, *, a], for now, it doesn't matter if the variable is defined or not. That will be checked later
*/






