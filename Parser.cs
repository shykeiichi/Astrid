using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Astrid;

public enum Types
{
    String,
    Int,
    Float
}

public class AST { }

public class ASTVariableDefine : AST
{
    public string label;
    public Types type;
    public ASTExpression value;

    public ASTVariableDefine(string label, Types type, ASTExpression value)
    {
        this.label = label;
        this.type = type;
        this.value = value;
    }
}

public class ASTVariableReassign : AST
{
    public string label;
    public ASTExpression value;
    public AssignOp asop;

    public ASTVariableReassign(string label, ASTExpression value, AssignOp asop)
    {
        this.label = label;
        this.value = value;
        this.asop = asop;
    }
}

public class ASTFunctionDefine : AST
{
    public string label;
    public List<(string, Types)> parameters;
    public Types? returnType;
    public List<AST> block;

    public ASTFunctionDefine(string label, List<(string, Types)> parameters, Types? returnType, List<AST> block)
    {
        this.label = label;
        this.parameters = parameters;
        this.returnType = returnType;
        this.block = block;
    }
}

public class ASTFunctionCall : AST
{
    public string label;
    public Dictionary<string, ASTExpression> value;

    public ASTFunctionCall(string label, Dictionary<string, ASTExpression> value)
    {
        this.label = label;
        this.value = value;
    }
}

public class ASTExpression : AST
{
    public List<object> expression;

    public ASTExpression(List<object> expression)
    {
        this.expression = expression;
    }
}

public class ASTConditional : AST
{
    public  ASTExpression condition;
    public List<AST> block;

    public ASTConditional(ASTExpression condition, List<AST> block)
    {
        this.condition = condition;
        this.block = block;

    }
}

public class ASTReturn : AST
{
    public ASTExpression expression;

    public ASTReturn(ASTExpression expression)
    {
        this.expression = expression;
    }
}

public class ASTWhile : AST
{
    public  ASTExpression condition;
    public List<AST> block;

    public ASTWhile(ASTExpression condition, List<AST> block)
    {
        this.condition = condition;
        this.block = block;

    }
}

public class ASTMatch : AST
{
    public ASTExpression expr;
    public Dictionary<ASTExpression, List<AST>> matches;

    public ASTMatch(ASTExpression expr, Dictionary<ASTExpression, List<AST>> matches)
    {
        this.expr = expr;
        this.matches = matches;
    }
}

public enum AssignOp 
{
    Assign,
    Plus,
    Minus,
    Divide,
    Multiply,
    Power,
    Modulo
}

