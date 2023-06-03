struct PSInput
{
	float4 position : SV_POSITION;
	float4 color : COLOR;
};

PSInput VSMain(int4 position : POSITION, float4 color : COLOR)
{

	PSInput result;

    //result.position = float4(position[0], position[1], position[2], 0);
    result.position = position;
	result.color = color;
    //pc = position;
    //if (i == 1)
    //{
    //    result.position = float4(0, 0, 0, 0);
    //}
    //if (i == 2)
    //{
    //    result.position = float4(1, 0, 0, 0);
    //}   
    //if (i == 3)
    //{
    //    result.position = float4(0, 1, 0, 0);
    //}
        
    
	return result;
}

float4 PSMain(PSInput input) : SV_TARGET
{
    return input.color;
    //return float4(1, 1, 0, 1);
}