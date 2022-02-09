# Calculator
## Evaluating expressions
Upon launching the application, it will put the user into the solve/calc mode, where the user types equations for the program to solve. The expressions must be valid, otherwise, the program will output nothing.

	> 3 + 8 * ((4 + 3) * 2 + 1) - 6 / (2 + 1)
	121
	> 1 + 2 ^ 5
	33
	> 1 + 3(
	>



## Variables
The user can assign values to variables (initialize variables) using the following syntax: "variable name = variable value". The variable name must be a single word consisting of only letters (so no numbers). The variables "variable" or "a" are valid variable names, while the variables "a variable", "variabl1" and "variable 1" are not valid variable names. Trying to assign values to these variables will result in an error.
For the variable value, it can be any valid expression, which means that it can be any number (like a = 5), or it can be a valid expression. An expression is valid when all variables in it are defined (i.e all the variables in them are initialized) and the equation's syntax is correct. The user can print the value of an assigned variable by entering its name. If the user enters a non-initialized variable, the program will throw an error message.

	> a = 5
	assigned!
	> b = 2 * (3 + 4) + 1
	assigned!
	> c2 = 1
	Invalid assignment					//variable names cannot have numbers
	> vari abl = 2001
	Invalid assignment					//variable names cannot have spaces
	> c = 2a + 1
	assigned!
	> d
	The variable "d" is not initialized!
	> d = c
	assigned!
	> d 
	11
	> e = 3 + (4 + -
	Invalid assignment					//the variable value is an invalid expression
If the value of a variable in an expression changes, the value of the expression changes as well.
	
	> a = 1
	assigned!
	> b = a + 1
	assigned!
	> c = b + 1
	assigned!
	> c
	3
	> a = 0
	assigned!
	> c
	2



## Constants
As of the moment, there are only three constants available: pi, tau, and e. They behave just as normal variables, but are non re-assignable (they are read only). Attempting to re-assign a constant will throw an error message.

	> pi
	3.141592653589793
	> PI
	3.141592653589793
	> pi = 22/7
	"pi" is already defined as a constant!



## Commands
As of right now, there are only four commands: /clear, /exit, /help, and /angle. **/clear** only clears the console. **/exit** exits the user from the application. **/help** displays some (helpful) information about the current mode. **/angle angleType** changes the calculator from radians to degrees. If the user attempts to switch to an unknown angle type, the program will throw an error message and not change its current angle type.

	> /angle radians
			or
	> /angle degrees




## Trigonometric Functions
The calculator has nine trigonometric functions: sin, cos, tan, csc, sec, cot, arcsin, arccos, arctan. The calculator's default angle is degrees. To use these functions, simply enter the function and then the desired input. This feature is somewhat buggy (as of now) since it sometimes does not return exact values.

	> /angle degrees
	Angle mode set to degrees/deg
	> sin 30
	0.49999999999999994
	> tan 45
	0.9999999999999999
	> sin 90
	1
	> cos90
	6.123233995736766E-17					//which is basically 0 at this point
	>										//enter to skip a line
	> /angle rad
	Angle mode set to radians/rad
	> sin 2pi
	-2.4492935982947064E-16					//which, again, is extremely close to zero
	> sin (pi/2)
	1
	> sin(pi/6)							
	0.49999999999999994
The feature does exist, but at the moment it is quite unreliable for more exact values. For example, in degrees, sin 45 is 1/√2 or √2/2,  but is outputted as 0.7071067811865476.



## Other functions
The program also has support for exponents, and radicals, with radial functions up to the quartic degree. That means that the program has the functions sqrt, cbrt, and qtrt, which stand for square root, cubic root, and the quartic root respectively. Alternatively, the user can have greater radicands by putting fractions into exponents. They behave similarly enough to the mentioned functions, but are somewhat more time consuming.
	
	> sqrt4
	2
	> cbrt 8
	2
	> qtrt 16
	2
	> 16^(1/4)
	2
	> 32^(1/5)
	2



### Remarks
The program is far from completed. What's meant by that is that it could be expanded more in the future. However, it serves its purpose enough to be considered somewhat of a finished prototype.
