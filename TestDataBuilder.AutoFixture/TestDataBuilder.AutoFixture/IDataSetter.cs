using System;

namespace TestDataBuilder.AutoFixture
{
    public interface IDataSetter<TEntity>
    {
        Func<TEntity, TEntity> GetFunc();
    }
}
