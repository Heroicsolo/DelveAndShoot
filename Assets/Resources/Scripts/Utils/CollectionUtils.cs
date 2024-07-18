using OpenCover.Framework.Model;
using System;
using System.Collections;
using System.Collections.Generic;

public static class CollectionUtils
{
    public delegate T Operation<T>(T a, T b);
    public static IEnumerable<T> Comulative<T>(this IEnumerable<T> collection, Operation<T> operation)
    {
        T lastComulative = default(T);
        foreach (T item in collection)
        {
            lastComulative = operation(lastComulative, item);
            yield return lastComulative;
        }
    }

    public static IEnumerable<T> ComulativeSum<T>(this IEnumerable<T> collection) where T : IComparable<T> =>
        collection.Comulative((a, b) => (dynamic)a + (dynamic)b);
}