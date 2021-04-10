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