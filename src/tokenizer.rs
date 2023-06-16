use crate::tokens::{Token, TokenProperties};

pub fn tokenize_file(input_file_vec: Vec<String>)
{
    let input_file: Vec<String> = input_file_vec.iter().map(|f| String::from(f.split("//").nth(0).unwrap())).collect();

    let mut is_string = false;
    let mut tokens: Vec<Token> = vec![];
    let mut current_word = String::new();

    let mut line_idx = -1;
    let mut char_idx = -1;

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
                    handle_current_word(&current_word, line_idx, char_idx, is_string, &mut tokens)
                    current_word = String::new();
                },
                _ => {}
            }
        }
    }
}