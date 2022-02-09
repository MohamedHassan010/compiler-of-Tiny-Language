using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
public enum Token_Class
{
    Begin, Call, Declare, End, Do, Else, EndIf, EndUntil, EndWhile, If, Data_Type_INT, Assign,
    Parameters, Procedure, Program, Read, Real, Set, Then, Until, While, Write,
    Dot, Semicolon, Comma, LParanthesis, RParanthesis, EqualConOp, LessThanOp,
    GreaterThanOp, NotEqualOp, PlusOp, MinusOp, MultiplyOp, DivideOp,
    Identifier, Number, Left_CurlyBracket, Data_Type_Float, String, main, Repeat, Else_If, EndL, Return, AndOperator, OrOperator, Comment, Right_CurlyBracket
}
namespace JASON_Compiler
{


    public class Token
    {
        public string lex;
        public Token_Class token_type;

    }

    public class Scanner
    {
        public List<Token> Tokens = new List<Token>();
        public bool fromOperation = false;
        Dictionary<string, Token_Class> ReservedWords = new Dictionary<string, Token_Class>();
        Dictionary<string, Token_Class> Operators = new Dictionary<string, Token_Class>();

        public Scanner()
        {
            ReservedWords.Add("if", Token_Class.If);
            ReservedWords.Add("begin", Token_Class.Begin);
            ReservedWords.Add("call", Token_Class.Call);
            ReservedWords.Add("declare", Token_Class.Declare);
            ReservedWords.Add("end", Token_Class.End);
            ReservedWords.Add("else", Token_Class.Else);
            ReservedWords.Add("elseif", Token_Class.Else_If);
            ReservedWords.Add("int", Token_Class.Data_Type_INT);
            ReservedWords.Add("parameters", Token_Class.Parameters);
            // ReservedWords.Add("PROCEDURE", Token_Class.Procedure);
            ReservedWords.Add("program", Token_Class.Program);
            ReservedWords.Add("read", Token_Class.Read);
            // ReservedWords.Add("REAL", Token_Class.Real);
            // ReservedWords.Add("SET", Token_Class.Set);
            ReservedWords.Add("then", Token_Class.Then);
            ReservedWords.Add("repeat", Token_Class.Repeat);
            ReservedWords.Add("until", Token_Class.Until);
            // ReservedWords.Add("WHILE", Token_Class.While);
            ReservedWords.Add("write", Token_Class.Write);
            ReservedWords.Add("float", Token_Class.Data_Type_Float);
            ReservedWords.Add("string", Token_Class.String);
            ReservedWords.Add("main", Token_Class.main);
            ReservedWords.Add("return", Token_Class.Return);
            ReservedWords.Add("endl", Token_Class.EndL);

            Operators.Add(".", Token_Class.Dot);
            Operators.Add("{", Token_Class.Left_CurlyBracket);
            Operators.Add(";", Token_Class.Semicolon);
            Operators.Add(",", Token_Class.Comma);
            Operators.Add("(", Token_Class.LParanthesis);
            Operators.Add(")", Token_Class.RParanthesis);
            Operators.Add(":=", Token_Class.Assign);
            Operators.Add("=", Token_Class.EqualConOp);
            Operators.Add("<", Token_Class.LessThanOp);
            Operators.Add(">", Token_Class.GreaterThanOp);
            Operators.Add("<>", Token_Class.NotEqualOp);
            Operators.Add("+", Token_Class.PlusOp);
            Operators.Add("-", Token_Class.MinusOp);
            Operators.Add("*", Token_Class.MultiplyOp);
            Operators.Add("/", Token_Class.DivideOp);
            Operators.Add("&&", Token_Class.AndOperator);
            Operators.Add("||", Token_Class.OrOperator);
            Operators.Add("}", Token_Class.Right_CurlyBracket);
            Operators.Add("–", Token_Class.MinusOp);

        }

