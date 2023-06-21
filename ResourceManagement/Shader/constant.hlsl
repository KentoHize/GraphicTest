struct FrameVariables
{
    float4x4 transformMatrix;
};
ConstantBuffer<FrameVariables> tm : register(b0);

//struct DrawingSetting
//{
//    int textureIndex;
//};
//ConstantBuffer<DrawingSetting> ds : register(b1);

//struct Common
//{
//    int2 ResourceTable[];
//};
//ConstantBuffer<Common> cm : register(b0);
Texture2D textures[10] : register(t0);
SamplerState normal_sampler : register(s0);