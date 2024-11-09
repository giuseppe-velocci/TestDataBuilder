using System;
using System.Linq.Expressions;

namespace TestDataBuilder.AutoFixture
{
    public interface ITestDataBuilder<T>
    {
        T Create();
        ITestDataBuilder<T> With<TProperty>(Expression<Func<T, TProperty>> expression, TProperty value);
        ITestDataBuilder<T> With<TProperty>(Expression<Func<T, TProperty>> expression, Func<TProperty> factory);
        ITestDataBuilder<T> Without<TProperty>(Expression<Func<T, TProperty>> expression);
    }
}