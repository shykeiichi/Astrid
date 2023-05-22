namespace Astrid;

public static class Interpreter
{
    public static Token? Run(List<AST> block, Dictionary<string, (Types, Token)> Variables, Dictionary<string, (List<(string, Types)>, object, Types?)> Functions, string[] preAddedVars = null!, string[] preAddedFuncs = null!)
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
                var value = RunExpression(call.value, Variables, Functions);
                Types type = Types.Int;

                // Console.WriteLine("reached as");

                if(!Variables.Keys.Contains(call.label))
                    throw new Exception($"Variable {call.label} doens't exist so it can't be reassigned");

                switch(Variables[call.label].Item1)
                {
                    case Types.Int:
                        if(value.GetType() != typeof(TokenInt))
                            Error.Throw($"Cannot assign ${value.GetType()} to int", value);
                        type = Types.Int;
                        break;
                    case Types.Float:
                        if(value.GetType() != typeof(TokenFloat))
                            Error.Throw($"Cannot assign ${value.GetType()} to float", value);
                        type = Types.Float;
                        break;
                    case Types.String:
                        if(value.GetType() != typeof(TokenString))
                            Error.Throw($"Cannot assign ${value.GetType()} to string", value);
                        type = Types.String;
                        break;
                    default:
                        Error.Throw($"Invalid type in reassign {Variables[call.label].Item1}", Variables[call.label].Item2); break;
                }

