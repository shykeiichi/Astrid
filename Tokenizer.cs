namespace Astrid;

public static class Tokenizer {
    public static List<string> Keywords = new() {"enum", "class", "if"};

    public static Token[] TokenizeFromFile(string path) {
        return TokenizeFromMemory(File.ReadAllLines(path));
    }   

    public static Token[] TokenizeFromMemory(string[] fileLines) {
        for(var i = 0; i < fileLines.Length; i++) {
            fileLines[i] = fileLines[i].Split("//")[0];
        } 

        List<Token> tokens = new List<Token>();

        string currentWord = "";
        
        int lineIdx;
        int charIdx;

        bool isString = false;
        string[] boolValues = new string[] {"false", "true"};

        void HandleCurrentWord() {
            if(currentWord != "") {
                if(isString) {
                    tokens.Add(new TokenString(currentWord, lineIdx, lineIdx, charIdx - currentWord.Length, charIdx));
                } else {
                    float value;
                    if(float.TryParse(currentWord, out value)) {
                        tokens.Add(new TokenNumber(value, lineIdx, lineIdx, charIdx - value.ToString().Length, charIdx));
                    } else {
                        if(boolValues.Contains(currentWord.ToLower())) {
                            tokens.Add(new TokenBoolean(currentWord.ToLower() == "true", lineIdx, lineIdx, charIdx - currentWord.Length, charIdx));
                        } else {
                            if(Keywords.Contains(currentWord))
                            {
                                
                            } else {
                                tokens.Add(new TokenIdentifier(currentWord, lineIdx, lineIdx, charIdx - currentWord.Length, charIdx));
                            }
                        }
                    }
                }
            }
            currentWord = "";
        }

        lineIdx = -1;
        foreach(string line in fileLines) {
            lineIdx++;
            charIdx = -1;
            var skipNext = false;
            foreach(char c in line) {
                charIdx++;
                if(skipNext)
                {
                    skipNext = false;
                    continue;
                }

                if(isString) {
                    if(c == '"') {
                        HandleCurrentWord();
                        isString = false;
                        continue;
                    }
                    currentWord += c;
                    continue;
                }

                switch(c) {
                    case '=': {
                        HandleCurrentWord();

                        tokens.Add(new TokenAssign(lineIdx, lineIdx, charIdx - 1, charIdx));
                    } break;
                    case '"': {
                        isString = true;
                    } break;
                    case ',': {
                        HandleCurrentWord();

                        tokens.Add(new TokenComma(lineIdx, lineIdx, charIdx - 1, charIdx));
                    } break;

                    case '[': {
                        HandleCurrentWord();

                        tokens.Add(new TokenArrayStart(lineIdx, lineIdx, charIdx - 1, charIdx));
                    } break;
                    case ']': {
                        HandleCurrentWord();

                        tokens.Add(new TokenArrayEnd(lineIdx, lineIdx, charIdx - 1, charIdx));
                    } break;
                    case '(': {
                        HandleCurrentWord();

                        tokens.Add(new TokenParenStart(lineIdx, lineIdx, charIdx - 1, charIdx));
                    } break;
                    case ')': {
                        HandleCurrentWord();

                        tokens.Add(new TokenParenEnd(lineIdx, lineIdx, charIdx - 1, charIdx));
                    } break;
                    case '{': {
                        HandleCurrentWord();

                        tokens.Add(new TokenBlockStart(lineIdx, lineIdx, charIdx - 1, charIdx));
                    } break;
                    case '}': {
                        HandleCurrentWord();

                        tokens.Add(new TokenBlockEnd(lineIdx, lineIdx, charIdx - 1, charIdx));
                    } break;
                    case ':': {
                        HandleCurrentWord();

                        if(line[charIdx + 1] != ':')
                            tokens.Add(new TokenColon(lineIdx, lineIdx, charIdx - 1, charIdx));
                        else {
                            tokens.Add(new TokenDoubleColon(lineIdx, lineIdx, charIdx - 1, charIdx + 1));
                            skipNext = true;
                        }
                    } break;
                    case ';': {
                        HandleCurrentWord();

                        tokens.Add(new TokenEOL(lineIdx, lineIdx, charIdx - 1, charIdx));
                    } break;
                    case '+': {
                        HandleCurrentWord();

                        tokens.Add(new TokenPlus(lineIdx, lineIdx, charIdx - 1, charIdx));
                    } break;
                    case '-': {
                        HandleCurrentWord();

                        tokens.Add(new TokenMinus(lineIdx, lineIdx, charIdx - 1, charIdx));
                    } break;
                    case '*': {
                        HandleCurrentWord();

                        if(line[charIdx + 1] != '*')
                            tokens.Add(new TokenMultiply(lineIdx, lineIdx, charIdx - 1, charIdx));
                        else {
                            tokens.Add(new TokenPower(lineIdx, lineIdx, charIdx - 1, charIdx + 1));
                            skipNext = true;
                        }
                    } break;
                    case '/': {
                        HandleCurrentWord();

                        tokens.Add(new TokenDivide(lineIdx, lineIdx, charIdx - 1, charIdx));
                    } break;
                    case '.': {
                        HandleCurrentWord();

                        tokens.Add(new TokenNamespaceSeparator(lineIdx, lineIdx, charIdx - 1, charIdx));
                    } break;
                    case '|': {
                        HandleCurrentWord();

                        if(line[charIdx + 1] != '|')
                            goto default;
                        else {
                            tokens.Add(new TokenOr(lineIdx, lineIdx, charIdx - 1, charIdx + 1));
                            skipNext = true;
                        }
                    } break;
                    case '&': {
                        HandleCurrentWord();

                        if(line[charIdx + 1] != '&')
                            goto default;
                        else {
                            tokens.Add(new TokenAnd(lineIdx, lineIdx, charIdx - 1, charIdx + 1));
                            skipNext = true;
                        }
                    } break;
                    case '!': {
                        HandleCurrentWord();

                        tokens.Add(new TokenNot(lineIdx, lineIdx, charIdx - 1, charIdx));
                    } break;
                    default: {
                        if(c != ' ') {
                            currentWord += c;
                        } else {
                            HandleCurrentWord();
                        }
                    } break;
                }
            }
        }

        return tokens.ToArray();
    }

    internal static string GetTokenAsHuman(Token t)
    {
        if (t.GetType() == typeof(TokenString))
        {
            return $"[{t}: \"{((dynamic)t).value}\"]";
        }
        else
        {
            return $"[{t}: '{((dynamic)t).value}']";
        }
    }
}