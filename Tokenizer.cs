namespace Astrid;

public static class Tokenizer {
    public static List<string> Keywords = new() {"enum", "class", "if", "while", "return", "match"};

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
            if(isString) {
                tokens.Add(new TokenString(currentWord, lineIdx, lineIdx, charIdx - currentWord.Length - 1, charIdx + 1));
            }
            if(currentWord != "") {
                if(!isString)
                {
                    int ivalue;
                    float fvalue;
                    string currentWordwithoutEndSuffix = currentWord.ToString();
                    currentWordwithoutEndSuffix = currentWordwithoutEndSuffix.Remove(currentWordwithoutEndSuffix.Length - 1, 1);
                    if(int.TryParse(currentWord, out ivalue))
                        tokens.Add(new TokenInt(ivalue.ToString(), lineIdx, lineIdx, charIdx - ivalue.ToString().Length, charIdx));
                    else if(float.TryParse(currentWord, out fvalue))
                        tokens.Add(new TokenFloat(fvalue.ToString(), lineIdx, lineIdx, charIdx - fvalue.ToString().Length, charIdx));
                    else 
                    {
                        bool floatsuffixsatisifed = false;
                        if(float.TryParse(currentWordwithoutEndSuffix, out fvalue)) {
                            if(currentWord.Last() == 'f')
                            {
                                tokens.Add(new TokenFloat(fvalue.ToString(), lineIdx, lineIdx, charIdx - fvalue.ToString().Length - 1, charIdx));
                                floatsuffixsatisifed = true;
                            }
                        }
                        
                        if(!floatsuffixsatisifed)
                        {
                            if(boolValues.Contains(currentWord.ToLower())) {
                                tokens.Add(new TokenBoolean((currentWord.ToLower() == "true").ToString().ToLower(), lineIdx, lineIdx, charIdx - currentWord.Length, charIdx));
                            } else {
                                if(Keywords.Contains(currentWord))
                                {
                                    tokens.Add(new TokenKeyword(currentWord, lineIdx, lineIdx, charIdx - currentWord.Length, charIdx));
                                } else {
                                    tokens.Add(new TokenIdentifier(currentWord, lineIdx, lineIdx, charIdx - currentWord.Length, charIdx));
                                }
                            }
                        }
                    }
                }
            }// else if(currentWord == "" && isString)
            // {
            //     tokens.Add(new TokenString(currentWord, lineIdx, lineIdx, charIdx - currentWord.Length - 1, charIdx + 1));
            // }
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