                switch(call.asop)
                {
                    case AssignOp.Assign: {
                        Variables[call.label] = (type, value);
                    } break;
                    case AssignOp.Plus: {
                        switch(type)
                        {
                            case Types.Int: {
                                Variables[call.label] = (type, new TokenInt((int.Parse(Variables[call.label].Item2.value) + int.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
                            } break;
                            case Types.Float: {
                                Variables[call.label] = (type, new TokenFloat((float.Parse(Variables[call.label].Item2.value) + float.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
                            } break;
                            case Types.String: {
                                Variables[call.label] = (type, new TokenString(Variables[call.label].Item2.value + value.value, value.lineStart, value.lineEnd, value.charStart, value.charEnd));
                            } break;
                        }
                    } break;
                    case AssignOp.Minus: {
                        switch(type)
                        {
                            case Types.Int: {
                                Variables[call.label] = (type, new TokenInt((int.Parse(Variables[call.label].Item2.value) - int.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
                            } break;
                            case Types.Float: {
                                Variables[call.label] = (type, new TokenFloat((float.Parse(Variables[call.label].Item2.value) - float.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
                            } break;
                            default: {
                                Error.Throw($"Can't use operator minus with {type}", Variables[call.label].Item2); 
                            } break;
                        }
                    } break;
                    case AssignOp.Multiply: {
                        switch(type)
                        {
                            case Types.Int: {
                                Variables[call.label] = (type, new TokenInt((int.Parse(Variables[call.label].Item2.value) * int.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
                            } break;
                            case Types.Float: {
                                Variables[call.label] = (type, new TokenFloat((float.Parse(Variables[call.label].Item2.value) * float.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
                            } break;
                            default: {
                                Error.Throw($"Can't use operator multiply with {type}", Variables[call.label].Item2); 
                            } break;
                        }
                    } break;
                    case AssignOp.Divide: {
                        switch(type)
                        {
                            case Types.Int: {
                                Variables[call.label] = (type, new TokenInt((int.Parse(Variables[call.label].Item2.value) / int.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
                            } break;
                            case Types.Float: {
                                Variables[call.label] = (type, new TokenFloat((float.Parse(Variables[call.label].Item2.value) / float.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
                            } break;
                            default: {
                                Error.Throw($"Can't use operator divide with {type}", Variables[call.label].Item2); 
                            } break;
                        }
                    } break;
                    case AssignOp.Power: {
                        switch(type)
                        {
                            case Types.Int: {
                                Variables[call.label] = (type, new TokenInt(Math.Pow(int.Parse(Variables[call.label].Item2.value), int.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
                            } break;
                            case Types.Float: {
                                Variables[call.label] = (type, new TokenFloat(Math.Pow(float.Parse(Variables[call.label].Item2.value), float.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
                            } break;
                            default: {
                                Error.Throw($"Can't use operator power with {type}", Variables[call.label].Item2); 
                            } break;
                        }
                    } break;
                    case AssignOp.Modulo: {
                        switch(type)
                        {
                            case Types.Int: {
                                Variables[call.label] = (type, new TokenInt((int.Parse(Variables[call.label].Item2.value) % int.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
                            } break;
                            case Types.Float: {
                                Variables[call.label] = (type, new TokenFloat((float.Parse(Variables[call.label].Item2.value) % float.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
                            } break;
                            default: {
                                Error.Throw($"Can't use operator modulo with {type}", Variables[call.label].Item2); 
                            } break;
                        }
                    } break;
                }
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

    public static Token? RunFunction(ASTFunctionCall fc, Dictionary<string, (Types, Token)> Variables, Dictionary<string, (List<(string, Types)>, object, Types?)> Functions)
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
                Error.Throw($"Parameter {i.Key} isn't satisifed", new(0, 0, 0, 0));
            }
        }

        // Functions[fc.label]

        if(Functions[fc.label].Item2 is List<AST> block) 
        {
            string[] preAdd = fc.value.Keys.ToArray();

            fc.value.ToList().ForEach(e => {
                var Tok = RunExpression(e.Value, Variables, Functions);
                Variables.Add(e.Key, (Parser.GetTypeFromValue(Tok), Tok));
            });
            return Run(block, Variables, Functions, preAdd);
        } else 
        {
            var asd = (dynamic message) => {};

            List<Token> parameters = fc.value.Select(e => RunExpression(e.Value, Variables, Functions)).ToList();

            if(parameters.Count < Functions[fc.label].Item1.Count)
            {
                Error.Throw($"Parameters aren't satisifed in {fc.label}", new(0, 0, 0, 0));
            }

            var val = ((dynamic)Functions[fc.label].Item2).Invoke(parameters);
            return val;
        }
    }


    public static Token RunExpression(ASTExpression _expr, Dictionary<string, (Types, Token)> Variables, Dictionary<string, (List<(string, Types)>, object, Types?)> Functions)
    {
        List<object> tokensOld = _expr.expression;

        // Console.WriteLine("a");
        // foreach(var i in tokensOld)
        // {
        //     Console.WriteLine(i);
        // }

        List<Token> tokens = new();

        foreach(var tok in tokensOld)
        {
            if(tok.GetType() != typeof(ASTFunctionCall))
            {
                tokens.Add((Token)tok);
            } else 
            {
                // Console.WriteLine("func call");
                var val = RunFunction((ASTFunctionCall)tok, Variables, Functions);
                if(val == null && Functions[((ASTFunctionCall)tok).label].Item3 != null)
                {
                    Error.Throw($"Return type must be {Functions[((ASTFunctionCall)tok).label].Item3}", new(0, 0, 0, 0));
                } else if(Parser.GetTypeFromValue(val!) != Functions[((ASTFunctionCall)tok).label].Item3)
                {
                    Error.Throw($"Return type must be {Functions[((ASTFunctionCall)tok).label].Item3}", val!);
                }
                if(val == null)
                    Error.Throw(((ASTFunctionCall)tok).label, default(Token)!);
                
                tokens.Add(val!);
            }
        }

        // foreach(var i in tokens)
        // {
        //     Console.WriteLine("tok " + Tokenizer.GetTokenAsHuman(i));
        // }

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
                if(new [] {typeof(TokenEquals), typeof(TokenNotEquals), typeof(TokenGreater), typeof(TokenGreaterEquals), typeof(TokenLesser), typeof(TokenLesserEquals)}.Contains(token.GetType()))                {
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

                    if(left.GetType() == typeof(TokenInt) && right.GetType() == typeof(TokenInt))
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
                        } else if(token.GetType() == typeof(TokenModulo))
                        {
                            // Console.WriteLine($"{int.Parse(left.value)} ^ {int.Parse(right.value)}");
                            value = (int.Parse(left.value) % int.Parse(right.value)).ToString().ToLower();
                        }

                        // Console.WriteLine("final value: " + value);
                        stack.Add(new TokenInt(value, left.lineStart, left.lineEnd, left.charStart, left.charEnd));
                        stack.RemoveAt(stack.Count - 2);
                        stack.RemoveAt(stack.Count - 2);
                    } else if(left.GetType() == typeof(TokenFloat) && right.GetType() == typeof(TokenFloat))
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
                        } else if(token.GetType() == typeof(TokenModulo))
                        {
                            // Console.WriteLine($"{int.Parse(left.value)} * {int.Parse(right.value)}");
                            value = (float.Parse(left.value) % float.Parse(right.value)).ToString().ToLower();
                        } 

                        // Console.WriteLine("final value: " + value);
                        stack.Add(new TokenFloat(value, left.lineStart, left.lineEnd, left.charStart, left.charEnd));
                        stack.RemoveAt(stack.Count - 2);
                        stack.RemoveAt(stack.Count - 2);
                    } else if(left.GetType() == typeof(TokenString) && right.GetType() == typeof(TokenString))
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
            string st = "Stack exited longer than 1 ";
            stack.ForEach(e => st += Tokenizer.GetTokenAsHuman(e));
            Error.Throw(st, token);
        }

        return stack[0];
    }
}