﻿struct TranformMatrix
{
    float4x4 transformMatrix;
};
ConstantBuffer<TranformMatrix> tm : register(b0);

struct DrawingSetting
{
    int textureIndex;
};
ConstantBuffer<DrawingSetting> ds : register(b1);


SamplerState normal_sampler : register(s0);

struct PSInput
{
	float4 position : SV_POSITION;        
    float2 uv : TEXCOORD;
};

//PSInput VSMain(int3 position : POSITION, float2 uv : TEXCOORD)
PSInput VSMain(int3 position : POSITION, float3 normal : NORMAL, float2 uv : TEXCOORD)
{
	PSInput result;   
    int4 p = int4(position[0], position[1], position[2], 1);
    float4 p2;
    float4x4 normalTF = float4x4(0.001, 0, 0, 0, 0, 0.001, 0, 0, 0, 0, 0.001, 0, 0, 0, 0, 1);
    p2 = mul(p, tm.transformMatrix);
    //p2 = mul(p, normalTF);
    result.position = p2;    
    result.uv = uv;
	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
    //return float4(1, 0, 0, 0);
    return float4((float)ds.textureIndex, 0, 0, 1);    
}