namespace Astrid;

public static class Interpreter 
{
    public static void Run(List<AST> ast)
    {
        Dictionary<string, Token> Variables = new();

        foreach(var a in ast)
        {
            if(a.GetType() == typeof(ASTVariableDefine))
            {
                var call = (ASTVariableDefine)a;
                var value = RunExpression(call.value, Variables);

                switch(call.type)
                {
                    case "int":
                        if(value.GetType() != typeof(TokenInt))
                            throw new Exception($"Cannot assign ${value.GetType()} to int");
                        break;
                    case "float":
                        if(value.GetType() != typeof(TokenFloat))
                            throw new Exception($"Cannot assign ${value.GetType()} to float");
                        break;
                    case "string":
                        if(value.GetType() != typeof(TokenString))
                            throw new Exception($"Cannot assign ${value.GetType()} to string");
                        break;
                }

                Variables.Add(call.label, value);                
            }
        }

        foreach(var kvp in Variables)
        {
            Console.WriteLine(kvp.Key + " " + Tokenizer.GetTokenAsHuman(kvp.Value));
        }
    }

    public static Token RunExpression(AstExpression _expr, Dictionary<string, Token> variables)
    {
        List<Token> tokens = _expr.expression;

        List<Token> stack = new();

        while(tokens.Count > 0)
        {
            var token = tokens[0];
            tokens = tokens.Skip(1).ToList();

            if(Parser.ExprValues.Contains(token.GetType()))
            {
                if(token.GetType() != typeof(TokenIdentifier))
                    stack.Add(token);
                else {
                    Console.WriteLine("Id " + token.value + " to " + Tokenizer.GetTokenAsHuman(variables[token.value]));
                    stack.Add(variables[token.value]);
                }
            } else if(Parser.ExprOperators.Keys.Contains(token.GetType()))
            {
                if(new [] {typeof(TokenEquals), typeof(TokenNotEquals), typeof(TokenGreater), typeof(TokenGreaterEquals), typeof(TokenLesser), typeof(TokenLesserEquals)}.Contains(token.GetType()))
                {
                    if(stack.Count < 2)
                        throw new Exception("Stack doesn't contains two values");
                    
                    Token left = stack[stack.Count - 2];
                    Token right = stack[stack.Count - 1];

                    if(left.GetType() == right.GetType())
                    {
                        string value = "";
                        if(token.GetType() == typeof(TokenEquals))
                        {
                            value = (left.value == right.value).ToString().ToLower();
                        } else if(token.GetType() == typeof(TokenNotEquals))
                        {
                            value = (left.value != right.value).ToString().ToLower();
                        } else if(token.GetType() == typeof(TokenGreater))
                        {   
                            if(!new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(token.GetType()))
                                throw new Exception("Cannot apply greater than to non int or float type");

                            value = (float.Parse(left.value) > float.Parse(right.value)).ToString().ToLower();
                        } else if(token.GetType() == typeof(TokenGreaterEquals))
                        {   
                            if(!new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(token.GetType()))
                                throw new Exception("Cannot apply greater equals than to non int or float type");

                            value = (float.Parse(left.value) >= float.Parse(right.value)).ToString().ToLower();
                        } else if(token.GetType() == typeof(TokenLesser))
                        {   
                            if(!new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(token.GetType()))
                                throw new Exception("Cannot apply lesser than to non int or float type");

                            value = (float.Parse(left.value) < float.Parse(right.value)).ToString().ToLower();
                        } else if(token.GetType() == typeof(TokenLesserEquals))
                        {   
                            if(!new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(token.GetType()))
                                throw new Exception("Cannot apply lesser equals than to non int or float type");

                            value = (float.Parse(left.value) <= float.Parse(right.value)).ToString().ToLower();
                        }

                        stack.Add(new TokenBoolean(value, left.lineStart, left.lineEnd, left.charStart, left.charEnd));
                        stack.RemoveAt(stack.Count - 1);
                        stack.RemoveAt(stack.Count - 1);
                    } else 
                    {
                        throw new Exception($"Types don't match in expression equals. Matching left: {left.GetType()}, right: {right.GetType()}");    
                    }
                } else if(Parser.ExprOperatorsAlgebraic.Keys.Contains(token.GetType()))
                {
                    if(stack.Count < 2)
                        throw new Exception("Stack doesn't contains two values");
                    
                    Token left = stack[stack.Count - 2];
                    Token right = stack[stack.Count - 1];

                    if(left.GetType() == typeof(TokenInt) || right.GetType() == typeof(TokenInt))
                    {
                        string value = "";
                        // Console.Write("int ");
                        if(token.GetType() == typeof(TokenPlus))
                        {
                            // Console.WriteLine($"{int.Parse(left.value)} + {int.Parse(right.value)}");
                            value = (int.Parse(left.value) + int.Parse(right.value)).ToString().ToLower();
                        } else if(token.GetType() == typeof(TokenMinus))
                        {
                            // Console.WriteLine($"{int.Parse(left.value)} - {int.Parse(right.value)}");
                            value = (int.Parse(left.value) - int.Parse(right.value)).ToString().ToLower();
                        } else if(token.GetType() == typeof(TokenDivide))
                        {
                            // Console.WriteLine($"{int.Parse(left.value)} / {int.Parse(right.value)}");
                            value = (int.Parse(left.value) / int.Parse(right.value)).ToString().ToLower();
                        } else if(token.GetType() == typeof(TokenMultiply))
                        {
                            // Console.WriteLine($"{int.Parse(left.value)} * {int.Parse(right.value)}");
                            value = (int.Parse(left.value) * int.Parse(right.value)).ToString().ToLower();
                        } else if(token.GetType() == typeof(TokenPower))
                        {
                            // Console.WriteLine($"{int.Parse(left.value)} ^ {int.Parse(right.value)}");
                            value = ((int)Math.Pow(int.Parse(left.value), int.Parse(right.value))).ToString().ToLower();
                        }

                        // Console.WriteLine("final value: " + value);
                        stack.Add(new TokenInt(value, left.lineStart, left.lineEnd, left.charStart, left.charEnd));
                        stack.RemoveAt(stack.Count - 2);
                        stack.RemoveAt(stack.Count - 2);
                    } else if(left.GetType() == typeof(TokenFloat) || right.GetType() == typeof(TokenFloat))
                    {
                        string value = "";
                        // Console.Write("float ");
                        if(token.GetType() == typeof(TokenPlus))
                        {
                            // Console.WriteLine($"{int.Parse(left.value)} + {int.Parse(right.value)}");
                            value = (float.Parse(left.value) + float.Parse(right.value)).ToString().ToLower();
                        } else if(token.GetType() == typeof(TokenMinus))
                        {
                            // Console.WriteLine($"{int.Parse(left.value)} - {int.Parse(right.value)}");
                            value = (float.Parse(left.value) - float.Parse(right.value)).ToString().ToLower();
                        } else if(token.GetType() == typeof(TokenDivide))
                        {
                            // Console.WriteLine($"{int.Parse(left.value)} / {int.Parse(right.value)}");
                            value = (float.Parse(left.value) / float.Parse(right.value)).ToString().ToLower();
                        } else if(token.GetType() == typeof(TokenMultiply))
                        {
                            // Console.WriteLine($"{int.Parse(left.value)} * {int.Parse(right.value)}");
                            value = (float.Parse(left.value) * float.Parse(right.value)).ToString().ToLower();
                        } else if(token.GetType() == typeof(TokenPower))
                        {
                            // Console.WriteLine($"{int.Parse(left.value)} ^ {int.Parse(right.value)}");
                            value = ((float)Math.Pow(float.Parse(left.value), float.Parse(right.value))).ToString().ToLower();
                        }

                        // Console.WriteLine("final value: " + value);
                        stack.Add(new TokenFloat(value, left.lineStart, left.lineEnd, left.charStart, left.charEnd));
                        stack.RemoveAt(stack.Count - 2);
                        stack.RemoveAt(stack.Count - 2);
                    } else 
                    {
                        throw new Exception($"Types don't match in expression equals. Matching left: {left.GetType()}, right: {right.GetType()}");    
                    }
                }
            }
        }

        if(stack.Count > 1)
        {
            throw new Exception("Stack exited longer than 1");
        }

        return stack[0];
    }
}