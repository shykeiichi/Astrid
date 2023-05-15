using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Astrid;

public class AST { }

public class FunctionCall : AST
{
    public string function;
    public Dictionary<string, object> arguments;

    public FunctionCall(string function, Dictionary<string, object> arguments)
    {
        this.function = function;
        this.arguments = arguments;
    }
}

public class VariableDefine : AST
{
    public string label;
    public string type;
    public object value;

    public VariableDefine(string label, string type, object value)
    {
        this.label = label;
        this.type = type;
        this.value = value;
    }
}

public static class Parser
{
    public static List<AST> ParseTokens(List<Token> tokens)
    {
        List<AST> ast = new();

        while(tokens.Count > 0)
        {
            var token = tokens[0];
            if (token.GetType() == typeof(TokenIdentifier))
            {
                var identifierLabel = ((TokenIdentifier)token).value;
                tokens = tokens.Skip(1).ToList();
                token = tokens[0];

                if(token.GetType() == typeof(TokenParenStart))
                {
                    tokens = tokens.Skip(1).ToList();
                    token = tokens[0];
                    var parameters = ParseFunctionParameters(tokens.ToArray());
                    FunctionCall fc = new(identifierLabel, parameters.Item1);
                    ast.Add(fc);
                    tokens = parameters.Item2.ToList();

                    tokens = tokens.Skip(1).ToList();
                    token = tokens[0];

                    if(token.GetType() == typeof(TokenEOL))
                    {
                        tokens = tokens.Skip(1).ToList();
                        continue;
                    }
                } else if(token.GetType() == typeof(TokenColon))
                {
                    tokens = tokens.Skip(1).ToList();
                    token = tokens[0];

                    TokenIdentifier type;

                    if(token.GetType() == typeof(TokenIdentifier))
                    {
                        type = (TokenIdentifier)token;
                        tokens = tokens.Skip(1).ToList();
                        token = tokens[0];
                    } else {
                        throw new InvalidTokenTypeException($"Expected 'TokenIdentifier' got {Tokenizer.GetTokenAsHuman(token)}", token);
                    }

                    if(token.GetType() != typeof(TokenAssign))
                    {
                        throw new InvalidTokenTypeException($"Expected 'TokenAssign' got {Tokenizer.GetTokenAsHuman(token)}", token);
                    }

                    tokens = tokens.Skip(1).ToList();
                    token = tokens[0];

                    var value = token;

                    VariableDefine vd = new(identifierLabel, type.value, value);
                    ast.Add(vd);

                    tokens = tokens.Skip(1).ToList();
                    token = tokens[0];
                }
            }

            if(token.GetType() == typeof(TokenEOL))
            {
                tokens = tokens.Skip(1).ToList();
                continue;
            }
            Console.WriteLine(Tokenizer.GetTokenAsHuman(token));
        }

        return ast;
    }

    public static (Dictionary<string, object>, Token[]) ParseFunctionParameters(Token[] _tokens)
    {
        Dictionary<string, object> parameters = new();
        bool run = true;

        List<Token> tokens = _tokens.ToList();

        while (tokens.Count > 0 || !run)
        {
            var token = tokens[0];

            if(token.GetType() == typeof(TokenComma))
            {
                tokens = tokens.Skip(1).ToList();
                token = tokens[0];
            }

            TokenIdentifier parameterLabel;
            if (token.GetType() == typeof(TokenIdentifier))
            {
                parameterLabel = (TokenIdentifier)token;
            }
            else if (token.GetType() == typeof(TokenParenEnd))
            {
                run = false;

                return (parameters, tokens.ToArray());
            }
            else
            {
                throw new InvalidTokenTypeException($"Expected 'TokenIdentifier' got '{Tokenizer.GetTokenAsHuman(token)}", token);
            }

            tokens = tokens.Skip(1).ToList();
            token = tokens[0];

            if (token.GetType() != typeof(TokenColon))
                throw new InvalidTokenTypeException($"Expected 'TokenComma' got '{Tokenizer.GetTokenAsHuman(token)}", token);

            tokens = tokens.Skip(1).ToList();
            token = tokens[0];

            Token parameterValue;
            if (token.GetType() == typeof(TokenIdentifier) || token.GetType() == typeof(TokenString))
            {
                parameterValue = token;
            }
            else
            {
                throw new InvalidTokenTypeException($"Expected 'TokenIdentifier' or 'TokenString' got '{Tokenizer.GetTokenAsHuman(token)}", token);
            }

            if (parameterValue.GetType() == typeof(TokenIdentifier))
                parameters.Add(((TokenIdentifier)parameterLabel).value, ((TokenIdentifier)parameterValue).value);
            else if (parameterValue.GetType() == typeof(TokenString))
                parameters.Add(((TokenIdentifier)parameterLabel).value, ((TokenString)parameterValue).value);

            tokens = tokens.Skip(1).ToList();
            token = tokens[0];
        }

        return (parameters, tokens.ToArray());
    }

    public static void ParseEquation(Token[] tokens)
    {
        
    }
}

