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
        Device Device;
        //int bCount, tCount, uCount, sCount;
        List<Type> bTypes;
        List<Type> tTypes;
        List<Type> uTypes;
        RootSignatureDescription rsd;

        public HLSLShaderRPM(Device device) {
            Device = device;
            //bCount = tCount = uCount = sCount = 0;
            bTypes = new List<Type>();
            tTypes = new List<Type>();
            uTypes = new List<Type>();
            rsd = new RootSignatureDescription();
            rsd.Flags = RootSignatureFlags.AllowInputAssemblerInputLayout;
        }

        //一個用符 二個用Heap //泛用
        public void SetParameter<T>(T obj, RootParameterType rpt)
        {          
            //rpt = RootParameterType.
        }

        public void SetParameter<T>(T[] objs, RootParameterType rpt)
        {

        }

        public void SetParameter<TKey, TValue>(IDictionary<TKey, TValue> dics, RootParameterType rpt, bool load = true)
        {
            switch(rpt)
            {
                case RootParameterType.ConstantBufferView:
                    bTypes.Add(typeof(TValue));                    
                    break;
                case RootParameterType.ShaderResourceView:
                    tTypes.Add(typeof(TValue));
                    break;
                case RootParameterType.UnorderedAccessView:
                    uTypes.Add(typeof(TValue));
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public void UploadData()
        { }

        public RootSignatureDescription GetRootSignatureDescription()
        {
            return rsd;
        }

    }

    //public enum RootParameterType
    //{
    //    Constant32,
    //    //ShaderResourceViewDT,
    //    ShaderResourceView,
    //   // ConstantBufferViewDT,
    //    ConstantBufferView,
    //    //UnorderedAccessViewDT,
    //    UnorderedAccessView,
    //    Sampler
    //}
}
