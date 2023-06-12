using SharpDX.Direct3D12;
using Device11 = SharpDX.Direct3D11.Device;

namespace D3D11on12.Other
{
    public class OtherClass
    {
        public OtherClass()
        {
            //SharpDX.Direct3D11.Device11On12
            Device d3d12 = new Device(null, SharpDX.Direct3D.FeatureLevel.Level_11_0);
            Device11 device11 = Device11.CreateFromDirect3D12(d3d12, SharpDX.Direct3D11.DeviceCreationFlags.BgraSupport, null, null);
        }
    }
}
