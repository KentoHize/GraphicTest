using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GraphicLibrary;
using GraphicLibrary.Items;
using SharpDX.Direct3D12;
using Device11 = SharpDX.Direct3D11.Device;

namespace WriteText
{
    public class SharpDXEngine
    {
        Device device;
        public SharpDXEngine()
        {
#if DEBUG
            DebugInterface.Get().EnableDebugLayer();
#endif 
            device = new Device(null, SharpDX.Direct3D.FeatureLevel.Level_11_0);
            Device11 device11 = Device11.CreateFromDirect3D12(device, SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport, null, null);
        }
    }
}
