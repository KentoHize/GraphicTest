struct TranformMatrix
{
    float4x4 transformMatrix;
};
ConstantBuffer<TranformMatrix> tm : register(b0);

struct PSInput
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};

PSInput VSMain(int3 position : POSITION, float4 color : COLOR)
{
	PSInput result;   
    int4 p = int4(position[0], position[1], position[2], 1);    
    float4 p2 = mul(p, tm.transformMatrix);
    result.position = float4((float) p2[0] / 1024, (float) p2[1] / 1024, (float) p2[2] / 1024, 1);
	result.color = color;
	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
    return input.color;
}