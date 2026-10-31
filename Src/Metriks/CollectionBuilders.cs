// Metriks
// Copyright (c) KryKom 2026

using System;
using System.Collections.Generic;

#if !NET8_0_OR_GREATER
namespace System.Runtime.CompilerServices {
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Interface, Inherited = false)]
    internal sealed class CollectionBuilderAttribute : Attribute {
        public CollectionBuilderAttribute(Type builderType, string methodName) {
            BuilderType = builderType;
            MethodName = methodName;
        }
        public Type BuilderType { get; }
        public string MethodName { get; }
    }
}
#endif

namespace Metriks {

    internal static class CollectionBuilders {
        public static List2D<T> Create2D<T>(ReadOnlySpan<IEnumerable<T>> items) {
            var list = items.Length > 0 ? new List2D<T>(items.Length, 4) : new List2D<T>();

            foreach (var item in items) {
                list.Add(item);
            }

            return list;
        }

        public static List3D<T> Create3D<T>(ReadOnlySpan<IEnumerable2D<T>> items) {
            var list = items.Length > 0 ? new List3D<T>(items.Length) : new List3D<T>();

            foreach (var item in items) {
                list.Add(item);
            }

            return list;
        }

        public static List4D<T> Create4D<T>(ReadOnlySpan<IEnumerable3D<T>> items) {
            var list = items.Length > 0 ? new List4D<T>(items.Length) : new List4D<T>();

            foreach (var item in items) {
                list.Add(item);
            }

            return list;
        }
    }

}