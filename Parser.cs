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
    public List<AST> value;

    public ASTVariableDefine(string label, string type, List<AST> value)
    {
        this.label = label;
        this.type = type;
        this.value = value;
    }
}

public class ASTExprValue : AST {
    public Token value;

    public ASTExprValue(Token value)
    {
        this.value = value;
    }
}
public class ASTExprBinary : AST {
    public AST left;
    public Token op;
    public AST right;

    public ASTExprBinary(AST left, Token op, AST right)
    {
        this.left = left;
        this.op = op;
        this.right = right;
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
                } else {
                    throw new Exception("Expected Colon");
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

        // var (newtokens, newast) = ParseExpression(tokens.ToArray());

        // // tokens = newtokens.ToList();
        
        // // ast.Add(new ASTVariableDefine(label, variableType, newast));

        return (tokens.ToArray(), ast);
    }

    public static List<Token> ParseExpression(Token[] tokens_)
    {
        List<Token> tokens = tokens_.ToList();
        List<AST> ast = new();

        Dictionary<Type, (int, bool)> Operators = new()
        {
            {typeof(TokenPlus), (0, false)},
            {typeof(TokenMinus), (0, false)},
            {typeof(TokenDivide), (1, false)},
            {typeof(TokenMultiply), (1, false)},
            {typeof(TokenPower), (2, true)},
        };

        Type[] Values = new [] {typeof(TokenInt), typeof(TokenFloat)};

        List<Token> OperatorQueue = new();
        List<Token> OutputQueue = new();

        while(tokens.Count != 1)
        {
            var token = tokens[0];
            tokens = tokens.Skip(1).ToList();

            if(Values.Contains(token.GetType()))
            {
                Console.WriteLine("Number " + Tokenizer.GetTokenAsHuman(token));
                OutputQueue.Add(token);
            } else if(Operators.Keys.Contains(token.GetType()))
            {
                Console.WriteLine("Operator " + Tokenizer.GetTokenAsHuman(token));

                while(true)
                {
                    if(OperatorQueue.Count == 0)
                        break;

                    bool satisfied = false;

                    if(!new [] {typeof(TokenParenStart), typeof(TokenParenEnd)}.Contains(OperatorQueue.Last().GetType()))
                    {
                        if(Operators[OperatorQueue.Last().GetType()].Item1 > Operators[token.GetType()].Item1)
                        {
                            satisfied = true;
                        } else if(Operators[OperatorQueue.Last().GetType()].Item1 == Operators[token.GetType()].Item1)
                        {
                            if(Operators[OperatorQueue.Last().GetType()].Item2 == false)
                                satisfied = true;
                        }
                    }

                    satisfied = satisfied && OperatorQueue.Last().GetType() != typeof(TokenParenStart);

                    if(!satisfied)
                        break;

                    OutputQueue.Add(OperatorQueue.Last());
                    OperatorQueue.RemoveAt(OperatorQueue.Count - 1);
                }

                OperatorQueue.Add(token);
            } else if(token.GetType() == typeof(TokenParenStart))
            {
                Console.WriteLine("Left " + Tokenizer.GetTokenAsHuman(token));
                OperatorQueue.Add(token);
            } else if(token.GetType() == typeof(TokenParenEnd))
            {
                Console.WriteLine("Right " + Tokenizer.GetTokenAsHuman(token));

                while(true)
                {
                    if(OperatorQueue.Count == 0)
                        break;

                    if(OperatorQueue.Last().GetType() == typeof(TokenParenStart))
                        break;

                    OutputQueue.Add(OperatorQueue.Last());
                    OperatorQueue.RemoveAt(OperatorQueue.Count - 1);
                }

                if(OperatorQueue.Count != 0 && OperatorQueue.Last().GetType() == typeof(TokenParenStart))
                {
                    OperatorQueue.RemoveAt(OperatorQueue.Count - 1);
                }
            }
        }

        OutputQueue.AddRange(OperatorQueue);

        return OutputQueue;
    }
}

