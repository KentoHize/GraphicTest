﻿
struct r
{
    float4 a;
	float4 b;
    float4 c;
};
RWStructuredBuffer<r> aa : register(u0);
	
[numthreads(1, 1, 1)]
void CS(uint3 Gid : SV_GroupID,
	uint3 DTid : SV_DispatchThreadID,
	uint3 GTid : SV_GroupThreadID,
	uint GI : SV_GroupIndex)
{
    aa[0].a = float4(1, 1, 1, 1);
    aa[0].b = float4(2, 2, 2, 2);
    aa[0].c = float4(3, 3, 3, 3);
    aa[1].a = float4(4, 4, 4, 4);
    aa[1].b = float4(5, 5, 5, 5);
    aa[1].c = float4(6, 6, 6, 6);
    aa[2].a = float4(7, 7, 7, 7);
    aa[2].b = float4(8, 8, 8, 8);
    aa[2].c = float4(9, 9, 9, 9);
}
