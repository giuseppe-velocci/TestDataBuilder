using System;
using System.Linq.Expressions;

namespace TestDataBuilder.AutoFixture.Internals
{
    internal class DataSetterBuilder<T> : IDataSetterBuilder<T>
    {
        public IDataSetter<T> Create<TProperty>(Expression<Func<T, TProperty>> expression, TProperty value)
        {
            return DataSetter<T>.Create(expression, () => value);
        }

        public IDataSetter<T> Create<TProperty>(Expression<Func<T, TProperty>> expression, Func<TProperty> factory)
        {
            return DataSetter<T>.Create(expression, factory);
        }

    }
}