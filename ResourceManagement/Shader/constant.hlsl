struct FrameVariables
{
    int3 translateVector;
    float3 rotateVector;
    float3x3 rotateMatrix;
    float scale;
};
ConstantBuffer<FrameVariables> fv : register(b0);

Texture2D textures[8] : register(t0);
SamplerState normal_sampler : register(s0);