public static class Parser
{
    public static (Token[], List<AST>) ParseBlock(Token[] tokens_)
    {
        List<Token> tokens = tokens_.ToList();
        List<AST> ast = new();

        while(tokens.Count > 0) 
        {
            var token = tokens[0];

            // Tokenizer.Print(token);

            if(token.GetType() == typeof(TokenEOL))
            {
                tokens = tokens.Skip(1).ToList();
            } else if(token.GetType() == typeof(TokenBlockEnd)) 
            { 
                tokens = tokens.Skip(1).ToList();
                return (tokens.ToArray(), ast);
            } else if(token.GetType() == typeof(TokenIdentifier))
            {
                var identifierLabel = token.value;
                tokens = tokens.Skip(1).ToList();
                token = tokens[0];

                if(token.GetType() == typeof(TokenDoubleColon))
                {
                    tokens = tokens.Skip(1).ToList();
                    var (newtokens, newast) = ParseFunctionDefine(tokens.ToArray(), identifierLabel);
                    tokens = newtokens.ToList();
                    ast.Add(newast);
                } else if(token.GetType() == typeof(TokenParenStart))
                {
                    tokens = tokens.Skip(1).ToList();
                    var (newtokens, newast) = ParseFunctionCall(tokens.ToArray(), identifierLabel);
                    tokens = newtokens.ToList();
                    ast.Add(newast);
                } else if(token.GetType() == typeof(TokenColon))
                {
                    tokens = tokens.Skip(1).ToList();
                    var (newtokens, newast) = ParseVariableDefine(tokens.ToArray(), identifierLabel);
                    tokens = newtokens.ToList();
                    ast.Add(newast);   
                } else if(new [] {typeof(TokenAssign), typeof(TokenAssignDivide), typeof(TokenAssignMinus), typeof(TokenAssignMultiply), typeof(TokenAssignPlus), typeof(TokenAssignPower), typeof(TokenAssignModulo)}.Contains(token.GetType()))
                {
                    tokens = tokens.Skip(1).ToList();
                    var (newtokens, newast) = ParseVariableReassign(tokens.ToArray(), identifierLabel, token);
                    tokens = newtokens.ToList();
                    ast.Add(newast);   
                } else 
                {
                    Error.Throw("Unexpected token after identifier", token);
                }
            } else if(token.GetType() == typeof(TokenKeyword))
            {
                if(token.value == "return")
                {
                    tokens = tokens.Skip(1).ToList();
                    var (newtokens, newast) = ParseReturn(tokens.ToArray());
                    tokens = newtokens.ToList();
                    ast.Add(newast);
                } else if(token.value == "if")
                {
                    tokens = tokens.Skip(1).ToList();
                    var (newtokens, newast) = ParseConditional(tokens.ToArray());
                    tokens = newtokens.ToList();
                    ast.Add(newast);
                } else if(token.value == "while")
                {
                    tokens = tokens.Skip(1).ToList();
                    var (newtokens, newast) = ParseWhile(tokens.ToArray());
                    tokens = newtokens.ToList();
                    ast.Add(newast);
                } else if(token.value == "match")
                {
                    tokens = tokens.Skip(1).ToList();
                    var (newtokens, newast) = ParseMatch(tokens.ToArray());
                    tokens = newtokens.ToList();
                    ast.Add(newast);
                } else 
                {
                    Error.Throw("Unimplemented keyword", token);
                }
            } else 
            {
                Error.Throw("Unexpected token", token);
            }
        }

        return (tokens.ToArray(), ast);
    }
    
    public static Types GetTypeFromToken(Token t)
    {
        switch(t.value)
        {
            case "int": return Types.Int;
            case "float": return Types.Float;
            case "string": return Types.String;
            default: Error.Throw($"Invalid type", t); return Types.Int;
        }
    }

    public static Types GetTypeFromValue(Token t)
    {
        if(t.GetType() == typeof(TokenInt))
        {
            return Types.Int;
        } else if(t.GetType() == typeof(TokenFloat))
        {
            return Types.Float;
        } else if(t.GetType() == typeof(TokenString))
        {
            return Types.String;
        } 
        Error.Throw($"Invalid type", t); 
        return Types.Int;
    }

    public static (Token[], AST) ParseConditional(Token[] tokens_)
    {
        List<Token> tokens = tokens_.ToList();
        AST ast = new();

        var (newtokens, expr) = ParseExpression(tokens.ToArray());
        tokens = newtokens.ToList();

        (newtokens, var astblock) = ParseBlock(tokens.ToArray());
        tokens = newtokens.ToList();

        return (tokens.ToArray(), new ASTConditional(expr, astblock));   
    }

    public static (Token[], AST) ParseWhile(Token[] tokens_)
    {
        List<Token> tokens = tokens_.ToList();
        AST ast = new();

        var (newtokens, expr) = ParseExpression(tokens.ToArray());
        tokens = newtokens.ToList();

        (newtokens, var astblock) = ParseBlock(tokens.ToArray());
        tokens = newtokens.ToList();

        return (tokens.ToArray(), new ASTWhile(expr, astblock));   
    }

    public static (Token[], AST) ParseMatch(Token[] tokens_)
    {
        List<Token> tokens = tokens_.ToList();
        AST ast = new();
        
        var (newtokens, expr) = ParseExpression(tokens.ToArray());
        tokens = newtokens.ToList();

        var token = tokens[0];

        if(token.GetType() == typeof(TokenParenStart))
        {
            tokens = tokens.Skip(1).ToList();
            token = tokens[0];
        } else 
        {
            Error.Throw("Expected left parenthesis '(' after double colon '::'", token);   
        }        
    }

