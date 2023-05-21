// namespace Astrid;

// public enum Types
// {
//     String,
//     Int,
//     Float,
//     Any
// }

// public static class Interpreter 
// {
//     public static void CheckBuiltinParameters(Dictionary<string, Types> target, Dictionary<string, Token> sent)
//     {
//         foreach(var a in target)
//         {
//             if(!sent.Keys.Contains(a.Key))
//             {
//                 throw new Exception($"Parameter {a.Key} isn't satisfied");
//             }
//         }

//         foreach(var a in target)
//         {
//             switch(a.Value)
//             {
//                 case Types.Int:
//                     if(sent[a.Key].value.GetType() != typeof(TokenInt))
//                         throw new Exception($"Parameter {a.Key} expected int found {sent[a.Key].value.GetType()}");
//                     break;
//                 case Types.Float:
//                     if(sent[a.Key].value.GetType() != typeof(TokenFloat))
//                         throw new Exception($"Parameter {a.Key} expected float found {sent[a.Key].value.GetType()}");
//                     break;
//                 case Types.String:
//                     if(sent[a.Key].value.GetType() != typeof(TokenString))
//                         throw new Exception($"Parameter {a.Key} expected string found {sent[a.Key].value.GetType()}");
//                     break;
//                 case Types.Any:
//                     break;
//             }
//         }
//     }

//     public static void Run(List<AST> ast, Dictionary<string, (Types, Token)> Variables)
//     {
//         List<string> AddedVariables = new();

//         Dictionary<string, (Dictionary<string, Types>, List<AST>)> Functions = new();
//         Dictionary<string, (Dictionary<string, Types>, Action<Dictionary<string, Token>>)> Builtins = new()
//         {
//             {
//                 "print",
//                 (
//                     new () 
//                     {
//                         {
//                             "message",
//                             Types.Any
//                         }
//                     },
//                     (Dictionary<string, Token> parameters) => {
//                         CheckBuiltinParameters(new() {{"message",Types.Any}}, parameters);

//                         Console.WriteLine(parameters["message"].value);
//                     }
//                 )
//             }
//         };

//         foreach(var a in ast)
//         {
//             if(a.GetType() == typeof(ASTVariableDefine))
//             {
//                 var call = (ASTVariableDefine)a;
//                 var value = RunExpression(call.value, Variables, Builtins, Functions);
//                 Types type;

//                 switch(call.type)
//                 {
//                     case "int":
//                         if(value.GetType() != typeof(TokenInt))
//                             throw new Exception($"Cannot assign ${value.GetType()} to int");
//                         type = Types.Int;
//                         break;
//                     case "float":
//                         if(value.GetType() != typeof(TokenFloat))
//                             throw new Exception($"Cannot assign ${value.GetType()} to float");
//                         type = Types.Float;
//                         break;
//                     case "string":
//                         if(value.GetType() != typeof(TokenString))
//                             throw new Exception($"Cannot assign ${value.GetType()} to string");
//                         type = Types.String;
//                         break;
//                     case "any":
//                         type = Types.Any;
//                         break;
//                     default:
//                         throw new Exception($"Invalid type {call.type}");
//                 }

//                 AddedVariables.Add(call.label);

//                 Variables.Add(call.label, (type, value));                
//             } else if(a.GetType() == typeof(ASTVariableReassign))
//             {
//                 var call = (ASTVariableReassign)a;
//                 var value = RunExpression(call.value, Variables, Builtins, Functions);
//                 Types type;

//                 // Console.WriteLine("reached as");

//                 if(!Variables.Keys.Contains(call.label))
//                     throw new Exception($"Variable {call.label} doens't exist so it can't be reassigned");

