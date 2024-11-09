# TestDataBuilder
A small extension that enables AutoFixture to assign values to public readonly properties and fields.

## Usage
TestDataBuilder provides an alternative Builder for AutoFixture that can set values for:
- Public properties with public getters and setters
- Public properties with public getters and private setters
- Public properties with only public getters
- Public fields
- Public readonly fields

To create a builder, you can use the following two extension methods for `IFixture`:

- To create a builder with default customizations:
```csharp
var builder = fixture.NewTestDataBuilder<T>();
```

- To create a builder with default customizations:
```csharp
Fixture fixture = new();
var builder = fixture
    .NewTestDataBuilder<T>(
        x => x.Create(y => y.Property, "defaultValue"),
        x => x.Create(y => y.NextProperty, () => 12 * 60)
    );
```

Adding default customizations at declaration time can be helpful when you need to use the builder to create multiple objects that share some common characteristics.

When a builder is created, it exposes the following methods:

```csharp
ITestDataBuilder<T> With<TProperty>(Expression<Func<T, TProperty>> expression, TProperty value);
ITestDataBuilder<T> With<TProperty>(Expression<Func<T, TProperty>> expression, Func<TProperty> factory);
ITestDataBuilder<T> Without<TProperty>(Expression<Func<T, TProperty>> expression);
T Create();
```

For a smoother transition, the syntax is deliberately similar to that used by AutoFixture.

## Limitations
At this time, the library cannot:
- create multiple objects (no equivalent to the CreateMany() method)
- assign values to readonly properties and fields without an explicit setup using the .With() methods
- assign with one declaration a value to a readonly field or property that is a nested object

There are simple strategied to overcome those limitations that will be further detailed in the next section of this guide.

## How to crete multiple objects that share some common customizations
The simplest way to create multiple objects that share some traits is to store a customized builder with defaults in a variable and then call the `.Create()` method as often as required. Each instance thus created will be new.

```csharp
Fixture fixture = new();
var builder = fixture
    .NewTestDataBuilder<MyClass>(
        x => x.Create(y => y.Property, "defaultValue"),
        x => x.Create(y => y.NextProperty, 12)
    );

var input1 = builder.Create();
var input2 = builder.Create();

Assert.Equal(input1.Property, input2.Property); // true
Assert.Equal(input1.NextProperty, input2.NextProperty); // true
Assert.Equal(input1, input2); //false
```

## How to assign a customized value to a readonly property or field that is a reference type
The only way to assign custom values to a property or field that is a reference type is to create a custom builder for that value and pass an instance to the main builder.

```csharp
class Root
{
    public NestedItem Item { get; }
}

class NestedItem
{
    public int Value { get; }
}

//...

// test definition
Fixture fixture = new();
var nestedItemBuilder = fixture
    .NewTestDataBuilder<NestedItem>(x => x.Create(y => y.Value, 12));

var rootItem = fixture
    .NewTestDataBuilder<Root>()
    .With(x => x.Item, nestedItemBuilder.Create())
    .Create();
```