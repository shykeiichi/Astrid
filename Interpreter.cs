namespace Astrid;

public static class Interpreter
{
    public static Token? Run(List<AST> block, Dictionary<string, (Types, Token)> Variables, Dictionary<string, (List<(string, Types)>, List<AST>, Types)> Functions, string[] preAddedVars = default(string[])!)
    {
        List<string> addedVars = new();
        addedVars.AddRange(preAddedVars.ToList());

        foreach(var ast in block)
        {
            if(ast.GetType() == typeof(ASTConditional))
            {
                var call = (ASTConditional)ast;
                var cond = RunExpression(call.condition, Variables, Functions);

                if(cond.GetType() == typeof(TokenBoolean))
                {
                    var condbool = cond.value == "true";

                    if(condbool)
                    {
                        var t = Run(call.block, Variables, Functions);
                        if(t != null)
                        {
                            Error.Throw("Can't use return in if condition", t);
                        }
                    }
                } else 
                {
                    Error.Throw("Condition must be bool in if", cond);
                }
            } else if(ast.GetType() == typeof(ASTWhile))
            {
                var call = (ASTWhile)ast;
                var cond = RunExpression(call.condition, Variables, Functions);

                if(cond.GetType() == typeof(TokenBoolean))
                {
                    var condbool = cond.value == "true";

                    while(condbool)
                    {
                        cond = RunExpression(call.condition, Variables, Functions);
                        condbool = cond.value == "true";

                        var t = Run(call.block, Variables, Functions);
                        if(t != null)
                        {
                            Error.Throw("Can't use return in while", t);
                        }
                    }
                } else 
                {
                    Error.Throw("Condition must be bool in while", cond);
                }
            } else if(ast.GetType() == typeof(ASTFunctionCall))
            {

            }
        }

        foreach(var d in addedVars)
        {
            Variables.Remove(d);
        }

        return null;
    }

    public static Token RunFunction(ASTFunctionCall fc, Dictionary<string, (Types, Token)> Variables, Dictionary<string, (List<(string, Types)>, List<AST>, Types)> Functions)
    {
        if(!Functions.Keys.Contains(fc.label))
        {
            Error.Throw("Function doesn't exist", default(Token)!);
        }

        foreach(var i in fc.value)
        {
            bool contains = false;
            Functions[fc.label].Item1.ForEach(e => contains = e.Item1 == i.Key);
            if(!contains)
            {
                Error.Throw($"Variable {i.Key} isn't satisifed", default(Token)!);
            }
        }

        // Functions[fc.label]

        Run(Functions[fc.label].Item2, Variables, Functions);
    }


    public static Token RunExpression(ASTExpression _expr, Dictionary<string, (Types, Token)> Variables, Dictionary<string, (List<(string, Types)>, List<AST>, Types)> Functions)
    {
        List<object> tokensOld = _expr.expression;

        List<Token> tokens = new();

        foreach(var tok in tokensOld)
        {
            if(tok.GetType() != typeof(ASTFunctionCall))
            {
                tokens.Add((Token)tok);
            } else 
            {
                RunFunction((ASTFunctionCall)tok, Variables, Functions);
            }
        }

        List<Token> stack = new();
        var token = tokens[0];

        while(tokens.Count > 0)
        {
            token = tokens[0];
            tokens = tokens.Skip(1).ToList();

            // Console.WriteLine("Intepret " + token);

            if(Parser.ExprValues.Contains(token.GetType()))
            {
                if(token.GetType() != typeof(TokenIdentifier))
                    stack.Add(token);
                else {
                    // Console.WriteLine("Id " + token.value + " to " + Tokenizer.GetTokenAsHuman(variables[token.value]));
                    stack.Add(variables[token.value].Item2);
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
                            if(!new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(left.GetType()) || !new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(right.GetType()))
                                Error.Throw($"Cannot apply greater than to non int or float type, {Tokenizer.GetTokenAsHuman(left)}, {Tokenizer.GetTokenAsHuman(right)}", left);

                            value = (float.Parse(left.value) > float.Parse(right.value)).ToString().ToLower();
                        } else if(token.GetType() == typeof(TokenGreaterEquals))
                        {   
                          if(!new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(left.GetType()) || !new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(right.GetType()))
                                Error.Throw($"Cannot apply greaterequals than to non int or float type, {Tokenizer.GetTokenAsHuman(left)}, {Tokenizer.GetTokenAsHuman(right)}", left);

                            value = (float.Parse(left.value) >= float.Parse(right.value)).ToString().ToLower();
                        } else if(token.GetType() == typeof(TokenLesser))
                        {   
                          if(!new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(left.GetType()) || !new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(right.GetType()))
                                Error.Throw($"Cannot apply lesser than to non int or float type, {Tokenizer.GetTokenAsHuman(left)}, {Tokenizer.GetTokenAsHuman(right)}", left);

                            value = (float.Parse(left.value) < float.Parse(right.value)).ToString().ToLower();
                        } else if(token.GetType() == typeof(TokenLesserEquals))
                        {   
                          if(!new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(left.GetType()) || !new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(right.GetType()))
                                Error.Throw($"Cannot apply lesserequals than to non int or float type, {Tokenizer.GetTokenAsHuman(left)}, {Tokenizer.GetTokenAsHuman(right)}", left);

                            value = (float.Parse(left.value) <= float.Parse(right.value)).ToString().ToLower();
                        }

                        stack.Add(new TokenBoolean(value, left.lineStart, left.lineEnd, left.charStart, left.charEnd));
                        stack.RemoveAt(stack.Count - 2);
                        stack.RemoveAt(stack.Count - 2);
                    } else 
                    {
                        //Console.WriteLine("expr");
                        Error.Throw($"Types don't match in expression equals. Matching left: {left.GetType()}, right: {right.GetType()}", left);    
                    }
                } else if(Parser.ExprOperatorsAlgebraic.Keys.Contains(token.GetType()))
                {
                    if(stack.Count < 2)
                        Error.Throw("Stack doesn't contain two values", token);
                    
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
                    } else if(left.GetType() == typeof(TokenString) || right.GetType() == typeof(TokenString))
                    {
                        string value = "";
                        // Console.Write("float ");
                        if(token.GetType() == typeof(TokenPlus))
                        {
                            // Console.WriteLine($"{int.Parse(left.value)} + {int.Parse(right.value)}");
                            value = left.value + right.value;
                        } else
                        {
                            Error.Throw($"Can't use operator {token.GetType()} with string", token);
                        }

                        // Console.WriteLine("final value: " + value);
                        stack.Add(new TokenString(value, left.lineStart, left.lineEnd, left.charStart, left.charEnd));
                        stack.RemoveAt(stack.Count - 2);
                        stack.RemoveAt(stack.Count - 2);
                    } else 
                    {
                        Error.Throw($"Types don't match in expression equals. Matching left: {left.GetType()}, right: {right.GetType()}", left);    
                    }
                }
            }
        }

        if(stack.Count > 1)
        {
            Error.Throw("Stack exited longer than 1", token);
        }

        return stack[0];
    }
}