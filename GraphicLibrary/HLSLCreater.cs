using SharpDX.Direct3D11;
using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary
{
    public static class HLSLCreater
    {
        public static void SaveToFile(string file, string s)
        {

        }

        public static string GetVariablesHLSL(RootSignatureDescription rsd, List<(Type, string)> variables, string[] names)
        {
            if (rsd == null)
                throw new ArgumentNullException(nameof(rsd));

            int nameIndex = 0, n = 0;
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < rsd.Parameters.Length; i++)
            {
                switch(rsd.Parameters[i].ParameterType)
                {
                    case RootParameterType.Constant32Bits:
                        break;
                    case RootParameterType.ShaderResourceView:
                        break;
                    case RootParameterType.ConstantBufferView:
                        sb.AppendLine($"struct cb{n}");
                        sb.AppendLine("{");
                            //for(int j = 0; j < variables.)
                        sb.AppendLine("};");
                        if (rsd.Parameters[i].Descriptor.RegisterSpace == 0)
                            sb.AppendLine($"ConstantBuffercb<{n}> : register(b{rsd.Parameters[i].Descriptor.ShaderRegister})");
                        else
                            sb.AppendLine($"ConstantBuffercb<{n}> : register(b{rsd.Parameters[i].Descriptor.ShaderRegister}, space{rsd.Parameters[i].Descriptor.RegisterSpace})");

                        break;
                    case RootParameterType.DescriptorTable:
                        break;
                }
                //rsd.Parameters[i]
            }

            return sb.ToString();
            //RootSignatureDescription
        }

        //(int, int, int) GetInfoFromParameter(RootParameter rp)
        //{
        //    switch(rp.ParameterType)
        //    {
        //        case RootParameterType.
        //    }
        //    rp.Descriptor.RegisterSpace
            
        //}
        //string Get
    }
}
