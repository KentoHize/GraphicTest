namespace GraphicLibrary2.Items
{
    public class SharpDXInitializeSetting
    {
        public int AdapterIndex { get; set; } // -1 = default
        public DirectX12FeatureLevel FeatureLevel { get; set; }
        public IntPtr OutputHandle { get; set; }
        public int BufferCount { get; set; }
        public int ScreenWidth {get; set;}
        public int ScreenHeight { get; set; }
        public bool IsWindowed { get; set; }
        public int RefreshRate { get; set; }
        public int SampleCount { get; set; }
        public int SampleQuality { get; set; }
        public SharpDXInitializeSetting(IntPtr outputHandle, int screenWidth, int screenHeight, bool isWindowed = true, int adapterIndex = -1, DirectX12FeatureLevel featureLevel = DirectX12FeatureLevel.Level_12_1, int bufferCount = 2, int refreshRate = 60, int sampleCount = 1, int sampleQuality = 0)
        {
            AdapterIndex = adapterIndex;
            FeatureLevel = featureLevel;
            BufferCount = bufferCount;
            OutputHandle = outputHandle;
            ScreenWidth = screenWidth;
            ScreenHeight = screenHeight;
            IsWindowed = isWindowed;
            RefreshRate = refreshRate;
            SampleCount = sampleCount;
            SampleQuality = sampleQuality;            
        } 
    }
}
