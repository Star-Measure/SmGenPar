using System.ComponentModel;
using System.Reflection;
using JetBrains.Annotations;
using static System.Runtime.InteropServices.CollectionsMarshal;

namespace SmGenPar.Logic.Reflection;

[PublicAPI]
public static class Extensions
{
    public static ref TValue? GetValueRefOrCreate<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey                          key,
        Func<TKey, TValue>            factory) where TKey : notnull
    {
        ref var values = ref GetValueRefOrAddDefault(dictionary, key, out bool exists);

        if (!exists || values is null) {
            values = factory(key);
        }

        return ref values;
    }

    public static ref TValue? GetValueRefOrCreate<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey                          key,
        Func<TValue>                  factory) where TKey : notnull
    {
        ref var values = ref GetValueRefOrAddDefault(dictionary, key, out bool exists);

        if (!exists || values is null) {
            values = factory();
        }

        return ref values;
    }
    public static PropertyInfo[] GetPropertiesCached(this Type type)
    {
        return Cache.PropertyInfo.GetValueRefOrCreate(type, type.GetProperties)!;
    }
    public static Attribute[] GetAttributesCached(this PropertyInfo propertyInfo)
    {
        return Cache.Attributes.GetValueRefOrCreate(propertyInfo, propertyInfo.GetCustomAttributes().ToArray)!;
    }
}

[PublicAPI]
public static class Cache
{
    public static readonly Dictionary<Type, PropertyInfo[]>      PropertyInfo = new();
    public static readonly Dictionary<PropertyInfo, Attribute[]> Attributes   = new();
}