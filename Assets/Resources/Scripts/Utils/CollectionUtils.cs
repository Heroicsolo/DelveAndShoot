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

    public static IEnumerable<float> ComulativeSum(this IEnumerable<float> collection) =>
        collection.Comulative((a, b) => a + b);
}