//                 switch(Variables[call.label].Item1)
//                 {
//                     case Types.Int:
//                         if(value.GetType() != typeof(TokenInt))
//                             throw new Exception($"Cannot assign ${value.GetType()} to int");
//                         type = Types.Int;
//                         break;
//                     case Types.Float:
//                         if(value.GetType() != typeof(TokenFloat))
//                             throw new Exception($"Cannot assign ${value.GetType()} to float");
//                         type = Types.Float;
//                         break;
//                     case Types.String:
//                         if(value.GetType() != typeof(TokenString))
//                             throw new Exception($"Cannot assign ${value.GetType()} to string");
//                         type = Types.String;
//                         break;
//                     case Types.Any:
//                         type = Types.Any;
//                         break;
//                     default:
//                         throw new Exception($"Invalid type in reassign {Variables[call.label].Item1}");
//                 }

//                 switch(call.asop)
//                 {
//                     case AssignOp.Assign: {
//                         Variables[call.label] = (type, value);
//                     } break;
//                     case AssignOp.Plus: {
//                         switch(type)
//                         {
//                             case Types.Int: {
//                                 Variables[call.label] = (type, new TokenInt((int.Parse(Variables[call.label].Item2.value) + int.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
//                             } break;
//                             case Types.Float: {
//                                 Variables[call.label] = (type, new TokenFloat((float.Parse(Variables[call.label].Item2.value) + float.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
//                             } break;
//                             case Types.String: {
//                                 Variables[call.label] = (type, new TokenString(Variables[call.label].Item2.value + value.value, value.lineStart, value.lineEnd, value.charStart, value.charEnd));
//                             } break;
//                         }
//                     } break;
//                     case AssignOp.Minus: {
//                         switch(type)
//                         {
//                             case Types.Int: {
//                                 Variables[call.label] = (type, new TokenInt((int.Parse(Variables[call.label].Item2.value) - int.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
//                             } break;
//                             case Types.Float: {
//                                 Variables[call.label] = (type, new TokenFloat((float.Parse(Variables[call.label].Item2.value) - float.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
//                             } break;
//                             default: {
//                                 throw new Exception($"Can't use operator minus with {type}");
//                             }
//                         }
//                     } break;
//                     case AssignOp.Multiply: {
//                         switch(type)
//                         {
//                             case Types.Int: {
//                                 Variables[call.label] = (type, new TokenInt((int.Parse(Variables[call.label].Item2.value) * int.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
//                             } break;
//                             case Types.Float: {
//                                 Variables[call.label] = (type, new TokenFloat((float.Parse(Variables[call.label].Item2.value) * float.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
//                             } break;
//                             default: {
//                                 throw new Exception($"Can't use operator multiply with {type}");
//                             }
//                         }
//                     } break;
//                     case AssignOp.Divide: {
//                         switch(type)
//                         {
//                             case Types.Int: {
//                                 Variables[call.label] = (type, new TokenInt((int.Parse(Variables[call.label].Item2.value) / int.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
//                             } break;
//                             case Types.Float: {
//                                 Variables[call.label] = (type, new TokenFloat((float.Parse(Variables[call.label].Item2.value) / float.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
//                             } break;
//                             default: {
//                                 throw new Exception($"Can't use operator divide with {type}");
//                             }
//                         }
//                     } break;
//                     case AssignOp.Power: {
//                         switch(type)
//                         {
//                             case Types.Int: {
//                                 Variables[call.label] = (type, new TokenInt(Math.Pow(int.Parse(Variables[call.label].Item2.value), int.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
//                             } break;
//                             case Types.Float: {
//                                 Variables[call.label] = (type, new TokenFloat(Math.Pow(float.Parse(Variables[call.label].Item2.value), float.Parse(value.value)).ToString(), value.lineStart, value.lineEnd, value.charStart, value.charEnd));
//                             } break;
//                             default: {
//                                 throw new Exception($"Can't use operator power with {type}");
//                             }
//                         }
//                     } break;
//                 }
                              
//             } else if(a.GetType() == typeof(ASTFunctionCall))
//             {
//                 var call = (ASTFunctionCall)a;

//                 RunFunction(call, Builtins, Functions, Variables);
//             } else if(a.GetType() == typeof(ASTConditional))
//             {
//                 var call = (ASTConditional)a;
                
//                 var result = RunExpression(call.condition, Variables, Builtins, Functions);

