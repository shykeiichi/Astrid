﻿namespace Astrid;

class Program
{
    static void Main(string[] args)
    {
        Tokenizer.TokenizeFromFile("./examples/HelloWorld.as").ToList().ForEach((a) => Console.WriteLine(Tokenizer.GetTokenAsHuman(a)));

        Token[] tokens = Tokenizer.TokenizeFromFile("./examples/HelloWorld.as");

        List<AST> parsed = Parser.ParseTokens(tokens.ToList());
        foreach(var p in parsed)
        {
            if(p.GetType() == typeof(FunctionCall))
            {
                FunctionCall call = (FunctionCall)p;
                Console.WriteLine("Function Call:");
                Console.WriteLine(call.function);
                foreach(var a in call.arguments)
                {
                    Console.WriteLine(a);
                }
            }
        }
    }
}