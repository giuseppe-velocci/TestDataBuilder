using AutoFixture;
using System.Linq;
using TestDataBuilder.AutoFixture.Internals;

namespace TestDataBuilder.AutoFixture
{
    public static class AutoFixtureExtensions
    {
        public static ITestDataBuilder<T> NewTestDataBuilder<T>(this IFixture fixture)
        {
            return new TestDataBuilder<T>(fixture);
        }

        public static ITestDataBuilder<T> NewTestDataBuilder<T>(
            this IFixture fixture,
            params System.Func<IDataSetterBuilder<T>, IDataSetter<T>>[] builderInitializations)
        {
            DataSetterBuilder<T> dataSetter = new DataSetterBuilder<T>();
            var builder = new TestDataBuilder<T>(fixture);
            return new TestDataBuilder<T>(fixture, builderInitializations.Select(x => x(dataSetter)));
        }
    }
}
