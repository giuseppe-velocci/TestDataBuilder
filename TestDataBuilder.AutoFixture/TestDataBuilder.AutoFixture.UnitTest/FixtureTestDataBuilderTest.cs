using AutoFixture;
using AutoFixture.Kernel;
using FluentAssertions;

namespace TestDataBuilder.AutoFixture.UnitTest
{
    public class FixtureTestDataBuilderTest
    {
        [Fact]
        public void Create_WhenInvokedOnFixtureWithSpecimenBuilder_ShouldKeepTheSetupValues()
        {
            //Arrange
            Fixture fixture = new();
            fixture.Customize(new MyCustomization());
            string innerDescription = "inner";

            //Act
            var result = fixture
                .NewTestDataBuilder<WithBoth>()
                .With(x => x.InnerDescription, innerDescription)
                .Create();

            //Assert
            result.Description.Should().Be("Custom String");
            result.InnerDescription.Should().Be(innerDescription);
        }

        [Fact]
        public void Create_WhenInvokedOnFixtureWithSpecimenBuilderAndDefaultTestDataBuilder_ShouldKeepTheSetupValues()
        {
            //Arrange
            Fixture fixture = new();
            fixture.Customize(new MyCustomization());
            string innerDescription = "inner";

            //Act
            var result = fixture
                .NewTestDataBuilder<WithBoth>(x => x.Create(x => x.InnerDescription, innerDescription))
                .Create();

            //Assert
            result.Description.Should().Be("Custom String");
            result.InnerDescription.Should().Be(innerDescription);
        }

        [Fact]
        public void Create_WhenInvokedOnClassHavingPrivateSetter_ShouldSetThoseValues()
        {
            //Arrange
            Fixture fixture = new();

            //Act
            var result = fixture
                    .NewTestDataBuilder<ClassWithoutCtor>()
                    .Create();

            //Assert
            result.Description.Should().NotBeNull();
        }

        [Fact]
        public void With_WhenInvokedOnClassNotHavingPrivateSetterUsingWithStatement_ShouldPopulateThoseValues()
        {
            //Arrange
            Fixture fixture = new();
            string expected = "exp";

            //Act
            var result = fixture
                .NewTestDataBuilder<ClassWithoutCtorAndSet>()
                .With(x => x.Description, () => expected)
                .Create();

            //Assert
            result.Description.Should().Be(expected);
        }

        [Fact]
        public void With_WhenInvokedOnNestedClassNotHavingPrivateSetter_ShouldNotPopulateThoseValues()
        {
            //Arrange
            Fixture fixture = new();

            //Act
            var result = fixture.NewTestDataBuilder<NestedClassWithoutCtorAndSet>()
                .Create();

            //Assert
            result.Item.Should().BeNull();
        }

        [Fact]
        public void With_WhenInvokedOnNestedClassNotHavingPrivateSetterUsingWith_ShouldPopulateThoseValues()
        {
            //Arrange
            Fixture fixture = new();
            string expected = "exp";

            //Act
            var result = fixture
                .NewTestDataBuilder<NestedClassWithoutCtorAndSet>()
                .With(x => x.Item, () => fixture
                    .NewTestDataBuilder<ClassWithoutCtorAndSet>()
                    .With(x => x.Description, () => expected)
                    .Create())
                .Create();

            //Assert
            result.Item.Description.Should().Be(expected);
        }

        [Fact]
        public void With_WhenInvokedOnNestedClassNotHavingPrivateSetterUsingDefault_ShouldPopulateThoseValues()
        {
            //Arrange
            Fixture fixture = new();
            string expected = "exp";
            var internalFixture = fixture
                .NewTestDataBuilder<ClassWithoutCtorAndSet>(x => x.Create(x => x.Description, () => expected));

            //Act
            var result = fixture
                .NewTestDataBuilder<NestedClassWithoutCtorAndSet>(x => x.Create(x => x.Item, () => internalFixture.Create()))
                .Create();

            //Assert
            result.Item.Description.Should().Be(expected);
        }

        [Fact]
        public void Create_WhenInvokedOnClassHavingPrivateSetterAfterSetupUsingDefaults_ShouldPopulateThoseValues()
        {
            //Arrange
            Fixture fixture = new();
            string expected = "descr";

            //Act
            var result = fixture
                .NewTestDataBuilder<ClassWithoutCtor>(x => x.Create(x => x.Description, () => expected))
                .Create();

            //Assert
            result.Description.Should().Be(expected);
        }

        [Fact]
        public void Create_WhenInvokedOnClassHavingPrivateSetterButPublicCtor_ShouldUseTheCtor()
        {
            //Arrange
            Fixture fixture = new();

            //Act
            var result = fixture
                .NewTestDataBuilder<ClassWithCtor>()
                .Create();

            //Assert
            result.Description.Should().NotBeNull();
        }

        [Fact]
        public void Create_WhenInvokedOnClassHavingVirtualProperties_ShouldSetThoseProperties()
        {
            //Arrange
            Fixture fixture = new();
            int expected = 12;
            int expected1 = 34;

            //Act
            var result = fixture
                .NewTestDataBuilder<WithVirtual>(
                    x => x.Create(x => x.IdPrivateSet, () => expected),
                    x => x.Create(x => x.IdReadonly, () => expected1)
                )
                .Create();

            //Assert
            result.IdPrivateSet.Should().Be(expected);
            result.IdReadonly.Should().Be(expected1);
        }

        // fields
        [Fact]
        public void Create_WhenInvokedOnClassHavingPublicField_ShouldSetIt()
        {
            //Arrange
            Fixture fixture = new();

            //Act
            var result = fixture
                .NewTestDataBuilder<WithFields>()
                .Create();

            //Assert
            result._number.Should().BeGreaterThan(0);
        }

