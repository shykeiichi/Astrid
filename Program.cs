namespace Astrid;

class Program
{
    static void Main(string[] args)
    {
        // Tokenizer.TokenizeFromFile("./examples/Expression.as").ToList().ForEach((a) => Console.WriteLine(Tokenizer.GetTokenAsHuman(a)));

        Token[] tokens = Tokenizer.TokenizeFromMemory(new [] {"3 + 4 * 2 / ( 1 - 5 ) ^ 2 ^ 3"});

        Parser.ParseExpression(tokens).ForEach(e => Console.WriteLine(e.value));

        // Parser.ParseExpression(tokens).ForEach(e => Console.WriteLine(Tokenizer.GetTokenAsHuman(e)));        

        // var parsed = Parser.ParseBlock(tokens);

        // PrintAST(parsed);

        // Console.WriteLine("Run: =======================");
        
    }

    // static void PrintAST(List<AST> a)
    // {
    //     foreach(var d in a)
    //     {
    //         if(d.GetType() == typeof(ASTVariableDefine))
    //         {
    //             Console.WriteLine("Variable Define:");
    //             Console.WriteLine(" Label: " + ((ASTVariableDefine)d).label);
    //             Console.WriteLine(" Type : " + ((ASTVariableDefine)d).type);
    //             Console.WriteLine(" Value: " + ((ASTVariableDefine)d).value);
    //         } else if(d.GetType() == typeof(ASTExpression))
    //         {

    //         }
    //     }
    // }
}