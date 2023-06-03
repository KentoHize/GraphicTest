struct PSInput
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};

PSInput VSMain(int3 position : POSITION, float4 color : COLOR)
{
	PSInput result;   
    result.position = float4((float) position[0] / 1024, (float) position[1] / 1024, (float) position[2] / 1024, 1);
	result.color = color;
	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
    return input.color;
}