#include "Param.hlsl"

struct PSInput
{
	int index : INDEX;
	float4 position : SV_Position;
};

PSInput VS(int index : INDEX)
{
	PSInput result;
	result.index = index;
	result.position = float4(1, 1, 1, 1);
	return result;
}

float4 PS(PSInput input) : SV_TARGET
{
	return float4(1, 1, 1, 1);
}