﻿using GraphicLibrary.Items;
using System.Runtime.InteropServices;

namespace ResourceManagement
{
    [StructLayout(LayoutKind.Explicit, Size = 80)]
    //[StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct DirectX12FrameVariables
    {
        [FieldOffset(0)]
        ArIntVector3 _TranslateVector;
        [FieldOffset(16)]
        ArFloatVector3 _RotateVector;
        [FieldOffset(32)]
        ArFloatMatrix33 _RotateMatrix;
        [FieldOffset(76)]
        float _Scale;
        //public Dictionary<int, int> ReplaceMaterialIndices { get; set; }        
        public ArIntVector3 TranslateVector { get => _TranslateVector; set => _TranslateVector = value; }
        public ArFloatVector3 RotateVector { get => _RotateVector; set => _RotateVector = value; }
        public ArFloatMatrix33 RotateMatrix { get => _RotateMatrix; set => _RotateMatrix = value; }
        public float Scale { get => _Scale; set => _Scale = value; }
    }
}
