using GraphicLibrary.Items;

namespace GraphicLibrary
{
    public class SharpDXData
    {
        public SharpDXBundleData[] VerticesData { get; set; }
        public ArFloatVector4 BackgroundColor { get; set; }
        public ArFloatMatrix44 TransformMartrix { get; set; }
    }
}
