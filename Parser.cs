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
    public AstExpression value;

    public ASTVariableDefine(string label, string type, AstExpression value)
    {
        this.label = label;
        this.type = type;
        this.value = value;
    }
}

public class AstExpression : AST
{
    public List<Token> expression;

    public AstExpression(List<Token> expression)
    {
        this.expression = expression;
    }
}

public static class Parser
{
    public static List<AST> ParseBlock(Token[] tokens_)
    {
        List<Token> tokens = tokens_.ToList();
        List<AST> ast = new();

        while(tokens.Count > 0) 
        {
            var token = tokens[0];
            Console.WriteLine(Tokenizer.GetTokenAsHuman(token));
            
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

                } else 
                {
                    throw new Exception($"{token} cannot follow identifier");
                }
            }
        }
        
        return ast;
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

    public static (Token[], AstExpression) ParseExpression(Token[] tokens_)
    {
        List<Token> tokens = tokens_.ToList();
        List<AST> ast = new();

        List<Token> OperatorQueue = new();
        List<Token> OutputQueue = new();

        while(tokens.Count > 0)
        {
            var token = tokens[0];
            tokens = tokens.Skip(1).ToList();

            // Console.WriteLine(Tokenizer.GetTokenAsHuman(token));

            if(new [] {typeof(TokenEOL), typeof(TokenBlockStart)}.Contains(token.GetType()))
            {
                while(OperatorQueue.Count > 0)
                {
                    if(OperatorQueue.Last().GetType() == typeof(TokenParenStart))
                        throw new Exception("Can't be left parnthesis");

                    OutputQueue.Add(OperatorQueue.Last());
                    OperatorQueue.RemoveAt(OperatorQueue.Count - 1);
                }

                return (tokens.ToArray(), new AstExpression(OutputQueue));
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

        return (tokens.ToArray(), new AstExpression(OutputQueue));
    }
}

