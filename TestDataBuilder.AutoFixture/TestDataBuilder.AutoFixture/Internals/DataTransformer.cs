using System;
using System.Linq.Expressions;
using System.Reflection;

namespace TestDataBuilder.AutoFixture.Internals
{
    internal class DataTransformer<T>
    {
        internal T SetPropertyValue<TProperty>(
            T t,
            Expression<Func<T, TProperty>> expression,
            TProperty value)
        {
            var setter = CreateSetter(expression);
            setter(t, value);
            return t;
        }

        private static Action<TEntity, TProperty> CreateSetter<TEntity, TProperty>(
            Expression<Func<TEntity, TProperty>> selector)
        {
            MemberInfo info = (selector.Body as MemberExpression)?.Member;
            if (TryConvertPropertyInfo(info, out PropertyInfo propertyInfo))
            {
                if (propertyInfo.CanWrite)
                {
                    return (item, val) => propertyInfo.SetValue(item, val);
                }
                else
                {
                    Type entityType = typeof(TEntity);
                    var fieldInfo = entityType.GetField($"<{propertyInfo.Name}>k__BackingField", BindingFlags.Instance | BindingFlags.NonPublic);
                    if (fieldInfo == null)
                    {
                        throw new InvalidOperationException("Cannot set a nested property that has no setter. " +
                            "Add a private setter or setup a new Builder to create an instance fo the class that has the property");
                    }
                    else
                    {
                        return (item, val) => fieldInfo.SetValue(item, val);
                    }
                }
            }
            else if (TryConvertFieldInfo(info, out FieldInfo fieldInfo))
            {
                return (item, val) => fieldInfo.SetValue(item, val);
            }
            else
            {
                throw new InvalidOperationException("Accessed value must be a field or a property");
            }
        }

        private static bool TryConvertPropertyInfo(MemberInfo info, out PropertyInfo propertyInfo)
        {
            propertyInfo = info as PropertyInfo;
            return propertyInfo != null;
        }

        private static bool TryConvertFieldInfo(MemberInfo info, out FieldInfo fieldInfo)
        {
            fieldInfo = info as FieldInfo;
            return fieldInfo != null;
        }
    }
}