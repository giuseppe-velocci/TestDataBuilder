using System;
using System.Linq.Expressions;

namespace TestDataBuilder.AutoFixture.Internals
{
    internal sealed class ConcreteDataSetter<TEntity, TProperty> : IDataSetter<TEntity>
    {
        private Expression<Func<TEntity, TProperty>> Setter { get; }
        private Func<TProperty> Factory { get; }

        public ConcreteDataSetter(Expression<Func<TEntity, TProperty>> setter, Func<TProperty> value)
        {
            Setter = setter;
            Factory = value;
        }

        public Func<TEntity, TEntity> GetFunc() =>
            (t) => new DataTransformer<TEntity>().SetPropertyValue(t, Setter, Factory());
    }
}
