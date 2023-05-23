namespace Astrid;

class Program
{
    public static string sourceFile = "";

    static void Main(string[] args)
    {
        var now = DateTime.Now;
        foreach(var kvp in Parser.ExprOperatorsEquality)
        {
            Parser.ExprOperators.Add(kvp.Key, kvp.Value);
        }
        foreach(var kvp in Parser.ExprOperatorsAlgebraic)
        {
            Parser.ExprOperators.Add(kvp.Key, kvp.Value);
        }
        foreach(var kvp in Parser.ExprOperatorsBoolean)
        {
            Parser.ExprOperators.Add(kvp.Key, kvp.Value);
        }

        if(args.Length == 0)
        {
            Console.WriteLine("No input file given");
            return;
        } else {
            sourceFile = args[0];
        }

        // Tokenizer.TokenizeFromFile("./examples/Expression.as").ToList().ForEach((a) => Console.WriteLine(Tokenizer.GetTokenAsHuman(a)));

        Token[] tokens = Tokenizer.TokenizeFromFile(args[0]);

        // tokens.ToList().ForEach(e => Console.WriteLine(Tokenizer.GetTokenAsHuman(e)));

        // Parser.ParseExpression(tokens).ForEach(e => Console.WriteLine(Tokenizer.GetTokenAsHuman(e)));        

        var parsed = Parser.ParseBlock(tokens);

        // PrintAST(parsed.Item2);

        // Console.WriteLine("Run: =======================");
        
        Func<List<Token>, object> printFunc = 
            (List<Token> parameters) => 
            {
                Console.WriteLine(parameters[0].value);
                return null!;
            };

        Func<List<Token>, object> printTokenFunc = 
            (List<Token> parameters) => 
            {
                Console.WriteLine(Tokenizer.GetTokenAsHuman(parameters[0]));
                return null!;
            };

        Func<List<Token>, object> inputFunc = 
            (List<Token> parameters) => 
            {
                Console.Write(parameters[0].value);
                string a = Console.ReadLine()!;
                return new TokenString(a, 0, 0, 0, 0);
            };

        Func<List<Token>, object> strFunc = 
            (List<Token> parameters) => 
            {
                return new TokenString(parameters[0].value, parameters[0].lineStart, parameters[0].lineEnd, parameters[0].charStart, parameters[0].charEnd);
            };

        Func<List<Token>, object> intFunc = 
            (List<Token> parameters) => 
            {
                return new TokenInt(parameters[0].value, parameters[0].lineStart, parameters[0].lineEnd, parameters[0].charStart, parameters[0].charEnd);
            };

        Func<List<Token>, object> floatFunc = 
            (List<Token> parameters) => 
            {
                return new TokenFloat(parameters[0].value, parameters[0].lineStart, parameters[0].lineEnd, parameters[0].charStart, parameters[0].charEnd);
            };

        Interpreter.Run(parsed.Item2, new(), new() {
            {
                "print",
                (
                    new() {
                        ("message", Types.String)
                    },
                    printFunc,
                    null
                )
            },
            {
                "printtoken",
                (
                    new() {
                        ("message", Types.String)
                    },
                    printTokenFunc,
                    null
                )
            },
            {
                "input",
                (
                    new() {
                        ("message", Types.String)
                    },
                    inputFunc,
                    Types.String
                )
            },
            {
                "str",
                (
                    new() {
                        ("from", Types.String)
                    },
                    strFunc,
                    Types.String
                )
            },
            {
                "int",
                (
                    new() {
                        ("from", Types.String)
                    },
                    intFunc,
                    Types.Int
                )
            },
            {
                "float",
                (
                    new() {
                        ("from", Types.String)
                    },
                    floatFunc,
                    Types.Float
                )
            }
        });

        // Console.WriteLine($"Time elapsed: {DateTime.Now.Subtract(now).TotalSeconds}");
    }

