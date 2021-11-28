// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using static TerraFX.Threading.VolatileState;

namespace TerraFX.Graphics;

/// <inheritdoc />
public sealed unsafe class D3D12GraphicsBuffer<TMetadata> : D3D12GraphicsBuffer
    where TMetadata : struct, IGraphicsMemoryRegionCollection<GraphicsResource>.IMetadata
{
#pragma warning disable IDE0044
    private TMetadata _metadata;
#pragma warning restore IDE0044

    internal D3D12GraphicsBuffer(D3D12GraphicsDevice device, GraphicsBufferKind kind, in GraphicsMemoryRegion<GraphicsMemoryHeap> heapRegion, GraphicsResourceCpuAccess cpuAccess)
        : base(device, kind, in heapRegion, cpuAccess)
    {
        var heap = heapRegion.Collection;

        var minimumAllocatedRegionMarginSize = heap.MinimumAllocatedRegionMarginSize;
        var minimumFreeRegionSizeToRegister = heap.MinimumFreeRegionSizeToRegister;

        _metadata = new TMetadata();
        _metadata.Initialize(this, heapRegion.Size, minimumAllocatedRegionMarginSize, minimumFreeRegionSizeToRegister);

        _ = _state.Transition(to: Initialized);
    }

    /// <inheritdoc />
    public override int AllocatedRegionCount
        => _metadata.AllocatedRegionCount;

    /// <inheritdoc />
    public override int Count
        => _metadata.Count;

    /// <inheritdoc />
    public override bool IsEmpty
        => _metadata.IsEmpty;

    /// <inheritdoc />
    public override ulong LargestFreeRegionSize
        => _metadata.LargestFreeRegionSize;

    /// <inheritdoc />
    public override ulong MinimumFreeRegionSizeToRegister
        => _metadata.MinimumFreeRegionSizeToRegister;

    /// <inheritdoc />
    public override ulong MinimumAllocatedRegionMarginSize
        => _metadata.MinimumAllocatedRegionMarginSize;

    /// <inheritdoc />
    public override ulong TotalFreeRegionSize
        => _metadata.TotalFreeRegionSize;

    /// <inheritdoc />
    public override GraphicsMemoryRegion<GraphicsResource> Allocate(ulong size, ulong alignment = 1)
        => _metadata.Allocate(size, alignment);

    /// <inheritdoc />
    public override void Clear()
        => _metadata.Clear();

    /// <inheritdoc />
    public override void Free(in GraphicsMemoryRegion<GraphicsResource> region)
        => _metadata.Free(in region);

    /// <inheritdoc />
    public override IEnumerator<GraphicsMemoryRegion<GraphicsResource>> GetEnumerator()
        => _metadata.GetEnumerator();

    /// <inheritdoc />
    public override bool TryAllocate(ulong size, [Optional, DefaultParameterValue(1UL)] ulong alignment, out GraphicsMemoryRegion<GraphicsResource> region)
        => _metadata.TryAllocate(size, alignment, out region);
}