                        if(line[charIdx + 1] != '=')
                            tokens.Add(new TokenAssign(lineIdx, lineIdx, charIdx, charIdx + 1));
                        else {
                            tokens.Add(new TokenEquals(lineIdx, lineIdx, charIdx, charIdx + 1 + 1));
                            skipNext = true;
                        }
                    } break;
                    case '"': {
                        isString = true;
                    } break;
                    case ',': {
                        HandleCurrentWord();

                        tokens.Add(new TokenComma(lineIdx, lineIdx, charIdx, charIdx + 1));
                    } break;

                    case '[': {
                        HandleCurrentWord();

                        tokens.Add(new TokenArrayStart(lineIdx, lineIdx, charIdx, charIdx + 1));
                    } break;
                    case ']': {
                        HandleCurrentWord();

                        tokens.Add(new TokenArrayEnd(lineIdx, lineIdx, charIdx, charIdx + 1));
                    } break;
                    case '(': {
                        HandleCurrentWord();

                        tokens.Add(new TokenParenStart(lineIdx, lineIdx, charIdx, charIdx + 1));
                    } break;
                    case ')': {
                        HandleCurrentWord();

                        tokens.Add(new TokenParenEnd(lineIdx, lineIdx, charIdx, charIdx + 1));
                    } break;
                    case '{': {
                        HandleCurrentWord();

                        tokens.Add(new TokenBlockStart(lineIdx, lineIdx, charIdx, charIdx + 1));
                    } break;
                    case '}': {
                        HandleCurrentWord();

                        tokens.Add(new TokenBlockEnd(lineIdx, lineIdx, charIdx, charIdx + 1));
                    } break;
                    case ':': {
                        HandleCurrentWord();


                        if(line.Length <= charIdx + 1)
                        {
                            tokens.Add(new TokenColon(lineIdx, lineIdx, charIdx, charIdx + 1));
                            break;
                        }

                        if(line[charIdx + 1] != ':')
                            tokens.Add(new TokenColon(lineIdx, lineIdx, charIdx, charIdx + 1));
                        else {
                            tokens.Add(new TokenDoubleColon(lineIdx, lineIdx, charIdx, charIdx + 1 + 1));
                            skipNext = true;
                        }
                    } break;
                    case ';': {
                        HandleCurrentWord();

                        tokens.Add(new TokenEOL(lineIdx, lineIdx, charIdx, charIdx + 1));
                    } break;
                    case '+': {
                        HandleCurrentWord();

                        if(line[charIdx + 1] != '=')
                            tokens.Add(new TokenPlus(lineIdx, lineIdx, charIdx, charIdx + 1));
                        else {
                            tokens.Add(new TokenAssignPlus(lineIdx, lineIdx, charIdx, charIdx + 1 + 1));
                            skipNext = true;
                        }
                    } break;
                    case '-': {
                        HandleCurrentWord();

                        if(line[charIdx + 1] != '=')
                            tokens.Add(new TokenMinus(lineIdx, lineIdx, charIdx, charIdx + 1));
                        else {
                            tokens.Add(new TokenAssignMinus(lineIdx, lineIdx, charIdx, charIdx + 1 + 1));
                            skipNext = true;
                        }
                    } break;
                    case '*': {
                        HandleCurrentWord();

                        if(line[charIdx + 1] != '=')
                            tokens.Add(new TokenMultiply(lineIdx, lineIdx, charIdx, charIdx + 1));
                        else {
                            tokens.Add(new TokenAssignMultiply(lineIdx, lineIdx, charIdx, charIdx + 1 + 1));
                            skipNext = true;
                        }
                    } break;
                    case '/': {
                        HandleCurrentWord();

                        if(line[charIdx + 1] != '=')
                            tokens.Add(new TokenDivide(lineIdx, lineIdx, charIdx, charIdx + 1));
                        else {
                            tokens.Add(new TokenAssignDivide(lineIdx, lineIdx, charIdx, charIdx + 1 + 1));
                            skipNext = true;
                        }
                    } break;
                    case '^': {
                        HandleCurrentWord();

                        if(line[charIdx + 1] != '=')
                            tokens.Add(new TokenPower(lineIdx, lineIdx, charIdx, charIdx + 1));
                        else {
                            tokens.Add(new TokenAssignPower(lineIdx, lineIdx, charIdx, charIdx + 1 + 1));
                            skipNext = true;
                        }
                    } break;
                    case '%': {
                        HandleCurrentWord();

                        if(line[charIdx + 1] != '=')
                            tokens.Add(new TokenModulo(lineIdx, lineIdx, charIdx, charIdx + 1));
                        else {
                            tokens.Add(new TokenAssignPower(lineIdx, lineIdx, charIdx, charIdx + 1 + 1));
                            skipNext = true;
                        }
                    } break;
                    case '.': {
                        if(!float.TryParse(currentWord, out float fva))
                        {
                            HandleCurrentWord();

                            tokens.Add(new TokenNamespaceSeparator(lineIdx, lineIdx, charIdx, charIdx + 1));
                        } else 
                        {
                            goto default;
                        }
                    } break;
                    case '|': {
                        HandleCurrentWord();

                        if(line[charIdx + 1] != '|')
                            goto default;
                        else {
                            tokens.Add(new TokenOr(lineIdx, lineIdx, charIdx, charIdx + 1 + 1));
                            skipNext = true;
                        }
                    } break;
                    case '&': {
                        HandleCurrentWord();

                        if(line[charIdx + 1] != '&')
                            goto default;
                        else {
                            tokens.Add(new TokenAnd(lineIdx, lineIdx, charIdx, charIdx + 1 + 1));
                            skipNext = true;
                        }
                    } break;
                    case '!': {
                        HandleCurrentWord();

                        if(line[charIdx + 1] != '=')
                            tokens.Add(new TokenNot(lineIdx, lineIdx, charIdx, charIdx + 1));
                        else {
                            tokens.Add(new TokenNotEquals(lineIdx, lineIdx, charIdx, charIdx + 1 + 1));
                            skipNext = true;
                        }
                    } break;
                    case '>': {
                        HandleCurrentWord();

                        if(line[charIdx + 1] != '=')
                            tokens.Add(new TokenGreater(lineIdx, lineIdx, charIdx, charIdx + 1));
                        else {
                            tokens.Add(new TokenGreaterEquals(lineIdx, lineIdx, charIdx, charIdx + 1 + 1));
                            skipNext = true;
                        }
                    } break;
                    case '<': {
                        HandleCurrentWord();

                        if(line[charIdx + 1] != '=')
                            tokens.Add(new TokenLesser(lineIdx, lineIdx, charIdx, charIdx + 1));
                        else {
                            tokens.Add(new TokenLesserEquals(lineIdx, lineIdx, charIdx, charIdx + 1 + 1));
                            skipNext = true;
                        }
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
        string ret = "";
        if (t.GetType() == typeof(TokenString))
        {
            ret += $"[{t}: \"{((dynamic)t).value}\"]";
        }
        else
        {
            ret += $"[{t}: '{((dynamic)t).value}']";
        }

        // ret += $" cs: {t.charStart}, ce: {t.charEnd}, ls: {t.lineStart}, le: {t.lineEnd}";
        return ret;
    }

    internal static void Print(Token t)
    {
        Console.WriteLine(GetTokenAsHuman(t));
    }
}