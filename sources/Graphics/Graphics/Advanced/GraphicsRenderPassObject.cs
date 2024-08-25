// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using System.Reflection;

namespace TerraFX.Graphics.Advanced;

/// <summary>An object which is created for a graphics render pass.</summary>
/// <remarks>Initializes a new instance of the <see cref="GraphicsRenderPassObject" /> class.</remarks>
/// <param name="renderPass">The render pass for which the object is being created.</param>
/// <param name="name">The name of the object or <c>null</c> to use <see cref="MemberInfo.Name" />.</param>
/// <exception cref="ArgumentNullException"><paramref name="renderPass" /> is <c>null</c>.</exception>
public abstract class GraphicsRenderPassObject(GraphicsRenderPass renderPass, string? name = null) : GraphicsDeviceObject(renderPass.Device, name)
{
    private readonly GraphicsRenderPass _renderPass = renderPass;

    /// <summary>Gets the render pass for which the object was created.</summary>
    public GraphicsRenderPass RenderPass => _renderPass;
}
