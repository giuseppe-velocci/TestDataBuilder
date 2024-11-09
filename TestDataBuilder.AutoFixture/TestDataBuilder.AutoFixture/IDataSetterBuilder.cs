using System;
using System.Linq.Expressions;

namespace TestDataBuilder.AutoFixture
{
    public interface IDataSetterBuilder<T>
    {
        IDataSetter<T> Create<TProperty>(Expression<Func<T, TProperty>> expression, TProperty value);
        IDataSetter<T> Create<TProperty>(Expression<Func<T, TProperty>> expression, Func<TProperty> factory);
    }
}