//                 // call.condition.expression.ForEach(e => Console.Write(Tokenizer.GetTokenAsHuman(e) + " "));
//                 // Console.WriteLine();

//                 if(result.GetType() != typeof(TokenBoolean))
//                     throw new Exception($"If condition needs to be of type boolean not {Tokenizer.GetTokenAsHuman(result)}");

//                 if(result.value == "true")
//                 {
//                     Run(call.block, Variables);
//                 }
//             } else if(a.GetType() == typeof(ASTWhile))
//             {
//                 var call = (ASTWhile)a;
                
//                 var result = RunExpression(call.condition, Variables, Builtins, Functions);

//                 // call.condition.expression.ForEach(e => Console.Write(Tokenizer.GetTokenAsHuman(e) + " "));
//                 // Console.WriteLine();

//                 bool run = result.value == "true";

//                 while(run)
//                 {
//                     result = RunExpression(call.condition, Variables, Builtins, Functions);

//                     if(result.value != "true")
//                     {
//                         run = false;
//                         break;
//                     }

//                     if(result.GetType() != typeof(TokenBoolean))
//                         throw new Exception($"While condition needs to be of type boolean not {Tokenizer.GetTokenAsHuman(result)}");


//                     Run(call.block, Variables);
//                 }
//             }
//         }

//         foreach(string vari in AddedVariables)
//         {
//             Variables.Remove(vari);
//         }

//         // foreach(var kvp in Variables)
//         // {
//         //     Console.WriteLine(kvp.Key + " " + Tokenizer.GetTokenAsHuman(kvp.Value.Item2));
//         // }
//     }

//     public static void RunFunction(ASTFunctionCall call, Dictionary<string, (Dictionary<string, Types>, Action<Dictionary<string, Token>>)> Builtins, Dictionary<string, (Dictionary<string, Types>, List<AST>)> Functions, Dictionary<string, (Types, Token)> Variables)
//     {
//         Dictionary<string, Token> fixedParameters = new();

//         foreach(var b in call.value)
//         {
//             fixedParameters.Add(b.Key, RunExpression(b.Value, Variables, Builtins, Functions));
//         }

//         if(Builtins.Keys.Contains(call.label))
//         {
//             Builtins[call.label].Item2(fixedParameters);
//         } else {
//             throw new Exception($"Function {call.label} not found");
//         }
//     }

//     public static Token RunExpression(ASTExpression _expr, Dictionary<string, (Types, Token)> variables, Dictionary<string, (Dictionary<string, Types>, Action<Dictionary<string, Token>>)> Builtins, Dictionary<string, (Dictionary<string, Types>, List<AST>)> Functions)
//     {
//         List<object> tokensOld = _expr.expression;

//         List<Token> tokens = new();

//         foreach(var tok in tokensOld)
//         {
//             if(tok.GetType() == typeof(Token))
//             {
//                 tokens.Add((Token)tok);
//             } else if(tok.GetType() == typeof(ASTFunctionCall))
//             {
//                 RunFunction((ASTFunctionCall)tok, Builtins, Functions, variables);
//             }
//         }

//         if(tokens.Count == 0)
//             throw new Exception("Expression can't be empty");

//         List<Token> stack = new();

//         while(tokens.Count > 0)
//         {
//             var token = tokens[0];
//             tokens = tokens.Skip(1).ToList();

//             if(Parser.ExprValues.Contains(token.GetType()))
//             {
//                 if(token.GetType() != typeof(TokenIdentifier))
//                     stack.Add(token);
//                 else {
//                     // Console.WriteLine("Id " + token.value + " to " + Tokenizer.GetTokenAsHuman(variables[token.value]));
//                     stack.Add(variables[token.value].Item2);
//                 }
//             } else if(Parser.ExprOperators.Keys.Contains(token.GetType()))
//             {
//                 if(new [] {typeof(TokenEquals), typeof(TokenNotEquals), typeof(TokenGreater), typeof(TokenGreaterEquals), typeof(TokenLesser), typeof(TokenLesserEquals)}.Contains(token.GetType()))
//                 {
//                     if(stack.Count < 2)
//                         throw new Exception("Stack doesn't contains two values");
                    
