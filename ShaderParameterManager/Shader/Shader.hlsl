#include "Param.hlsl"

struct PSInput
{
	int index : INDEX;
};

PSInput VS(int index : INDEX)
{
	PSInput result;
	result.index = index;
	return result;
}

float4 PS(PSInput input)
{
	return float4(1, 1, 1, 1);
}