    public static (Token[], AST) ParseFunctionDefine(Token[] tokens_, string label)
    {
        List<Token> tokens = tokens_.ToList();
        AST ast = new();

        var token = tokens[0];

        if(token.GetType() == typeof(TokenParenStart))
        {
            tokens = tokens.Skip(1).ToList();
            token = tokens[0];
        } else 
        {
            Error.Throw("Expected left parenthesis '(' after double colon '::'", token);   
        }

        List<(string, Types)> parameters = new();

        bool parametersDone = false;
        while(tokens.Count > 0 || !parametersDone)
        {
            token = tokens[0];

            if(token.GetType() == typeof(TokenParenEnd))
            {
                tokens = tokens.Skip(1).ToList();
                token = tokens[0];
                parametersDone = true;
                break;
            }

            var parameterLabel = "";
            if(token.GetType() == typeof(TokenIdentifier))
            {   
                parameterLabel = token.value;
                tokens = tokens.Skip(1).ToList();
                token = tokens[0];
            } else {
                Error.Throw("Expected identifier as paramter label", token);
            }

            if(token.GetType() == typeof(TokenColon))
            {
                tokens = tokens.Skip(1).ToList();
                token = tokens[0];
            } else 
            {
                Error.Throw("Expected colon after parameter label", token);   
            }

            Token type = default(Token)!;
            if(token.GetType() == typeof(TokenIdentifier))
            {   
                type = token;
                tokens = tokens.Skip(1).ToList();
                token = tokens[0];
            } else {
                Error.Throw("Expected type", token);
            }

            // Console.WriteLine("i");
            parameters.Add((parameterLabel, GetTypeFromToken(type)));

            if(token.GetType() == typeof(TokenParenEnd))
            {
                parametersDone = true;
                tokens = tokens.Skip(1).ToList();
                token = tokens[0];

                break;
            }

            if(token.GetType() == typeof(TokenComma))
            {
                tokens = tokens.Skip(1).ToList();
                token = tokens[0];
            } else 
            {
                Error.Throw("Expected comma after parameter label", token);   
            }
        }

        Types? returnType = null;
        if(token.GetType() == typeof(TokenIdentifier))
        {   
            if(token.value == "void")
            {
                returnType = null;
            } else 
            {
                returnType = GetTypeFromToken(token);
            }
            // Console.WriteLine("2");
            tokens = tokens.Skip(1).ToList();
            token = tokens[0];
        } else {
            Error.Throw("Expected return type", token);
        }

        List<AST> block = new();
        if(token.GetType() == typeof(TokenBlockStart))
        {
            tokens = tokens.Skip(1).ToList();
            token = tokens[0];

            var (newtokens, newast) = ParseBlock(tokens.ToArray());
            block = newast;
            tokens = newtokens.ToList();
        } else {
            Error.Throw("Expected block start", token);
        }

        ast = new ASTFunctionDefine(label, parameters, returnType, block);

        return (tokens.ToArray(), ast);
    }

    public static (Token[], AST) ParseFunctionCall(Token[] tokens_, string function)
    {
        List<Token> tokens = tokens_.ToList();
        AST ast = new();

        Dictionary<string, ASTExpression> parameters = new();

        bool parametersDone = false;
        while(tokens.Count > 0 || !parametersDone)
        {
            var token = tokens[0];

            if(token.GetType() == typeof(TokenParenEnd))
            {
                tokens = tokens.Skip(1).ToList();
                token = tokens[0];
                parametersDone = true;
                break;
            }

            var parameterLabel = "";
            if(token.GetType() == typeof(TokenIdentifier))
            {   
                parameterLabel = token.value;
                tokens = tokens.Skip(1).ToList();
                token = tokens[0];
            } else {
                Error.Throw("Expected identifier as paramter label", token);
            }

            if(token.GetType() == typeof(TokenColon))
            {
                tokens = tokens.Skip(1).ToList();
                token = tokens[0];
            } else 
            {
                Error.Throw("Expected colon after parameter label", token);   
            }

            var (newtokens, newast) = ParseExpression(tokens.ToArray());
            // Console.WriteLine(tokens[tokens.Count - newtokens.Length - 1]);

            var newtok = newtokens.ToList();
            newtok = newtok.Prepend(tokens[tokens.Count - newtokens.Length - 1]).ToList();
            tokens = newtok.ToArray().ToList();
            token = tokens[0];

            parameters.Add(parameterLabel, newast);

            if(token.GetType() == typeof(TokenParenEnd) || token.GetType() == typeof(TokenEOL))
            {
                tokens = tokens.Skip(1).ToList();
                return (tokens.ToArray(), new ASTFunctionCall(function, parameters));
            } else if(token.GetType() == typeof(TokenComma))
            {
                tokens = tokens.Skip(1).ToList();
            }
        }
        return (tokens.ToArray(), new ASTFunctionCall(function, parameters));
    }

