using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;

//using SharpDX;

namespace ShaderParameterManager
{
    public class HLSLShaderRPM
    {
        Device _Device;
        List<HLSLParameterInfo> _HLSLParameters;
        Dictionary<string, StaticSamplerDescription> _Samplers;
        RootSignatureFlags _RootSignatureFlags;
        int samplerCount;

        public HLSLShaderRPM(Device device, RootSignatureFlags rootSignatureFlags = RootSignatureFlags.AllowInputAssemblerInputLayout)
        {
            _Device = device;
            _RootSignatureFlags = rootSignatureFlags;
            _HLSLParameters = new List<HLSLParameterInfo>();
            _Samplers = new Dictionary<string, StaticSamplerDescription>();
        }

        //一個用符 二個用Heap //泛用
        public void SetParameter<T>(T obj, ParameterType rpt)
        {
            //rpt = RootParameterType.
        }

        public void SetParameter<T>(T[] objs, ParameterType rpt)
        {

        }

        public void SetParameter<TKey, TValue>(string name, IDictionary<TKey, TValue> dics, ParameterType rpt, SpecificType specificType = SpecificType.NotSet, bool load = true, ShaderVisibility visibility = ShaderVisibility.All)
        {
            _HLSLParameters.Add(new HLSLParameterInfo
            {
                Name = name,
                Count = dics.Count,
                RootParameterType = rpt,
                Type = typeof(TValue),
                SpecificType = specificType,
                Visibility = visibility
            });
        }

        public void SetStaticSampler(string name, StaticSamplerDescription ssd)
        {   
            _Samplers[name] = ssd;
        }

        public void Clear()
        {
            _HLSLParameters.Clear();
            _Samplers.Clear();
        }

        //public void UploadData()
        //{ }

        public RootSignatureDescription GetRootSignatureDescription()
        {
            RootSignatureDescription rsd = new RootSignatureDescription();
            rsd.Flags = _RootSignatureFlags;
            int bCount = 0, tCount = 0, uCount = 0, i;
            rsd.Parameters = new RootParameter[_HLSLParameters.Count];
            rsd.StaticSamplers = new StaticSamplerDescription[_Samplers.Count];

            for (i = 0; i < _HLSLParameters.Count; i++)
            {
                switch (_HLSLParameters[i].RootParameterType)
                {
                    case ParameterType.ConstantBuffer:
                        rsd.Parameters[i] = new RootParameter(_HLSLParameters[i].Visibility, new DescriptorRange[] {
                            new DescriptorRange(DescriptorRangeType.ConstantBufferView, _HLSLParameters[i].Count, bCount, 0, 0) });
                        bCount += _HLSLParameters[i].Count;
                        break;
                    case ParameterType.ShaderResource:
                        rsd.Parameters[i] = new RootParameter(_HLSLParameters[i].Visibility, new DescriptorRange[] {
                            new DescriptorRange(DescriptorRangeType.ShaderResourceView, _HLSLParameters[i].Count, tCount, 0, 0) });
                        tCount += _HLSLParameters[i].Count;
                        break;
                    case ParameterType.UnorderedAccess:
                        rsd.Parameters[i] = new RootParameter(_HLSLParameters[i].Visibility, new DescriptorRange[] {
                            new DescriptorRange(DescriptorRangeType.UnorderedAccessView, _HLSLParameters[i].Count, uCount, 0, 0) });
                        uCount += _HLSLParameters[i].Count;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            i = 0;
            foreach (KeyValuePair<string, StaticSamplerDescription> kvp in _Samplers)
            {
                rsd.StaticSamplers[i] = kvp.Value;
                i++;
            }

            return rsd;
        }        
      
        public void CreateRootParameterShaderFile(string file)
        {
            using (StreamWriter sw = new StreamWriter(file))
            {
                sw.Write(GetRootParameterHLSL());
                sw.Close();
            }            
        }

        public string GetRootParameterHLSL()
        {
            int i;
            int countIndex;
            int[] registerCount = new int[3];
            StringBuilder sb = new StringBuilder();
            string types1 = "", types2 = "";
            for (i = 0; i < _HLSLParameters.Count; i++)
            {
                switch (_HLSLParameters[i].RootParameterType)
                {
                    case ParameterType.ConstantBuffer:
                        countIndex = 0;
                        types1 = "b";
                        types2 = "ConstantBuffer";
                        break;
                    case ParameterType.ShaderResource:
                        countIndex = 1;
                        types1 = "t";
                        types2 = "StructuredBuffer";
                        break;
                    case ParameterType.UnorderedAccess:
                        countIndex = 2;
                        types1 = "u";
                        types2 = "RWStructuredBuffer";
                        break;
                    default:
                        throw new NotImplementedException();
                }

                if (_HLSLParameters[i].SpecificType != SpecificType.NotSet)
                {
                    sb.AppendLine($"{_HLSLParameters[i].SpecificType} {_HLSLParameters[i].Name} : register({types1}{registerCount[countIndex]});");
                }
                else
                {
                    sb.AppendLine($"struct {types1}{registerCount[countIndex]}");
                    sb.AppendLine("{");

                    PropertyInfo[] pis = _HLSLParameters[i].Type.GetProperties();
                    foreach (PropertyInfo pi in pis)
                    {
                        sb.AppendLine($"\t{pi.PropertyType.Name} {pi.Name};");
                    }
                    sb.AppendLine("};");
                    if (_HLSLParameters[i].Count == 1)
                        sb.AppendLine($"{types2}<{types1}{registerCount[countIndex]}> {_HLSLParameters[i].Name} : register({types1}{registerCount[countIndex]});");
                    else
                        sb.AppendLine($"{types2}<{types1}{registerCount[countIndex]}> {_HLSLParameters[i].Name}[{_HLSLParameters[i].Count}] : register({types1}{registerCount[countIndex]});");
                }
                registerCount[countIndex] += _HLSLParameters[i].Count;
            }

            i = 0;
            foreach (KeyValuePair<string, StaticSamplerDescription> kvp in _Samplers)
            {
                sb.AppendLine($"SamplerState {kvp.Key} : register(s{i});");
                i++;
            }

            return sb.ToString();
        }

    }
    public enum ParameterType
    {
        Constant32,
        ShaderResource,
        ConstantBuffer,
        UnorderedAccess,
    }
}
