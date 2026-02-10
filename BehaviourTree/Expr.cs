// ***********************************************************************************
// * Author : LiNan
// * File : Expr.cs
// * Date : 2023-06-16-16:43
// ************************************************************************************
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace Vena.BehaviourTree
{
    #region expr

    public abstract class Expr
    {
        public abstract Value Evaluate(IBlackboard context);
    }

    public interface IUnaryExpr
    {
    }

    public interface IBinaryExpr
    {
    }

    public interface ITernaryExpr
    {
    }

    public sealed class ConstExpr : Expr
    {
        public readonly Value value;

        public ConstExpr(in Value value)
        {
            this.value = value;
        }

        public override Value Evaluate(IBlackboard context) => value;
    }

    [Serializable]
    public class ConstExprData
    {
        public int value;
    }

    public sealed class VariableExpr : Expr
    {
        public readonly int varId;

        public VariableExpr(in int varId)
        {
            this.varId = varId;
        }

        public override Value Evaluate(IBlackboard context) => context.GetValue(varId);
    }

    [Serializable]
    public class VariableExprData
    {
        public int varId;
    }

    [Serializable]
    public class BinaryExprData
    {
        public int left;
        public int right;
    }

    public sealed class OrExpr : Expr, IBinaryExpr
    {
        public readonly Expr left;
        public readonly Expr right;

        public OrExpr(in Expr left, in Expr right)
        {
            this.left = left;
            this.right = right;
        }

        public override Value Evaluate(IBlackboard context)
        {
            bool bool1 = left.Evaluate(context);
            if (bool1) return true;
            return right.Evaluate(context);
        }
    }

    public sealed class AndExpr : Expr, IBinaryExpr
    {
        public readonly Expr left;
        public readonly Expr right;

        public AndExpr(in Expr left, in Expr right)
        {
            this.left = left;
            this.right = right;
        }

        public override Value Evaluate(IBlackboard context)
        {
            bool bool1 = left.Evaluate(context);
            if (!bool1) return false;
            return right.Evaluate(context);
        }
    }

    public sealed class PlusExpr : Expr, IBinaryExpr
    {
        public readonly Expr left;
        public readonly Expr right;

        public PlusExpr(in Expr left, in Expr right)
        {
            this.left = left;
            this.right = right;
        }

        public override Value Evaluate(IBlackboard context)
        {
            Value value1 = left.Evaluate(context);
            Value value2 = right.Evaluate(context);
            return value1 + value2;
        }
    }

    public sealed class MinusExpr : Expr, IBinaryExpr
    {
        public readonly Expr left;
        public readonly Expr right;

        public MinusExpr(in Expr left, in Expr right)
        {
            this.left = left;
            this.right = right;
        }

        public override Value Evaluate(IBlackboard context)
        {
            Value value1 = left.Evaluate(context);
            Value value2 = right.Evaluate(context);
            return value1 - value2;
        }
    }

    public sealed class MulExpr : Expr, IBinaryExpr
    {
        public readonly Expr left;
        public readonly Expr right;

        public MulExpr(in Expr left, in Expr right)
        {
            this.left = left;
            this.right = right;
        }

        public override Value Evaluate(IBlackboard context)
        {
            Value value1 = left.Evaluate(context);
            Value value2 = right.Evaluate(context);
            return value1 * value2;
        }
    }

    public sealed class DivExpr : Expr, IBinaryExpr
    {
        public readonly Expr left;
        public readonly Expr right;

        public DivExpr(in Expr left, in Expr right)
        {
            this.left = left;
            this.right = right;
        }

        public override Value Evaluate(IBlackboard context)
        {
            Value value1 = left.Evaluate(context);
            Value value2 = right.Evaluate(context);
            return value1 / value2;
        }
    }

    public sealed class PowExpr : Expr, IBinaryExpr
    {
        public readonly Expr left;
        public readonly Expr right;

        public PowExpr(in Expr left, in Expr right)
        {
            this.left = left;
            this.right = right;
        }

        public override Value Evaluate(IBlackboard context)
        {
            return left.Evaluate(context).Pow(right.Evaluate(context));
        }
    }

    public sealed class EqualExpr : Expr
    {
        public readonly Expr left;
        public readonly Expr right;

        public EqualExpr(in Expr left, in Expr right)
        {
            this.left = left;
            this.right = right;
        }

        public override Value Evaluate(IBlackboard context)
        {
            Value value1 = left.Evaluate(context);
            Value value2 = right.Evaluate(context);
            return value1 == value2;
        }
    }

    public sealed class NotEqualExpr : Expr
    {
        public readonly Expr left;
        public readonly Expr right;

        public NotEqualExpr(in Expr left, in Expr right)
        {
            this.left = left;
            this.right = right;
        }

        public override Value Evaluate(IBlackboard context)
        {
            Value value1 = left.Evaluate(context);
            Value value2 = right.Evaluate(context);
            return value1 != value2;
        }
    }

    public sealed class LessExpr : Expr
    {
        public readonly Expr left;
        public readonly Expr right;

        public LessExpr(in Expr left, in Expr right)
        {
            this.left = left;
            this.right = right;
        }

        public override Value Evaluate(IBlackboard context)
        {
            Value value1 = left.Evaluate(context);
            Value value2 = right.Evaluate(context);
            return value1 < value2;
        }
    }

    public sealed class LessEqExpr : Expr
    {
        public readonly Expr left;
        public readonly Expr right;

        public LessEqExpr(in Expr left, in Expr right)
        {
            this.left = left;
            this.right = right;
        }

        public override Value Evaluate(IBlackboard context)
        {
            Value value1 = left.Evaluate(context);
            Value value2 = right.Evaluate(context);
            return value1 <= value2;
        }
    }

    public sealed class GreatExpr : Expr
    {
        public readonly Expr left;
        public readonly Expr right;

        public GreatExpr(in Expr left, in Expr right)
        {
            this.left = left;
            this.right = right;
        }

        public override Value Evaluate(IBlackboard context)
        {
            Value value1 = left.Evaluate(context);
            Value value2 = right.Evaluate(context);
            return value1 > value2;
        }
    }

    public sealed class GreatEqExpr : Expr
    {
        public readonly Expr left;
        public readonly Expr right;

        public GreatEqExpr(in Expr left, in Expr right)
        {
            this.left = left;
            this.right = right;
        }

        public override Value Evaluate(IBlackboard context)
        {
            Value value1 = left.Evaluate(context);
            Value value2 = right.Evaluate(context);
            return value1 >= value2;
        }
    }

    public sealed class BranchExpr : Expr, ITernaryExpr
    {
        public readonly Expr condition;
        public readonly Expr trueExpr;
        public readonly Expr falseExpr;

        public BranchExpr(Expr condition, in Expr trueExpr, in Expr falseExpr)
        {
            this.condition = condition;
            this.trueExpr = trueExpr;
            this.falseExpr = falseExpr;
        }

        public override Value Evaluate(IBlackboard context)
        {
            if (condition.Evaluate(context))
                return trueExpr.Evaluate(context);
            return falseExpr.Evaluate(context);
        }
    }

    #endregion


    #region Sign And Parser

    public enum Op
    {
        // 单目
        Cst,
        Var,

        // 双目
        And, // "&$"      true&&true = true
        Or, // "||"      true||false = true

        Plus, // +        A+B = B+A
        Minus, // -        A-B = -(B-A)
        Mul, // *        A*B = B*A
        Div, // /        A/B = 1/(B/A)
        Pow, // ^        A^5 = A*A*A*A*A 

        Equal, // =        A=B = !(A!=B)
        NotEqual, // !=       A~B = !(A=B)
        Less, // <        A<B = !(A>=B)
        LessEq, // <=       A<=B = !(A>B)
        Great, // >        A>B = !(A<=B)
        GreatEq, // >=       A>=B = !(A<B)

        // 三目
    }

    [StructLayout(LayoutKind.Explicit, Pack = 2)]
    public readonly struct OP : IEquatable<OP>
    {
        [FieldOffset(0)] private readonly char _c1;
        [FieldOffset(1)] private readonly char _c2;
        [FieldOffset(0)] private readonly ushort _ushort;

        private OP(char c1)
        {
            _ushort = 0;
            _c1 = c1;
            _c2 = '\0';
        }

        private OP(ReadOnlySpan<char> span)
        {
            _ushort = 0;
            _c1 = span[0];
            _c2 = '\0';
            if (span.Length > 1)
                _c2 = span[1];
        }

        public static implicit operator OP(string s)
        {
            ReadOnlySpan<char> span = s.AsSpan();
            return new OP(span);
        }

        public static implicit operator OP(char c)
        {
            return new OP(c);
        }

        public static implicit operator ushort(OP op)
        {
            return op._ushort;
        }

        public static bool operator ==(OP a, OP b)
        {
            return a._ushort == b._ushort;
        }

        public static bool operator !=(OP a, OP b)
        {
            return a._ushort != b._ushort;
        }

        public bool Equals(OP other)
        {
            return _ushort == other._ushort;
        }

        public override bool Equals(object obj)
        {
            return obj is OP other && Equals(other);
        }

        public override int GetHashCode() => _ushort;
    }

    public static class ExprHelper
    {
        public static readonly OP notEqual = "!=", lessEq = "<=", greatEq = ">=";
        public static readonly OP plus = '+', minus = '-', mul = '*', div = '/', pow = '^';
        public static readonly OP equal = '=', less = '<', great = '>';
        public static readonly OP and = "&&", or = "||";

        public static readonly OP leftBracket = '(', rightBracket = ')';
        public static readonly OP leftSquareBracket = '[', rightSquareBracket = ']';

        private static readonly OP[] OpChars =
            { plus, minus, mul, div, pow, equal, notEqual, less, great, and, or, lessEq, greatEq };

        private static readonly OP[] BracketChars =
            { leftBracket, rightBracket, leftSquareBracket, rightSquareBracket };

        private static readonly Dictionary<ushort, Op> char2Op = new()
        {
            [plus] = Op.Plus, [minus] = Op.Minus, [mul] = Op.Mul, [div] = Op.Div, [pow] = Op.Pow,
            [equal] = Op.Equal, [notEqual] = Op.NotEqual, [less] = Op.Less, [great] = Op.Great, [lessEq] = Op.LessEq,
            [greatEq] = Op.GreatEq,
            [and] = Op.And, [or] = Op.Or,
        };

        public static Expr Parse(string expr)
        {
            return Parse(expr, 0, expr.Length);
        }

        private static Expr Parse(string expr, int start, int length)
        {
            ReadOnlySpan<char> span = expr.AsSpan(start, length);


            // Remove brackets
            return null;
        }
    }

    #endregion
}