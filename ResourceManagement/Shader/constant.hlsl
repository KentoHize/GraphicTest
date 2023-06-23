struct FrameVariables
{
    int3 translateVector;
    float3 rotateVector;
    float scale;
};
ConstantBuffer<FrameVariables> fv : register(b0);
//struct FrameVariables
//{
//    float4 t;
//    //int t1;
//    //int t2;
//    //int t3;
//    float r1;
//    float r2;
//    float r3;
//    float s;
//};
//ConstantBuffer<FrameVariables> fv : register(b0);


//float4x4 transformMatrix;
//struct DrawingSetting
//{
//    int textureIndex;
//};
//ConstantBuffer<DrawingSetting> ds : register(b1);

Texture2D textures[8] : register(t0);
SamplerState normal_sampler : register(s0);