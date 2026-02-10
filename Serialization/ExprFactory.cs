using System;

namespace Vena
{
    public interface IExpressionFactory
    {
        IExpression CreateExpression(ExprType exprType);

        void DestroyExpression(IExpression expression);
    }

    public static partial class Expr
    {
        private static IExpressionFactory _internalFactory;

        public static void SetFactory(IExpressionFactory factory)
        {
            _internalFactory = factory;
        }

        /// <summary>
        /// new an expression instance.
        /// </summary>
        /// <param name="exprType"></param>
        /// <returns></returns>
        internal static IExpression New(ExprType exprType)
        {
            if (_internalFactory != null)
            {
                return _internalFactory.CreateExpression(exprType);
            }

            return CreateInstance(exprType);
        }

        /// <summary>
        /// delete an expression instance.
        /// </summary>
        /// <param name="expression"></param>
        private static void Delete(IExpression expression)
        {
            _internalFactory?.DestroyExpression(expression);
        }

        /// <summary>
        /// Create an instance of the specified expression type.
        /// </summary>
        /// <param name="exprType"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        public static IExpression CreateInstance(ExprType exprType)
        {
            switch (exprType.OperaterType)
            {
                case OperaterType.Variant: return CreateGetterExpression(exprType.ValueType);
                case OperaterType.Constant: return CreateConstExpression(exprType.ValueType);

                case OperaterType.Yes: return CreateYesExpression(exprType.ValueType);
                case OperaterType.Not: return CreateNotExpression(exprType.ValueType);

                case OperaterType.And: return CreateAndExpression(exprType.ValueType);
                case OperaterType.Or: return CreateOrExpression(exprType.ValueType);
                case OperaterType.Equal: return CreateEqualExpression(exprType.ValueType);
                case OperaterType.NotEqual: return CreateNotEqualExpression(exprType.ValueType);
                case OperaterType.Greater: return CreateGreaterExpression(exprType.ValueType);
                case OperaterType.Less: return CreateLessExpression(exprType.ValueType);
                case OperaterType.GreaterEqual: return CreateGreaterEqualExpression(exprType.ValueType);
                case OperaterType.LessEqual: return CreateLessEqualExpression(exprType.ValueType);

                case OperaterType.Abs: return CreateAbsExpression(exprType.ValueType);
                case OperaterType.Negate: return CreateNegExpression(exprType.ValueType);

                case OperaterType.Add: return CreateAddExpression(exprType.ValueType);
                case OperaterType.Subtract: return CreateSubExpression(exprType.ValueType);
                case OperaterType.Multiply: return CreateMulExpression(exprType.ValueType);
                case OperaterType.Divide: return CreateDivExpression(exprType.ValueType);
                case OperaterType.Mod: return CreateModExpression(exprType.ValueType);
                case OperaterType.Min: return CreateMinExpression(exprType.ValueType);
                case OperaterType.Max: return CreateMaxExpression(exprType.ValueType);
                case OperaterType.Mid: return CreateMidExpression(exprType.ValueType);

                case OperaterType.Clamp: return CreateClampExpression(exprType.ValueType);

                default:
                    throw new ArgumentException($"Unknown expression type: {exprType}");
            }
        }

