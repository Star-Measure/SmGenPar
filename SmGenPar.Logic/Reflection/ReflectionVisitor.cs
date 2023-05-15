using System.Reflection;
using JetBrains.Annotations;
using static System.Runtime.InteropServices.CollectionsMarshal;

namespace SmGenPar.Logic.Reflection;

public static class CacheExtensions
{
    public static ref TValue? GetValueRefOrCreate<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey                          key,
        Func<TKey, TValue>            factory) where TKey : notnull
    {
        ref var values = ref GetValueRefOrAddDefault(dictionary, key, out bool exists);

        if (!exists) {
            values = factory(key);
        }
        
        return ref values;
    }

    public static TValue? GetValueOrCreate<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey                          key,
        Func<TKey, TValue>            factory) where TKey : notnull
    {
        ref var values = ref GetValueRefOrAddDefault(dictionary, key, out bool exists);

        if (!exists) {
            values = factory(key);
        }

        return values;
    }
}

public static class Cached
{
    public static readonly Dictionary<Type, PropertyInfo[]>      PropertyInfo = new();
    public static readonly Dictionary<PropertyInfo, Attribute[]> Attributes   = new();
}

[PublicAPI]
public static class ReflectionVisitor
{
    public static void VisitProperties(Type type, Action<PropertyInfo> action)
    {
        var properties = Cached.PropertyInfo.GetValueOrCreate(type, t => t.GetProperties())!;
        var span       = properties.AsSpan();

        foreach (var propertyInfo in span) {
            action(propertyInfo);
        }
    }

    public static void VisitAttributes(PropertyInfo propertyInfo, Action<Attribute[]> action)
    {
        var attributes = Cached.Attributes.GetValueOrCreate(
            propertyInfo, info => info.GetCustomAttributes().ToArray()
            )!;
        action(attributes);
    }
}

public static class Extensions
{
    public static bool TryMatch<TType, TMatch>(this TType[] values, out TMatch match)
    {
        var span = values.AsSpan();
        for (int index = 0; index < values.Length; index++) {
            var value = span[index];
            if (value is not TMatch result) continue;
            match = result;
            return true;
        }

        match = default!;
        return false;
    }
}