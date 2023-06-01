float4x4 tf;

struct PSInput
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};

PSInput VSMain(float4 position : POSITION, float4 color : COLOR)
{	
    
    
	PSInput result;
    //float op = position[0];
    //for (int i = 0; i < 10000000; i++)
    //{
    //    if(i % 2 == 0)
    //        position[0] += i;
    //    else
    //        position[0] -= i;
    //}
    //position[0] = op;
    result.position = position;
	result.color = color;
	
	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
    //float4 result = float4(1.0f, 0, 0, 1.0f);
    return float4(1.0f, 0, 0, 1.0f);
	//return result;
	//return input.color;
}