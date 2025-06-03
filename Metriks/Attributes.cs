//
// Metriks
//  Copyright (c) MIT License 2025, KryKom & ZlomenyMesic
//

// ReSharper disable EmptyConstructor
// ReSharper disable UnusedParameter.Local
// ReSharper disable CheckNamespace

#if NETSTANDARD2_0

namespace System.Diagnostics.CodeAnalysis;

[AttributeUsage(AttributeTargets.Parameter)]
internal class NotNullAttribute : Attribute {
    public NotNullAttribute() { }
}

[AttributeUsage(AttributeTargets.Parameter)]  
internal class DoesNotReturnIfAttribute : Attribute {
    public DoesNotReturnIfAttribute(bool parameterValue) { }
}

#endif