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

public class ASTExprBinary : AST
{
    public Token left;
    public Token op;
    public Token right;

    public ASTExprBinary(Token left, Token op, Token right)
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

    public class Expr {}
    public class ExprValue : Expr {
        public Token value;

        public ExprValue(Token value)
        {
            this.value = value;
        }
    }
    public class ExprBinary : Expr {
        public Expr left;
        public Token op;
        public Expr right;

        public ExprBinary(Expr left, Token op, Expr right)
        {
            this.left = left;
            this.op = op;
            this.right = right;
        }
    }

    public static List<Token> ParseExpression(Token[] tokens_)
    {
        List<Token> tokens = tokens_.ToList();
        List<AST> ast = new();

        Dictionary<Type, (int, bool)> Operators = new()
        {
            {typeof(TokenEquals), (0, false)},
            {typeof(TokenNotEquals), (0, false)},
            {typeof(TokenGreater), (1, false)},
            {typeof(TokenGreaterEquals), (1, false)},
            {typeof(TokenLesser), (1, false)},
            {typeof(TokenLesserEquals), (1, false)},
            {typeof(TokenPlus), (2, false)},
            {typeof(TokenMinus), (2, false)},
            {typeof(TokenDivide), (3, false)},
            {typeof(TokenMultiply), (3, false)},
            {typeof(TokenPower), (4, true)},
            {typeof(TokenNot), (4, false)},
        };

        Type[] Values = new [] {typeof(TokenInt), typeof(TokenFloat)};

        while(tokens.Count != 1)
        {
            
        }

        return OutputQueue;
    }
}

