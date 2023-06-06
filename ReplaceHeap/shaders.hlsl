struct DrawingSetting
{
    int textureIndex;
};
ConstantBuffer<DrawingSetting> ds : register(b0);

struct PSInput
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};

PSInput VSMain(int3 position : POSITION, float4 color : COLOR)
{
	PSInput result;   
    int4 p = int4(position[0], position[1], position[2], 1);
    //float4 p2 = mul(p, tm.transformMatrix);
    float4 p2 = asfloat(p);
    result.position = p2;
	result.color = color;
	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
    return flaot4(ds.textureIndex, 0, 0, 1);
    return input.color;
}