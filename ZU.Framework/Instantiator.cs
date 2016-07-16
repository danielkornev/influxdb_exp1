using System;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace ZU.Framework
{
    public static class Instantiator<TInstance>
    {
        static Instantiator()
        {
            Debug.Assert(typeof(TInstance).IsValueType || (typeof(TInstance).IsClass && !typeof(TInstance).IsAbstract),
                String.Concat("The type ", typeof(TInstance).Name, " is not constructable."));
        }

        public static TInstance New()
        {
            return InstantiatorImpl.CtorFunc();
        }

        public static TInstance New<TA>(TA valueA)
        {
            return InstantiatorImpl<TA>.CtorFunc(valueA);
        }

        public static TInstance New<TA, TB>(TA valueA, TB valueB)
        {
            return InstantiatorImpl<TA, TB>.CtorFunc(valueA, valueB);
        }

        public static TInstance New<TA, TB, TC>(TA valueA, TB valueB, TC valueC)
        {
            return InstantiatorImpl<TA, TB, TC>.CtorFunc(valueA, valueB, valueC);
        }

        public static TInstance New<TA, TB, TC, TD>(TA valueA, TB valueB, TC valueC, TD valueD)
        {
            return InstantiatorImpl<TA, TB, TC, TD>.CtorFunc(valueA, valueB, valueC, valueD);
        }

        private static Expression<TDelegate> CreateLambdaExpression<TDelegate>(params Type[] argTypes)
        {
            Debug.Assert(argTypes != null);

            ParameterExpression[] paramExpressions = new ParameterExpression[argTypes.Length];

            for (int i = 0; i < paramExpressions.Length; i++)
            {
                paramExpressions[i] = Expression.Parameter(argTypes[i], String.Concat("arg", i));
            }

            ConstructorInfo ctorInfo = typeof(TInstance).GetConstructor(argTypes);
            if (ctorInfo == null)
            {
                throw new ArgumentException(String.Concat("The type ", typeof(TInstance).Name, " has no constructor with the argument type(s) ", String.Join(", ", argTypes.Select(t => t.Name).ToArray()), "."),
                    "argTypes");
            }

            return Expression.Lambda<TDelegate>(Expression.New(ctorInfo, paramExpressions), paramExpressions);
        }

        private static class InstantiatorImpl
        {
            public static readonly Func<TInstance> CtorFunc = Expression.Lambda<Func<TInstance>>(Expression.New(typeof(TInstance))).Compile();
        }

        private static class InstantiatorImpl<TA>
        {
            public static readonly Func<TA, TInstance> CtorFunc = Instantiator<TInstance>.CreateLambdaExpression<Func<TA, TInstance>>(typeof(TA)).Compile();
        }

        private static class InstantiatorImpl<TA, TB>
        {
            public static readonly Func<TA, TB, TInstance> CtorFunc = Instantiator<TInstance>.CreateLambdaExpression<Func<TA, TB, TInstance>>(typeof(TA), typeof(TB)).Compile();
        }

        private static class InstantiatorImpl<TA, TB, TC>
        {
            public static readonly Func<TA, TB, TC, TInstance> CtorFunc = Instantiator<TInstance>.CreateLambdaExpression<Func<TA, TB, TC, TInstance>>(typeof(TA), typeof(TB), typeof(TC)).Compile();
        }

        private static class InstantiatorImpl<TA, TB, TC, TD>
        {
            public static readonly Func<TA, TB, TC, TD, TInstance> CtorFunc = Instantiator<TInstance>.CreateLambdaExpression<Func<TA, TB, TC, TD, TInstance>>(typeof(TA), typeof(TB), typeof(TC), typeof(TD)).Compile();
        }
    }
}