        [Fact]
        public void Create_WhenInvokedOnClassHavingPublicReadonlyFieldUsingWithStatement_ShouldSetIt()
        {
            //Arrange
            Fixture fixture = new();

            //Act
            var result = fixture
                .NewTestDataBuilder<WithReadonlyFields>()
                .With(x => x._readonlyNumber, () => 123)
                .Create();

            //Assert
            result._readonlyNumber.Should().BeGreaterThan(0);
        }

        //builder
        [Fact]
        public void Create_WhenInvokedOnBuilderWithDeafults_ShouldReturnDifferentInstances()
        {
            //Arrange
            Fixture fixture = new();
            var builder = fixture
                .NewTestDataBuilder<ClassWithoutCtor>(x => x.Create(x => x.Description, () => "value"));

            //Act
            var result = builder.Create();
            var result1 = builder.Create();

            //Assert
            result.Description.Should().Be(result1.Description);
            result.Should().NotBe(result1);
        }

        [Fact]
        public void Create_WhenInvokedMultipleTimesOnBuilderWithDeafults_ShouldReturnDifferentInstancesWithExpectedData()
        {
            //Arrange
            string expected = "old";
            string expected1 = "new";
            Fixture fixture = new();
            var builder = fixture
                .NewTestDataBuilder<ClassWithoutCtor>(x => x.Create(x => x.Description, () => expected));

            //Act
            var result = builder.Create();
            var result1 = builder
                .With(x => x.Description, () => expected1)
                .Create();

            //Assert
            result.Description.Should().Be(expected);
            result1.Description.Should().Be(expected1);
        }

        [Fact]
        public void Create_WhenInvokedOnRecordClassHavingPrivateSetter_ShouldPopulateThoseValues()
        {
            //Arrange
            Fixture fixture = new();
            string expected = "descr";

            //Act
            var result = fixture
                .NewTestDataBuilder<RecordWithPrivateSetter>()
                .With(x => x.Description, () => expected)
                .Create();

            //Assert
            result.Description.Should().Be(expected);
        }

        [Fact]
        public void Create_WhenInvokedOnRecordClassHavingReadonlyField_ShouldPopulateThoseValues()
        {
            //Arrange
            Fixture fixture = new();
            string expected = "descr";

            //Act
            var result = fixture
                .NewTestDataBuilder<RecordWithReadonlyField>()
                .With(x => x.description, () => expected)
                .Create();

            //Assert
            result.description.Should().Be(expected);
        }

        //without
        [Fact]
        public void Create_WhenInvokedAfterWithoutOnClassHavingPrivateSetter_ShouldRemoveThoseValues()
        {
            //Arrange
            Fixture fixture = new();

            //Act
            var result = fixture
                .NewTestDataBuilder<ClassWithoutCtorAndSet>()
                .Without(x => x.Description)
                .Create();

            //Assert
            result.Description.Should().BeNull();
        }

        [Fact]
        public void Create_WhenInvokedAfterWithoutOnClassHavingReadonlyField_ShouldRemoveThoseValues()
        {
            //Arrange
            Fixture fixture = new();

            //Act
            var result = fixture
                .NewTestDataBuilder<WithReadonlyFields>()
                .Without(x => x._readonlyNumber)
                .Create();

            //Assert
            result._readonlyNumber.Should().Be(0);
        }

        [Fact]
        public void Create_WhenInvokedAfterWithoutOnRecordClassHavingPrivateSetterField_ShouldRemoveThoseValues()
        {
            //Arrange
            Fixture fixture = new();

            //Act
            var result = fixture
                .NewTestDataBuilder<RecordWithPrivateSetter>()
                .Without(x => x.Description)
                .Create();

            //Assert
            result.Description.Should().BeNull();
        }

        [Fact]
        public void Create_WhenInvokedAfterWithoutOnRecordClassHavingReadonlyField_ShouldRemoveThoseValues()
        {
            //Arrange
            Fixture fixture = new();

            //Act
            var result = fixture
                .NewTestDataBuilder<RecordWithReadonlyField>()
                .Without(x => x.description)
                .Create();

            //Assert
            result.description.Should().BeNull();
        }
    }

    // customizations
    public class MyCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customizations.Add(new MyCustomizationLogic());
        }
    }

    public class MyCustomizationLogic : ISpecimenBuilder
    {
        public object Create(object request, ISpecimenContext context)
        {
            // Implement your customization logic here
            if (request is Type type && type == typeof(WithBoth))
            {
                return new WithBoth { Description = "Custom String" };
            }
            return new NoSpecimen();
        }
    }

    // classes with fields
    public class WithFields
    {
        public int _number;
    }

    public class WithReadonlyFields
    {
        public readonly int _readonlyNumber;
    }

    // classes with props
    public class WithBoth
    {
        public string Description { get; set; } = null!;
        public string InnerDescription { get; } = null!;
    }

    public class MyClass(int id, string name)
    {
        public int Id { get; private set; } = id;
        public string Name { get; private set; } = name;
    }

    public class ClassWithoutCtor
    {
        public string Description { get; private set; } = string.Empty;
    }

    public class ClassWithoutCtorAndSet
    {
        public string Description { get; } = string.Empty;
    }

    public class NestedClassWithoutCtorAndSet
    {
        public ClassWithoutCtorAndSet Item { get; } = null!;
    }

    public class ClassWithCtor(string description)
    {
        public string Description { get; private set; } = description;
    }

    public class WithVirtual
    {
        public virtual int IdPrivateSet { get; private set; }
        public virtual int IdReadonly { get; }
    }

    // records
    public class RecordWithPrivateSetter
    {
        public string Description { get; private set; } = string.Empty;
    }

    public class RecordWithReadonlyField
    {
        public readonly string description = string.Empty;
    }
}