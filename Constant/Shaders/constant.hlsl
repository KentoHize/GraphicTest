struct SomeConstant
{
    int a;
    int b;
};
ConstantBuffer<SomeConstant> sc : register(b0);

struct AnotherConstant
{
    int c;
    int d;
};
ConstantBuffer<AnotherConstant> ac : register(b1);

Texture2D fontTexture : register(t0);
SamplerState normal_sampler : register(s0);