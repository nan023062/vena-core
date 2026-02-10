using System;

namespace Vena
{
    public interface IExprTree : IDisposable
    {
        void Serialize(ref ISerializer writer);

        void Deserialize(ref ISerializer reader);
    }

    public struct ExprTree<T> : IExprTree where T : struct, IValue<T>
    {
        public IBlackboard blackboard;

        public IExpression<T> expr;

        public ExprTree(IBlackboard blackboard, IExpression<T> expr = default)
        {
            this.blackboard = blackboard;

            this.expr = expr;
        }

        public T Evaluate()
        {
            return expr?.Evaluate(ref blackboard) ?? Value<T>.Default;
        }

        public void Dispose()
        {
            if (null != expr)
            {
                using var expression = expr;
                expr = default;
            }
        }

        public void Serialize(ref ISerializer writer)
        {
            IExpression<T> expression = expr ?? Expr.Constant<T>.Default;

            ExprType exprType = expression.exprType;

            writer.WriteShort(exprType);

            expression.Serialize(ref writer);
        }

        public void Deserialize(ref ISerializer reader)
        {
            ExprType exprType = reader.ReadShort();

            if (null == expr || expr.exprType != exprType)
            {
                using var expression = expr;

                expr = (IExpression<T>)Expr.New(exprType);
            }

            expr.Deserialize(ref reader);
        }
    }
}