//                     Token left = stack[stack.Count - 2];
//                     Token right = stack[stack.Count - 1];

//                     if(left.GetType() == right.GetType())
//                     {
//                         string value = "";
//                         if(token.GetType() == typeof(TokenEquals))
//                         {
//                             value = (left.value == right.value).ToString().ToLower();
//                         } else if(token.GetType() == typeof(TokenNotEquals))
//                         {
//                             value = (left.value != right.value).ToString().ToLower();
//                         } else if(token.GetType() == typeof(TokenGreater))
//                         {   
//                             if(!new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(left.GetType()) || !new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(right.GetType()))
//                                 throw new Exception($"Cannot apply greater than to non int or float type, {Tokenizer.GetTokenAsHuman(left)}, {Tokenizer.GetTokenAsHuman(right)}");

//                             value = (float.Parse(left.value) > float.Parse(right.value)).ToString().ToLower();
//                         } else if(token.GetType() == typeof(TokenGreaterEquals))
//                         {   
//                           if(!new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(left.GetType()) || !new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(right.GetType()))
//                                 throw new Exception($"Cannot apply greaterequals than to non int or float type, {Tokenizer.GetTokenAsHuman(left)}, {Tokenizer.GetTokenAsHuman(right)}");

//                             value = (float.Parse(left.value) >= float.Parse(right.value)).ToString().ToLower();
//                         } else if(token.GetType() == typeof(TokenLesser))
//                         {   
//                           if(!new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(left.GetType()) || !new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(right.GetType()))
//                                 throw new Exception($"Cannot apply lesser than to non int or float type, {Tokenizer.GetTokenAsHuman(left)}, {Tokenizer.GetTokenAsHuman(right)}");

//                             value = (float.Parse(left.value) < float.Parse(right.value)).ToString().ToLower();
//                         } else if(token.GetType() == typeof(TokenLesserEquals))
//                         {   
//                           if(!new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(left.GetType()) || !new [] {typeof(TokenInt), typeof(TokenFloat)}.Contains(right.GetType()))
//                                 throw new Exception($"Cannot apply lesserequals than to non int or float type, {Tokenizer.GetTokenAsHuman(left)}, {Tokenizer.GetTokenAsHuman(right)}");

//                             value = (float.Parse(left.value) <= float.Parse(right.value)).ToString().ToLower();
//                         }

//                         stack.Add(new TokenBoolean(value, left.lineStart, left.lineEnd, left.charStart, left.charEnd));
//                         stack.RemoveAt(stack.Count - 2);
//                         stack.RemoveAt(stack.Count - 2);
//                     } else 
//                     {
//                         //Console.WriteLine("expr");
//                         throw new Exception($"Types don't match in expression equals. Matching left: {left.GetType()}, right: {right.GetType()}");    
//                     }
//                 } else if(Parser.ExprOperatorsAlgebraic.Keys.Contains(token.GetType()))
//                 {
//                     if(stack.Count < 2)
//                         throw new Exception("Stack doesn't contain two values");
                    
//                     Token left = stack[stack.Count - 2];
//                     Token right = stack[stack.Count - 1];

//                     if(left.GetType() == typeof(TokenInt) || right.GetType() == typeof(TokenInt))
//                     {
//                         string value = "";
//                         // Console.Write("int ");
//                         if(token.GetType() == typeof(TokenPlus))
//                         {
//                             // Console.WriteLine($"{int.Parse(left.value)} + {int.Parse(right.value)}");
//                             value = (int.Parse(left.value) + int.Parse(right.value)).ToString().ToLower();
//                         } else if(token.GetType() == typeof(TokenMinus))
//                         {
//                             // Console.WriteLine($"{int.Parse(left.value)} - {int.Parse(right.value)}");
//                             value = (int.Parse(left.value) - int.Parse(right.value)).ToString().ToLower();
//                         } else if(token.GetType() == typeof(TokenDivide))
//                         {
//                             // Console.WriteLine($"{int.Parse(left.value)} / {int.Parse(right.value)}");
//                             value = (int.Parse(left.value) / int.Parse(right.value)).ToString().ToLower();
//                         } else if(token.GetType() == typeof(TokenMultiply))
//                         {
//                             // Console.WriteLine($"{int.Parse(left.value)} * {int.Parse(right.value)}");
//                             value = (int.Parse(left.value) * int.Parse(right.value)).ToString().ToLower();
//                         } else if(token.GetType() == typeof(TokenPower))
//                         {
//                             // Console.WriteLine($"{int.Parse(left.value)} ^ {int.Parse(right.value)}");
//                             value = ((int)Math.Pow(int.Parse(left.value), int.Parse(right.value))).ToString().ToLower();
//                         }

