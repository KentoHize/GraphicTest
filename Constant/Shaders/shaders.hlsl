﻿
//struct TranformMatrix
//{
//    float4x4 transformMatrix;
//};
//ConstantBuffer<TranformMatrix> tm : register(b0);

//struct DrawingSetting
//{
//    int textureIndex;
//};
//ConstantBuffer<DrawingSetting> ds : register(b1);

//Texture2D t2d_Annette : register(t0);



#include "constant.hlsl"

struct PSInput
{
	float4 position : SV_POSITION;
    float2 uv : TEXCOORD;
};

PSInput VSMain(int3 position : POSITION, float2 uv : TEXCOORD)
{
	PSInput result;   
    int4 p = int4(position[0], position[1], position[2], 1);
    float4 p2;
    float4x4 normalTF = float4x4(0.001, 0, 0, 0, 0, 0.001, 0, 0, 0, 0, 0.001, 0, 0, 0, 0, 1);
    p2 = mul(p, normalTF);
    //p2 = mul(p, tm.transformMatrix);
    result.position = p2;    
    //result.position = float4(position[0], position[1], position[2], 1);
    result.uv = uv;
	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
    return fontTexture.Sample(normal_sampler, input.uv);
    //if(ac.c > 1)
        
    //else
    //    return float4((float) sc.a / 255, (float) sc.b / 255, (float) ac.c / 255, (float) ac.d);
    //return float4(input.uv[0], input.uv[1], 0, 1);
    //return float4(1, 0, 0, 1);
    //return float4((float) sc.a / 255, (float) sc.b / 255, 0, 1);
    //return float4((float) sc.a / 255, (float) sc.b / 255, 1, 1);
  
    
    //return float4((float) sc.a / 255, (float) sc.b / 255, 1, 1);
    
    
    
    //if(ds.textureIndex == 0)
    //    return t2d_Annette.Sample(normal_sampler, input.uv);
    //else
    //    return t2d_Clacier.Sample(normal_sampler, input.uv);
}