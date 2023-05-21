using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Astrid;

public class AST { }

public class ASTVariableDefine : AST
{
    public string label;
    public string type;
    public ASTExpression value;

    public ASTVariableDefine(string label, string type, ASTExpression value)
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
    public Types returnType;
    public List<AST> block;

    public ASTFunctionDefine(string label, List<(string, Types)> parameters, Types returnType, List<AST> block)
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

public enum AssignOp 
{
    Assign,
    Plus,
    Minus,
    Divide,
    Multiply,
    Power
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
            // Console.WriteLine(Tokenizer.GetTokenAsHuman(token));

            if(token.GetType() == typeof(TokenBlockEnd))
            {
                tokens = tokens.Skip(1).ToList();
                return (tokens.ToArray(), ast);
            }

            if(token.GetType() == typeof(TokenIdentifier))
            {
                var identifierLabel = tokens[0].value;
                
                tokens = tokens.Skip(1).ToList();
                token = tokens[0];

                if(token.GetType() == typeof(TokenColon))
                {
                    tokens = tokens.Skip(1).ToList();
                    token = tokens[0];

                    var (newtokens, newast) = ParseVariable(tokens.ToArray(), identifierLabel);

                    tokens = newtokens.ToList();
                    ast.AddRange(newast);
                } else if(token.GetType() == typeof(TokenParenStart)) 
                {
                    tokens = tokens.Skip(1).ToList();
                    token = tokens[0];

                    var (newtokens, newast) = ParseFunctionCall(tokens.ToArray(), identifierLabel);
                    tokens = newtokens.ToList();
                    ast.AddRange(newast);
                } else if(token.GetType() == typeof(TokenDoubleColon))
                {
                    tokens = tokens.Skip(1).ToList();
                    token = tokens[0];

                    var (newtokens, newast) = ParseFunctionDefine(tokens.ToArray(), identifierLabel);
                    tokens = newtokens.ToList();
                    ast.AddRange(newast);
                } else if(new [] {typeof(TokenAssign), typeof(TokenAssignPlus), typeof(TokenAssignMinus), typeof(TokenAssignDivide), typeof(TokenAssignMultiply), typeof(TokenAssignPower)}.Contains(token.GetType())) 
                { 
                    AssignOp asop;
                    if(token.GetType() == typeof(TokenAssign))
                    {
                        asop = AssignOp.Assign;
                    } else if(token.GetType() == typeof(TokenAssignPlus))
                    {
                        asop = AssignOp.Plus;
                    } else if(token.GetType() == typeof(TokenAssignMinus))
                    {
                        asop = AssignOp.Minus;
                    } else if(token.GetType() == typeof(TokenAssignDivide))
                    {
                        asop = AssignOp.Divide;
                    } else if(token.GetType() == typeof(TokenAssignMultiply))
                    {
                        asop = AssignOp.Multiply;
                    } else if(token.GetType() == typeof(TokenAssignPower))
                    {
                        asop = AssignOp.Power;
                    } else 
                    {
                        throw new Exception("Invalid asop");
                    }

                    tokens = tokens.Skip(1).ToList();

                    var (newtokens, newast) = ParseVariableReassign(tokens.ToArray(), identifierLabel, asop);

                    tokens = newtokens.ToList();
                    ast.AddRange(newast);
                } else 
                {
                    throw new Exception($"{Tokenizer.GetTokenAsHuman(token)} cannot follow identifier");
                }
            } else if(token.GetType() == typeof(TokenEOL))
            {
                tokens = tokens.Skip(1).ToList();
            } else if(token.GetType() == typeof(TokenKeyword))
            {
                switch(token.value)
                {
                    case "if": {
                        tokens = tokens.Skip(1).ToList();
                        token = tokens[0];

                        var (newtokens, expr) = ParseExpression(tokens.ToArray());
                        tokens = newtokens.ToList();

                        (newtokens, var newast) = ParseBlock(tokens.ToArray());
                        tokens = newtokens.ToList();

                        ast.Add(new ASTConditional(expr, newast));
                    } break;
                    case "while": {
                        tokens = tokens.Skip(1).ToList();
                        token = tokens[0];

                        var (newtokens, expr) = ParseExpression(tokens.ToArray());
                        tokens = newtokens.ToList();

                        (newtokens, var newast) = ParseBlock(tokens.ToArray());
                        tokens = newtokens.ToList();

                        ast.Add(new ASTWhile(expr, newast));
                    } break;
                    case "return": {
                        tokens = tokens.Skip(1).ToList();
                        token = tokens[0];

                        var (newtokens, expr) = ParseExpression(tokens.ToArray());
                        tokens = newtokens.ToList();

                        ast.Add(new ASTReturn(expr));
                    } break;
                }
            }
        }
        
