using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using SharpDX;
using System.Runtime.InteropServices;
using EasyHook;
using System.Threading;
using System.IO;
using Capture.Interface;
using SharpDX.Direct3D;
using Device = SharpDX.Direct3D11.Device;
using MapFlags = SharpDX.Direct3D11.MapFlags;
using Rectangle = System.Drawing.Rectangle;
using Resource = SharpDX.Direct3D11.Resource;

namespace Capture.Hook
{
    enum D3D11DeviceVTbl : short
    {
        // IUnknown
        QueryInterface = 0,
        AddRef = 1,
        Release = 2,

        // ID3D11Device
        CreateBuffer = 3,
        CreateTexture1D = 4,
        CreateTexture2D = 5,
        CreateTexture3D = 6,
        CreateShaderResourceView = 7,
        CreateUnorderedAccessView = 8,
        CreateRenderTargetView = 9,
        CreateDepthStencilView = 10,
        CreateInputLayout = 11,
        CreateVertexShader = 12,
        CreateGeometryShader = 13,
        CreateGeometryShaderWithStreamOutput = 14,
        CreatePixelShader = 15,
        CreateHullShader = 16,
        CreateDomainShader = 17,
        CreateComputeShader = 18,
        CreateClassLinkage = 19,
        CreateBlendState = 20,
        CreateDepthStencilState = 21,
        CreateRasterizerState = 22,
        CreateSamplerState = 23,
        CreateQuery = 24,
        CreatePredicate = 25,
        CreateCounter = 26,
        CreateDeferredContext = 27,
        OpenSharedResource = 28,
        CheckFormatSupport = 29,
        CheckMultisampleQualityLevels = 30,
        CheckCounterInfo = 31,
        CheckCounter = 32,
        CheckFeatureSupport = 33,
        GetPrivateData = 34,
        SetPrivateData = 35,
        SetPrivateDataInterface = 36,
        GetFeatureLevel = 37,
        GetCreationFlags = 38,
        GetDeviceRemovedReason = 39,
        GetImmediateContext = 40,
        SetExceptionMode = 41,
        GetExceptionMode = 42,
    }

    /// <summary>
    /// Direct3D 11 Hook - this hooks the SwapChain.Present to take screenshots
    /// </summary>
    internal class DXHookD3D11: BaseDXHook
    {
        const int D3D11_DEVICE_METHOD_COUNT = 43;

        public DXHookD3D11(CaptureInterface ssInterface)
            : base(ssInterface)
        {
        }

        List<IntPtr> _d3d11VTblAddresses = null;
        List<IntPtr> _dxgiSwapChainVTblAddresses = null;

        LocalHook DXGISwapChain_PresentHook = null;
        LocalHook DXGISwapChain_ResizeTargetHook = null;

        protected override string HookName
        {
            get
            {
                return "DXHookD3D11";
            }
        }

        public override void Hook()
        {
            this.DebugMessage("Hook: Begin");
            if (_d3d11VTblAddresses == null)
            {
                _d3d11VTblAddresses = new List<IntPtr>();
                _dxgiSwapChainVTblAddresses = new List<IntPtr>();

                #region Get Device and SwapChain method addresses
                // Create temporary device + swapchain and determine method addresses
                SharpDX.Direct3D11.Device device;
                SwapChain swapChain;
                using (SharpDX.Windows.RenderForm renderForm = new SharpDX.Windows.RenderForm())
                {
                    this.DebugMessage("Hook: Before device creation");
                    SharpDX.Direct3D11.Device.CreateWithSwapChain(
                        DriverType.Hardware,
                        DeviceCreationFlags.None,
                        DXGI.CreateSwapChainDescription(renderForm.Handle),
                        out device,
                        out swapChain);

                    if (device != null && swapChain != null)
                    {
                        this.DebugMessage("Hook: Device created");
                        using (device)
                        {
                            _d3d11VTblAddresses.AddRange(GetVTblAddresses(device.NativePointer, D3D11_DEVICE_METHOD_COUNT));

                            using (swapChain)
                            {
                                _dxgiSwapChainVTblAddresses.AddRange(GetVTblAddresses(swapChain.NativePointer, DXGI.DXGI_SWAPCHAIN_METHOD_COUNT));
                            }
                        }
                    }
                    else
                    {
                        this.DebugMessage("Hook: Device creation failed");
                    }
                }
                #endregion
            }

            // We will capture the backbuffer here
            DXGISwapChain_PresentHook = LocalHook.Create(
                _dxgiSwapChainVTblAddresses[(int)DXGI.DXGISwapChainVTbl.Present],
                new DXGISwapChain_PresentDelegate(PresentHook),
                this);
            
            // We will capture target/window resizes here
            DXGISwapChain_ResizeTargetHook = LocalHook.Create(
                _dxgiSwapChainVTblAddresses[(int)DXGI.DXGISwapChainVTbl.ResizeTarget],
                new DXGISwapChain_ResizeTargetDelegate(ResizeTargetHook),
                this);

            /*
             * Don't forget that all hooks will start deactivated...
             * The following ensures that all threads are intercepted:
             * Note: you must do this for each hook.
             */
            DXGISwapChain_PresentHook.ThreadACL.SetExclusiveACL(new Int32[1]);

            DXGISwapChain_ResizeTargetHook.ThreadACL.SetExclusiveACL(new Int32[1]);

            Hooks.Add(DXGISwapChain_PresentHook);
            Hooks.Add(DXGISwapChain_ResizeTargetHook);
        }

