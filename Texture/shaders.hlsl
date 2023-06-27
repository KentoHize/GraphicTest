struct TranformMatrix
{
    float4x4 transformMatrix;
};
ConstantBuffer<TranformMatrix> tm : register(b0);

struct DrawSetting
{
    int textureIndex;
};
ConstantBuffer<DrawSetting> ds : register(b1);

struct PSInput
{
	float4 position : SV_POSITION;
    float4 color : COLOR;
    float2 uv : TEXCOORD;
};

Texture2D t2d_Annette : register(t0);
Texture2D t2d_Clacier : register(t1);
SamplerState normal_sampler : register(s0);

PSInput VSMain(int3 position : POSITION, float4 color : COLOR, float2 texCoord : TEXCOORD)
{
	PSInput result;   
    int4 p = int4(position[0], position[1], position[2], 1);    
    float4 p2 = mul(p, tm.transformMatrix);
    result.position = p2;
	result.color = color;
    result.uv = texCoord;
	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
    //printf("1");
    
    if (input.color.x != 0 || input.color.y != 0 || input.color.z != 0) // detect have color
        return input.color;
    else if(input.color.w != 0 && input.color.w != 1)
    {
        //return float4(1, 1, 1, input.color.w);
        float4 or = t2d_Annette.Sample(normal_sampler, input.uv);
        return float4(or.x, or.y, or.z, input.color.w);

    }
    else
    {
        float4 or = t2d_Annette.Sample(normal_sampler, input.uv);
        return or;
    }
        //return float4(input.uv[0], input.uv[1], 0, 1);
        
}