//                         // Console.WriteLine("final value: " + value);
//                         stack.Add(new TokenInt(value, left.lineStart, left.lineEnd, left.charStart, left.charEnd));
//                         stack.RemoveAt(stack.Count - 2);
//                         stack.RemoveAt(stack.Count - 2);
//                     } else if(left.GetType() == typeof(TokenFloat) || right.GetType() == typeof(TokenFloat))
//                     {
//                         string value = "";
//                         // Console.Write("float ");
//                         if(token.GetType() == typeof(TokenPlus))
//                         {
//                             // Console.WriteLine($"{int.Parse(left.value)} + {int.Parse(right.value)}");
//                             value = (float.Parse(left.value) + float.Parse(right.value)).ToString().ToLower();
//                         } else if(token.GetType() == typeof(TokenMinus))
//                         {
//                             // Console.WriteLine($"{int.Parse(left.value)} - {int.Parse(right.value)}");
//                             value = (float.Parse(left.value) - float.Parse(right.value)).ToString().ToLower();
//                         } else if(token.GetType() == typeof(TokenDivide))
//                         {
//                             // Console.WriteLine($"{int.Parse(left.value)} / {int.Parse(right.value)}");
//                             value = (float.Parse(left.value) / float.Parse(right.value)).ToString().ToLower();
//                         } else if(token.GetType() == typeof(TokenMultiply))
//                         {
//                             // Console.WriteLine($"{int.Parse(left.value)} * {int.Parse(right.value)}");
//                             value = (float.Parse(left.value) * float.Parse(right.value)).ToString().ToLower();
//                         } else if(token.GetType() == typeof(TokenPower))
//                         {
//                             // Console.WriteLine($"{int.Parse(left.value)} ^ {int.Parse(right.value)}");
//                             value = ((float)Math.Pow(float.Parse(left.value), float.Parse(right.value))).ToString().ToLower();
//                         }

//                         // Console.WriteLine("final value: " + value);
//                         stack.Add(new TokenFloat(value, left.lineStart, left.lineEnd, left.charStart, left.charEnd));
//                         stack.RemoveAt(stack.Count - 2);
//                         stack.RemoveAt(stack.Count - 2);
//                     } else if(left.GetType() == typeof(TokenString) || right.GetType() == typeof(TokenString))
//                     {
//                         string value = "";
//                         // Console.Write("float ");
//                         if(token.GetType() == typeof(TokenPlus))
//                         {
//                             // Console.WriteLine($"{int.Parse(left.value)} + {int.Parse(right.value)}");
//                             value = left.value + right.value;
//                         } else
//                         {
//                             throw new Exception($"Can't use operator {token.GetType()} with string");
//                         }

//                         // Console.WriteLine("final value: " + value);
//                         stack.Add(new TokenString(value, left.lineStart, left.lineEnd, left.charStart, left.charEnd));
//                         stack.RemoveAt(stack.Count - 2);
//                         stack.RemoveAt(stack.Count - 2);
//                     } else 
//                     {
//                         throw new Exception($"Types don't match in expression equals. Matching left: {left.GetType()}, right: {right.GetType()}");    
//                     }
//                 }
//             }
//         }

//         if(stack.Count > 1)
//         {
//             throw new Exception("Stack exited longer than 1");
//         }

//         return stack[0];
//     }
// }