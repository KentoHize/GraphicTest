cbuffer CB1
{
    float4 position : p1;
    float4 color : c1;
};

cbuffer CB2
{
    float4 position2 : p2;
    float4 color2 : c2;
};

struct PSInput
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};

PSInput VSMain(float4 position : POSITION, float4 color : COLOR)
{
	PSInput result;   
    result.position = position;
	result.color = color;	
	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
    //return input.color;
    return float4(1.0f, 0, 0, 1.0f);
}