    public static (Token[], AST) ParseReturn(Token[] tokens)
    {
        var (newtokens, newast) = ParseExpression(tokens);   

        return (newtokens, new ASTReturn(newast));
    }

    public static (Token[], AST) ParseVariableDefine(Token[] tokens_, string label)
    {
        List<Token> tokens = tokens_.ToList();
        AST ast = new();

        var token = tokens[0];

        Types type = Types.Int;
        if(token.GetType() == typeof(TokenIdentifier))
        {
            // Console.WriteLine("3");
            type = GetTypeFromToken(token);
            tokens = tokens.Skip(1).ToList();
            token = tokens[0];
        } else 
        {
            Error.Throw("Expected type", token);   
        }

        if(token.GetType() == typeof(TokenAssign))
        {
            tokens = tokens.Skip(1).ToList();
            token = tokens[0];
        } else 
        {
            Error.Throw("Expected assign '='", token);   
        }

        var (newtokens, expr) = ParseExpression(tokens.ToArray());

        tokens = newtokens.ToList();

        return (tokens.ToArray(), new ASTVariableDefine(label, type, expr)); 
    }

    public static (Token[], AST) ParseVariableReassign(Token[] tokens_, string label, Token op) 
    {
        AssignOp asop = AssignOp.Assign;
        if(op.GetType() == typeof(TokenAssign))
        {
            asop = AssignOp.Assign;
        } else if(op.GetType() == typeof(TokenAssignDivide))
        {
            asop = AssignOp.Divide;
        } else if(op.GetType() == typeof(TokenAssignMinus))
        {
            asop = AssignOp.Minus;
        } else if(op.GetType() == typeof(TokenAssignMultiply))
        {
            asop = AssignOp.Multiply;
        } else if(op.GetType() == typeof(TokenAssignPlus))
        {
            asop = AssignOp.Plus;
        } else if(op.GetType() == typeof(TokenAssignPower))
        {
            asop = AssignOp.Power;
        } else if(op.GetType() == typeof(TokenAssignModulo))
        {
            asop = AssignOp.Power;
        }  else 
        {
            Error.Throw("Invalid operator", op);
        }

        List<Token> tokens = tokens_.ToList();

        var token = tokens[0];

        var (newtokens, expr) = ParseExpression(tokens.ToArray());

        return (newtokens, new ASTVariableReassign(label, expr, asop));
    }

    public static Type[] ExprValues = new [] {typeof(TokenInt), typeof(TokenFloat), typeof(TokenString), typeof(TokenIdentifier)};

    public static Dictionary<Type, (int, bool)> ExprOperatorsEquality = new()
    {
        {typeof(TokenEquals), (0, false)},
        {typeof(TokenNotEquals), (0, false)},
        {typeof(TokenGreater), (1, false)},
        {typeof(TokenGreaterEquals), (1, false)},
        {typeof(TokenLesser), (1, false)},
        {typeof(TokenLesserEquals), (1, false)}   
    };

    public static Dictionary<Type, (int, bool)> ExprOperatorsAlgebraic = new()
    {
        {typeof(TokenPlus), (2, false)},
        {typeof(TokenMinus), (2, false)},
        {typeof(TokenDivide), (3, false)},
        {typeof(TokenMultiply), (3, false)},
        {typeof(TokenModulo), (3, false)},
        {typeof(TokenPower), (4, true)},
    };

    public static Dictionary<Type, (int, bool)> ExprOperatorsBoolean = new()
    {
        {typeof(TokenNot), (4, true)}
    };

    public static Dictionary<Type, (int, bool)> ExprOperators = new();

