// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Reflection;

#pragma warning disable CA1062 // Validate arguments of public methods

namespace TerraFX.Graphics.Advanced;

/// <summary>An object which is created for a graphics device.</summary>
public abstract class GraphicsDeviceObject : GraphicsAdapterObject
{
    private readonly GraphicsDevice _device;

    /// <summary>Initializes a new instance of the <see cref="GraphicsDeviceObject" /> class.</summary>
    /// <param name="device">The device for which the object is being created.</param>
    /// <param name="name">The name of the object or <c>null</c> to use <see cref="MemberInfo.Name" />.</param>
    /// <exception cref="ArgumentNullException"><paramref name="device" /> is <c>null</c>.</exception>
    protected GraphicsDeviceObject(GraphicsDevice device, string? name = null) : base(device.Adapter, name)
    {
        _device = device;
    }

    /// <summary>Gets the device for which the object was created.</summary>
    public GraphicsDevice Device => _device;
}
