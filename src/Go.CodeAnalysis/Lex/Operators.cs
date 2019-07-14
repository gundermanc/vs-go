using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Go.CodeAnalysis.Lex
{
    public static class Operators
    {
        public const string Sum = "+";
        public const string BitwiseAnd = "&";
        public const string AddressOf = "&";
        public const string SumAssign = "+=";
        public const string BitwiseAndAssign = "&=";
        public const string ConditionalAnd = "&&";
        public const string Equal = "==";
        public const string NotEqual = "!=";
        public const string OpeningParenthesis = "(";
        public const string ClosingParenthesis = ")";
        public const string Difference = "-";
        public const string BitwiseOr = "|";
        public const string DifferenceAssign = "-=";
        public const string BitOrAssign = "|=";
        public const string ConditionalOr = "||";
        public const string Less = "<";
        public const string LessOrEqual = "<=";
        public const string OpeningSquareBrace = "[";
        public const string ClosingSquareBrace = "]";
        public const string PointerIndirection = "*";
        public const string Product = "*";
        public const string BitwiseXor = "^";
        public const string ProductAssign = "*=";
        public const string BitwiseXorAssign  = "^=";
        public const string Receive = "<-";
        public const string Greater = ">";
        public const string GreaterOrEqual = ">=";
        public const string OpeningCurlyBrace = "{";
        public const string ClosingCurlyBrace = "}";
        public const string Quotient = "/";
        public const string LeftShift = "<<";
        public const string QuotientAssign = "/=";
        public const string LeftShiteAssign = "<<=";
        public const string Increment = "++";
        public const string Assign = "=";
        public const string DeclarAssign = ":=";
        public const string Comma = ",";
        public const string Semicolon = ";";
        public const string Remainder = "%";
        public const string RightShift = ">>";
        public const string RemainderAssign = "%=";
        public const string RightShiftAssign = ">>=";
        public const string Decrement = "--";
        public const string Not = "!";
        public const string Variadic = "...";
        public const string BitClear = "&^";
        public const string BitClearAssign = "&^=";
    }
}