    public static (Token[], ASTExpression) ParseExpression(Token[] tokens_)
    {
        List<Token> tokens = tokens_.ToList();
        List<AST> ast = new();

        List<Token> OperatorQueue = new();
        List<object> OutputQueue = new();

        int leftParens = 0;
        int rightParens = 0;

        while(tokens.Count > 0)
        {
            var token = tokens[0];
            tokens = tokens.Skip(1).ToList();

            if(token.GetType() == typeof(TokenParenStart))
            {
                leftParens ++;
            } 
            if(token.GetType() == typeof(TokenParenEnd))
            {
                rightParens++;
            }

            // Console.WriteLine("expr " + Tokenizer.GetTokenAsHuman(token));

            if(new [] {typeof(TokenEOL), typeof(TokenBlockStart), typeof(TokenComma)}.Contains(token.GetType()) || (new [] {typeof(TokenEOL), typeof(TokenBlockStart), typeof(TokenComma)}.Contains(tokens[0].GetType()) && new [] {typeof(TokenParenEnd)}.Contains(token.GetType())) || (rightParens > leftParens))
            {
                while(OperatorQueue.Count > 0)
                {
                    if(OperatorQueue.Last().GetType() == typeof(TokenParenStart))
                        Error.Throw("Can't be left parnthesis", OperatorQueue.Last());

                    OutputQueue.Add(OperatorQueue.Last());
                    OperatorQueue.RemoveAt(OperatorQueue.Count - 1);
                }

                return (tokens.ToArray(), new ASTExpression(OutputQueue));
            }

            if(ExprValues.Contains(token.GetType()))
            {
                if(token.GetType() == typeof(TokenIdentifier))
                {
                    if(tokens[0].GetType() == typeof(TokenParenStart))
                    {
                        // Console.WriteLine("Parsing function");
                        tokens = tokens.Skip(1).ToList();
                        var (newtokens, newast) = ParseFunctionCall(tokens.ToArray(), token.value);
                        tokens = newtokens.ToList();
                        OutputQueue.Add(newast);
                        // Console.Write("Parsed fc ");
                        // Tokenizer.Print(tokens[0]);
                        // Console.WriteLine("done parse");
                    } else {
                        OutputQueue.Add(token);
                    }
                } else 
                {
                    OutputQueue.Add(token);
                }
                
            } else if(ExprOperators.Keys.Contains(token.GetType()))
            {
                while(
                    (OperatorQueue.Count > 0 && OperatorQueue.Last().GetType() != typeof(TokenParenStart)) 
                    && 
                    (
                        ExprOperators[OperatorQueue.Last().GetType()].Item1 > ExprOperators[token.GetType()].Item1 
                        || 
                        (ExprOperators[OperatorQueue.Last().GetType()].Item1 == ExprOperators[token.GetType()].Item1 && !ExprOperators[token.GetType()].Item2)
                    )
                )
                {
                    OutputQueue.Add(OperatorQueue.Last());
                    OperatorQueue.RemoveAt(OperatorQueue.Count - 1);
                }
                
                OperatorQueue.Add(token);
            } else if(token.GetType() == typeof(TokenParenStart))
            {
                OperatorQueue.Add(token);
            } else if(token.GetType() == typeof(TokenParenEnd))
            {
                while(
                    OperatorQueue.Last().GetType() != typeof(TokenParenStart)
                ) {
                    if(OperatorQueue.Count == 0)
                        Error.Throw("Unmatched parnthesis", OperatorQueue.Last());

                    OutputQueue.Add(OperatorQueue.Last());
                    OperatorQueue.RemoveAt(OperatorQueue.Count - 1);
                }

                // if(OperatorQueue.Last().GetType() == typeof(TokenParenStart))
                //     Error.Throw("Expected left parnthesis", OperatorQueue.Last());

                OperatorQueue.RemoveAt(OperatorQueue.Count - 1);                
            }
        }

        while(OperatorQueue.Count > 0)
        {
            if(OperatorQueue.Last().GetType() == typeof(TokenParenStart))
                Error.Throw("Can't be left parnthesis", OperatorQueue.Last());

            OutputQueue.Add(OperatorQueue.Last());
            OperatorQueue.RemoveAt(OperatorQueue.Count - 1);
        }

        return (tokens.ToArray(), new ASTExpression(OutputQueue));
    }
}