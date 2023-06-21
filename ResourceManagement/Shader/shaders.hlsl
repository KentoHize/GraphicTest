//#include "constant.hlsl"

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
    int4 p = int4(position[0], position[1], position[2], 1);
    float4 p2;
    float4x4 normalTF = float4x4(0.001, 0, 0, 0, 0, 0.001, 0, 0, 0, 0, 0.001, 0, 0, 0, 0, 1);    
    //p2 = mul(p, tm.transformMatrix);
    p2 = mul(p, normalTF);
    result.position = p2;  
    result.shadow = shadow;
    result.tex = tex;
    result.texIndex = texIndex;
	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
    return float4(1, 0, 0, 1);
    //if(input.color[3] != 0)
    //    return input.color;
    //else
    //    return float4(input.uv[0], input.uv[1], 0, 1);
}