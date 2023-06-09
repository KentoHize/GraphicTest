struct SomeConstant
{
    int a;
    int b;
};
ConstantBuffer<SomeConstant> sc : register(b0);

struct AnotherConstant
{
    int c;
    int d;
};
ConstantBuffer<AnotherConstant> ac : register(b1);