//struct a
//{
    
//};



struct r
{
    float4 a;
};
RWStructuredBuffer<r> aa : register(u0);
	
[numthreads(1, 1, 1)]
void CS(uint3 Gid : SV_GroupID,
	uint3 DTid : SV_DispatchThreadID,
	uint3 GTid : SV_GroupThreadID,
	uint GI : SV_GroupIndex)
{
    aa[0].a = float4(1, 1, 1, 1);
}