        public override void Cleanup()
        {
            try
            {
                if (DXGISwapChain_PresentHook != null)
                {
                    DXGISwapChain_PresentHook.Dispose();
                    DXGISwapChain_PresentHook = null;
                }
                if (DXGISwapChain_ResizeTargetHook != null)
                {
                    DXGISwapChain_ResizeTargetHook.Dispose();
                    DXGISwapChain_ResizeTargetHook = null;
                }
                
                if (_overlayEngine != null)
                {
                    _overlayEngine.Dispose();
                    _overlayEngine = null;
                }

                this.Request = null;
            }
            catch
            {
            }
        }

        /// <summary>
        /// The IDXGISwapChain.Present function definition
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate int DXGISwapChain_PresentDelegate(IntPtr swapChainPtr, int syncInterval, /* int */ SharpDX.DXGI.PresentFlags flags);

        /// <summary>
        /// The IDXGISwapChain.ResizeTarget function definition
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        [UnmanagedFunctionPointer(CallingConvention.StdCall, CharSet = CharSet.Unicode, SetLastError = true)]
        delegate int DXGISwapChain_ResizeTargetDelegate(IntPtr swapChainPtr, ref ModeDescription newTargetParameters);

        /// <summary>
        /// Hooked to allow resizing a texture/surface that is reused. Currently not in use as we create the texture for each request
        /// to support different sizes each time (as we use DirectX to copy only the region we are after rather than the entire backbuffer)
        /// </summary>
        /// <param name="swapChainPtr"></param>
        /// <param name="newTargetParameters"></param>
        /// <returns></returns>
        int ResizeTargetHook(IntPtr swapChainPtr, ref ModeDescription newTargetParameters)
        {
            SwapChain swapChain = (SharpDX.DXGI.SwapChain)swapChainPtr;
            //using (SharpDX.DXGI.SwapChain swapChain = SharpDX.DXGI.SwapChain.FromPointer(swapChainPtr))
            {
                // This version creates a new texture for each request so there is nothing to resize.
                // IF the size of the texture is known each time, we could create it once, and then possibly need to resize it here

                // Dispose of overlay engine (so it will be recreated)
                if (_overlayEngine != null)
                {
                    _overlayEngine.Dispose();
                    _overlayEngine = null;
                }

                swapChain.ResizeTarget(ref newTargetParameters);
                return SharpDX.Result.Ok.Code;
            }
        }

