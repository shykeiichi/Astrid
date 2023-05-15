namespace Astrid;

public class Token
{
    public string value = "";

    public int lineStart;
    public int lineEnd;
    public int charStart;
    public int charEnd;  
}

internal class TokenIdentifier : Token
{
    public new string value;

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenIdentifier(string value, int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.value = value;

        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenString : Token
{
    public new string value;

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenString(string value, int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.value = value;
        
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenNumber : Token
{
    public new float value;

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenNumber(float value, int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.value = value;
        
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenBoolean : Token
{
    public new bool value;

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenBoolean(bool value, int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.value = value;
        
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenAssign : Token
{
    public new string value = "=";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenAssign(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenComma : Token
{
    public new string value = ",";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenComma(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenArrayStart : Token
{
    public new string value = "[";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenArrayStart(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenArrayEnd : Token
{
    public new string value = "]";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenArrayEnd(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenParenStart : Token
{
    public new string value = "(";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenParenStart(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenParenEnd : Token
{
    public new string value = ")";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenParenEnd(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenColon : Token
{
    public new string value = ":";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenColon(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenDoubleColon : Token
{
    public new string value = "::";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenDoubleColon(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenBlockStart : Token
{
    public new string value = "{";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenBlockStart(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenBlockEnd : Token
{
    public new string value = "}";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenBlockEnd(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenEOL : Token
{
    public new string value = ";";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenEOL(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenNamespaceSeparator : Token
{
    public new string value = ".";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenNamespaceSeparator(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenPlus : Token
{
    public new string value = "+";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenPlus(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenMinus : Token
{
    public new string value = "-";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenMinus(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenMultiply : Token
{
    public new string value = "*";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenMultiply(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenDivide : Token
{
    public new string value = "/";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenDivide(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenPower : Token
{
    public new string value = "**";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenPower(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenOr : Token
{
    public new string value = "||";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenOr(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenAnd : Token
{
    public new string value = "&&";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenAnd(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenNot : Token
{
    public new string value = "!";

    public new int lineStart;
    public new int lineEnd;
    public new int charStart;
    public new int charEnd;

    public TokenNot(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

public class InvalidTokenTypeException : Exception
{
    public dynamic token;

    public InvalidTokenTypeException(string message, dynamic token) : base(message)
    {
        this.token = token;
    }
}