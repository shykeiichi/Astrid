use crate::tokens::{Token, TokenProperties};

pub fn tokenize_file(input_file_vec: Vec<String>) -> Vec<Token>
{
    let input_file: Vec<String> = input_file_vec.iter().map(|f| String::from(f.split("//").nth(0).unwrap())).collect();

    let mut is_string = false;
    let mut tokens: Vec<Token> = vec![];
    let mut current_word = String::new();

    let mut line_idx: i32;
    let mut char_idx: i32;

    fn handle_current_word(current_word: &str, line_idx: i32, char_idx: i32, is_string: bool, tokens: &mut Vec<Token>)
    {
        let bool_values = [String::from("false"), String::from("true")];
        let keywords = ["enum", "class", "if", "while", "return", "match"].iter().map(|e| {
            return String::from(*e);
        }).collect::<Vec<String>>();

        if is_string
        {
            let length = current_word.len();
            tokens.push(Token::String(current_word.to_string(), TokenProperties::new(line_idx, line_idx, char_idx - length as i32, char_idx + 1)));
        }
        if current_word != ""
        {
            if !is_string
            {
                let mut word_without_end_suffix = current_word.to_string();
                word_without_end_suffix.pop();
            
                if let Ok(value) = current_word.parse::<i32>()
                {
                    let length = value.to_string().len() as i32;
                    tokens.push(Token::Int(value, TokenProperties::new(line_idx, line_idx, char_idx - length, char_idx)));
                } else if let Ok(value) = current_word.parse::<f32>()
                {
                    let length = value.to_string().len() as i32;
                    tokens.push(Token::Float(value, TokenProperties::new(line_idx, line_idx, char_idx - length, char_idx)));
                } else 
                {
                    let mut float_suffix_satisfied = false;
                    if let Ok(value) = word_without_end_suffix.parse::<f32>()
                    {
                        if current_word.chars().last().unwrap() == 'f'
                        {
                            let length = value.to_string().len() as i32;
                            tokens.push(Token::Float(value, TokenProperties::new(line_idx, line_idx, char_idx - length - 1, char_idx)));
                            float_suffix_satisfied = true;
                        }
                    }

                    if !float_suffix_satisfied
                    {
                        if bool_values.contains(&current_word.to_lowercase())
                        {
                            tokens.push(Token::Boolean(current_word.to_lowercase() == "true", TokenProperties::new(line_idx, line_idx, char_idx - current_word.len() as i32, char_idx)));
                        } else {
                            if keywords.contains(&current_word.to_string())
                            {
                                tokens.push(Token::Keyword(current_word.to_string(), TokenProperties::new(line_idx, line_idx, char_idx - current_word.len() as i32, char_idx)));
                            } else {
                                tokens.push(Token::Identifier(current_word.to_string(), TokenProperties::new(line_idx, line_idx, char_idx - current_word.len() as i32, char_idx)));
                            }
                        }
                    }
                }
            }
        }
    }

    line_idx = -1;
    for line in input_file
    {
        line_idx += 1;
        char_idx = -1;
        let mut skip_next = false;
        for c in line.chars()
        {
            char_idx += 1;
            if skip_next
            {
                skip_next = false;
                continue;
            }

            if is_string
            {
                if c == '"'
                {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();
                    is_string = false;
                    continue;
                }
                current_word.push(c);
                continue;
            }

            match c
            {
                '=' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();

                    if line.chars().nth((char_idx + 1) as usize).unwrap() != '='
                    {
                        tokens.push(Token::Assign(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                    } else 
                    {
                        tokens.push(Token::Equals(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 2)));
                        skip_next = true;
                    }
                },
                '"' => is_string = true,
                ',' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();

                    tokens.push(Token::Comma(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                },
                '[' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();

                    tokens.push(Token::ArrayStart(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                },
                ']' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();

                    tokens.push(Token::ArrayEnd(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                },
                '(' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();

                    tokens.push(Token::ParenStart(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                },
                ')' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();

                    tokens.push(Token::ParenEnd(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                },
                '{' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();

                    tokens.push(Token::BlockStart(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                },
                '}' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();

                    tokens.push(Token::BlockEnd(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                },
                ':' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();
                    
                    if line.len() as i32 <= char_idx + 1
                    {
                        tokens.push(Token::Colon(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                        break
                    }

                    if line.chars().nth((char_idx + 1) as usize).unwrap() != ':'
                    {
                        tokens.push(Token::Colon(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                    } else 
                    {
                        tokens.push(Token::DoubleColon(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 2)));
                        skip_next = true;
                    }
                },
                ';' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();

                    tokens.push(Token::EOL(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                },
                '+' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();
                    
                    if line.len() as i32 <= char_idx + 1
                    {
                        tokens.push(Token::Plus(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                        break
                    }

                    if line.chars().nth((char_idx + 1) as usize).unwrap() != '='
                    {
                        tokens.push(Token::Plus(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                    } else 
                    {
                        tokens.push(Token::AssignPlus(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 2)));
                        skip_next = true;
                    }
                },
                '-' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();
                    
                    if line.len() as i32 <= char_idx + 1
                    {
                        tokens.push(Token::Minus(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                        break
                    }

                    if line.chars().nth((char_idx + 1) as usize).unwrap() != '='
                    {
                        tokens.push(Token::Minus(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                    } else 
                    {
                        tokens.push(Token::AssignMinus(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 2)));
                        skip_next = true;
                    }
                },
                '*' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();
                    
                    if line.len() as i32 <= char_idx + 1
                    {
                        tokens.push(Token::Multiply(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                        break
                    }

                    if line.chars().nth((char_idx + 1) as usize).unwrap() != '='
                    {
                        tokens.push(Token::Multiply(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                    } else 
                    {
                        tokens.push(Token::AssignMultiply(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 2)));
                        skip_next = true;
                    }
                },
                '/' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();
                    
                    if line.len() as i32 <= char_idx + 1
                    {
                        tokens.push(Token::Divide(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                        break
                    }

                    if line.chars().nth((char_idx + 1) as usize).unwrap() != '='
                    {
                        tokens.push(Token::Divide(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                    } else 
                    {
                        tokens.push(Token::AssignDivide(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 2)));
                        skip_next = true;
                    }
                },
                '^' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();
                    
                    if line.len() as i32 <= char_idx + 1
                    {
                        tokens.push(Token::Power(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                        break
                    }

                    if line.chars().nth((char_idx + 1) as usize).unwrap() != '='
                    {
                        tokens.push(Token::Power(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                    } else 
                    {
                        tokens.push(Token::AssignPower(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 2)));
                        skip_next = true;
                    }
                },
                '%' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();
                    
                    if line.len() as i32 <= char_idx + 1
                    {
                        tokens.push(Token::Modulo(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                        break
                    }

                    if line.chars().nth((char_idx + 1) as usize).unwrap() != '='
                    {
                        tokens.push(Token::Modulo(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                    } else 
                    {
                        tokens.push(Token::AssignModulo(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 2)));
                        skip_next = true;
                    }
                },
                '.' => {
                    if let Err(_) = current_word.parse::<f32>()
                    {
                        handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                        current_word = String::new();

                        tokens.push(Token::NamespaceSeparator(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));                    
                    } else 
                    {
                        if c != ' '  {
                            current_word.push(c);
                        } else {
                            handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                            current_word = String::new();
                        }
                    } 
                },
                '|' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();
                    
                    if line.chars().nth((char_idx + 1) as usize).unwrap() != '|'
                    {
                        if c != ' '  {
                            current_word.push(c);
                        } else {
                            handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                            current_word = String::new();
                        }
                    } else
                    {
                        tokens.push(Token::Or(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 2)));
                        skip_next = true;
                    }
                },
                '&' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();
                    
                    if line.chars().nth((char_idx + 1) as usize).unwrap() != '&'
                    {
                        if c != ' '  {
                            current_word.push(c);
                        } else {
                            handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                            current_word = String::new();
                        }
                    } else
                    {
                        tokens.push(Token::And(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 2)));
                        skip_next = true;
                    }
                },
                '!' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();
                    
                    if line.chars().nth((char_idx + 1) as usize).unwrap() != '='
                    {
                        tokens.push(Token::Not(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                    } else
                    {
                        tokens.push(Token::NotEquals(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 2)));
                        skip_next = true;
                    }
                },
                '>' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();
                    
                    if line.chars().nth((char_idx + 1) as usize).unwrap() != '='
                    {
                        tokens.push(Token::Greater(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                    } else
                    {
                        tokens.push(Token::GreaterEquals(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 2)));
                        skip_next = true;
                    }
                },
                '<' => {
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                    current_word = String::new();
                    
                    if line.chars().nth((char_idx + 1) as usize).unwrap() != '='
                    {
                        tokens.push(Token::Lesser(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 1)));
                    } else
                    {
                        tokens.push(Token::LesserEquals(TokenProperties::new(line_idx, line_idx, char_idx, char_idx + 2)));
                        skip_next = true;
                    }
                },
                _ => 
                {
                    if c != ' '  {
                        current_word.push(c);
                    } else {
                        handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens);
                        current_word = String::new();
                    }
                }
            }
        }
    }

    tokens
}