        public void StartScanning(string SourceCode)
        {

            for (int i = 0; i < SourceCode.Length; i++)
            {
                // j: Inner loop to check on each character in a single lexeme.
                int j = i;
                char CurrentChar = SourceCode[i];
                string CurrentLexeme = CurrentChar.ToString();

                if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\n')
                    continue;

                if (char.IsLetter(CurrentChar))
                {
                    // The possible Token Classes that begin with a character are
                    // an Idenifier or a Reserved Word.

                    // (1) Update the CurrentChar and validate its value.

                    // (2) Iterate to build the rest of the lexeme while satisfying the
                    // conditions on how the Token Classes should be.
                    // (2.1) Append the CurrentChar to CurrentLexeme.
                    // (2.2) Update the CurrentChar.
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        if (char.IsLetterOrDigit(CurrentChar))
                            CurrentLexeme += CurrentChar.ToString();
                        else
                            break;
                    }

                    // (3) Call FindTokenClass on the CurrentLexeme.
                    FindTokenClass(CurrentLexeme);
                    // (4) Update the outer loop pointer (i) to point on the next lexeme.
                    i = j - 1;
                }
                else if (char.IsDigit(CurrentChar) || CurrentChar == '.')
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        if (char.IsDigit(CurrentChar) || CurrentChar == '.')
                            CurrentLexeme += CurrentChar.ToString();
                        else
                            break;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;

                }
                else if (i + 1 < SourceCode.Length && CurrentChar == '&' && SourceCode[i + 1] == '&')
                {
                    CurrentLexeme += CurrentChar.ToString();
                    i++;
                    FindTokenClass(CurrentLexeme);
                }
                else if (i + 1 < SourceCode.Length && CurrentChar == '|' && SourceCode[i + 1] == '|')
                {
                    CurrentLexeme += CurrentChar.ToString();
                    i++;
                    FindTokenClass(CurrentLexeme);
                }
                //string Error
                else if (CurrentChar == '"')
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {

                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();
                        if (CurrentChar == '"') break;

                    }
                    FindTokenClass(CurrentLexeme);
                    i = j;
                }
                else if (CurrentChar == '/' && i + 1 < SourceCode.Length && SourceCode[i + 1] == '*')
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {
                        CurrentChar = SourceCode[j];
                        CurrentLexeme += CurrentChar.ToString();
                        if (CurrentChar == '*' && j + 1 < SourceCode.Length && SourceCode[j + 1] == '/')
                        {
                            j++;
                            CurrentLexeme += SourceCode[j].ToString();
                            break;

                        }

                    }
                    FindTokenClass(CurrentLexeme);
                    i = j;
                }
                else if (CurrentChar == '*' && i + 1 < SourceCode.Length && SourceCode[i + 1] == '/')
                {


                    CurrentLexeme += SourceCode[i + 1];
                    i++;
                    FindTokenClass(CurrentLexeme);
                }
                else
                {
                    for (j = i + 1; j < SourceCode.Length; j++)
                    {

                        CurrentChar = SourceCode[j];
                        if (char.IsSymbol(CurrentChar))
                        {
                            fromOperation = true;
                            CurrentLexeme += CurrentChar;
                        }
                        else
                            break;
                    }
                    FindTokenClass(CurrentLexeme);
                    i = j - 1;
                }

                JASON_Compiler.TokenStream = Tokens;
            }
        }
        void FindTokenClass(string Lex)
        {
            Token_Class TC;
            Token Tok = new Token();
            Tok.lex = Lex;
            //Is it a reserved word?
            if (isReservedWords(Lex))
            {
                Tok.token_type = ReservedWords[Lex];
                Tokens.Add(Tok);
            }
            //Is it a operator?
            else if (isOperators(Lex))
            {
                Tok.token_type = Operators[Lex];
                Tokens.Add(Tok);
            }

            //Is it a numner?
            else if (isNumber(Lex))
            {
                Tok.token_type = Token_Class.Number;
                Tokens.Add(Tok);
            }
            // Is it a string?
            else if (isString(Lex))
            {
                Tok.token_type = Token_Class.String;
                Tokens.Add(Tok);
            }
            else if (isComment(Lex))
            {
                Tok.token_type = Token_Class.Comment;
                Tokens.Add(Tok);
            }
            //Is it an identifier?
            else if (isIdentifier(Lex))
            {
                Tok.token_type = Token_Class.Identifier;
                Tokens.Add(Tok);
            }
            //Is it an undefined?
            else
            {
                Console.WriteLine(Lex);
                Errors.Error_List.Add(Lex);
            }

        }
        bool isOperators(string Lex)
        {
            if (Operators.ContainsKey(Lex))
                return true;
            return false;
        }

        bool isReservedWords(string Lex)
        {
            if (ReservedWords.ContainsKey(Lex))
                return true;
            return false;
        }
        bool isNumber(string Lex)
        {
            Regex number = new Regex(@"^[+|-]?[0-9]*(\.[0-9]+)?$", RegexOptions.Compiled);
            if (number.IsMatch(Lex))
                return true;
            return false;
        }
        bool isIdentifier(string lex)
        {

            var rx1 = new Regex(@"^[a-zA-Z](\w)*", RegexOptions.Compiled);
            if (rx1.IsMatch(lex))
                return true;
            return false;
        }
        /* bool isConstant(string lex)
         {
             bool isValid = true;
             // Check if the lex is a constant (Number) or not.

             return isValid;
         }*/
        bool isComment(string lex)
        {

            Regex regex = new Regex(@"/\*[\s\W\w]*\*/", RegexOptions.Compiled);
            if (regex.IsMatch(lex))
                return true;
            return false;
        }
        bool isConditionalOperation(string lex)
        {

            Regex regex = new Regex(@"[< |>| =| <>]", RegexOptions.Compiled);
            if (regex.IsMatch(lex))
                return true;
            return false;
        }
        bool isBoolOperation(string lex)
        {

            var rx1 = new Regex(@"(&&)|(\|\|)", RegexOptions.Compiled);
            if (rx1.IsMatch(lex))
                return true;
            return false;

        }
        bool isString(string lex)
        {
            var checker = new Regex(@"[""]+[\w\s\W]*[""]+", RegexOptions.Compiled);
            if (checker.IsMatch(lex))
                return true;
            return false;

        }

    }
}
