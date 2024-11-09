using System;
using System.Linq.Expressions;

namespace TestDataBuilder.AutoFixture.Internals
{
    internal static class DataSetter<TEntity>
    {
        public static IDataSetter<TEntity> Create<TProperty>(Expression<Func<TEntity, TProperty>> setter, Func<TProperty> value)
            => new ConcreteDataSetter<TEntity, TProperty>(setter, value);
    }
}
