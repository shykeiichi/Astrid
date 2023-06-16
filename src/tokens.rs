#[derive(Debug)]
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

#[derive(Debug)]
pub enum Token
{
    Identifier(String, TokenProperties),
    Keyword(String, TokenProperties),

    String(String, TokenProperties),
    Int(i32, TokenProperties),
    Float(f32, TokenProperties),
    Boolean(bool, TokenProperties),

    Assign(TokenProperties),
    AssignMinus(TokenProperties),
    AssignPlus(TokenProperties),
    AssignDivide(TokenProperties),
    AssignMultiply(TokenProperties),
    AssignPower(TokenProperties),
    AssignModulo(TokenProperties),

    Comma(TokenProperties),
    ArrayStart(TokenProperties),
    ArrayEnd(TokenProperties),
    ParenStart(TokenProperties),
    ParenEnd(TokenProperties),
    Colon(TokenProperties),
    DoubleColon(TokenProperties),
    BlockStart(TokenProperties),
    BlockEnd(TokenProperties),
    EOL(TokenProperties),
    NamespaceSeparator(TokenProperties),

    Plus(TokenProperties),
    Minus(TokenProperties),
    Multiply(TokenProperties),
    Divide(TokenProperties),
    Power(TokenProperties),
    Modulo(TokenProperties),

    Or(TokenProperties),
    And(TokenProperties),
    Not(TokenProperties),
    Equals(TokenProperties),
    NotEquals(TokenProperties),
    Greater(TokenProperties),
    GreaterEquals(TokenProperties),
    Lesser(TokenProperties),
    LesserEquals(TokenProperties),
}

impl Token
{
    fn get_value(self) -> String
    {
        return match self
        {
            Token::Identifier(a, _) => a,
            Token::Keyword(a, _) => a,

            Token::String(a, _) => a,
            Token::Int(a, _) => a.to_string(),
            Token::Float(a, _) => a.to_string(),
            Token::Boolean(a, _) => a.to_string(),

            Token::Assign(_) => String::from("="),
            Token::AssignMinus(_) => String::from("-="),
            Token::AssignPlus(_) => String::from("+="),
            Token::AssignDivide(_) => String::from("/="),
            Token::AssignMultiply(_) => String::from("*="),
            Token::AssignPower(_) => String::from("^="),
            Token::AssignModulo(_) => String::from("%="),

            Token::Comma(_) => String::from(","),
            Token::ArrayStart(_) => String::from("["),
            Token::ArrayEnd(_) => String::from("]"),
            Token::ParenStart(_) => String::from("("),
            Token::ParenEnd(_) => String::from(")"),
            Token::Colon(_) => String::from(":"),
            Token::DoubleColon(_) => String::from("::"),
            Token::BlockStart(_) => String::from("{"),
            Token::BlockEnd(_) => String::from("}"),
            Token::EOL(_) => String::from(";"),
            Token::NamespaceSeparator(_) => String::from("."),

            Token::Minus(_) => String::from("-"),
            Token::Plus(_) => String::from("+"),
            Token::Divide(_) => String::from("/"),
            Token::Multiply(_) => String::from("*"),
            Token::Power(_) => String::from("^"),
            Token::Modulo(_) => String::from("%"),

            Token::Or(_) => String::from("||"),
            Token::And(_) => String::from("&&"),
            Token::Not(_) => String::from("!"),
            Token::Equals(_) => String::from("=="),
            Token::NotEquals(_) => String::from("!="),
            Token::Greater(_) => String::from(">"),
            Token::GreaterEquals(_) => String::from(">="),
            Token::Lesser(_) => String::from("<"),
            Token::LesserEquals(_) => String::from("<="),
            
        }
    }
}