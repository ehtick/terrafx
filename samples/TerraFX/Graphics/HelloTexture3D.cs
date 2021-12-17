// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.

using System;
using TerraFX.ApplicationModel;
using TerraFX.Graphics;
using TerraFX.Numerics;
using static TerraFX.Utilities.UnsafeUtilities;

namespace TerraFX.Samples.Graphics;

/// <summary>
/// Demonstrates the use of
/// * Quads (Vertex buffer + Index Buffer, see HelloQuad)
/// * Texture3D (256x256x256, representing the RGB cube, extension of HelloTexture)
/// * ConstBuffer (transformation matrix as in HelloConstBuffer, but here to animate the 3D texture coordinates)
/// Will show a quad cutting through the RGB cube and being animated to move back and forth in texture coordinate space.
/// </summary>
public class HelloTexture3D : HelloWindow
{
    private GraphicsBuffer _constantBuffer = null!;
    private GraphicsBuffer _indexBuffer = null!;
    private GraphicsPrimitive _quadPrimitive = null!;
    private GraphicsTexture _texture3D = null!;
    private GraphicsBuffer _uploadBuffer = null!;
    private GraphicsBuffer _vertexBuffer = null!;
    private float _texturePosition;

    public HelloTexture3D(string name, ApplicationServiceProvider serviceProvider)
        : base(name, serviceProvider)
    {
    }

    public override void Cleanup()
    {
        _quadPrimitive?.Dispose();

        _constantBuffer?.Dispose();
        _indexBuffer?.Dispose();
        _texture3D?.Dispose();
        _uploadBuffer?.Dispose();
        _vertexBuffer?.Dispose();

        base.Cleanup();
    }

    /// <summary>Initializes the GUI for this sample.</summary>
    /// <param name="application">The hosting <see cref="Application" />.</param>
    /// <param name="timeout">The <see cref="TimeSpan" /> after which this sample should stop running.</param>
    /// <param name="windowLocation">The <see cref="Vector2" /> that defines the initial window location.</param>
    /// <param name="windowSize">The <see cref="Vector2" /> that defines the initial window client rectangle size.</param>
    public override void Initialize(Application application, TimeSpan timeout, Vector2? windowLocation, Vector2? windowSize)
    {
        base.Initialize(application, timeout, windowLocation, windowSize);

        var graphicsDevice = GraphicsDevice;

        _constantBuffer = graphicsDevice.CreateConstantBuffer(64 * 1024, GraphicsResourceCpuAccess.Write);
        _indexBuffer = graphicsDevice.CreateIndexBuffer(64 * 1024);
        _texture3D = graphicsDevice.CreateTexture3D(GraphicsFormat.R8G8B8A8_UNORM, 64, 64, 64);
        _uploadBuffer = graphicsDevice.CreateUploadBuffer(2 * 1024 * 1024);
        _vertexBuffer = graphicsDevice.CreateVertexBuffer(64 * 1024);

        var graphicsCopyContext = graphicsDevice.RentCopyContext();
        {
            graphicsCopyContext.Reset();
            _quadPrimitive = CreateQuadPrimitive(graphicsCopyContext);

            graphicsCopyContext.Flush();
            graphicsDevice.WaitForIdle();
        }
        graphicsDevice.ReturnContext(graphicsCopyContext);

        _uploadBuffer.DisposeAllViews();
    }

    protected override unsafe void Update(TimeSpan delta)
    {
        var graphicsSurface = GraphicsRenderPass.Surface;
        var scale255_256 = 255f / 256f;
        var aspectRatio = graphicsSurface.Width / graphicsSurface.Height;
        var scaleX = scale255_256;
        var scaleY = scale255_256 / aspectRatio;
        var scaleZ = scale255_256;

        const float TranslationSpeed = MathF.PI;

        var radians = _texturePosition;
        {
            radians += (float)(TranslationSpeed * delta.TotalSeconds);
            radians %= MathF.Tau;
        }
        _texturePosition = radians;
        var z = scaleZ * (0.5f + (0.5f * MathF.Cos(radians)));

        var constantBufferView = _quadPrimitive.InputResourceViews[1].As<GraphicsBufferView>();
        var constantBufferSpan = constantBufferView.Map<Matrix4x4>();
        {
            // Shaders take transposed matrices, so we want to set X.W
            constantBufferSpan[0] = Matrix4x4.Create(
                Vector4.Create(scaleX, 0.0f, 0.0f, 0.5f), // +0.5 since the input coordinates are in range [-.5, .5]  but output needs to be [0, 1]
                Vector4.Create(0.0f, scaleY, 0.0f, 0.5f), // +0.5 since the input coordinates are in range [-.5, .5]  but output needs to be [0, 1]
                Vector4.Create(0.0f, 0.0f, 1.0f, z),
                Vector4.UnitW
            );
        }
        constantBufferView.UnmapAndWrite();
    }

