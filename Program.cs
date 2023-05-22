namespace Astrid;

class Program
{
    public static string sourceFile = "";

    static void Main(string[] args)
    {
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
        
        Interpreter.Run(parsed.Item2, new(), new());
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