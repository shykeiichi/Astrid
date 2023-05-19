﻿namespace Astrid;

class Program
{
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

        // Tokenizer.TokenizeFromFile("./examples/Expression.as").ToList().ForEach((a) => Console.WriteLine(Tokenizer.GetTokenAsHuman(a)));

        Token[] tokens = Tokenizer.TokenizeFromFile("examples/HelloWorld.as");

        // tokens.ToList().ForEach(e => Console.WriteLine(Tokenizer.GetTokenAsHuman(e)));

        // Parser.ParseExpression(tokens).ForEach(e => Console.WriteLine(Tokenizer.GetTokenAsHuman(e)));        

        var parsed = Parser.ParseBlock(tokens);

        // PrintAST(parsed.Item2);

        // Console.WriteLine("Run: =======================");
        
        Interpreter.Run(parsed.Item2, new());
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
                call.value.expression.ForEach(e => Console.Write(e.value + " "));
                Console.WriteLine();
            } else if(d.GetType() == typeof(ASTFunctionCall))
            {
                var call = ((ASTFunctionCall)d);
                Console.WriteLine("Function Call:");
                Console.WriteLine(" Label: " + call.label);
                Console.Write(" Parameters: ");
                call.value.ToList().ForEach(e => Console.Write($"{e.Key}: {e.Value}, "));
                Console.WriteLine();
            } else if(d.GetType() == typeof(ASTConditional))
            {
                var call = ((ASTConditional)d);
                Console.WriteLine("Conditional:");
                Console.WriteLine(" Condition: " + call.condition);
                PrintAST(call.block);
            }
        }
    }
}