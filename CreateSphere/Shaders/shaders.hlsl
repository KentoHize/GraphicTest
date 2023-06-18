#include "constant.hlsl"

struct PSInput
{
	float4 position : SV_POSITION;
    float2 uv : TEXCOORD;
};

struct GSInput
{   
    float4 position : SV_POSITION;
    float2 uv : TEXCOORD;
};

struct GSOutput
{
    float4 position : SV_POSITION;
    float2 uv : TEXCOORD;
};

GSInput VSMain(int3 position : POSITION, float2 uv : TEXCOORD)
{
    GSInput result;
    int4 p = int4(position[0], position[1], position[2], 1);
    float4 p2;
    float4x4 normalTF = float4x4(0.001, 0, 0, 0, 0, 0.001, 0, 0, 0, 0, 0.001, 0, 0, 0, 0, 1);
    //p2 = mul(p, normalTF);
    p2 = mul(p, tm.transformMatrix);    
    result.position = p2;    
    //result.position = float4(position[0], position[1], position[2], 1);
    result.uv = uv;
	return result;
}

float4 PSMain(GSOutput input) : SV_TARGET
{
    return annetteTexture.Sample(normal_sampler, input.uv);
}

//[maxvertexcount(2)]
//void GSMain(line GSInput input[2], inout LineStream<GSOutput> outStream)
//{
//    //To do
//}

float4 MidPoint(float4 posa, float4 posb)
{
    
    return float4((posa[0] + posb[0]) / 2, (posa[1] + posb[1]) / 2, (posa[2] + posb[2]) / 2, (posa[3] + posb[3]) / 2);
}


float4 Normal(float4 posa, float4 posb, float4 posc)
{
    float3 c1 = posa - posb;
    float3 c2 = posb - posc;
    
    return float4(normalize(cross(c1, c2)), 0);
}

GSOutput GetVertex(GSOutput a)
{
    GSOutput result;
    result.position = a.position;
    result.uv = a.uv;
    return result;
}

[maxvertexcount(12)]
void GSMain(triangle GSInput input[3], inout TriangleStream<GSOutput> outStream)
{
    
    
    //提供切的方式
    GSOutput result;
    
    GSOutput a, b, c;
    GSOutput mid1, mid2, mid3;
    a.position = input[0].position;
    a.uv = input[0].uv;
    b.position = input[1].position;
    b.uv = input[1].uv;
    c.position = input[2].position;
    c.uv = input[2].uv;
    
    float4 n = Normal(input[0].position, input[1].position, input[2].position);
    n /= 10;
    mid1.position = (input[1].position - input[0].position) / 2 + input[0].position; //a - b
    mid1.position = (input[1].position + input[0].position) / 2 + n; //a - b
    mid1.uv = (input[0].uv + input[1].uv) / 2;
    mid2.position = (input[2].position - input[0].position) / 2 + input[0].position; //a - c
    mid2.position = (input[2].position + input[0].position) / 2 + n; //a - c
    mid2.uv = (input[0].uv + input[2].uv) / 2;
    mid3.position = (input[2].position - input[1].position) / 2 + input[1].position; // b - c
    mid3.position = (input[2].position + input[1].position) / 2 + n; //b - c
    mid3.uv = (input[1].uv + input[2].uv) / 2;
    
    //outStream.Append(a);
    //outStream.Append(b);
    //outStream.Append(c);
    
    outStream.Append(GetVertex(mid2));
    outStream.Append(GetVertex(a));
    outStream.Append(GetVertex(mid1));
    outStream.RestartStrip();
    
    outStream.Append(GetVertex(mid3));
    outStream.Append(GetVertex(mid1));
    outStream.Append(GetVertex(b));
    outStream.RestartStrip();
    
    outStream.Append(GetVertex(mid3));
    outStream.Append(GetVertex(c));
    outStream.Append(GetVertex(mid2));
    outStream.RestartStrip();
    
    outStream.Append(GetVertex(mid2));
    outStream.Append(GetVertex(mid1));
    outStream.Append(GetVertex(mid3));
    outStream.RestartStrip();
}


