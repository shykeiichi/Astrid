namespace Astrid;

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

        PrintAST(parsed);

        Console.WriteLine("Run: =======================");
        
        Interpreter.Run(parsed);
    }

    static void PrintAST(List<AST> a)
    {
        foreach(var d in a)
        {
            if(d.GetType() == typeof(ASTVariableDefine))
            {
                Console.WriteLine("Variable Define:");
                Console.WriteLine(" Label: " + ((ASTVariableDefine)d).label);
                Console.WriteLine(" Type : " + ((ASTVariableDefine)d).type);
                Console.Write(" Value: ");
                ((ASTVariableDefine)d).value.expression.ForEach(e => Console.Write(e.value + " "));
                Console.WriteLine();
            }
        }
    }
}