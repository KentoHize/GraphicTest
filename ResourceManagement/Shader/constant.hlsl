struct FrameVariables
{
    float4x4 transformMatrix;
};
ConstantBuffer<FrameVariables> fv : register(b0);

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
//Texture2D textureA : register(t0);
Texture2D textures[8] : register(t0);
SamplerState normal_sampler : register(s0);