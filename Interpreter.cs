namespace Astrid;

public static class Interpreter
{
    public static Token? Run(List<AST> block, Dictionary<string, (Types, Token)> Variables, Dictionary<string, (List<(string, Types)>, object, Types)> Functions, string[] preAddedVars = null!, string[] preAddedFuncs = null!)
    {
        List<string> addedVars = new();
        if(preAddedVars != null)
            addedVars.AddRange(preAddedVars.ToList());

        List<string> addedFuncs = new();
        if(preAddedFuncs != null)
            addedFuncs.AddRange(preAddedFuncs.ToList());

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
                var call = (ASTFunctionCall)ast;
                
                RunFunction(call, Variables, Functions);
            } else if(ast.GetType() == typeof(ASTFunctionDefine))
            {
                var call = (ASTFunctionDefine)ast;

                addedFuncs.Add(call.label);

                Functions.Add(call.label, (
                    call.parameters,
                    call.block,
                    call.returnType
                ));
            } else if(ast.GetType() == typeof(ASTReturn))
            {
                var call = (ASTReturn)ast;

                return RunExpression(call.expression, Variables, Functions);
            } else if(ast.GetType() == typeof(ASTVariableDefine))
            {
                var call = (ASTVariableDefine)ast;

                Variables.Add(call.label, (call.type, RunExpression(call.value, Variables, Functions)));
                addedVars.Add(call.label);
            } else if(ast is ASTVariableReassign call)
            {
                if(!Variables.ContainsKey(call.label))
                    Error.Throw("No variable exists", default(Token)!);

                var result = RunExpression(call.value, Variables, Functions);

                if(Variables[call.label].Item1 != Parser.GetTypeFromValue(result))
                    Error.Throw($"Types don't match {Variables[call.label].Item1} {Parser.GetTypeFromValue(result)}", result);

                Variables[call.label] = (Variables[call.label].Item1, result);    
            }
        }

        foreach(var d in addedVars)
        {
            Variables.Remove(d);
        }

        foreach(var d in addedFuncs)
        {
            Functions.Remove(d);
        }

        return null;
    }

    public static Token? RunFunction(ASTFunctionCall fc, Dictionary<string, (Types, Token)> Variables, Dictionary<string, (List<(string, Types)>, object, Types)> Functions)
    {
        if(!Functions.Keys.Contains(fc.label))
        {
            Error.Throw("Function doesn't exist", new(0, 0, 0, 0));
        }

        // fc.value.ToList().ForEach(e => Console.WriteLine(e.Key));
        // Functions[fc.label].Item1.ForEach(e => Console.WriteLine(e.Item1));

        foreach(var i in fc.value)
        {
            bool contains = false;
            Functions[fc.label].Item1.ForEach(e => {
                if(!contains)
                    contains = e.Item1 == i.Key;
            });
            if(!contains)
            {
                Error.Throw($"Variable {i.Key} isn't satisifed", new(0, 0, 0, 0));
            }
        }

        // Functions[fc.label]

        string[] preAdd = fc.value.Keys.ToArray();

        fc.value.ToList().ForEach(e => {
            var Tok = RunExpression(e.Value, Variables, Functions);
            Variables.Add(e.Key, (Parser.GetTypeFromToken(Tok), Tok));
        });

        if(Functions[fc.label].Item2 is List<AST> block)
            return Run(block, Variables, Functions, preAdd);
        else 
        {
            var asd = (dynamic message) => {};
            return ((dynamic)Functions[fc.label].Item2).Invoke();
        }
    }


    public static Token RunExpression(ASTExpression _expr, Dictionary<string, (Types, Token)> Variables, Dictionary<string, (List<(string, Types)>, object, Types)> Functions)
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
                var val = RunFunction((ASTFunctionCall)tok, Variables, Functions);
                if(val == null)
                    Error.Throw(((ASTFunctionCall)tok).label, default(Token)!);
                
                tokens.Add(val!);
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
                    stack.Add(Variables[token.value].Item2);
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