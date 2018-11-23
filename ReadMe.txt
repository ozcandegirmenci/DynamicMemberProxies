A helper project for invoking Type members (Fields, Methods, Properties and Constructors) dynamically using DynamicMethods.
The main aim of this project is increasing reflection operations performance by generating dynamic methods which has IL's for calling type members on the fly like as in the standart code.

Project has 4 proxy classes which you can use as follows;
1 – RuntimeConstructorProxy: Generates a dynamic method for invoking both parameterless and parameterized constructors of a type to create instance of that type dynamically.
2 – RuntimeMethodProxy: Generates a dynamic method for invoking a type method dynamically. It supports both static and instance methods.
3 – RuntimeFieldProxy: Generates dynamic methods for getting and setting value of a type field dynamically. It supports both static and instance fields.
4 – RuntimePropertyProxy: Generates dynamic methods for getting and setting value of a type property dynamically. It supports both static and instance properties. It also supports indexed properties too.

PS: Because of the ref and out keywords are not supported in my code, you will get an error if you try to use DynamicMethodProxy with methods which has ref or out parameter(s).