        return (tokens.ToArray(), ast);
    }

    public static (Token[], List<AST>) ParseFunctionCall(Token[] tokens_, string function)
    {
        List<Token> tokens = tokens_.ToList();
        List<AST> ast = new();
        Dictionary<string, ASTExpression> parameters = new();       

        while(tokens.Count > 0) 
        {
            var token = tokens[0];

            // Console.WriteLine("fc " + Tokenizer.GetTokenAsHuman(token));

            if(token.GetType() == typeof(TokenParenEnd) || token.GetType() == typeof(TokenEOL))
            {
                ast.Add(new ASTFunctionCall(function, parameters));
                tokens = tokens.Skip(1).ToList();

                return (tokens.ToArray(), ast);
            }

            if(token.GetType() == typeof(TokenIdentifier))
            {   
                string parameterLabel = token.value;

                tokens = tokens.Skip(1).ToList();
                token = tokens[0];
                
                if(token.GetType() == typeof(TokenColon))
                {
                    tokens = tokens.Skip(1).ToList();
                    token = tokens[0];
                } else 
                {
                    throw new Exception($"Expected colon after identifier in function call found ${Tokenizer.GetTokenAsHuman(token)}");
                }

                var (newtokens, expr) = ParseExpression(tokens.ToArray());
                parameters.Add(parameterLabel, expr);

                tokens = newtokens.ToList();

                token = tokens[0];
            } else
            {
                throw new Exception($"Expected identifier found {Tokenizer.GetTokenAsHuman(token)}");
            }
        }

        ast.Add(new ASTFunctionCall(function, parameters));

        return (tokens.ToArray(), ast);
    }

    public static (Token[], List<AST>) ParseFunctionDefine(Token[] tokens_, string label)
    {
        List<Token> tokens = tokens_.ToList();
        List<AST> ast = new();

        var token = tokens[0];

        if(token.GetType() == typeof(TokenParenStart))
        {
            tokens = tokens.Skip(1).ToList();
            token = tokens[0];
        } else
        {
            throw new Exception($"Expected paren start found {Tokenizer.GetTokenAsHuman(token)}");
        }

        List<(string, Types)> parameters = new();

        while(token.GetType() != typeof(TokenParenEnd))
        {
            var name = "";
            if(token.GetType() == typeof(TokenIdentifier))
            {
                name = token.value;
                tokens = tokens.Skip(1).ToList();
                token = tokens[0];
            } else 
            {
                throw new Exception($"Expected identifier found {Tokenizer.GetTokenAsHuman(token)}");
            }

            if(token.GetType() == typeof(TokenColon))
            {
                tokens = tokens.Skip(1).ToList();
                token = tokens[0];
            } else 
            {
                throw new Exception($"Expected colon after identifier found {Tokenizer.GetTokenAsHuman(token)}");
            }

            var type = "";
            if(token.GetType() == typeof(TokenIdentifier))
            {
                type = token.value;
                tokens = tokens.Skip(1).ToList();
                token = tokens[0];
            } else 
            {
                throw new Exception($"Expected type found {Tokenizer.GetTokenAsHuman(token)}");
            }

            Types astype;
            switch(type)
            {
                case "int": astype = Types.Int; break;
                case "float": astype = Types.Float; break;
                case "string": astype = Types.String; break;
                case "any": astype = Types.Any; break;
                default: throw new Exception($"Invalid type {type}");
            }

            if(token.GetType() != typeof(TokenParenEnd))
            {
                if(token.GetType() == typeof(TokenComma))
                {
                    tokens = tokens.Skip(1).ToList();
                    token = tokens[0];
                } else 
                {
                    throw new Exception($"Expected comma after type found {Tokenizer.GetTokenAsHuman(token)}");
                }
            }

            parameters.Add((name, astype));
        }
        tokens = tokens.Skip(1).ToList();
        token = tokens[0];

        Types returnType;
        if(token.GetType() == typeof(TokenIdentifier))
        {
            switch(token.value)
            {
                case "int": returnType = Types.Int; break;
                case "float": returnType = Types.Float; break;
                case "string": returnType = Types.String; break;
                case "any": returnType = Types.Any; break;
                default: throw new Exception($"Invalid type {token.value}");
            }
            tokens = tokens.Skip(1).ToList();
            token = tokens[0];
        } else {
            throw new Exception($"Expected return type after parameters found {Tokenizer.GetTokenAsHuman(token)}");
        }

        if(token.GetType() == typeof(TokenBlockStart))
        {
            tokens = tokens.Skip(1).ToList();
            token = tokens[0];
        } else {
            throw new Exception($"Expected block start after function parameters found {Tokenizer.GetTokenAsHuman(token)}");
        }

        var (newtokens, newast) = ParseBlock(tokens.ToArray());
        tokens = newtokens.ToList();

        ast.Add(new ASTFunctionDefine(label, parameters, returnType, newast));

        return (tokens.ToArray(), ast);
    }

    public static (Token[], List<AST>) ParseVariable(Token[] tokens_, string label)
    {
        List<Token> tokens = tokens_.ToList();
        List<AST> ast = new();

        var token = tokens[0];

        string variableType = "";
        if(token.GetType() == typeof(TokenIdentifier))
        {
            variableType = token.value;
        } else {
            throw new Exception("Expected type found " + token.ToString());
        }

        switch(variableType)
        {
            case "int": break;
            case "float": break;
            case "string": break;
            case "any": break;
            default: throw new Exception($"Invalid type {variableType}");
        }

        tokens = tokens.Skip(1).ToList();
        token = tokens[0];

        if(token.GetType() == typeof(TokenAssign))
        {
            tokens = tokens.Skip(1).ToList();
            token = tokens[0];
        } else {
            throw new Exception("Expected assign found " + token.ToString());
        }

        var (newtokens, newast) = ParseExpression(tokens.ToArray());

        tokens = newtokens.ToList();
        
        ast.Add(new ASTVariableDefine(label, variableType, newast));

        return (tokens.ToArray(), ast);
    }

    public static (Token[], List<AST>) ParseVariableReassign(Token[] tokens_, string label, AssignOp asop)
    {
        List<Token> tokens = tokens_.ToList();
        List<AST> ast = new();

        var token = tokens[0];

        var (newtokens, newast) = ParseExpression(tokens.ToArray());

        tokens = newtokens.ToList();
        
        ast.Add(new ASTVariableReassign(label, newast, asop));

        return (tokens.ToArray(), ast);
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
        {typeof(TokenPower), (4, false)},
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

        while(tokens.Count > 0)
        {
            var token = tokens[0];
            tokens = tokens.Skip(1).ToList();

            // Console.WriteLine("expr " + Tokenizer.GetTokenAsHuman(token));

            if(new [] {typeof(TokenEOL), typeof(TokenBlockStart), typeof(TokenComma)}.Contains(token.GetType()) || (new [] {typeof(TokenEOL), typeof(TokenBlockStart), typeof(TokenComma)}.Contains(tokens[0].GetType()) && new [] {typeof(TokenParenEnd)}.Contains(token.GetType())))
            {
                while(OperatorQueue.Count > 0)
                {
                    if(OperatorQueue.Last().GetType() == typeof(TokenParenStart))
                        throw new Exception("Can't be left parnthesis");

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
                        OutputQueue.Add(newast[0]);
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
                        throw new Exception("Unmatched parenthesis");

                    OutputQueue.Add(OperatorQueue.Last());
                    OperatorQueue.RemoveAt(OperatorQueue.Count - 1);
                }

                if(OperatorQueue.GetType() == typeof(TokenParenStart))
                    throw new Exception("Expected left parenthesis");

                OperatorQueue.RemoveAt(OperatorQueue.Count - 1);                
            }
        }

        while(OperatorQueue.Count > 0)
        {
            if(OperatorQueue.Last().GetType() == typeof(TokenParenStart))
                throw new Exception("Can't be left parnthesis");

            OutputQueue.Add(OperatorQueue.Last());
            OperatorQueue.RemoveAt(OperatorQueue.Count - 1);
        }

        return (tokens.ToArray(), new ASTExpression(OutputQueue));
    }
}