    static void PrintAST(List<AST> a)
    {
        foreach(var d in a)
        {
            if(d.GetType() == typeof(ASTVariableDefine))
            {
                var call = ((ASTVariableDefine)d);
                Console.WriteLine("Variable Define:");
                Console.WriteLine(" Label: " + call.label);
                Console.WriteLine(" Type : " + call.type);
                Console.Write(" Value: ");
                call.value.expression.ForEach(e => {
                    if(e.GetType() != typeof(ASTFunctionCall))
                        Console.Write(((dynamic)e).value + " ");
                    else
                        PrintFunctionCall((ASTFunctionCall)e);
                });
                Console.WriteLine();
            } else if(d.GetType() == typeof(ASTVariableReassign))
            {
                var call = ((ASTVariableReassign)d);
                Console.WriteLine("Variable Reassgin:");
                Console.WriteLine(" Label: " + call.label);
                Console.WriteLine(" Assign: " + call.asop);
                Console.Write(" Value: ");
                call.value.expression.ForEach(e => {
                    if(e.GetType() != typeof(ASTFunctionCall))
                        Console.Write(((dynamic)e).value + " ");
                    else
                        PrintFunctionCall((ASTFunctionCall)e);
                });
                Console.WriteLine();
            } else if(d.GetType() == typeof(ASTFunctionCall))
            {
                var call = ((ASTFunctionCall)d);
                PrintFunctionCall(call);
            } else if(d.GetType() == typeof(ASTConditional))
            {
                var call = ((ASTConditional)d);
                Console.WriteLine("Conditional:");
                Console.Write(" Condition: ");
                call.condition.expression.ForEach(e => {
                    if(e.GetType() != typeof(ASTFunctionCall))
                        Console.Write(((dynamic)e).value + " ");
                    else
                        PrintFunctionCall((ASTFunctionCall)e);
                });
                Console.WriteLine("{");
                PrintAST(call.block);
                Console.WriteLine("}");
            } else if(d.GetType() == typeof(ASTWhile))
            {
                var call = ((ASTWhile)d);
                Console.WriteLine("While:");
                Console.Write(" Condition: ");
                call.condition.expression.ForEach(e => {
                    if(e.GetType() != typeof(ASTFunctionCall))
                        Console.Write(((dynamic)e).value + " ");
                    else
                        PrintFunctionCall((ASTFunctionCall)e);
                });
                Console.WriteLine("{");
                PrintAST(call.block);
                Console.WriteLine("}");
            } else if(d.GetType() == typeof(ASTFunctionDefine))
            {
                var call = ((ASTFunctionDefine)d);
                Console.WriteLine("Function Define:");
                Console.Write(" Parameters: ");
                call.parameters.ForEach(e => {
                    Console.Write($"{e.Item1}: {e.Item2}, ");
                });
                Console.WriteLine("\nBody: {");
                PrintAST(call.block);
                Console.WriteLine("}");
            } else if(d.GetType() == typeof(ASTReturn))
            {
                var call = ((ASTReturn)d);
                Console.WriteLine("Return:");
                Console.Write(" ");
                call.expression.expression.ForEach(e => {
                    if(e.GetType() == typeof(ASTFunctionCall))
                    {
                        PrintFunctionCall((ASTFunctionCall)e);
                    } else 
                    {
                        Console.Write(((Token)e).value + " ");
                    }
                });
                Console.WriteLine();
            } else if(d.GetType() == typeof(ASTMatch))
            {
                var call = ((ASTMatch)d);
                Console.WriteLine("Match: ");
                Console.Write(" Expr: ");
                call.expr.expression.ForEach(e => {
                    if(e.GetType() != typeof(ASTFunctionCall))
                        Console.Write(((dynamic)e).value + " ");
                    else
                        PrintFunctionCall((ASTFunctionCall)e);
                });
                Console.WriteLine("{");
                call.matches.ToList().ForEach(e => {
                    Console.WriteLine(e.Key.expression.Count);
                    e.Key.expression.ForEach(e => {
                        if(e.GetType() != typeof(ASTFunctionCall))
                            Console.Write(((dynamic)e).value + " ");
                        else
                            PrintFunctionCall((ASTFunctionCall)e);
                    });

                    Console.WriteLine(": {");
                    PrintAST(e.Value);
                    Console.WriteLine("}");
                });
                Console.WriteLine("}");
            }
        }
    }

    static void PrintFunctionCall(ASTFunctionCall call)
    {
        Console.Write($"{call.label}(");
        call.value.ToList().ForEach(e => {
            Console.Write($"{e.Key}: ");
            e.Value.expression.ForEach(f => {
                if(f.GetType() == typeof(ASTFunctionCall))
                {
                    PrintFunctionCall((ASTFunctionCall)f);
                } else 
                {

                    Console.Write(((Token)f).value + " ");
                }
            });
            Console.Write(", ");
        });
        Console.WriteLine(")");
    }
}