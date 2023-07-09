using SharpDX.Direct3D12;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using Accessibility;
using System.Security.Cryptography;
//using SharpDX;

namespace ShaderParameterManager
{
    public class HLSLShaderRPM
    {
        Device _Device;
        List<HLSLParameterInfo> _HLSLParameters;
        Dictionary<string, SamplerStateDescription> _Samplers;
        RootSignatureFlags _RootSignatureFlags;
        int samplerCount;

        public HLSLShaderRPM(Device device, RootSignatureFlags rootSignatureFlags = RootSignatureFlags.None)
        {
            _Device = device;
            _RootSignatureFlags = rootSignatureFlags;
            _HLSLParameters = new List<HLSLParameterInfo>();
            _Samplers = new Dictionary<string, SamplerStateDescription>();
        }

        //一個用符 二個用Heap //泛用
        public void SetParameter<T>(T obj, RootParameterType rpt)
        {
            //rpt = RootParameterType.
        }

        public void SetParameter<T>(T[] objs, RootParameterType rpt)
        {

        }

        public void SetParameter<TKey, TValue>(string name, IDictionary<TKey, TValue> dics, RootParameterType rpt, bool load = true, ShaderVisibility visibility = ShaderVisibility.All)
        {
            _HLSLParameters.Add(new HLSLParameterInfo
            {
                Name = name,
                Count = dics.Count,
                RootParameterType = rpt,
                Type = typeof(TValue),
                Visibility = visibility
            });
        }

        public void SetStaticSampler(string name, SamplerStateDescription ssd)
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
                    case RootParameterType.ConstantBufferView:
                        rsd.Parameters[i] = new RootParameter(_HLSLParameters[i].Visibility, new DescriptorRange[] {
                            new DescriptorRange(DescriptorRangeType.ConstantBufferView, _HLSLParameters[i].Count, bCount, 0, 0) });
                        bCount += _HLSLParameters[i].Count;
                        break;
                    case RootParameterType.ShaderResourceView:
                        rsd.Parameters[i] = new RootParameter(_HLSLParameters[i].Visibility, new DescriptorRange[] {
                            new DescriptorRange(DescriptorRangeType.ShaderResourceView, _HLSLParameters[i].Count, tCount, 0, 0) });
                        tCount += _HLSLParameters[i].Count;
                        break;
                    case RootParameterType.UnorderedAccessView:
                        rsd.Parameters[i] = new RootParameter(_HLSLParameters[i].Visibility, new DescriptorRange[] {
                            new DescriptorRange(DescriptorRangeType.UnorderedAccessView, _HLSLParameters[i].Count, uCount, 0, 0) });
                        uCount += _HLSLParameters[i].Count;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            i = 0;
            foreach (KeyValuePair<string, SamplerStateDescription> kvp in _Samplers)
            {
                rsd.StaticSamplers[i] = new StaticSamplerDescription(kvp.Value, ShaderVisibility.All, i, 0);//All to Pixel
                i++;
            }

            return rsd;
        }

        public string GetRootParameterHLSL()
        {
            return null;
        }

    }
    public enum RootParameterType
    {
        Constant32,
        //    //ShaderResourceViewDT,
        ShaderResourceView,
        //   // ConstantBufferViewDT,
        ConstantBufferView,
        //    //UnorderedAccessViewDT,
        UnorderedAccessView,
        //    Sampler
    }
}
