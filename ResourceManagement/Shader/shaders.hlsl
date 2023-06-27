#include "constant.hlsl"

struct PSInput
{
	float4 position : SV_POSITION;    
    uint texIndex : TEXINDEX;
    float2 tex : TEXCOORD;
    float3 shadow : SHADOWCOORD;
};

PSInput VSMain(int3 position : POSITION, uint texIndex : TEXINDEX, float2 tex : TEXCOORD, float3 shadow : SHADOWCOORD)
{
	PSInput result;   
    //int3 p = int4(position[0], position[1], position[2], 1);
    float3 p2;
    float4x4 normalTF = float4x4(0.001, 0, 0, 0, 0, 0.001, 0, 0, 0, 0, 0.001, 0, 0, 0, 0, 1);    
    //p2 = mul(p, fv.transformMatrix);
    //p2 = mul(p, normalTF);
    //p2 = float4(p.x, p.y, p.z, 1);
    
    //scaling
    if (fv.scale != 1)
        p2 = float3((float) position.x / 1024 * fv.scale, (float) position.y / 1024 * fv.scale, (float) position.z / 1024 * fv.scale);
    else    
        p2 = float3((float) position.x / 1024, (float) position.y / 1024, (float) position.z / 1024);
    
    //rotate

    float4x4 i = float4x4(1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
    float cosa = cos(fv.rotateVector.x);
    float sina = sin(fv.rotateVector.x);
    float4x4 r = float4x4(cosa, -sina, 0, 0, sina, cosa, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1);
    i = mul(i, r);
    cosa = cos(fv.rotateVector.y);
    sina = sin(fv.rotateVector.y);
    r = float4x4(cosa, 0, -sina, 0, 0, 1, 0, 0, sina, 0, cosa, 0, 0, 0, 0, 1);
    i = mul(i, r);
    cosa = cos(fv.rotateVector.z);
    sina = sin(fv.rotateVector.z);
    r = float4x4(1, 0, 0, 0, 0, cosa, -sina, 0, 0, sina, cosa, 0, 0, 0, 0, 1);
    i = mul(i, r);
    p2 = mul(p2, i);
    
    //rotate2
    //p2 = mul(fv.rotateMatrix, p2);
    
    //transform
    p2.x += (float) fv.translateVector.x / 1024;
    p2.y += (float) fv.translateVector.y / 1024;
    p2.z += (float) fv.translateVector.z / 1024;
    
    result.position = float4(p2.x, p2.y, p2.z, 1);
    result.shadow = shadow;
    result.tex = tex;
    result.texIndex = texIndex;
	return result;
}

uint CombineChar(uint char1, uint char2, uint char3, uint char4)
{
    return char1 + (char2 << 8) + (char3 << 16) + (char4 << 24);
}

float4 PSMain(PSInput input) : SV_TARGET
{
    //return float4(1, 0, 0, 1);
    //return float4(fv.scale, input.shadow[1], input.shadow[2], 1);
    //return float4(input.shadow[0], input.shadow[1], input.shadow[2], 1);
    //di.InterlockedCompareStore() = CombineChar('a', 'a', 'a', 'a');    
    //input.position.x * 
    //dii.Store(0, dii.Load(0) + 16);
    ////input.position.z;
    DebugInfo d;
    d.pos = input.position;
    d.uv = input.tex;
    di[d.pos.x * 100 + d.pos.y] = d;//.
    //di.Append(d);
    
    //di.Store(dii.Load(0) + 4, (int) input.position.y);
    //di.Store(dii.Load(0) + 8, (int) input.position.z);
    //di.Store(dii.Load(0) + 12, (int) input.texIndex);
    //di.Store(dii.Load(0), CombineChar(input.position.x, input.position.y, input.position.z, input.texIndex));
    if(input.texIndex == -1)
        return float4(1, 1, 1, 1);
    else
    //return textureA.Sample(normal_sampler, input.tex);
    return textures[input.texIndex].Sample(normal_sampler, input.tex);
    //return float4(input.tex[0], input.tex[1], 0, 1);
}



//(cos(fv.rotateVector.x + fv.rotateVector.y) + cos(fv.rotateVector.x - fv.rotateVector.y)) / 2
    //(cos(fv.rotateVector.x + fv.rotateVector.y + fv.rotateVector.z) - cos(fv.rotateVector.x - fv.rotateVector.y + fv.rotateVector.z) - cos(fv.rotateVector.x + fv.rotateVector.y - fv.rotateVector.z) + cos(fv.rotateVector.x - fv.rotateVector.y - fv.rotateVector.z) - 2 * sin(fv.rotateVector.x + fv.rotateVector.z) - 2 * sin(fv.rotateVector.x - fv.rotateVector.z)) / 4
    //(-2 * cos(fv.rotateVector.x + fv.rotateVector.z) + 2 * cos(fv.rotateVector.x - fv.rotateVector.z) - sin(fv.rotateVector.x + fv.rotateVector.y + fv.rotateVector.z) + sin(fv.rotateVector.x - fv.rotateVector.y + fv.rotateVector.z) - sin(fv.rotateVector.x + fv.rotateVector.y - fv.rotateVector.z) + sin(fv.rotateVector.x - fv.rotateVector.y - fv.rotateVector.z)) / 4
    //0

    //(sin(fv.rotateVector.x + fv.rotateVector.y) + sin(fv.rotateVector.x - fv.rotateVector.y)) / 2
    //(2 * cos(fv.rotateVector.x + fv.rotateVector.z) + 2 * cos(fv.rotateVector.x - fv.rotateVector.z) + sin(fv.rotateVector.x + fv.rotateVector.y + fv.rotateVector.z) - sin(fv.rotateVector.x - fv.rotateVector.y + fv.rotateVector.z) - sin(fv.rotateVector.x + fv.rotateVector.y - fv.rotateVector.z) + sin(fv.rotateVector.x - fv.rotateVector.y - fv.rotateVector.z)) / 4
    //(cos(fv.rotateVector.x + fv.rotateVector.y + fv.rotateVector.z) - cos(fv.rotateVector.x - fv.rotateVector.y + fv.rotateVector.z) + cos(fv.rotateVector.x + fv.rotateVector.y - fv.rotateVector.z) - cos(fv.rotateVector.x - fv.rotateVector.y - fv.rotateVector.z) - 2 * sin(fv.rotateVector.x + fv.rotateVector.z) + 2 * sin(fv.rotateVector.x - fv.rotateVector.z)) / 4
    //0

    //sin(fv.rotateVector.y)
    //(sin(fv.rotateVector.y + fv.rotateVector.z) - sin(fv.rotateVector.y - fv.rotateVector.z)) / 2
    //(cos(fv.rotateVector.y + fv.rotateVector.z) + cos(fv.rotateVector.y - fv.rotateVector.z)) / 2
    
    //if (!(fv.rotateVector.x == 0 && fv.rotateVector.y == 0 && fv.rotateVector.z == 0))
    //{
    //    float _11 = (cos(fv.rotateVector.x + fv.rotateVector.y) + cos(fv.rotateVector.x - fv.rotateVector.y)) / 2;
    //    float _12 = (cos(fv.rotateVector.x + fv.rotateVector.y + fv.rotateVector.z) - cos(fv.rotateVector.x - fv.rotateVector.y + fv.rotateVector.z) - cos(fv.rotateVector.x + fv.rotateVector.y - fv.rotateVector.z) + cos(fv.rotateVector.x - fv.rotateVector.y - fv.rotateVector.z) - 2 * sin(fv.rotateVector.x + fv.rotateVector.z) - 2 * sin(fv.rotateVector.x - fv.rotateVector.z)) / 4;
    //    float _13 = (-2 * cos(fv.rotateVector.x + fv.rotateVector.z) + 2 * cos(fv.rotateVector.x - fv.rotateVector.z) - sin(fv.rotateVector.x + fv.rotateVector.y + fv.rotateVector.z) + sin(fv.rotateVector.x - fv.rotateVector.y + fv.rotateVector.z) - sin(fv.rotateVector.x + fv.rotateVector.y - fv.rotateVector.z) + sin(fv.rotateVector.x - fv.rotateVector.y - fv.rotateVector.z)) / 4;
    //////float _14 = 0;
    //    float _21 = (sin(fv.rotateVector.x + fv.rotateVector.y) + sin(fv.rotateVector.x - fv.rotateVector.y)) / 2;
    //    float _22 = (2 * cos(fv.rotateVector.x + fv.rotateVector.z) + 2 * cos(fv.rotateVector.x - fv.rotateVector.z) + sin(fv.rotateVector.x + fv.rotateVector.y + fv.rotateVector.z) - sin(fv.rotateVector.x - fv.rotateVector.y + fv.rotateVector.z) - sin(fv.rotateVector.x + fv.rotateVector.y - fv.rotateVector.z) + sin(fv.rotateVector.x - fv.rotateVector.y - fv.rotateVector.z)) / 4;
    //    float _23 = (cos(fv.rotateVector.x + fv.rotateVector.y + fv.rotateVector.z) - cos(fv.rotateVector.x - fv.rotateVector.y + fv.rotateVector.z) + cos(fv.rotateVector.x + fv.rotateVector.y - fv.rotateVector.z) - cos(fv.rotateVector.x - fv.rotateVector.y - fv.rotateVector.z) - 2 * sin(fv.rotateVector.x + fv.rotateVector.z) + 2 * sin(fv.rotateVector.x - fv.rotateVector.z)) / 4;
    //////float _24 = 0;
    //    float _31 = sin(fv.rotateVector.y);
    //    float _32 = (sin(fv.rotateVector.y + fv.rotateVector.z) - sin(fv.rotateVector.y - fv.rotateVector.z)) / 2;
    //    float _33 = (cos(fv.rotateVector.y + fv.rotateVector.z) + cos(fv.rotateVector.y - fv.rotateVector.z)) / 2;
    ////float _34 = 0;
    
    //    p2.x = _11 * p2.x + _12 * p2.y + _13 * p2.z;
    //    p2.y = _21 * p2.x + _22 * p2.y + _23 * p2.z;
    //    p2.z = _31 * p2.x + _32 * p2.y + _33 * p2.z;
    //}

//[CosA, -SinA, 0, 0]
//[SinA, CosA, 0, 0]
//[0, 0, 1, 0]
//[0, 0, 0, 1]
//的答案是

//[CosB, 0, -SinB, 0]
//[0, 1, 0, 0]
//[SinB, 0, CosB, 0]
//[0, 0, 0, 1]

//1, 0, 0, 0,
//0, cos, sin * -1, 0,
//0, sin, cos, 0,
//0, 0, 0, 1

//CosA * CosB	-SinA * CosC+ CosA * -SinB * SinC	-SinA * -SinC+ CosA * -SinB *
//CosC0	
//SinA*
//CosB CosA* CosC+ SinA * -SinB *
//SinC CosA* -SinC+ SinA * -SinB *
//CosC0	
//SinBCosB *
//SinC CosB*
//CosC0	
//0	0	0	1	