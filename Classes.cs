namespace Astrid;

public class Token
{
    public string value;

    public int lineStart;
    public int lineEnd;
    public int charStart;
    public int charEnd;

    public Token(string value, int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.value = value;
        
        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }

    public Token(int lineStart, int lineEnd, int charStart, int charEnd)
    {
        this.value = "";

        this.lineStart = lineStart;
        this.lineEnd = lineEnd;
        this.charStart = charStart;
        this.charEnd = charEnd;
    }
}

internal class TokenIdentifier : Token
{
    public TokenIdentifier(string value, int lineStart, int lineEnd, int charStart, int charEnd) : base(value, lineStart, lineEnd, charStart, charEnd)
    {
    }
}

internal class TokenKeyword : Token
{
    public TokenKeyword(string value, int lineStart, int lineEnd, int charStart, int charEnd) : base(value, lineStart, lineEnd, charStart, charEnd)
    {
    }
}

internal class TokenString : Token
{
    public TokenString(string value, int lineStart, int lineEnd, int charStart, int charEnd) : base(value, lineStart, lineEnd, charStart, charEnd)
    {
    }
}

internal class TokenFloat : Token
{
    public TokenFloat(string value, int lineStart, int lineEnd, int charStart, int charEnd) : base(value, lineStart, lineEnd, charStart, charEnd)
    {
    }
}

internal class TokenInt : Token
{
    public TokenInt(string value, int lineStart, int lineEnd, int charStart, int charEnd) : base(value, lineStart, lineEnd, charStart, charEnd)
    {
    }
}

internal class TokenBoolean : Token
{
    public TokenBoolean(string value, int lineStart, int lineEnd, int charStart, int charEnd) : base(value, lineStart, lineEnd, charStart, charEnd)
    {
    }
}

internal class TokenAssign : Token
{
    public TokenAssign(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "=";
    }
}

internal class TokenAssignMinus : Token
{
    public TokenAssignMinus(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "-=";
    }
}

internal class TokenAssignPlus : Token
{
    public TokenAssignPlus(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "+=";
    }
}

internal class TokenAssignDivide : Token
{
    public TokenAssignDivide(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "/=";
    }
}

internal class TokenAssignMultiply : Token
{
    public TokenAssignMultiply(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "*=";
    }
}

internal class TokenAssignPower : Token
{
    public TokenAssignPower(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "^=";
    }
}

internal class TokenComma : Token
{
    public TokenComma(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = ",";
    }
}

internal class TokenArrayStart : Token
{
    public TokenArrayStart(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "[";
    }
}

internal class TokenArrayEnd : Token
{
    public TokenArrayEnd(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "]";
    }
}

internal class TokenParenStart : Token
{
    public TokenParenStart(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "(";
    }
}

internal class TokenParenEnd : Token
{
    public TokenParenEnd(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = ")";
    }
}

internal class TokenColon : Token
{
    public TokenColon(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = ":";
    }
}

internal class TokenDoubleColon : Token
{
    public TokenDoubleColon(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "::";
    }
}

internal class TokenBlockStart : Token
{
    public TokenBlockStart(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "{";
    }
}

internal class TokenBlockEnd : Token
{
    public TokenBlockEnd(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "}";
    }
}

internal class TokenEOL : Token
{
    public TokenEOL(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = ";";
    }
}

internal class TokenNamespaceSeparator : Token
{
    public TokenNamespaceSeparator(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = ".";
    }
}

internal class TokenPlus : Token
{
    public TokenPlus(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "+";
    }
}

internal class TokenMinus : Token
{
    public TokenMinus(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "-";
    }
}

internal class TokenMultiply : Token
{
    public TokenMultiply(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "*";
    }
}

internal class TokenDivide : Token
{
    public TokenDivide(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "/";
    }
}

internal class TokenPower : Token
{
    public TokenPower(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "^";
    }
}

internal class TokenOr : Token
{
    public TokenOr(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "||";
    }
}

internal class TokenAnd : Token
{
    public TokenAnd(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "&&";
    }
}

internal class TokenNot : Token
{
    public TokenNot(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "!";
    }
}

internal class TokenEquals : Token
{
    public TokenEquals(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "==";
    }
}

internal class TokenNotEquals : Token
{
    public TokenNotEquals(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "!=";
    }
}

internal class TokenGreater : Token
{
    public TokenGreater(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = ">";
    }
}

internal class TokenGreaterEquals : Token
{
    public TokenGreaterEquals(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = ">=";
    }
}

internal class TokenLesser : Token
{
    public TokenLesser(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "<";
    }
}

internal class TokenLesserEquals : Token
{
    public TokenLesserEquals(int lineStart, int lineEnd, int charStart, int charEnd) : base(lineStart, lineEnd, charStart, charEnd)
    {
        this.value = "<=";
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