        private static IExpression CreateGetterExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Bool: return new Variant<Bool>();
                case ValueType.Int: return new Variant<Int>();
                case ValueType.Int2: return new Variant<Int2>();
                case ValueType.Int3: return new Variant<Int3>();
                case ValueType.UInt: return new Variant<UInt>();
                case ValueType.Long: return new Variant<Long>();
                case ValueType.ULong: return new Variant<ULong>();
                case ValueType.Float: return new Variant<Float>();
                case ValueType.Float2: return new Variant<Float2>();
                case ValueType.Float3: return new Variant<Float3>();
                case ValueType.Float4: return new Variant<Float4>();
                default:
                    throw new ArgumentException($"Unsupported Getter<{valueType}>");
            }
        }


        private static IExpression CreateConstExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Bool: return new Constant<Bool>();
                case ValueType.Int: return new Constant<Int>();
                case ValueType.Int2: return new Constant<Int2>();
                case ValueType.Int3: return new Constant<Int3>();
                case ValueType.UInt: return new Constant<UInt>();
                case ValueType.Long: return new Constant<Long>();
                case ValueType.ULong: return new Constant<ULong>();
                case ValueType.Float: return new Constant<Float>();
                case ValueType.Float2: return new Constant<Float2>();
                case ValueType.Float3: return new Constant<Float3>();
                case ValueType.Float4: return new Constant<Float4>();
                default:
                    throw new ArgumentException($"Unsupported Var<{valueType}>");
            }
        }

        private static IExpression CreateYesExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Bool: return new Yes<Bool>();
                default:
                    throw new ArgumentException($"Unsupported Yes<{valueType}>");
            }
        }

        private static IExpression CreateNotExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Bool: return new Not<Bool>();
                default:
                    throw new ArgumentException($"Unsupported Not<{valueType}>");
            }
        }

        private static IExpression CreateAndExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Bool: return new And<Bool>();
                default:
                    throw new ArgumentException($"Unsupported And<{valueType}>");
            }
        }

        private static IExpression CreateOrExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Bool: return new Or<Bool>();
                default:
                    throw new ArgumentException($"Unsupported Or<{valueType}>");
            }
        }

        private static IExpression CreateEqualExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int: return new Equal<Int>();
                case ValueType.Int2: return new Equal<Int2>();
                case ValueType.Int3: return new Equal<Int3>();
                case ValueType.UInt: return new Equal<UInt>();
                case ValueType.Long: return new Equal<Long>();
                case ValueType.ULong: return new Equal<ULong>();
                case ValueType.Float: return new Equal<Float>();
                case ValueType.Float2: return new Equal<Float2>();
                case ValueType.Float3: return new Equal<Float3>();
                case ValueType.Float4: return new Equal<Float4>();
                default:
                    throw new ArgumentException($"Unsupported Equal<{valueType}>");
            }
        }

        private static IExpression CreateNotEqualExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int: return new NotEqual<Int>();
                case ValueType.Int2: return new NotEqual<Int2>();
                case ValueType.Int3: return new NotEqual<Int3>();
                case ValueType.UInt: return new NotEqual<UInt>();
                case ValueType.Long: return new NotEqual<Long>();
                case ValueType.ULong: return new NotEqual<ULong>();
                case ValueType.Float: return new NotEqual<Float>();
                case ValueType.Float2: return new NotEqual<Float2>();
                case ValueType.Float3: return new NotEqual<Float3>();
                case ValueType.Float4: return new NotEqual<Float4>();
                default:
                    throw new ArgumentException($"Unsupported NotEqual<{valueType}>");
            }
        }

        private static IExpression CreateGreaterExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int: return new Greater<Int>();
                case ValueType.UInt: return new Greater<UInt>();
                case ValueType.Long: return new Greater<Long>();
                case ValueType.ULong: return new Greater<ULong>();
                case ValueType.Float: return new Greater<Float>();
                default:
                    throw new ArgumentException($"Unsupported Var<{valueType}>");
            }
        }

        private static IExpression CreateGreaterEqualExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int: return new GreaterEqual<Int>();
                case ValueType.UInt: return new GreaterEqual<UInt>();
                case ValueType.Long: return new GreaterEqual<Long>();
                case ValueType.ULong: return new GreaterEqual<ULong>();
                case ValueType.Float: return new GreaterEqual<Float>();
                default:
                    throw new ArgumentException($"Unsupported GreaterEqual<{valueType}>");
            }
        }

        private static IExpression CreateLessExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int: return new Less<Int>();
                case ValueType.UInt: return new Less<UInt>();
                case ValueType.Long: return new Less<Long>();
                case ValueType.ULong: return new Less<ULong>();
                case ValueType.Float: return new Less<Float>();
                default:
                    throw new ArgumentException($"Unsupported Less<{valueType}>");
            }
        }

        private static IExpression CreateLessEqualExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int: return new LessEqual<Int>();
                case ValueType.UInt: return new LessEqual<UInt>();
                case ValueType.Long: return new LessEqual<Long>();
                case ValueType.ULong: return new LessEqual<ULong>();
                case ValueType.Float: return new LessEqual<Float>();
                default:
                    throw new ArgumentException($"Unsupported LessEqual<{valueType}>");
            }
        }

        private static IExpression CreateAbsExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int: return new Abs<Int>();
                case ValueType.Int2: return new Abs<Int2>();
                case ValueType.Int3: return new Abs<Int3>();
                case ValueType.UInt: return new Abs<UInt>();
                case ValueType.Long: return new Abs<Long>();
                case ValueType.ULong: return new Abs<ULong>();
                case ValueType.Float: return new Abs<Float>();
                case ValueType.Float2: return new Abs<Float2>();
                case ValueType.Float3: return new Abs<Float3>();
                case ValueType.Float4: return new Abs<Float4>();
                default:
                    throw new ArgumentException($"Unsupported Abs<{valueType}>");
            }
        }

        private static IExpression CreateNegExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int: return new Negate<Int>();
                case ValueType.Int2: return new Negate<Int2>();
                case ValueType.Int3: return new Negate<Int3>();
                case ValueType.UInt: return new Negate<UInt>();
                case ValueType.Long: return new Negate<Long>();
                case ValueType.ULong: return new Negate<ULong>();
                case ValueType.Float: return new Negate<Float>();
                case ValueType.Float2: return new Negate<Float2>();
                case ValueType.Float3: return new Negate<Float3>();
                case ValueType.Float4: return new Negate<Float4>();
                default:
                    throw new ArgumentException($"Unsupported Negate<{valueType}>");
            }
        }

        private static IExpression CreateAddExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int: return new Add<Int>();
                case ValueType.Int2: return new Add<Int2>();
                case ValueType.Int3: return new Add<Int3>();
                case ValueType.UInt: return new Add<UInt>();
                case ValueType.Long: return new Add<Long>();
                case ValueType.ULong: return new Add<ULong>();
                case ValueType.Float: return new Add<Float>();
                case ValueType.Float2: return new Add<Float2>();
                case ValueType.Float3: return new Add<Float3>();
                case ValueType.Float4: return new Add<Float4>();
                default:
                    throw new ArgumentException($"Unsupported Add<{valueType}>");
            }
        }

        private static IExpression CreateSubExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int: return new Subtract<Int>();
                case ValueType.Int2: return new Subtract<Int2>();
                case ValueType.Int3: return new Subtract<Int3>();
                case ValueType.UInt: return new Subtract<UInt>();
                case ValueType.Long: return new Subtract<Long>();
                case ValueType.ULong: return new Subtract<ULong>();
                case ValueType.Float: return new Subtract<Float>();
                case ValueType.Float2: return new Subtract<Float2>();
                case ValueType.Float3: return new Subtract<Float3>();
                case ValueType.Float4: return new Subtract<Float4>();
                default:
                    throw new ArgumentException($"Unsupported Subtract<{valueType}>");
            }
        }

        private static IExpression CreateMulExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int: return new Multiply<Int>();
                case ValueType.Int2: return new Multiply<Int2>();
                case ValueType.Int3: return new Multiply<Int3>();
                case ValueType.UInt: return new Multiply<UInt>();
                case ValueType.Long: return new Multiply<Long>();
                case ValueType.ULong: return new Multiply<ULong>();
                case ValueType.Float: return new Multiply<Float>();
                case ValueType.Float2: return new Multiply<Float2>();
                case ValueType.Float3: return new Multiply<Float3>();
                case ValueType.Float4: return new Multiply<Float4>();
                default:
                    throw new ArgumentException($"Unsupported Multiply<{valueType}>");
            }
        }

        private static IExpression CreateDivExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int: return new Divide<Int>();
                case ValueType.Int2: return new Divide<Int2>();
                case ValueType.Int3: return new Divide<Int3>();
                case ValueType.UInt: return new Divide<UInt>();
                case ValueType.Long: return new Divide<Long>();
                case ValueType.ULong: return new Divide<ULong>();
                case ValueType.Float: return new Divide<Float>();
                case ValueType.Float2: return new Divide<Float2>();
                case ValueType.Float3: return new Divide<Float3>();
                case ValueType.Float4: return new Divide<Float4>();
                default:
                    throw new ArgumentException($"Unsupported Divide<{valueType}>");
            }
        }

        private static IExpression CreateModExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int: return new Mod<Int>();
                case ValueType.Int2: return new Mod<Int2>();
                case ValueType.Int3: return new Mod<Int3>();
                case ValueType.UInt: return new Mod<UInt>();
                case ValueType.Long: return new Mod<Long>();
                case ValueType.ULong: return new Mod<ULong>();
                case ValueType.Float: return new Mod<Float>();
                case ValueType.Float2: return new Mod<Float2>();
                case ValueType.Float3: return new Mod<Float3>();
                case ValueType.Float4: return new Mod<Float4>();
                default:
                    throw new ArgumentException($"Unsupported Mod<{valueType}>");
            }
        }

        private static IExpression CreateMinExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int: return new Min<Int>();
                case ValueType.Int2: return new Min<Int2>();
                case ValueType.Int3: return new Min<Int3>();
                case ValueType.UInt: return new Min<UInt>();
                case ValueType.Long: return new Min<Long>();
                case ValueType.ULong: return new Min<ULong>();
                case ValueType.Float: return new Min<Float>();
                case ValueType.Float2: return new Min<Float2>();
                case ValueType.Float3: return new Min<Float3>();
                case ValueType.Float4: return new Min<Float4>();
                default:
                    throw new ArgumentException($"Unsupported Min<{valueType}>");
            }
        }

        private static IExpression CreateMaxExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int: return new Max<Int>();
                case ValueType.Int2: return new Max<Int2>();
                case ValueType.Int3: return new Max<Int3>();
                case ValueType.UInt: return new Max<UInt>();
                case ValueType.Long: return new Max<Long>();
                case ValueType.ULong: return new Max<ULong>();
                case ValueType.Float: return new Max<Float>();
                case ValueType.Float2: return new Max<Float2>();
                case ValueType.Float3: return new Max<Float3>();
                case ValueType.Float4: return new Max<Float4>();
                default:
                    throw new ArgumentException($"Unsupported Max<{valueType}>");
            }
        }

        private static IExpression CreateMidExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int: return new Mid<Int>();
                case ValueType.Int2: return new Mid<Int2>();
                case ValueType.Int3: return new Mid<Int3>();
                case ValueType.UInt: return new Mid<UInt>();
                case ValueType.Long: return new Mid<Long>();
                case ValueType.ULong: return new Mid<ULong>();
                case ValueType.Float: return new Mid<Float>();
                case ValueType.Float2: return new Mid<Float2>();
                case ValueType.Float3: return new Mid<Float3>();
                case ValueType.Float4: return new Mid<Float4>();
                default:
                    throw new ArgumentException($"Unsupported Mid<{valueType}>");
            }
        }

        private static IExpression CreateClampExpression(ValueType valueType)
        {
            switch (valueType)
            {
                case ValueType.Int: return new Clamp<Int>();
                case ValueType.Int2: return new Clamp<Int2>();
                case ValueType.Int3: return new Clamp<Int3>();
                case ValueType.UInt: return new Clamp<UInt>();
                case ValueType.Long: return new Clamp<Long>();
                case ValueType.ULong: return new Clamp<ULong>();
                case ValueType.Float: return new Clamp<Float>();
                case ValueType.Float2: return new Clamp<Float2>();
                case ValueType.Float3: return new Clamp<Float3>();
                case ValueType.Float4: return new Clamp<Float4>();
                default:
                    throw new ArgumentException($"Unsupported Clamp<{valueType}>");
            }
        }
    }
}