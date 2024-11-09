using AutoFixture;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace TestDataBuilder.AutoFixture.Internals
{
    internal sealed class TestDataBuilder<T> : ITestDataBuilder<T>
    {
        private T Instance;
        private readonly List<IDataSetter<T>> BuilderInitializations = new List<IDataSetter<T>>();

        internal TestDataBuilder(IFixture fixture)
        {
            BuilderInitializations.Add(new InitialDataSetter<T>(() => fixture.Create<T>()));
            Instance = fixture.Create<T>();
        }

        internal TestDataBuilder(IFixture fixture, IEnumerable<IDataSetter<T>> builderInitializations) : this(fixture)
        {
            foreach (var expression in builderInitializations)
            {
                BuilderInitializations.Add(expression);
                expression.GetFunc()(Instance);
            }
        }

        public ITestDataBuilder<T> With<TProperty>(Expression<Func<T, TProperty>> expression, TProperty value)
        {
            var transformer = new Func<T, T>((t) => new DataTransformer<T>().SetPropertyValue(t, expression, value));
            Instance = transformer(Instance);
            return this;
        }

        public ITestDataBuilder<T> With<TProperty>(Expression<Func<T, TProperty>> expression, Func<TProperty> factory)
        {
            var transformer = new Func<T, T>((t) => new DataTransformer<T>().SetPropertyValue(t, expression, factory()));
            Instance = transformer(Instance);
            return this;
        }

        public ITestDataBuilder<T> Without<TProperty>(Expression<Func<T, TProperty>> expression)
        {
            var transformer = new Func<T, T>((t) => new DataTransformer<T>().SetPropertyValue(t, expression, default));
            Instance = transformer(Instance);
            return this;
        }

        public T Create()
        {
            T temp = Instance;
            foreach (var builder in BuilderInitializations)
            {
                Instance = builder.GetFunc()(Instance);
            }
            return temp;
        }
    }

    internal class InitialDataSetter<T> : IDataSetter<T>
    {
        private readonly Func<T> Builder;

        public InitialDataSetter(Func<T> builder)
        {
            Builder = builder;
        }

        public Func<T, T> GetFunc()
        {
            return (_) => Builder();
        }
    }
}