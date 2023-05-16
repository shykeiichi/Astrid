namespace Astrid;

public static class Interpreter
{
    public static void Run(List<AST> ast)
    {
        Dictionary<string, object> Variables = new();
        Dictionary<string, Action<string>> Builtins = new() 
        {
            {
                "print", 
                (message) => Console.WriteLine(message)
            }
        }; 
        Dictionary<string, List<AST>> Functions = new();

        foreach(AST p in ast)
        {
            if(p.GetType() == typeof(VariableDefine))
            {
                VariableDefine call = (VariableDefine)p;
                Variables.Add(call.label, call.value);
            } else if(p.GetType() == typeof(FunctionCall)) {
                FunctionCall call = (FunctionCall)p;

                if(Builtins.Keys.ToList().Contains(call.function))
                {
                    Builtins[call.function]((string)call.arguments["message"]);
                }
            } else if(p.GetType() == typeof(Conditional))
            {
                Conditional call = (Conditional)p;
                if(call.conditional)
                {
                    Run(call.block);
                }
            }
        }
    }
}