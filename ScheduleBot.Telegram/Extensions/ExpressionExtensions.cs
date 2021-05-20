using System;
using System.Linq.Expressions;
using System.Reflection;

namespace ScheduleBot.Extensions
{
    public static class ExpressionExtensions
    {
        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(this Expression<Func<TSource, TProperty>> property)
        {
            var type = typeof(TSource);
            var member = property.Body as MemberExpression;

            if (member is null)
            {
                throw new ArgumentException($"Expression \"{property}\" refers to a method, not a property");
            }

            var propertyInfo = member.Member as PropertyInfo;

            if (propertyInfo is null)
            {
                throw new ArgumentException($"Expression \"{property}\" refers to a field, not a property");
            }

            if (type != propertyInfo.ReflectedType && !type.IsSubclassOf(propertyInfo.ReflectedType))
            {
                throw new ArgumentException($"Expression \"{property}\" refers to a property that is not from type {type}");
            }

            return propertyInfo;
        }
    }
}
