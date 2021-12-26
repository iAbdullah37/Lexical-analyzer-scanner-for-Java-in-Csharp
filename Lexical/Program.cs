using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Lexical
{
    class Detect_comments
    {
        public void Single(ref string file)
        {
            // bool for detect " "
            bool isstring = true;
            for (int i = 0; i < file.Length; i++)
            {
                if (file[i] == '\"') isstring = !isstring;
                // detect start of single comment
                if (i+1 < file.Length && file[i]=='/' && file[i+1] == '/' && isstring)
                {
                    int cnt = 0,start = i; 
                    while (i + 1 < file.Length && (file[i].ToString()+ file[i+1]) != "\r\n")
                    {
                        cnt++;
                        i++;  
                    }
                    // remove Single Comment from file
                    string newstring = file.Remove(start, cnt+1);
                    file = newstring;
                    i -= (cnt+1);
                }
            }
        }
        public void Multi(ref string file)
        {
            // bool for detect " "
            bool isstring = true;
            for (int i = 0; i < file.Length; i++)
            {    //detect is not in string
                if (file[i] == '\"') isstring = !isstring;
                if (file[i] == '\r') isstring = true;
                // detect start of multiline comment
                if (i + 1 < file.Length && file[i] == '/' && file[i + 1] == '*' && isstring)
                {
                    int cnt = 0, start = i; i += 2;
                    while (i+1<file.Length && (file[i].ToString() + file[i + 1]) != "*/")
                    {
                        cnt++;
                        i++;

                    }
                    //remove multiline Comment from file
                    string newstring = file;

                    // check if returned string from remove not empty
                    try
                    {
                        newstring = file.Remove(start, cnt + 4);
                    }
                    catch
                    {
                        newstring = file;
                    }
                    file = newstring;
                    i -= cnt;
                    if (i < 0) i = 0;
                }
            }
        }
    }

    class Detect
    {
        private static Dictionary<string, bool> Escape_Sequences = new Dictionary<string, bool>();
        public static void insertEscape_Sequences()
        {
            Escape_Sequences.Add("\\b", true); Escape_Sequences.Add("\\t", true);
            Escape_Sequences.Add("\\n", true); Escape_Sequences.Add("\\f", true);
            Escape_Sequences.Add("\\r", true); Escape_Sequences.Add("\\\'", true);
            Escape_Sequences.Add("\\\"", true); Escape_Sequences.Add("\\\\", true);
        }
        private bool isLetter(char x)
        {
            if ((x >= 'a' && x <= 'z') || (x >= 'A' && x <= 'Z'))
            {
                return true;
            }
            return false;
        }
        private bool isDigit(char x)
        {
            if (x >= '0' && x <= '9')
            {
                return true;
            }
            return false;
        }
        private bool isHexD(char x)
        {
            if (isDigit(x) || x == 'a' || x == 'b' || x == 'c' || x == 'd' || x == 'e' || x == 'f' ||
                x == 'A' || x == 'B' || x == 'C' || x == 'D' || x == 'E' || x == 'F')
            {
                return true;
            }
            return false;
        }
        private bool isOctal(char x)
        {
            if (x >= '0' && x <= '7')
            {
                return true;
            }
            return false;
        }
        private bool isExponentPart(string word, int index)
        {
            if (index == word.Length - 1) return false;
            index++;
            // check if char after e is not + or - or digit 
            if (!(word[index] == '+' || word[index] == '-' || isDigit(word[index])))
            {
                return false;
            }
            if (word[index] == '+' || word[index] == '-') index++;
            for (int i = index; i < word.Length - 1; i++)
            {
                if (!isDigit(word[i]))
                {
                    return false;
                }
            }
            char x = word[word.Length - 1];
            if (!(isDigit(x) || x == 'f' || x == 'F' || x == 'D' || x == 'd'))
            {
                return false;
            }
            return true;
        }
        private bool isUnicodeEscape(string word)
        {
            if (word.Length != 8) return false;
            if (!(word[0] == '\'' && word[1] == '\\' && word[2] == 'u' && isHexD(word[3]) &&
                isHexD(word[4]) && isHexD(word[5]) && isHexD(word[6]) && word[7] == '\''))
            {
                return false;
            }
            return true;
        }
        public bool detect_Identifier(string word)
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (i == 0)
                {
                    if (!(word[0] == '_' || isLetter(word[0])))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!(word[i] == '_' || isLetter(word[i]) || isDigit(word[i])))
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        public bool detect_Decimal(string word)
        {
            if (word.Length == 1 && isDigit(word[0]))
            {
                return true;
            }
            for (int i = 0; i < word.Length; i++)
            {
                if (i == 0)
                {
                    if (word[i] == '0')
                    {
                        if (word.Length == 2 && (word[1] == 'l' || word[1] == 'L'))
                        {
                            return true;
                        }
                        return false;
                    }
                    else if (!isDigit(word[i]))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!isDigit(word[i]) && i == word.Length - 1)
                    {
                        if (word[i] == 'L' || word[i] == 'l')
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    if (!isDigit(word[i]))
                    {
                        return false;
                    }

                }

            }
            return true;
        }
        public bool detect_Hex(string word)
        {
            if (word.Length > 2)
            {
                if (word[0] == '0' && (word[1] == 'X' || word[1] == 'x'))
                {
                    if (word.Length == 3 && (word[2] == 'l' || word[2] == 'L')) return false;
                    for (int i = 2; i < word.Length; i++)
                    {
                        if (!isHexD(word[i]) && i == word.Length - 1)
                        {
                            if (word[i] == 'L' || word[i] == 'l')
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        if (!isHexD(word[i]))
                        {
                            return false;
                        }
                    }
                    return true;
                }

            }
            return false;
        }
        public bool detect_Octal(string word)
        {
            if (word.Length > 1)
            {
                if (word[0] == '0')
                {
                    if (word.Length == 2 && (word[1] == 'l' || word[1] == 'L')) return false;
                    for (int i = 1; i < word.Length; i++)
                    {
                        if (!isOctal(word[i]) && i == word.Length - 1)
                        {
                            if (word[i] == 'L' || word[i] == 'l')
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        if (!isOctal(word[i]))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        public bool detect_Binary(string word)
        {
            if (word.Length > 2)
            {
                if (word[0] == '0' && (word[1] == 'b' || word[1] == 'B'))
                {
                    if (word.Length == 3 && (word[2] == 'l' || word[2] == 'L')) return false;
                    for (int i = 2; i < word.Length; i++)
                    {
                        if (!(word[i] == '1' || word[i] == '0') && i == word.Length - 1)
                        {
                            if (word[i] == 'L' || word[i] == 'l')
                            {
                                return true;
                            }
                            else
                            {
                                return false;
                            }
                        }
                        if (!(word[i] == '1' || word[i] == '0'))
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            return false;
        }
        public bool detect_Float(string word)
        {
            if (word.Contains('.'))
            {
                // [.][digit][ExponentPart]* [FloatTypeSuffix]*
                if (word[0] == '.')
                {
                    if (!(isDigit(word[1])))
                    {
                        return false;
                    }
                    int index = 1;
                    while (index<word.Length && isDigit(word[index]))
                    {
                        index++;
                    }
                    if (index == word.Length) return true;
                    if (!(word[index] == 'e' || word[index] == 'E' || word[index] == 'F' || word[index] == 'f' ||
                        word[index] == 'd' || word[index] == 'D'))
                    {
                        return false;
                    }
                    // contain [ExponentPart]
                    if (word[index] == 'e' || word[index] == 'E')
                    {
                        if (!isExponentPart(word, index))
                        {
                            return false;
                        }
                    }
                    // contain [FloatTypeSuffix]
                    else
                    {
                        if (index != word.Length - 1)
                        {
                            return false;
                        }
                    }
                    return true;
                }
                // [digit][.][digit]* [ExponentPart]* [FloatTypeSuffix]*
                else
                {
                    int index = 0; string leftD = "";
                    for (index = 0; index < word.Length; index++)
                    {
                        if (!isDigit(word[index])) break;
                        leftD += word[index];
                    }
                    if (!(detect_Decimal(leftD) && leftD[leftD.Length - 1].ToString().ToLower() != "l") || index == word.Length) return false;
                    if (word[index] != '.') return false;
                    index++;
                    if (index == word.Length) return true;
                    if (!(word[index] == 'e' || word[index] == 'E' || word[index] == 'F' || word[index] == 'f' ||
                        word[index] == 'd' || word[index] == 'D' || isDigit(word[index])))
                    {
                        return false;
                    }
                    // contain Digits
                    if (isDigit(word[index]))
                    {
                        for (; index < word.Length; index++)
                        {
                            if (!isDigit(word[index])) break;
                        }
                    }
                    if (index == word.Length) return true;
                    if (!(word[index] == 'e' || word[index] == 'E' || word[index] == 'F' || word[index] == 'f' ||
                        word[index] == 'd' || word[index] == 'D'))
                    {
                        return false;
                    }
                    // contain ExponentPart
                    if (word[index] == 'e' || word[index] == 'E')
                    {
                        if (!isExponentPart(word, index))
                        {
                            return false;
                        }
                    }
                    else
                    {
                        if (index != word.Length - 1)
                        {
                            return false;
                        }
                    }
                    return true;
                }
            }
            else
            {
                // [Digits] [ExponentPart] [FloatTypeSuffix]* 
                if (word.Contains('e') || word.Contains('E'))
                {
                    int index = 0; string leftD = "";
                    for (index = 0; index < word.Length; index++)
                    {
                        if (!isDigit(word[index])) break;
                        leftD += word[index];
                    }
                    if (!(detect_Decimal(leftD) && leftD[leftD.Length - 1].ToString().ToLower() != "l") || index == word.Length) return false;
                    if (word[index] != 'e') return false;
                    if (!isExponentPart(word, index)) return false;

                }
                // [Digits] [ExponentPart]* FloatTypeSuffix
                else
                {
                    int index = 0; string leftD = "";
                    for (index = 0; index < word.Length; index++)
                    {
                        if (!isDigit(word[index])) break;
                        leftD += word[index];
                    }
                    if (leftD.Length == 0)
                    {
                        return false;
                    }

                    if (word[index] == 'e')
                    {
                        if (!isExponentPart(word, index)) return false;
                    }
                    if ((index == word.Length) || (word[index] != 'f' && word[index] != 'F' && word[index] != 'd' && word[index] != 'D')) return false;
                    else return true;
                }
            }
            return true;
        }
        public bool detect_Character_Literals(string word)
        {
            if (word.Length == 2) return true;
            if (word.Length == 3 && word[1] != '\\') return true;
            else
            {
                string not = "";
                for (int i = 1; i < word.Length - 1; i++)
                {
                    not += word[i];
                }
                if (isUnicodeEscape(word) || Escape_Sequences.ContainsKey(not)) return true;
            }
            return false;
        }
        public bool detect_String_Literals(string word)
        {
            for (int i = 0; i < word.Length; i++)
            {
                if (word[i] == '\\')
                {
                    string x = "\\";
                    if (i + 5 != word.Length && word[i + 1] == 'u')
                    {
                        x += word[i + 1];
                        x += word[i + 2];
                        x += word[i + 3];
                        x += word[i + 4];
                        x += word[i + 5];
                        if (!isUnicodeEscape('\'' + x + '\'')) return false;
                    }
                    else
                    {
                        i++;
                        if (i == word.Length) return false;
                        x += word[i];
                        if (!Escape_Sequences.ContainsKey(x)) return false;
                    }
                }
            }
            return true;
        }
        public bool detect_TextBlocks(string word)
        {
            for (int i = 0; i < word.Length - 3; i++)
            {
                if (word[i] == '\\')
                {
                    string x = "\\";
                    if (i + 5 != word.Length && word[i + 1] == 'u')
                    {
                        x += word[i + 1];
                        x += word[i + 2];
                        x += word[i + 3];
                        x += word[i + 4];
                        x += word[i + 5];
                        if (!isUnicodeEscape('\'' + x + '\'')) return false;
                    }
                    else
                    {
                        i++;
                        if (i == word.Length) return false;
                        x += word[i];
                        if (!Escape_Sequences.ContainsKey(x)) return false;
                    }
                }
            }
            return true;
        }
    }

    public class access
    {
        public string ww = "";
        public void insert(string file)
        {
            string[] vs = new string[1];
            vs[0] = file;
            Program.Main(vs);
        }
        public void sent(string x)
        {
            ww += x;
        }
    }

    public class Program 
    {
        public static Dictionary<string, bool> Keywords = new Dictionary<string, bool>();
        public static Dictionary<string, bool> Var_Type = new Dictionary<string, bool>();
        public static Dictionary<string, bool> Operators = new Dictionary<string, bool>();
        public static Dictionary<string, bool> Separators = new Dictionary<string, bool>();
        public static Dictionary<string, bool> Spaces = new Dictionary<string, bool>();
        public static List<KeyValuePair<string, string>> LexicalLexeme = new List<KeyValuePair<string, string>>();
        public static List<KeyValuePair<string, string>> Symbol_Table = new List<KeyValuePair<string, string>>();
        private static bool searchinList(ref List<KeyValuePair<string, string>> list,string key)
        {
            for(int i = 0; i < list.Count; i++)
            {
                if (list[i].Key == key) return true;
            }
            return false;
        }
        static void insertKeywords()
        {
            Keywords.Add("abstract", true); Keywords.Add("continue", true);
            Keywords.Add("for", true);  Keywords.Add("new", true);
            Keywords.Add("switch", true);Keywords.Add("assert", true);
            Keywords.Add("default", true);Keywords.Add("if", true);
            Keywords.Add("package", true);Keywords.Add("synchronized", true);
            Keywords.Add("boolean", true);Keywords.Add("do", true);
            Keywords.Add("goto", true);Keywords.Add("private", true);
            Keywords.Add("this", true);Keywords.Add("break", true);
            Keywords.Add("double", true);Keywords.Add("implements", true);
            Keywords.Add("protected", true);Keywords.Add("throw", true);
            Keywords.Add("byte", true);Keywords.Add("else", true);
            Keywords.Add("import", true);Keywords.Add("public", true);
            Keywords.Add("throws", true);Keywords.Add("case", true);
            Keywords.Add("enum", true);Keywords.Add("instanceof", true);
            Keywords.Add("return", true); Keywords.Add("transient", true);
            Keywords.Add("catch", true); Keywords.Add("extends", true);
            Keywords.Add("int", true); Keywords.Add("short", true);
            Keywords.Add("try", true); Keywords.Add("char", true);
            Keywords.Add("final", true); Keywords.Add("interface", true);
            Keywords.Add("static", true); Keywords.Add("void", true);
            Keywords.Add("class", true); Keywords.Add("finally", true);
            Keywords.Add("long", true); Keywords.Add("strictfp", true);
            Keywords.Add("volatile", true); Keywords.Add("const", true);
            Keywords.Add("float", true); Keywords.Add("native", true);
            Keywords.Add("super", true); Keywords.Add("while", true);
        }
        static void insertVarType()
        {
            Var_Type.Add("boolean", true); Var_Type.Add("float", true);
            Var_Type.Add("char", true); Var_Type.Add("short", true);
            Var_Type.Add("int", true); Var_Type.Add("byte", true);
            Var_Type.Add("double", true); Var_Type.Add("long", true);
            Var_Type.Add("void", true); Var_Type.Add("class", true);
        }
        static void insertOperators()
        {
            Operators.Add("+", true);   Operators.Add("++", true);
            Operators.Add("+=", true);  Operators.Add("-", true);
            Operators.Add("--", true);  Operators.Add("-=", true);
            Operators.Add("*", true);   Operators.Add("*=", true);
            Operators.Add("/", true);   Operators.Add("/=", true);
            Operators.Add("%", true);   Operators.Add("%=", true);
            Operators.Add("=", true);   Operators.Add("==", true);
            Operators.Add("<", true);   Operators.Add("<=", true);
            Operators.Add(">", true);   Operators.Add(">=", true);
            Operators.Add("&", true);   Operators.Add("&&", true);
            Operators.Add("&=", true);  Operators.Add("|", true);
            Operators.Add("||", true);  Operators.Add("|=", true);
            Operators.Add("^", true);   Operators.Add("^=", true);
            Operators.Add("!", true);   Operators.Add("~", true);
            Operators.Add("?", true);   Operators.Add("->", true);
            Operators.Add("<<", true);  Operators.Add(">>", true);
            Operators.Add("!=", true);  Operators.Add(":", true);
        }
        static void insertSeparators()
        {
            Separators.Add("(", true);  Separators.Add(")", true);
            Separators.Add("{", true);  Separators.Add("}", true);
            Separators.Add("[", true);  Separators.Add("]", true);
            Separators.Add(";", true);  Separators.Add(",", true);
            Separators.Add(".", true);  Separators.Add("...", true);
            Separators.Add("@", true);  Separators.Add("::", true);
        }
        static void insertSpaces()
        {
            Spaces.Add(" ", true);
            Spaces.Add("\t", true);
            Spaces.Add("\f", true);
            Spaces.Add("\n", true);
            Spaces.Add("\r", true);
            Spaces.Add("\r\n", true);
        }
        // variables to send data to GUI
        public static string res = "",table=""; 
        public static void Main(string[] args)
        {
            // To check if the elements had been inserted to render GUI
            if(Keywords.Count == 0)
            {
                insertKeywords();
                insertOperators();
                insertSeparators();
                insertSpaces();
                insertVarType();
                Detect.insertEscape_Sequences();
            }
            string file = args[0];
            // Detect All Comments and remove them
            Detect_comments detect = new Detect_comments();
            detect.Single(ref file);
            detect.Multi(ref file);
            //define object to detect the lexemes
            Detect det = new Detect();
            access acc = new access();
            //define word to check the lexemes
            string word = "";
            //loop on the file to split and detect
            for (int i = 0; i < file.Length; i++)
            {
                // detect Text Block
                if (i + 2 < file.Length && file[i] == '\"' && file[i + 1] == '\"' && file[i + 2] == '\"')
                {
                    word += "\"\"\"";
                    i += 3;
                    while (i < file.Length)
                    {
                        if (i + 2 < file.Length && file[i] == '\"' && file[i + 1] == '\"' && file[i + 2] == '\"')
                        {
                            word += "\"\"\"";
                            i += 3;
                            if (word.Length > 0 && det.detect_TextBlocks(word))
                            {
                                KeyValuePair<string, string> item = new KeyValuePair<string, string>(word, "TextBlock");
                                LexicalLexeme.Add(item);
                                res += word + "\t\t\t\t TextBlock\r\n";
                                Console.WriteLine("\n" + word + "--------> TextBlock\n");
                            }
                            break;
                        }
                        word += file[i];
                        i++;
                    }
                    word = "";
                    continue;
                }
                // detect Character Literals
                if (file[i] == '\'')
                {
                    word += file[i];
                    i++;
                    while (i<file.Length && file[i] != '\r')
                    {
                        if (file[i] == '\'')
                        {
                            if (i - 2 >= 0 && file[i - 1] == '\\' && file[i - 2] == '\\')
                            {
                                word += file[i];
                                if (word.Length > 0 && det.detect_Character_Literals(word))
                                {
                                    KeyValuePair<string, string> item = new KeyValuePair<string, string>(word, "CharacterLiterals");
                                    LexicalLexeme.Add(item);
                                    res += word + "\t\t\t\t CharacterLiterals\r\n";
                                    Console.WriteLine("\n" + word + "--------> CharacterLiterals\n");
                                }
                                else
                                {
                                    res += word + "\t\t\t\t NOT FOUND\r\n";
                                }
                                break;
                            }
                            else if (i - 1 >= 0 && file[i - 1] != '\\')
                            {
                                word += file[i];
                                if (word.Length > 0 && det.detect_Character_Literals(word))
                                {
                                    KeyValuePair<string, string> item = new KeyValuePair<string, string>(word, "CharacterLiterals");
                                    LexicalLexeme.Add(item);
                                    res += word + "\t\t\t\t CharacterLiterals\r\n";
                                    Console.WriteLine("\n" + word + "--------> CharacterLiterals\n");
                                }
                                else
                                {
                                    res += word + "\t\t\t\t NOT FOUND\r\n";
                                }
                                break;
                            }
                        }
                        word += file[i];
                        i++;
                    }
                    word = "";
                    continue;
                }
                // detect String Literals
                if (file[i] == '\"')
                {
                    word += file[i];
                    i++;
                    while (i < file.Length && file[i] != '\r')
                    {
                        if (file[i] == '\"')
                        {
                            if (i - 1 >= 0 && file[i - 1] != '\\')
                            {
                                word += file[i];
                                if (word.Length > 0 && det.detect_String_Literals(word))
                                {
                                    KeyValuePair<string, string> item = new KeyValuePair<string, string>(word, "StringLiterals");
                                    LexicalLexeme.Add(item);
                                    res += word + "\t\t\t\t StringLiterals\r\n";
                                    Console.WriteLine("\n" + word + "--------> StringLiterals\n");
                                }
                                else
                                {
                                    res += word + "\t\t\t\t NOT FOUND\r\n";
                                }
                                break;
                            }
                            else if (i - 2 >= 0 && file[i - 1] == '\\' && file[i - 2] == '\\')
                            {
                                word += file[i];
                                if (word.Length > 0 && det.detect_Character_Literals(word))
                                {
                                    KeyValuePair<string, string> item = new KeyValuePair<string, string>(word, "StringLiterals");
                                    LexicalLexeme.Add(item);
                                    res += word + "\t\t\t\t StringLiterals\r\n";
                                    Console.WriteLine("\n" + word + "--------> StringLiterals\n");
                                }
                                else
                                {
                                    res += word + "\t\t\t\t NOT FOUND\r\n";
                                }
                                break;
                            }
                        }
                        word += file[i];
                        i++;
                    }
                    word = "";
                    continue;
                }
                //split the file if we found operator ot seprator or white space
                if (Operators.ContainsKey(file[i].ToString()) || Separators.ContainsKey(file[i].ToString()) ||
                    Spaces.ContainsKey(file[i].ToString()))
                {
                    // detect floating point to skip it from Separators so it doesnt skip it
                    if (file[i] == '.' && ((i > 0 && file[i - 1] >= '0' && file[i - 1] <= '9') || (i < file.Length - 1 && file[i + 1] >= '0' && file[i + 1] <= '9')))
                    {
                        word += file[i];
                        continue;
                    }
                    // detect + or - of ExponentPart to skip it from Operators so it doesnt skip it
                    if ((file[i] == '+' || file[i] == '-') &&(i > 0)&& (file[i - 1] == 'e' || file[i - 1] == 'E'))
                    {
                        int index = i - 2;
                        bool isnot = false, containD = false;
                        while (index >= 0)
                        {// go back to make sure all are numbers
                            if (Operators.ContainsKey(file[index].ToString()) || Separators.ContainsKey(file[index].ToString()) ||
                                    Spaces.ContainsKey(file[index].ToString()))
                            {
                                if (file[index] == '.' && ((index > 0 && file[index - 1] >= '0' && file[index - 1] <= '9') || (index < file.Length - 1 && file[index + 1] >= '0' && file[index + 1] <= '9')))
                                {
                                    index--;
                                    continue;
                                }
                                else
                                {
                                    break;
                                }

                            }
                            else
                            {
                                if (!(file[index] >= '0' && file[index] <= '9'))
                                {
                                    isnot = true;
                                    break;
                                }
                                else
                                {
                                    containD = true;
                                }
                            }
                            index--;
                        }
                        if (isnot == false && containD == true)
                        {
                            word += file[i];
                            continue;
                        }
                    }
                    // detect identifiers & keywords & null & true & false
                    if (word.Length > 0 && det.detect_Identifier(word))
                    {
                        // Detect Keyword
                        if (Keywords.ContainsKey(word))
                        {
                            KeyValuePair<string, string> item = new KeyValuePair<string, string>(word,"Keyword");
                            LexicalLexeme.Add(item);
                            res += word + "\t\t\t\t Keyword\r\n";
                            Console.WriteLine("\n" + word + "--------> Keyword\n");
                        }
                        // Detect NullLiteral
                        else if (word.ToLower() == "null")
                        {
                            KeyValuePair<string, string> item = new KeyValuePair<string, string>(word, "NullLiteral");
                            LexicalLexeme.Add(item);
                            res += word + "\t\t\t\t NullLiteral\r\n";
                            Console.WriteLine("\n" + word + "--------> NullLiteral\n");
                        }
                        // Detect BooleanLiteral
                        else if (word.ToLower() == "false" || word.ToLower() == "true")
                        {
                            KeyValuePair<string, string> item = new KeyValuePair<string, string>(word, "BooleanLiteral");
                            LexicalLexeme.Add(item);
                            res += word + "\t\t\t\t BooleanLiteral\r\n";
                            Console.WriteLine("\n" + word + "--------> BooleanLiteral\n");
                        }
                        // Detect Identifiers
                        else
                        {
                            KeyValuePair<string, string> item = new KeyValuePair<string, string>(word, "Identifier");
                            LexicalLexeme.Add(item);
                            res += word + "\t\t\t\t Identifier\r\n";
                            Console.WriteLine("\n" + word + "--------> Identifier\n");
                        }
                    }
                    // Detect Decimal
                    else if (word.Length > 0 && det.detect_Decimal(word))
                    {
                        KeyValuePair<string, string> item = new KeyValuePair<string, string>(word, "DecimalNumeral");
                        LexicalLexeme.Add(item);
                        res += word + "\t\t\t\t DecimalNumeral\r\n";
                        Console.WriteLine("\n" + word + "--------> DecimalNumeral\n");
                    }
                    // Detect HexInteger
                    else if (word.Length > 0 && det.detect_Hex(word))
                    {
                        KeyValuePair<string, string> item = new KeyValuePair<string, string>(word, "HexIntegerLiteral");
                        LexicalLexeme.Add(item);
                        res += word + "\t\t\t\t HexIntegerLiteral\r\n";
                        Console.WriteLine("\n" + word + "--------> HexIntegerLiteral\n");
                    }
                    // Detect OctalInteger
                    else if (word.Length > 0 && det.detect_Octal(word))
                    {
                        KeyValuePair<string, string> item = new KeyValuePair<string, string>(word, "OctalIntegerLiteral");
                        LexicalLexeme.Add(item);
                        res += word + "\t\t\t\t OctalIntegerLiteral\r\n";
                        Console.WriteLine("\n" + word + "--------> OctalIntegerLiteral\n");
                    }
                    // Detect BinaryInteger
                    else if (word.Length > 0 && det.detect_Binary(word))
                    {
                        KeyValuePair<string, string> item = new KeyValuePair<string, string>(word, "BinaryIntegerLiteral");
                        LexicalLexeme.Add(item);
                        res += word + "\t\t\t\t BinaryIntegerLiteral\r\n";
                        Console.WriteLine("\n" + word + "--------> BinaryIntegerLiteral\n");
                    }
                    // Detect Floating-Point Literals
                    else if (word.Length > 0 && ((word[0] >= '0' && word[0] <= '9') || word[0] == '.') && det.detect_Float(word))
                    {
                        KeyValuePair<string, string> item = new KeyValuePair<string, string>(word, "Floating-Point Literals");
                        LexicalLexeme.Add(item);
                        res += word + "\t\t\t\t Floating Literals\r\n";
                        Console.WriteLine("\n" + word + "--------> Floating-Point Literals\n");
                    }
                    else
                    {
                        if (!Spaces.ContainsKey(word) && word!="")
                            res += word + "\t\t\t\t NOT FOUND\r\n";
                    }
                    // detect Separators
                    if (Separators.ContainsKey(file[i].ToString()))
                    {
                        // detect Separators contain of three characters
                        if (i + 2 < file.Length && (file[i].ToString() + file[i + 1] + file[i + 2] == "..."))
                        {
                            KeyValuePair<string, string> item = new KeyValuePair<string, string>("...", "Separator");
                            LexicalLexeme.Add(item);
                            res += "..." + "\t\t\t\t Separator\r\n";
                            Console.WriteLine("\n" + "..." + "--------> Separator\n");
                            i += 2;
                        }
                        // detect Separators contain of one characters
                        else
                        {
                            KeyValuePair<string, string> item = new KeyValuePair<string, string>(file[i].ToString(), "Separator");
                            LexicalLexeme.Add(item);
                            res += file[i] + "\t\t\t\t Separator\r\n";
                            Console.WriteLine("\n" + file[i] + "--------> Separator\n");
                        }
                    }
                    // detect Operators
                    else if (Operators.ContainsKey(file[i].ToString()))
                    {
                        if (file[i] == ':')
                        {//detect seprator have operator inside
                            if (i + 1 < file.Length && (file[i].ToString() + file[i + 1] == "::"))
                            {
                                KeyValuePair<string, string> item = new KeyValuePair<string, string>("::", "Separator");
                                LexicalLexeme.Add(item);
                                res += "::" + "\t\t\t\t Separator\r\n";
                                Console.WriteLine("\n" + "::" + "--------> Separator\n");
                            }
                        }
                        // detect operators contain of two characters
                        else if (i + 1 < file.Length && Operators.ContainsKey(file[i].ToString()+file[i + 1].ToString()))
                        {
                            KeyValuePair<string, string> item = new KeyValuePair<string, string>(file[i].ToString() + file[i + 1], "Operator");
                            LexicalLexeme.Add(item);
                            res += file[i].ToString() + file[i+1] + "\t\t\t\t Operator\r\n";
                            Console.WriteLine("\n" + file[i].ToString() + file[i + 1] + "--------> Operator\n");
                            i++;
                        }
                        // detect operators contain of one characters
                        else
                        {
                            KeyValuePair<string, string> item = new KeyValuePair<string, string>(file[i].ToString(), "Operator");
                            LexicalLexeme.Add(item);
                            res += file[i] + "\t\t\t\t Operator\r\n";
                            Console.WriteLine("\n" + file[i] + "--------> Operator\n");
                        }
                    }
                    // clear word after check it
                    word = "";
                }
                else
                {
                    word += file[i];
                }
                
            }
            // Create Symbol Table
            for(int i = 0; i < LexicalLexeme.Count; i++)
            {
                if(LexicalLexeme[i].Value == "Identifier")
                {
                    if (i > 0 && LexicalLexeme[i-1].Key == "class")
                    {
                        KeyValuePair<string, string> item = new KeyValuePair<string, string>(LexicalLexeme[i].Key, "class");
                        if(!searchinList(ref Symbol_Table, LexicalLexeme[i].Key))
                        {
                            Symbol_Table.Add(item);
                        }
                    }
                    else if(i>0&& Var_Type.ContainsKey(LexicalLexeme[i - 1].Key))
                    {
                        if((i<LexicalLexeme.Count-1 && LexicalLexeme[i + 1].Key == "(") || (LexicalLexeme[i - 1].Key == "void"))
                        {
                            KeyValuePair<string, string> item = new KeyValuePair<string, string>(LexicalLexeme[i].Key, "function");
                            if (!searchinList(ref Symbol_Table, LexicalLexeme[i].Key))
                            {
                                Symbol_Table.Add(item);
                            }
                        }
                        else
                        {
                            KeyValuePair<string, string> item = new KeyValuePair<string, string>(LexicalLexeme[i].Key, LexicalLexeme[i - 1].Key);
                            if (!searchinList(ref Symbol_Table, LexicalLexeme[i].Key))
                            {
                                Symbol_Table.Add(item);
                            }
                        }
                    }
                    else if(i > 2 && LexicalLexeme[i - 1].Key=="]" && LexicalLexeme[i - 2].Key == "[" && Var_Type.ContainsKey(LexicalLexeme[i - 3].Key))
                    {
                        KeyValuePair<string, string> item = new KeyValuePair<string, string>(LexicalLexeme[i].Key, LexicalLexeme[i - 3].Key);
                        if (!searchinList(ref Symbol_Table, LexicalLexeme[i].Key))
                        {
                            Symbol_Table.Add(item);
                        }
                    }
                }
            }
            // Print Symbol Table
            for(int i = 0; i < Symbol_Table.Count; i++)
            {
                table += Symbol_Table[i].Key + "\t\t\t";
                table += Symbol_Table[i].Value + "\r\n";
            }
            LexicalLexeme.Clear();
            Symbol_Table.Clear();
        }
    }
}




















