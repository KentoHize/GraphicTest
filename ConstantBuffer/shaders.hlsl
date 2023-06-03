struct CB1
{
    float4 position1;
    float4 color1;
};
ConstantBuffer<CB1> cb1 : register(b0);

struct CB2
{
    float4 position2;
    float4 color2;
};
ConstantBuffer<CB2> cb2 : register(b1);

struct CB3
{
    float4 position3;
    float4 color3;
};
ConstantBuffer<CB3> cb3 : register(b2);

struct PSInput
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};



PSInput VSMain(float4 position : POSITION, float4 color : COLOR)
{
	PSInput result;   
    result.position = position + cb3.position3;
	result.color = color;
	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
    //return cb1.color1;
    return cb3.color3;
    //return color1;
    //return input.color;
    //return float4(1.0f, 0, 0, 1.0f);
}