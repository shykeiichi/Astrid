namespace Astrid;

public static class Error 
{
    public static void Throw(string message, Token token)
    {
        string[] filearr = File.ReadAllLines(Program.sourceFile);

        if(token != null)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("token");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($": {Tokenizer.GetTokenAsHuman(token)}");
            Console.WriteLine();
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write("error");
        Console.ForegroundColor = ConsoleColor.White;
        Console.Write($": {message}");
        Console.WriteLine();



        if(token.lineStart > 0) {
            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write(token.lineStart);
            Console.Write(" | ");

            foreach(var j in filearr[token.lineStart - 1]) {
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write(j);
            }
            Console.WriteLine();
        }

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(token.lineStart + 1);
        Console.Write(" | ");

        int jIndex = -1;
        foreach(var j in filearr[token.lineStart]) {
            jIndex ++;
            if(jIndex >= token.charStart && jIndex < token.charEnd) {
                Console.ForegroundColor = ConsoleColor.Red;
            } else {
                Console.ForegroundColor = ConsoleColor.Gray;
            }
            Console.Write(j);
        }
        Console.WriteLine();
        
        Console.ForegroundColor = ConsoleColor.Blue;
        for(var j = 0; j < token.lineStart.ToString().Length; j++) {
            Console.Write(" ");
        }
        Console.Write(" | ");

        for(int j = 0; j < token.charStart; j++) {
            Console.Write(" ");
        }
        Console.ForegroundColor = ConsoleColor.Blue;
        for(var j = 0; j < token.charEnd - token.charStart; j++) {
            Console.Write("^");
        }
        Console.WriteLine();



        Console.ForegroundColor = ConsoleColor.Blue;
        Console.Write(token.lineStart + 2);
        Console.Write(" | ");

        foreach(var j in filearr[token.lineStart + 1]) {
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(j);
        }
        Console.WriteLine();

        Environment.Exit(1);
    }
}