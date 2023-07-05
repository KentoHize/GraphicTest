struct TranformMatrix
{
    float4x4 transformMatrix;
};
ConstantBuffer<TranformMatrix> tm : register(b0);

struct DrawingSetting
{
    int textureIndex;
};
ConstantBuffer<DrawingSetting> ds : register(b1);

Texture2D annetteTexture : register(t0);
SamplerState normal_sampler : register(s0);