    protected override void Draw(GraphicsRenderContext graphicsRenderContext)
    {
        graphicsRenderContext.Draw(_quadPrimitive);
        base.Draw(graphicsRenderContext);
    }

    private unsafe GraphicsPrimitive CreateQuadPrimitive(GraphicsCopyContext graphicsCopyContext)
    {
        var graphicsRenderPass = GraphicsRenderPass;
        var graphicsSurface = graphicsRenderPass.Surface;

        var graphicsPipeline = CreateGraphicsPipeline(graphicsRenderPass, "Texture3D", "main", "main");

        var constantBuffer = _constantBuffer;
        var uploadBuffer = _uploadBuffer;

        return GraphicsDevice.CreatePrimitive(
            graphicsPipeline,
            CreateVertexBufferView(graphicsCopyContext, _vertexBuffer, uploadBuffer, aspectRatio: graphicsSurface.Width / graphicsSurface.Height),
            CreateIndexBufferView(graphicsCopyContext, _indexBuffer, uploadBuffer),
            new GraphicsResourceView[3] {
                CreateConstantBufferView(graphicsCopyContext, constantBuffer),
                CreateConstantBufferView(graphicsCopyContext, constantBuffer),
                CreateTexture3DView(graphicsCopyContext, _texture3D, uploadBuffer),
            }
        );

        static GraphicsBufferView CreateConstantBufferView(GraphicsCopyContext graphicsCopyContext, GraphicsBuffer constantBuffer)
        {
            var constantBufferView = constantBuffer.CreateView<Matrix4x4>(1);
            var constantBufferSpan = constantBufferView.Map<Matrix4x4>();
            {
                constantBufferSpan[0] = Matrix4x4.Identity;
            }
            constantBufferView.UnmapAndWrite();
            return constantBufferView;
        }

        static GraphicsBufferView CreateIndexBufferView(GraphicsCopyContext graphicsCopyContext, GraphicsBuffer indexBuffer, GraphicsBuffer uploadBuffer)
        {
            var uploadBufferView = uploadBuffer.CreateView<ushort>(6);
            var indexBufferSpan = uploadBufferView.Map<ushort>();
            {
                // clockwise when looking at the triangle from the outside

                indexBufferSpan[0] = 0;
                indexBufferSpan[1] = 1;
                indexBufferSpan[2] = 2;

                indexBufferSpan[3] = 0;
                indexBufferSpan[4] = 2;
                indexBufferSpan[5] = 3;
            }
            uploadBufferView.UnmapAndWrite();

            var indexBufferView = indexBuffer.CreateView<ushort>(6);
            graphicsCopyContext.Copy(indexBufferView, uploadBufferView);
            return indexBufferView;
        }

        static GraphicsTextureView CreateTexture3DView(GraphicsCopyContext graphicsCopyContext, GraphicsTexture texture3D, GraphicsBuffer uploadBuffer)
        {
            var uploadBufferView = uploadBuffer.CreateView<byte>(checked((uint)texture3D.Size));
            var textureDataSpan = uploadBufferView.Map<byte>();
            {
                var width = texture3D.Width;

                var height = texture3D.Height;
                var rowPitch = texture3D.RowPitch;

                var depth = texture3D.Depth;
                var slicePitch = texture3D.SlicePitch;

                for (var z = 0u; z < depth; z++)
                {
                    var sliceIndex = z * slicePitch;

                    for (var y = 0u; y < height; y++)
                    {
                        var rowIndex = sliceIndex + (y * rowPitch);
                        var row = (uint*)textureDataSpan.GetPointer(rowIndex);

                        for (var x = 0u; x < width; x++)
                        {
                            var red = x * 256u / width;
                            var blue = y * 256u / height;
                            var green = z * 256u / depth;
                            var alpha = 0xFFu;

                            row[x] = (alpha << 24) | (green << 16) | (blue << 8) | (red << 0);
                        }
                    }
                }
            }
            uploadBufferView.UnmapAndWrite();

            var texture3DView = texture3D.CreateView(0, 1);
            graphicsCopyContext.Copy(texture3DView, uploadBufferView);
            return texture3DView;
        }

        static GraphicsBufferView CreateVertexBufferView(GraphicsCopyContext graphicsCopyContext, GraphicsBuffer vertexBuffer, GraphicsBuffer uploadBuffer, float aspectRatio)
        {
            var uploadBufferView = uploadBuffer.CreateView<Texture3DVertex>(4);
            var vertexBufferSpan = uploadBufferView.Map<Texture3DVertex>();
            {
                vertexBufferSpan[0] = new Texture3DVertex {                //
                    Position = Vector3.Create(-0.5f, 0.5f, 0.0f),       //   y          in this setup
                    UVW = Vector3.Create(0, 1, 0.5f),                   //   ^     z    the origin o
                };                                                      //   |   /      is in the middle
                                                                        //   | /        of the rendered scene
                vertexBufferSpan[1] = new Texture3DVertex {                //   o------>x
                    Position = Vector3.Create(0.5f, 0.5f, 0.0f),        //
                    UVW = Vector3.Create(1, 1, 0.5f),                   //   0 ----- 1
                };                                                      //   | \     |
                                                                        //   |   \   |
                vertexBufferSpan[2] = new Texture3DVertex {                //   |     \ |
                    Position = Vector3.Create(0.5f, -0.5f, 0.0f),       //   3-------2
                    UVW = Vector3.Create(1, 0, 0.5f),                   //
                };

                vertexBufferSpan[3] = new Texture3DVertex {
                    Position = Vector3.Create(-0.5f, -0.5f, 0.0f),
                    UVW = Vector3.Create(0, 0, 0.5f),
                };
            }
            uploadBufferView.UnmapAndWrite();

            var vertexBufferView = vertexBuffer.CreateView<Texture3DVertex>(4);
            graphicsCopyContext.Copy(vertexBufferView, uploadBufferView);
            return vertexBufferView;
        }

        GraphicsPipeline CreateGraphicsPipeline(GraphicsRenderPass graphicsRenderPass, string shaderName, string vertexShaderEntryPoint, string pixelShaderEntryPoint)
        {
            var graphicsDevice = graphicsRenderPass.Device;

            var signature = CreateGraphicsPipelineSignature(graphicsDevice);
            var vertexShader = CompileShader(graphicsDevice, GraphicsShaderKind.Vertex, shaderName, vertexShaderEntryPoint);
            var pixelShader = CompileShader(graphicsDevice, GraphicsShaderKind.Pixel, shaderName, pixelShaderEntryPoint);

            return graphicsRenderPass.CreatePipeline(signature, vertexShader, pixelShader);
        }

        static GraphicsPipelineSignature CreateGraphicsPipelineSignature(GraphicsDevice graphicsDevice)
        {
            var inputs = new GraphicsPipelineInput[1] {
                new GraphicsPipelineInput(
                    new GraphicsPipelineInputElement[2] {
                        new GraphicsPipelineInputElement(GraphicsPipelineInputElementKind.Position, GraphicsFormat.R32G32B32_SFLOAT, size: 12, alignment: 4),
                        new GraphicsPipelineInputElement(GraphicsPipelineInputElementKind.TextureCoordinate, GraphicsFormat.R32G32B32_SFLOAT, size: 12, alignment: 4),
                    }
                ),
            };

            var resources = new GraphicsPipelineResource[3] {
                new GraphicsPipelineResource(GraphicsPipelineResourceKind.ConstantBuffer, GraphicsShaderVisibility.Vertex),
                new GraphicsPipelineResource(GraphicsPipelineResourceKind.ConstantBuffer, GraphicsShaderVisibility.Vertex),
                new GraphicsPipelineResource(GraphicsPipelineResourceKind.Texture, GraphicsShaderVisibility.Pixel),
            };

            return graphicsDevice.CreatePipelineSignature(inputs, resources);
        }
    }
}
