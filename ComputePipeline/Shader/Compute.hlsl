struct b
{
    float a;
    float b;
    float c;
};
ConstantBuffer<b> bb : register(b0);



struct r
{
    float4 a;
	float4 b;
    float4 c;
};
RWStructuredBuffer<r> aa : register(u0);

RWByteAddressBuffer di : register(u1);

uint CombineChar(uint char1, uint char2, uint char3, uint char4)
{
    return char1 + (char2 << 8) + (char3 << 16) + (char4 << 24);
}

//shared uint sr[200];

//void StringToUIntArray(uint s[])
//{
//    return uint(3);
//}

	
[numthreads(3, 1, 1)]
void CS(uint3 Gid : SV_GroupID,
	uint3 DTid : SV_DispatchThreadID,
	uint3 GTid : SV_GroupThreadID,
	uint GI : SV_GroupIndex)
{   
    //uint c = CombineChar('a', 'p', 'p', 'l');
    //uint StringToUIntArray("aaaaaaaa");
       
    //sr[0] = 'a';
    //sr[1] = 'p';
    //di.Store(0, CombineChar('a', 'p', 'p', 'l'));
    //di.Store(4, CombineChar('e', 0, 0, 0));
  
    //di.Store(0, CombineChar(sr[0], 'p', 'p', 'l'));
    //di.Store(4, CombineChar('e', 0, 0, 0));
    //di.Store(4, 'p');
    //di.Store(0, 'a' << 24 + 'p' << 16 + 'p' << 8 + 'l');
    //di.Store(0, 'a' << 4 + 'p');
    //di.Store()
    //di.Store(4, 'p');
    //di.Store(8, 'p');
    //di.Store(12, 'l');
    //di.Store(16, 'e');
    //aa[Gid.x].a = float4(1, 1, 1, 1);
    //aa[Gid.x].b = float4(2, 2, 2, 2);
    //aa[Gid.x].c = float4(3, 3, 3, 3);
    aa[DTid.x].a = float4(bb.a, bb.b, bb.c, Gid.x);
    //aa[DTid.x].a = float4(Gid.x, Gid.y, Gid.z, GI);
    aa[DTid.x].b = float4(2, 2, 2, 2);
    aa[DTid.x].c = float4(3, 3, 3, 3);
    //aa[0].a = float4(1, 1, 1, 1);
    //aa[0].b = float4(2, 2, 2, 2);
    //aa[0].c = float4(3, 3, 3, 3);
    //aa[1].a = float4(4, 4, 4, 4);
    //aa[1].b = float4(5, 5, 5, 5);
    //aa[1].c = float4(6, 6, 6, 6);
    //aa[2].a = float4(7, 7, 7, 7);
    //aa[2].b = float4(8, 8, 8, 8);
    //aa[2].c = float4(9, 9, 9, 9);
}
