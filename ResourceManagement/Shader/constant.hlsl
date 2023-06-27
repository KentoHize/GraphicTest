struct FrameVariables
{
    int3 translateVector;
    float3 rotateVector;
    float3x3 rotateMatrix;
    float scale;
};
ConstantBuffer<FrameVariables> fv : register(b0);

struct DebugInfo
{   
    float3 pos;
    float2 uv;
};
RWStructuredBuffer<DebugInfo> di : register(u0);
//RWByteAddressBuffer dii : register(u0);

//RWByteAddressBuffer di : register(u1);


Texture2D textures[8] : register(t0);
SamplerState normal_sampler : register(s0);