// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Runtime.InteropServices;
using TerraFX.Advanced;

namespace TerraFX.Graphics.Advanced;

/// <summary>Defines a function which creates a graphics memory allocator.</summary>
/// <remarks>Initializes a new instance of the <see cref="GraphicsMemoryAllocatorCreateFunc" /> struct.</remarks>
/// <param name="value">A pointer to the function that will be called in <see cref="Invoke" />.</param>
[StructLayout(LayoutKind.Auto)]
public readonly unsafe struct GraphicsMemoryAllocatorCreateFunc(delegate*<GraphicsDeviceObject, in GraphicsMemoryAllocatorCreateOptions, GraphicsMemoryAllocator> value)
{
    private readonly delegate*<GraphicsDeviceObject, in GraphicsMemoryAllocatorCreateOptions, GraphicsMemoryAllocator> _value = value;

    /// <summary>Gets <c>true</c> if the backing function pointer is not null; otherwise, <c>false</c>.</summary>
    public bool IsNotNull => _value is not null;

    /// <summary>Gets <c>true</c> if the backing function pointer is null; otherwise, <c>false</c>.</summary>
    public bool IsNull => _value is null;

    /// <summary>Creates a new graphics memory allocator.</summary>
    /// <param name="deviceObject">The device object for which the allocator is managing memory.</param>
    /// <param name="createOptions">The options to use when creating the memory allocator.</param>
    /// <returns>A new graphics memory allocator.</returns>
    /// <exception cref="ArgumentNullException"><paramref name="deviceObject" /> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException"><see cref="GraphicsMemoryAllocatorCreateOptions.ByteLength" /> is <c>zero</c>.</exception>
    public GraphicsMemoryAllocator Invoke(GraphicsDeviceObject deviceObject, in GraphicsMemoryAllocatorCreateOptions createOptions)
        => _value(deviceObject, in createOptions);
}
