using GraphicLibrary2.Items;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary2
{
    public partial class SharpDXEngine
    {
        public string AdapterName => ReferenceEquals(adapter, null) ? "" : adapter.Description2.Description;
        public long SharedMemoryUsage => ReferenceEquals(adapter, null) ? throw new NullReferenceException() : adapter.QueryVideoMemoryInfo(0, MemorySegmentGroup.NonLocal).CurrentUsage;
        public long DedicatedMemoryUsage => ReferenceEquals(adapter, null) ? throw new NullReferenceException() : adapter.QueryVideoMemoryInfo(0, MemorySegmentGroup.Local).CurrentUsage;
        public long DedicatedVideoMemory => ReferenceEquals(adapter, null) ? throw new NullReferenceException() : adapter.Description2.DedicatedVideoMemory;
        public long SharedSystemMemory => ReferenceEquals(adapter, null) ? throw new NullReferenceException() : adapter.Description2.SharedSystemMemory;


        public bool TextureExist(int index)
        => TextureTable.ContainsKey(index);

        public void DeleteTexture(int index)
        {
            if (!TextureTable.ContainsKey(index))
                throw new ArgumentException(nameof(index));
            TextureTable[index].Dispose();
            TextureTable.Remove(index);
        }

        public void ClearTextures()
        {
            while (TextureTable.Count != 0)
                DeleteTexture(TextureTable.First().Key);
        }

        public void LoadMaterial(int index, ArMaterial material)
        {
            MaterialTable[index] = material;
        }

        public bool MaterialExist(int index)
            => MaterialTable.ContainsKey(index);

        public void DeleteMaterial(int index)
        {
            if (!MaterialTable.ContainsKey(index))
                throw new ArgumentException(nameof(index));
            MaterialTable.Remove(index);
        }

        public void ClearMaterial()
        {
            while (MaterialTable.Count != 0)
                DeleteTexture(MaterialTable.First().Key);
        }

        public void SetCamera(string name, ArCamera camera)
        {
            CameraList[name] = camera;
        }

        public bool CameraExist(string name)
            => CameraList.ContainsKey(name);

        public void DeleteCamera(string name)
        {
            if (!CameraList.ContainsKey(name))
                throw new ArgumentException(nameof(name));
            CameraList.Remove(name);
        }

        public void ClearCamera()
        {
            while (CameraList.Count != 0)
                DeleteCamera(CameraList.First().Key);
        }

        public void SetLight(int index, ArLight light)
        {
            LightList[index] = light;
        }

        public bool LightExist(int index)
            => LightList.ContainsKey(index);

        public void DeleteLight(int index)
        {
            if (!LightList.ContainsKey(index))
                throw new ArgumentException(nameof(index));
            LightList.Remove(index);
        }

        public void ClearLight()
        {
            while (LightList.Count != 0)
                DeleteLight(LightList.First().Key);
        }

        public void Dispose()
           => Close();

        public void Close()
        {
            //var dd = device.QueryInterface<DebugDevice>();
            //dd.ReportLiveDeviceObjects(ReportingLevel.Detail);
            ////dd.

            PLStateNormal?.Dispose();
            PLStateCompute?.Dispose();
            fence?.Dispose();
            if (renderTargets != null)
                for (int i = 0; i < renderTargets.Length; i++)
                    renderTargets[i]?.Dispose();
            renderTargetViewHeap?.Dispose();
            commandList?.Dispose();
            commandList2?.Dispose();
            commandQueue?.Dispose();
            swapChain?.Dispose();
            //device11?.Dispose();
            device?.Dispose();
        }
    }
}
