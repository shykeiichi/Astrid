namespace Astrid;

class Program
{
    static void Main(string[] args)
    {
        Tokenizer.TokenizeFromFile("examples/Calculator.as").ToList().ForEach((a) => Console.WriteLine(Tokenizer.GetTokenAsHuman(a)));
    }
}