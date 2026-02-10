using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Vena
{
    public interface IBlackboard
    {
        ref readonly TValue Get<TValue>(in Variable variable) where TValue : struct, IValue<TValue>;

        void Set<TValue>(in Variable variable, in TValue value) where TValue : struct, IValue<TValue>;

        void Modify(in Variable variable, IExpression expression);
    }

    public interface IExpression
    {
        ExprType exprType { get; }

        void Serialize(ref ISerializer writer);

        void Deserialize(ref ISerializer reader);
    }

    public interface IExpression<out T> : IExpression, IDisposable
    {
        T Evaluate(ref IBlackboard blackboard);
    }

    public static partial class Expr
    {
        public sealed class Variant<T> : IExpression<T> where T : struct, IValue<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Variant, Value<T>.TypeId);

            public Variable variable;

            public ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public T Evaluate(ref IBlackboard blackboard)
            {
                ref readonly var value = ref blackboard.Get<T>(variable);
                return value;
            }

            public void Serialize(ref ISerializer writer)
            {
                writer.WriteInt32(variable.value);
            }

            public void Deserialize(ref ISerializer reader)
            {
                variable = new Variable(reader.ReadInt32());
            }

            public void Dispose()
            {
                Delete(this);
            }
        }

        public sealed class Constant<T> : IExpression<T> where T : struct, IValue<T>
        {
            public static readonly Constant<T> Default = new Constant<T>();

            public static readonly ExprType Type = new ExprType(OperaterType.Constant, Value<T>.TypeId);

            public T Value;

            public ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public T Evaluate(ref IBlackboard blackboard)
            {
                return Value;
            }

            public void Serialize(ref ISerializer writer)
            {
                Value.Serialize(ref writer);
            }

            public void Deserialize(ref ISerializer reader)
            {
                Value.Deserialize(ref reader);
            }

            public void Dispose()
            {
                Value = default;

                Delete(this);
            }
        }

        #region Logical Operators

        /// <summary>
        /// 1元逻辑运算符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class UnaryLogical<T> : IExpression<Bool> where T : struct, IValue<T>
        {
            public IExpression<T> expr;

            protected IExpression<T> EnsureExpr
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => expr ?? Constant<T>.Default;
            }

            public abstract ExprType exprType { get; }

            public abstract Bool Evaluate(ref IBlackboard blackboard);

            public abstract void Serialize(ref ISerializer writer);

            public abstract void Deserialize(ref ISerializer reader);

            public void Dispose()
            {
                if (null != expr)
                {
                    using var expression = expr;
                    expr = null;
                }

                Delete(this);
            }
        }

        /// <summary>
        /// 2元逻辑运算符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class BinaryLogical<T> : IExpression<Bool> where T : struct, IValue<T>
        {
            public IExpression<T> lhs, rhs;

            protected IExpression<T> EnsureLhs
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => lhs ?? Constant<T>.Default;
            }

            protected IExpression<T> EnsureRhs
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => rhs ?? Constant<T>.Default;
            }

            public abstract ExprType exprType { get; }

            public abstract Bool Evaluate(ref IBlackboard blackboard);

            public abstract void Serialize(ref ISerializer writer);

            public abstract void Deserialize(ref ISerializer reader);

            public void Dispose()
            {
                if (null != lhs)
                {
                    using var expression = lhs;
                    lhs = null;
                }

                if (null != rhs)
                {
                    using var expression = rhs;
                    rhs = null;
                }

                Delete(this);
            }
        }

        public sealed class Yes<T> : UnaryLogical<T> where T : struct, IValue<T>, IYes<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Yes, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override Bool Evaluate(ref IBlackboard blackboard)
            {
                return EnsureExpr.Evaluate(ref blackboard).Yes();
            }

            public override void Serialize(ref ISerializer writer)
            {
                short type = EnsureExpr.exprType;

                writer.WriteShort(type);

                EnsureExpr.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType type = reader.ReadShort();

                if (expr == null || expr.exprType != type)
                {
                    using var expression = expr;

                    expr = (IExpression<T>)New(type);
                }

                expr.Deserialize(ref reader);
            }
        }

        public sealed class Not<T> : UnaryLogical<T> where T : struct, IValue<T>, INot<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Not, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public override Bool Evaluate(ref IBlackboard blackboard)
            {
                return EnsureExpr.Evaluate(ref blackboard).Not();
            }

            public override void Serialize(ref ISerializer writer)
            {
                short type = EnsureExpr.exprType;

                writer.WriteShort(type);

                EnsureExpr.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType type = reader.ReadShort();

                if (expr == null || expr.exprType != type)
                {
                    using var expression = expr;

                    expr = (IExpression<T>)New(type);
                }

                expr.Deserialize(ref reader);
            }
        }

        public sealed class And<T> : BinaryLogical<T> where T : struct, IValue<T>, IAnd<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.And, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override Bool Evaluate(ref IBlackboard blackboard)
            {
                return EnsureLhs.Evaluate(ref blackboard).And(EnsureRhs.Evaluate(ref blackboard));
            }

            public override void Serialize(ref ISerializer writer)
            {
                short lhsType = EnsureLhs.exprType;

                writer.WriteShort(lhsType);

                EnsureLhs.Serialize(ref writer);

                short rhsType = EnsureRhs.exprType;

                writer.WriteShort(rhsType);

                EnsureRhs.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType lhsType = reader.ReadShort();

                if (lhs == null || lhs.exprType != lhsType)
                {
                    using var expression = lhs;

                    lhs = (IExpression<T>)New(lhsType);
                }

                lhs.Deserialize(ref reader);

                ExprType rhsType = reader.ReadShort();

                if (rhs == null || rhs.exprType != rhsType)
                {
                    using var expression = rhs;

                    rhs = (IExpression<T>)New(rhsType);
                }

                rhs.Deserialize(ref reader);
            }
        }

        public sealed class Or<T> : BinaryLogical<T> where T : struct, IValue<T>, IOr<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Or, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override Bool Evaluate(ref IBlackboard blackboard)
            {
                return EnsureLhs.Evaluate(ref blackboard).Or(EnsureRhs.Evaluate(ref blackboard));
            }

            public override void Serialize(ref ISerializer writer)
            {
                short lhsType = EnsureLhs.exprType;

                writer.WriteShort(lhsType);

                EnsureLhs.Serialize(ref writer);

                short rhsType = EnsureRhs.exprType;

                writer.WriteShort(rhsType);

                EnsureRhs.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType lhsType = reader.ReadShort();

                if (lhs == null || lhs.exprType != lhsType)
                {
                    using var expression = lhs;

                    lhs = (IExpression<T>)New(lhsType);
                }

                lhs.Deserialize(ref reader);

                ExprType rhsType = reader.ReadShort();

                if (rhs == null || rhs.exprType != rhsType)
                {
                    using var expression = rhs;

                    rhs = (IExpression<T>)New(rhsType);
                }

                rhs.Deserialize(ref reader);
            }
        }

        public sealed class Equal<T> : BinaryLogical<T> where T : struct, IValue<T>, IEqual<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Equal, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override Bool Evaluate(ref IBlackboard blackboard)
            {
                return EnsureLhs.Evaluate(ref blackboard).Equal(EnsureRhs.Evaluate(ref blackboard));
            }

            public override void Serialize(ref ISerializer writer)
            {
                short lhsType = EnsureLhs.exprType;

                writer.WriteShort(lhsType);

                EnsureLhs.Serialize(ref writer);

                short rhsType = EnsureRhs.exprType;

                writer.WriteShort(rhsType);

                EnsureRhs.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType lhsType = reader.ReadShort();

                if (lhs == null || lhs.exprType != lhsType)
                {
                    using var expression = lhs;

                    lhs = (IExpression<T>)New(lhsType);
                }

                lhs.Deserialize(ref reader);

                ExprType rhsType = reader.ReadShort();

                if (rhs == null || rhs.exprType != rhsType)
                {
                    using var expression = rhs;

                    rhs = (IExpression<T>)New(rhsType);
                }

                rhs.Deserialize(ref reader);
            }
        }

        public sealed class NotEqual<T> : BinaryLogical<T> where T : struct, IValue<T>, INotEqual<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.NotEqual, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override Bool Evaluate(ref IBlackboard blackboard)
            {
                return EnsureLhs.Evaluate(ref blackboard).NotEqual(EnsureRhs.Evaluate(ref blackboard));
            }

            public override void Serialize(ref ISerializer writer)
            {
                short lhsType = EnsureLhs.exprType;

                writer.WriteShort(lhsType);

                EnsureLhs.Serialize(ref writer);

                short rhsType = EnsureRhs.exprType;

                writer.WriteShort(rhsType);

                EnsureRhs.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType lhsType = reader.ReadShort();

                if (lhs == null || lhs.exprType != lhsType)
                {
                    using var expression = lhs;

                    lhs = (IExpression<T>)New(lhsType);
                }

                lhs.Deserialize(ref reader);

                ExprType rhsType = reader.ReadShort();

                if (rhs == null || rhs.exprType != rhsType)
                {
                    using var expression = rhs;

                    rhs = (IExpression<T>)New(rhsType);
                }

                rhs.Deserialize(ref reader);
            }
        }

        public sealed class Greater<T> : BinaryLogical<T> where T : struct, IValue<T>, IGreater<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Greater, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override Bool Evaluate(ref IBlackboard blackboard)
            {
                return EnsureLhs.Evaluate(ref blackboard).Greater(EnsureRhs.Evaluate(ref blackboard));
            }

            public override void Serialize(ref ISerializer writer)
            {
                short lhsType = EnsureLhs.exprType;

                writer.WriteShort(lhsType);

                EnsureLhs.Serialize(ref writer);

                short rhsType = EnsureRhs.exprType;

                writer.WriteShort(rhsType);

                EnsureRhs.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType lhsType = reader.ReadShort();

                if (lhs == null || lhs.exprType != lhsType)
                {
                    using var expression = lhs;

                    lhs = (IExpression<T>)New(lhsType);
                }

                lhs.Deserialize(ref reader);

                ExprType rhsType = reader.ReadShort();

                if (rhs == null || rhs.exprType != rhsType)
                {
                    using var expression = rhs;

                    rhs = (IExpression<T>)New(rhsType);
                }

                rhs.Deserialize(ref reader);
            }
        }

        public sealed class GreaterEqual<T> : BinaryLogical<T> where T : struct, IValue<T>, IGreaterEqual<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.GreaterEqual, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override Bool Evaluate(ref IBlackboard blackboard)
            {
                return EnsureLhs.Evaluate(ref blackboard).GreaterEqual(EnsureRhs.Evaluate(ref blackboard));
            }

            public override void Serialize(ref ISerializer writer)
            {
                short lhsType = EnsureLhs.exprType;

                writer.WriteShort(lhsType);

                EnsureLhs.Serialize(ref writer);

                short rhsType = EnsureRhs.exprType;

                writer.WriteShort(rhsType);

                EnsureRhs.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType lhsType = reader.ReadShort();

                if (lhs == null || lhs.exprType != lhsType)
                {
                    using var expression = lhs;

                    lhs = (IExpression<T>)New(lhsType);
                }

                lhs.Deserialize(ref reader);

                ExprType rhsType = reader.ReadShort();

                if (rhs == null || rhs.exprType != rhsType)
                {
                    using var expression = rhs;

                    rhs = (IExpression<T>)New(rhsType);
                }

                rhs.Deserialize(ref reader);
            }
        }

        public sealed class Less<T> : BinaryLogical<T> where T : struct, IValue<T>, ILess<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Less, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override Bool Evaluate(ref IBlackboard blackboard)
            {
                return EnsureLhs.Evaluate(ref blackboard).Less(EnsureRhs.Evaluate(ref blackboard));
            }

            public override void Serialize(ref ISerializer writer)
            {
                short lhsType = EnsureLhs.exprType;

                writer.WriteShort(lhsType);

                EnsureLhs.Serialize(ref writer);

                short rhsType = EnsureRhs.exprType;

                writer.WriteShort(rhsType);

                EnsureRhs.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType lhsType = reader.ReadShort();

                if (lhs == null || lhs.exprType != lhsType)
                {
                    using var expression = lhs;

                    lhs = (IExpression<T>)New(lhsType);
                }

                lhs.Deserialize(ref reader);

                ExprType rhsType = reader.ReadShort();

                if (rhs == null || rhs.exprType != rhsType)
                {
                    using var expression = rhs;

                    rhs = (IExpression<T>)New(rhsType);
                }

                rhs.Deserialize(ref reader);
            }
        }

        public sealed class LessEqual<T> : BinaryLogical<T> where T : struct, IValue<T>, ILessEqual<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.LessEqual, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override Bool Evaluate(ref IBlackboard blackboard)
            {
                return EnsureLhs.Evaluate(ref blackboard).LessEqual(EnsureRhs.Evaluate(ref blackboard));
            }

            public override void Serialize(ref ISerializer writer)
            {
                short lhsType = EnsureLhs.exprType;

                writer.WriteShort(lhsType);

                EnsureLhs.Serialize(ref writer);

                short rhsType = EnsureRhs.exprType;

                writer.WriteShort(rhsType);

                EnsureRhs.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType lhsType = reader.ReadShort();

                if (lhs == null || lhs.exprType != lhsType)
                {
                    using var expression = lhs;

                    lhs = (IExpression<T>)New(lhsType);
                }

                lhs.Deserialize(ref reader);

                ExprType rhsType = reader.ReadShort();

                if (rhs == null || rhs.exprType != rhsType)
                {
                    using var expression = rhs;

                    rhs = (IExpression<T>)New(rhsType);
                }

                rhs.Deserialize(ref reader);
            }
        }

        #endregion

        #region Arithmetic Operators

        /// <summary>
        /// 1元算数运算符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class UnaryArithmetic<T> : IExpression<T> where T : struct, IValue<T>
        {
            public IExpression<T> expr;

            protected IExpression<T> EnsureExpr
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => expr ?? Constant<T>.Default;
            }

            public abstract ExprType exprType { get; }

            public abstract T Evaluate(ref IBlackboard blackboard);

            public abstract void Serialize(ref ISerializer writer);

            public abstract void Deserialize(ref ISerializer reader);

            public void Dispose()
            {
                if (null != expr)
                {
                    using var expression = expr;
                    expr = null;
                }

                Delete(this);
            }
        }

        /// <summary>
        /// 2元算数运算符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class BinaryArithmetic<T> : IExpression<T> where T : struct, IValue<T>
        {
            public IExpression<T> lhs, rhs;

            protected IExpression<T> EnsureLhs
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => lhs ?? Constant<T>.Default;
            }

            protected IExpression<T> EnsureRhs
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => rhs ?? Constant<T>.Default;
            }

            public abstract ExprType exprType { get; }

            public abstract T Evaluate(ref IBlackboard blackboard);

            public abstract void Serialize(ref ISerializer writer);

            public abstract void Deserialize(ref ISerializer reader);

            public void Dispose()
            {
                if (null != lhs)
                {
                    using var expression = lhs;
                    lhs = null;
                }

                if (null != rhs)
                {
                    using var expression = rhs;
                    rhs = null;
                }

                Delete(this);
            }
        }

        /// <summary>
        /// 3元算数运算符
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public abstract class TrinaryArithmetic<T> : IExpression<T> where T : struct, IValue<T>
        {
            public IExpression<T> a, b, c;

            protected IExpression<T> EnsureA
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => a ?? Constant<T>.Default;
            }

            protected IExpression<T> EnsureB
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => b ?? Constant<T>.Default;
            }

            protected IExpression<T> EnsureC
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => c ?? Constant<T>.Default;
            }

            public abstract ExprType exprType { get; }

            public abstract T Evaluate(ref IBlackboard blackboard);

            public abstract void Serialize(ref ISerializer writer);

            public abstract void Deserialize(ref ISerializer reader);

            public void Dispose()
            {
                if (null != a)
                {
                    using var expression = a;
                    a = null;
                }

                if (null != b)
                {
                    using var expression = b;
                    b = null;
                }

                if (null != c)
                {
                    using var expression = c;
                    c = null;
                }

                Delete(this);
            }
        }

        public sealed class Abs<T> : UnaryArithmetic<T> where T : struct, IValue<T>, IAbs<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Abs, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override T Evaluate(ref IBlackboard blackboard)
            {
                return EnsureExpr.Evaluate(ref blackboard).Abs();
            }

            public override void Serialize(ref ISerializer writer)
            {
                short type = EnsureExpr.exprType;

                writer.WriteShort(type);

                EnsureExpr.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType type = reader.ReadShort();

                if (expr == null || expr.exprType != type)
                {
                    using var expression = expr;

                    expr = (IExpression<T>)New(type);
                }

                expr.Deserialize(ref reader);
            }
        }

        public sealed class Negate<T> : UnaryArithmetic<T> where T : struct, IValue<T>, INegate<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Negate, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override T Evaluate(ref IBlackboard blackboard)
            {
                return EnsureExpr.Evaluate(ref blackboard).Negate();
            }

            public override void Serialize(ref ISerializer writer)
            {
                short type = EnsureExpr.exprType;

                writer.WriteShort(type);

                EnsureExpr.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType type = reader.ReadShort();

                if (expr == null || expr.exprType != type)
                {
                    using var expression = expr;

                    expr = (IExpression<T>)New(type);
                }

                expr.Deserialize(ref reader);
            }
        }

        public sealed class Add<T> : BinaryArithmetic<T> where T : struct, IValue<T>, IAdd<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Add, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override T Evaluate(ref IBlackboard blackboard)
            {
                return EnsureLhs.Evaluate(ref blackboard).Add(EnsureRhs.Evaluate(ref blackboard));
            }

            public override void Serialize(ref ISerializer writer)
            {
                short lhsType = EnsureLhs.exprType;

                writer.WriteShort(lhsType);

                EnsureLhs.Serialize(ref writer);

                short rhsType = EnsureRhs.exprType;

                writer.WriteShort(rhsType);

                EnsureRhs.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType lhsType = reader.ReadShort();

                if (lhs == null || lhs.exprType != lhsType)
                {
                    using var expression = lhs;

                    lhs = (IExpression<T>)New(lhsType);
                }

                lhs.Deserialize(ref reader);

                ExprType rhsType = reader.ReadShort();

                if (rhs == null || rhs.exprType != rhsType)
                {
                    using var expression = rhs;

                    rhs = (IExpression<T>)New(rhsType);
                }

                rhs.Deserialize(ref reader);
            }
        }

        public sealed class Subtract<T> : BinaryArithmetic<T> where T : struct, IValue<T>, ISubtract<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Subtract, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override T Evaluate(ref IBlackboard blackboard)
            {
                return EnsureLhs.Evaluate(ref blackboard).Subtract(EnsureRhs.Evaluate(ref blackboard));
            }

            public override void Serialize(ref ISerializer writer)
            {
                short lhsType = EnsureLhs.exprType;

                writer.WriteShort(lhsType);

                EnsureLhs.Serialize(ref writer);

                short rhsType = EnsureRhs.exprType;

                writer.WriteShort(rhsType);

                EnsureRhs.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType lhsType = reader.ReadShort();

                if (lhs == null || lhs.exprType != lhsType)
                {
                    using var expression = lhs;

                    lhs = (IExpression<T>)New(lhsType);
                }

                lhs.Deserialize(ref reader);

                ExprType rhsType = reader.ReadShort();

                if (rhs == null || rhs.exprType != rhsType)
                {
                    using var expression = rhs;

                    rhs = (IExpression<T>)New(rhsType);
                }

                rhs.Deserialize(ref reader);
            }
        }

        public sealed class Multiply<T> : BinaryArithmetic<T> where T : struct, IValue<T>, IMultiply<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Multiply, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override T Evaluate(ref IBlackboard blackboard)
            {
                return EnsureLhs.Evaluate(ref blackboard).Multiply(EnsureRhs.Evaluate(ref blackboard));
            }

            public override void Serialize(ref ISerializer writer)
            {
                short lhsType = EnsureLhs.exprType;

                writer.WriteShort(lhsType);

                EnsureLhs.Serialize(ref writer);

                short rhsType = EnsureRhs.exprType;

                writer.WriteShort(rhsType);

                EnsureRhs.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType lhsType = reader.ReadShort();

                if (lhs == null || lhs.exprType != lhsType)
                {
                    using var expression = lhs;

                    lhs = (IExpression<T>)New(lhsType);
                }

                lhs.Deserialize(ref reader);

                ExprType rhsType = reader.ReadShort();

                if (rhs == null || rhs.exprType != rhsType)
                {
                    using var expression = rhs;

                    rhs = (IExpression<T>)New(rhsType);
                }

                rhs.Deserialize(ref reader);
            }
        }

        public sealed class Divide<T> : BinaryArithmetic<T> where T : struct, IValue<T>, IDivide<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Divide, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override T Evaluate(ref IBlackboard blackboard)
            {
                return EnsureLhs.Evaluate(ref blackboard).Divide(EnsureRhs.Evaluate(ref blackboard));
            }

            public override void Serialize(ref ISerializer writer)
            {
                short lhsType = EnsureLhs.exprType;

                writer.WriteShort(lhsType);

                EnsureLhs.Serialize(ref writer);

                short rhsType = EnsureRhs.exprType;

                writer.WriteShort(rhsType);

                EnsureRhs.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType lhsType = reader.ReadShort();

                if (lhs == null || lhs.exprType != lhsType)
                {
                    using var expression = lhs;

                    lhs = (IExpression<T>)New(lhsType);
                }

                lhs.Deserialize(ref reader);

                ExprType rhsType = reader.ReadShort();

                if (rhs == null || rhs.exprType != rhsType)
                {
                    using var expression = rhs;

                    rhs = (IExpression<T>)New(rhsType);
                }

                rhs.Deserialize(ref reader);
            }
        }

        public sealed class Mod<T> : BinaryArithmetic<T> where T : struct, IValue<T>, IMod<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Mod, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override T Evaluate(ref IBlackboard blackboard)
            {
                return EnsureLhs.Evaluate(ref blackboard).Mod(EnsureRhs.Evaluate(ref blackboard));
            }

            public override void Serialize(ref ISerializer writer)
            {
                short lhsType = EnsureLhs.exprType;

                writer.WriteShort(lhsType);

                EnsureLhs.Serialize(ref writer);

                short rhsType = EnsureRhs.exprType;

                writer.WriteShort(rhsType);

                EnsureRhs.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType lhsType = reader.ReadShort();

                if (lhs == null || lhs.exprType != lhsType)
                {
                    using var expression = lhs;

                    lhs = (IExpression<T>)New(lhsType);
                }

                lhs.Deserialize(ref reader);

                ExprType rhsType = reader.ReadShort();

                if (rhs == null || rhs.exprType != rhsType)
                {
                    using var expression = rhs;

                    rhs = (IExpression<T>)New(rhsType);
                }

                rhs.Deserialize(ref reader);
            }
        }

        public sealed class Min<T> : BinaryArithmetic<T> where T : struct, IValue<T>, IMin<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Min, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override T Evaluate(ref IBlackboard blackboard)
            {
                return EnsureLhs.Evaluate(ref blackboard).Min(EnsureRhs.Evaluate(ref blackboard));
            }

            public override void Serialize(ref ISerializer writer)
            {
                short lhsType = EnsureLhs.exprType;

                writer.WriteShort(lhsType);

                EnsureLhs.Serialize(ref writer);

                short rhsType = EnsureRhs.exprType;

                writer.WriteShort(rhsType);

                EnsureRhs.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType lhsType = reader.ReadShort();

                if (lhs == null || lhs.exprType != lhsType)
                {
                    using var expression = lhs;

                    lhs = (IExpression<T>)New(lhsType);
                }

                lhs.Deserialize(ref reader);

                ExprType rhsType = reader.ReadShort();

                if (rhs == null || rhs.exprType != rhsType)
                {
                    using var expression = rhs;

                    rhs = (IExpression<T>)New(rhsType);
                }

                rhs.Deserialize(ref reader);
            }
        }

        public sealed class Max<T> : BinaryArithmetic<T> where T : struct, IValue<T>, IMax<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Max, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override T Evaluate(ref IBlackboard blackboard)
            {
                return EnsureLhs.Evaluate(ref blackboard).Max(EnsureRhs.Evaluate(ref blackboard));
            }

            public override void Serialize(ref ISerializer writer)
            {
                short lhsType = EnsureLhs.exprType;

                writer.WriteShort(lhsType);

                EnsureLhs.Serialize(ref writer);

                short rhsType = EnsureRhs.exprType;

                writer.WriteShort(rhsType);

                EnsureRhs.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType lhsType = reader.ReadShort();

                if (lhs == null || lhs.exprType != lhsType)
                {
                    using var expression = lhs;

                    lhs = (IExpression<T>)New(lhsType);
                }

                lhs.Deserialize(ref reader);

                ExprType rhsType = reader.ReadShort();

                if (rhs == null || rhs.exprType != rhsType)
                {
                    using var expression = rhs;

                    rhs = (IExpression<T>)New(rhsType);
                }

                rhs.Deserialize(ref reader);
            }
        }

        public sealed class Mid<T> : BinaryArithmetic<T> where T : struct, IValue<T>, IMid<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Mid, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override T Evaluate(ref IBlackboard blackboard)
            {
                return EnsureLhs.Evaluate(ref blackboard).Mid(EnsureRhs.Evaluate(ref blackboard));
            }

            public override void Serialize(ref ISerializer writer)
            {
                short lhsType = EnsureLhs.exprType;

                writer.WriteShort(lhsType);

                EnsureLhs.Serialize(ref writer);

                short rhsType = EnsureRhs.exprType;

                writer.WriteShort(rhsType);

                EnsureRhs.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType lhsType = reader.ReadShort();

                if (lhs == null || lhs.exprType != lhsType)
                {
                    using var expression = lhs;

                    lhs = (IExpression<T>)New(lhsType);
                }

                lhs.Deserialize(ref reader);

                ExprType rhsType = reader.ReadShort();

                if (rhs == null || rhs.exprType != rhsType)
                {
                    using var expression = rhs;

                    rhs = (IExpression<T>)New(rhsType);
                }

                rhs.Deserialize(ref reader);
            }
        }

        public sealed class Clamp<T> : TrinaryArithmetic<T> where T : struct, IValue<T>, IClamp<T>
        {
            public static readonly ExprType Type = new ExprType(OperaterType.Clamp, Value<T>.TypeId);

            public override ExprType exprType
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => Type;
            }

            public override T Evaluate(ref IBlackboard blackboard)
            {
                return EnsureA.Evaluate(ref blackboard)
                    .Clamp(EnsureB.Evaluate(ref blackboard), EnsureC.Evaluate(ref blackboard));
            }

            public override void Serialize(ref ISerializer writer)
            {
                short aType = EnsureA.exprType;

                writer.WriteShort(aType);

                EnsureA.Serialize(ref writer);

                short bType = EnsureB.exprType;

                writer.WriteShort(bType);

                EnsureB.Serialize(ref writer);

                short cType = EnsureC.exprType;

                writer.WriteShort(cType);

                EnsureC.Serialize(ref writer);
            }

            public override void Deserialize(ref ISerializer reader)
            {
                ExprType aType = reader.ReadShort();

                if (a == null || a.exprType != aType)
                {
                    using var expression = a;

                    a = (IExpression<T>)New(aType);
                }

                a.Deserialize(ref reader);

                ExprType bType = reader.ReadShort();

                if (b == null || b.exprType != bType)
                {
                    using var expression = b;

                    b = (IExpression<T>)New(bType);
                }

                b.Deserialize(ref reader);

                ExprType cType = reader.ReadShort();

                if (c == null || c.exprType != cType)
                {
                    using var expression = c;

                    c = (IExpression<T>)New(cType);
                }

                c.Deserialize(ref reader);
            }
        }

        #endregion
    }

    [StructLayout(LayoutKind.Explicit, Pack = 2)]
    public readonly struct ExprType : IEquatable<ExprType>
    {
        [FieldOffset(0)] public readonly OperaterType OperaterType;

        [FieldOffset(1)] public readonly ValueType ValueType;

        [FieldOffset(0)] private readonly short _value;

        private ExprType(short value)
        {
            OperaterType = 0;
            ValueType = 0;
            _value = value;
        }

        public override string ToString()
        {
            return $"ExprType({OperaterType}, {ValueType})";
        }

        public ExprType(OperaterType operaterType, ValueType valueType)
        {
            _value = 0;
            OperaterType = operaterType;
            ValueType = valueType;
        }

        public bool Equals(ExprType other)
        {
            return _value == other._value;
        }

        public override bool Equals(object obj)
        {
            return obj is ExprType other && Equals(other);
        }

        public override int GetHashCode()
        {
            return _value.GetHashCode();
        }

        public static implicit operator short(ExprType exprType)
        {
            return exprType._value;
        }

        public static implicit operator ExprType(short value)
        {
            return new ExprType(value);
        }

        public static bool operator ==(ExprType left, ExprType right)
        {
            return left._value == right._value;
        }

        public static bool operator !=(ExprType left, ExprType right)
        {
            return left._value != right._value;
        }
    }

    [System.Flags]
    public enum OperaterType : byte
    {
        Variant,
        Constant,

        #region Logical Operators

        Yes,
        Not,

        And,
        Or,
        Equal,
        NotEqual,
        Greater,
        GreaterEqual,
        Less,
        LessEqual,

        #endregion

        #region Arithmetic Operators

        Abs,
        Negate,

        Add,
        Subtract,
        Multiply,
        Divide,
        Mod,
        Min,
        Max,
        Mid,

        Clamp,

        #endregion
    }

    #region Logical Operators

    public interface IYes<T> where T : struct
    {
        Bool Yes();
    }

    public interface INot<T> where T : struct
    {
        Bool Not();
    }

    public interface IAnd<T> where T : struct
    {
        Bool And(in T rhs);
    }

    public interface IOr<T> where T : struct
    {
        Bool Or(in T rhs);
    }

    public interface IEqual<T> where T : struct
    {
        Bool Equal(in T rhs);
    }

    public interface INotEqual<T> where T : struct
    {
        Bool NotEqual(in T rhs);
    }

    public interface IGreater<T> where T : struct
    {
        Bool Greater(in T rhs);
    }

    public interface IGreaterEqual<T> where T : struct
    {
        Bool GreaterEqual(in T rhs);
    }

    public interface ILess<T> where T : struct
    {
        Bool Less(in T rhs);
    }

    public interface ILessEqual<T> where T : struct
    {
        Bool LessEqual(in T rhs);
    }

    /// <summary>
    /// 逻辑操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IBoolean<T> : IYes<T>, INot<T>, IAnd<T>, IOr<T> where T : struct
    {
    }

    /// <summary>
    /// 等式和不等式
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IEquat<T> : IEqual<T>, INotEqual<T> where T : struct
    {
    }

    /// <summary>
    /// 比较操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICompare<T> : IEqual<T>, INotEqual<T>, IGreater<T>, IGreaterEqual<T>, ILess<T>, ILessEqual<T>
        where T : struct
    {
    }

    #endregion

    #region Arithmetic Operators

    public interface IAbs<T> where T : struct
    {
        T Abs();
    }

    public interface INegate<T> where T : struct
    {
        T Negate();
    }

    public interface IAdd<T> where T : struct
    {
        T Add(in T rhs);
    }

    public interface ISubtract<T> where T : struct
    {
        T Subtract(in T rhs);
    }

    public interface IMultiply<T> where T : struct
    {
        T Multiply(in T rhs);
    }

    public interface IDivide<T> where T : struct
    {
        T Divide(in T rhs);
    }

    public interface IMod<T> where T : struct
    {
        T Mod(in T rhs);
    }

    public interface IMin<T> where T : struct
    {
        T Min(in T rhs);
    }

    public interface IMax<T> where T : struct
    {
        T Max(in T rhs);
    }

    public interface IMid<T> where T : struct
    {
        T Mid(in T rhs);
    }

    public interface IClamp<T> where T : struct
    {
        T Clamp(in T min, in T max);
    }

    /// <summary>
    /// 算术操作
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IArithmetic<T> : IAbs<T>, INegate<T>,
        IAdd<T>, ISubtract<T>, IMultiply<T>, IDivide<T>,
        IMod<T>, IMin<T>, IMax<T>, IMid<T>,
        IClamp<T>
        where T : struct
    {
    }

    #endregion
}