# Calculator
## About
Calculator is a simple scientific console calculator capable of handling complex arithmetic expressions along with some basic trigonometric functionality.

### Evaluating Expressions
Upon launching the application, the program will be in solve/calc mode, where the user can enter equations for the program to solve. The expressions must be valid, otherwise, the program will display an error message.

```c++
	> 3 + 8 * ((4 + 3) * 2 + 1) - 6 / (2 + 1)
	121
	> 1 + 2 ^ 5
	33
	> (( 4 + 1
	Invalid expression! Invalid parentheses order!
```

### Variables
The user can assign values to variables and can, consequently, create functions with nested variables. For example, in trying to find the root of the equation $x^2 + x - 3 = 0$ using Newton's iterative method, you can use the calculator's variable functionality.

By definition, $x_{n+1} = x_{n} - \frac{f(x)}{f\prime(x)}$, which means $x_{n+1} = x_{n} - \frac{x^2 + x - 3}{2x + 1}$

Letting x = 10 be a starting point, we can solve the problem as follows:

```c++
	> 10
	10
	> func = ans - (ans^2 + ans - 3)/(2ans + 1)
	Assigned!
	> func
	4.904761904761905
	> func
	2.503041745332494
	> func
	1.542638891596777
	> func
	1.316858983738793
	> func
	1.3028302211191816
	
	...
```

It is important to note that nested variables are dynamic, meaning that if a variable in a variable changes, the value of the parent variable changes as well. Another important note is that the variable 'ans' takes the value of the last computed expression and can be used in a variable as well - as demonstrated above.

### Trigonometric Functions
#### Changing the Angle
By default, the angle is set to degrees. This can be changed by entering

```c++
	> /angle rad	//or radians
	 or
	> /angle deg	//or degrees
```

Due to round-off errors using C#'s Math's class, not all answers will be 100% precise, as demonstrated below.

```c++
	> sin 30
	0.49999999999999994
	> tan 45
	0.9999999999999999
	> sin 90
	1
	> cos90
	6.123233995736766E-17					//which is basically 0 at this point
```

### Other Functions

```c++
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
```

## Medium
Made solely using C#.

## Process
As charming and function packed as the default Windows calculator was, it was always missing the support for copy paste solving expressions - like $3 + 8 * ((4 + 3) * 2 + 1) - 6 / (2 + 1)$. So this project started with that.

I first had to develop a system to split an equation into its components: $3(4+func)$ = { 3, \*, (, 4, +, func, ) }. Using mainly Regex, I was able to achieve turning a String equation into a String list with all corresponding components, adding extra components if necessary (like adding a * like the example above). After that, I coded an algorithm that changes the order of the components in the list from infix to postfix notation. I then used stacks to create a method that solves postfix expressions.

From there, I decided to add more features, like being able to assign variables. I had to add a hashmap that stores initialized variable names and their corresponding value. After that, before an expression gets solved, the program goes through all components, checking if it is a variable and then substituting its value if initialized. If not, the program throws an error.

After that, I added less important features, such as trigonometric functions; sqrt, cbrt, qtrt functions; etc.

## Download Instructions

N/A WAIT
