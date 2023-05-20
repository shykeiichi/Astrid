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
    public List<Token> expression;

    public ASTExpression(List<Token> expression)
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
                    throw new Exception($"{token} cannot follow identifier");
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
        List<Token> OutputQueue = new();

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
                OutputQueue.Add(token);
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