        /// <summary>
        /// Our present hook that will grab a copy of the backbuffer when requested. Note: this supports multi-sampling (anti-aliasing)
        /// </summary>
        /// <param name="swapChainPtr"></param>
        /// <param name="syncInterval"></param>
        /// <param name="flags"></param>
        /// <returns>The HRESULT of the original method</returns>
        int PresentHook(IntPtr swapChainPtr, int syncInterval, SharpDX.DXGI.PresentFlags flags)
        {
            this.Frame();
            SwapChain swapChain = (SharpDX.DXGI.SwapChain)swapChainPtr;
            try
            {
                #region Screenshot Request
                if (this.Request != null)
                {
                    //this.DebugMessage("PresentHook: Request Start");
                    Device device;
                    Texture2D textureDest;
                    using (Texture2D texture = Resource.FromSwapChain<Texture2D>(swapChain, 0))
                    {
                        device = texture.Device;

                        // Create destination texture
                        textureDest = new Texture2D(texture.Device, new Texture2DDescription()
                        {
                            CpuAccessFlags = CpuAccessFlags.Read,// CpuAccessFlags.Write | CpuAccessFlags.Read,
                            Format = SharpDX.DXGI.Format.B8G8R8A8_UNorm, // Supports BMP/PNG
                            Height = texture.Description.Height,
                            Usage = ResourceUsage.Staging,// ResourceUsage.Staging,
                            Width = texture.Description.Width,
                            ArraySize = 1,//texture.Description.ArraySize,
                            SampleDescription = new SharpDX.DXGI.SampleDescription(1, 0),// texture.Description.SampleDescription,
                            BindFlags = BindFlags.None,
                            MipLevels = 1,//texture.Description.MipLevels,
                            OptionFlags = ResourceOptionFlags.None
                        });

                        device.ImmediateContext.CopyResource(texture, textureDest);
                    }

                    DataBox mapSource = device.ImmediateContext.MapSubresource(textureDest, 0, MapMode.Read, MapFlags.None);

                    // Copy to memory and send back to host process on a background thread so that we do not cause any delay in the rendering pipeline
                    Guid requestId = this.Request.RequestId; // this.Request gets set to null, so copy the RequestId for use in the thread
                    ThreadPool.QueueUserWorkItem(delegate
                    {
                        using (MemoryStream ms = new MemoryStream())
                        {
                            // Create Drawing.Bitmap
                            Bitmap bitmap = new Bitmap(textureDest.Description.Width, textureDest.Description.Height, PixelFormat.Format32bppArgb);
                            Rectangle boundsRect = new Rectangle(0, 0, textureDest.Description.Width, textureDest.Description.Height);

                            // Copy pixels from screen capture Texture to GDI bitmap
                            BitmapData mapDest = bitmap.LockBits(boundsRect, ImageLockMode.WriteOnly, bitmap.PixelFormat);
                            IntPtr sourcePtr = mapSource.DataPointer;
                            IntPtr destPtr = mapDest.Scan0;
                            for (int y = 0; y < textureDest.Description.Height; y++)
                            {
                                // Copy a single line 
                                Utilities.CopyMemory(destPtr, sourcePtr, textureDest.Description.Width * 4);

                                // Advance pointers
                                sourcePtr = IntPtr.Add(sourcePtr, mapSource.RowPitch);
                                destPtr = IntPtr.Add(destPtr, mapDest.Stride);
                            }

                            // Release source and dest locks
                            bitmap.UnlockBits(mapDest);
                            device.ImmediateContext.UnmapSubresource(textureDest, 0);

                            bitmap.Save(ms, ImageFormat.Bmp);

                            ProcessCapture(ms, requestId);
                        }

                        // Free the textureDest as we no longer need it.
                        textureDest.Dispose();
                        textureDest = null;
                    });

                    // Prevent the request from being processed a second time
                    this.Request = null;
                }
                
                #endregion

                #region Draw overlay (after screenshot so we don't capture overlay as well)
                if (this.Config.ShowOverlay)
                {
                    // Initialise Overlay Engine
                    if (_swapChainPointer != swapChain.NativePointer || _overlayEngine == null)
                    {
                        if (_overlayEngine != null)
                            _overlayEngine.Dispose();

                        _overlayEngine = new DX11.DXOverlayEngine();
                        _overlayEngine.Overlays.Add(new Capture.Hook.Common.Overlay
                        {
                            Elements =
                            {
                                //new Capture.Hook.Common.TextElement(new System.Drawing.Font("Times New Roman", 22)) { Text = "Test", Location = new System.Drawing.Point(200, 200), Color = System.Drawing.Color.Yellow, AntiAliased = false},
                                new Capture.Hook.Common.FramesPerSecond(new System.Drawing.Font("Arial", 16)) { Location = new System.Drawing.Point(5,5), Color = System.Drawing.Color.Red, AntiAliased = true }
                            }
                        });
                        _overlayEngine.Initialise(swapChain);

                        _swapChainPointer = swapChain.NativePointer;
                    }
                    // Draw Overlay(s)
                    else if (_overlayEngine != null)
                    {
                        foreach (var overlay in _overlayEngine.Overlays)
                            overlay.Frame();
                        _overlayEngine.Draw();
                    }
                }
                #endregion
            }
            catch (Exception e)
            {
                // If there is an error we do not want to crash the hooked application, so swallow the exception
                this.DebugMessage("PresentHook: Exeception: " + e.GetType().FullName + ": " + e.ToString());
                //return unchecked((int)0x8000FFFF); //E_UNEXPECTED
            }

            // As always we need to call the original method, note that EasyHook has already repatched the original method
            // so calling it here will not cause an endless recursion to this function
            swapChain.Present(syncInterval, flags);
            return SharpDX.Result.Ok.Code;
        }

        Capture.Hook.DX11.DXOverlayEngine _overlayEngine;

        IntPtr _swapChainPointer = IntPtr.Zero;

        private static int _fileNameCounter = 1;
        private static string GetNextFilename(string path)
        {
            while (File.Exists(Path.Combine(path, string.Format("{0}.bmp", _fileNameCounter))))
            {
                ++_fileNameCounter;
            }

            return Path.Combine(path, string.Format("{0}.bmp", _fileNameCounter));
        }
    }
}
