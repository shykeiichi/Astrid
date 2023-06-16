pub struct TokenProperties
{
    pub line_start: i32,
    pub line_end: i32,
    pub char_start: i32,
    pub char_end: i32
}

impl TokenProperties
{
    pub fn new(line_start: i32, line_end: i32, char_start: i32, char_end: i32) -> TokenProperties
    {
        TokenProperties { line_start, line_end, char_start, char_end }
    }
}

pub enum Token
{
    Identifier(String, TokenProperties),
    Keyword(String, TokenProperties),

    String(String, TokenProperties),
    Int(i32, TokenProperties),
    Float(f32, TokenProperties),
    Boolean(bool, TokenProperties),

    Assign(String, TokenProperties),
    AssignMinus(String, TokenProperties),
    AssignPlus(String, TokenProperties),
    AssignDivide(String, TokenProperties),
    AssignMultiply(String, TokenProperties),
    AssignPower(String, TokenProperties),
    AssignModulo(String, TokenProperties),

    Comma(String, TokenProperties),
    ArrayStart(String, TokenProperties),
    ArrayEnd(String, TokenProperties),
    ParenStart(String, TokenProperties),
    ParenEnd(String, TokenProperties),
    Colon(String, TokenProperties),
    DoubleColon(String, TokenProperties),
    BlockStart(String, TokenProperties),
    BlockEnd(String, TokenProperties),
    EOL(String, TokenProperties),
    NamespaceSeparator(String, TokenProperties),

    Plus(String, TokenProperties),
    Minus(String, TokenProperties),
    Multiply(String, TokenProperties),
    Divide(String, TokenProperties),
    Power(String, TokenProperties),
    Modulo(String, TokenProperties),

    Or(String, TokenProperties),
    And(String, TokenProperties),
    Not(String, TokenProperties),
    Equals(String, TokenProperties),
    NotEquals(String, TokenProperties),
    Greater(String, TokenProperties),
    GreaterEquals(String, TokenProperties),
    Lesser(String, TokenProperties),
    LesserEquals(String, TokenProperties),
}