using GraphicLibrary.Items;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;

namespace GraphicLibrary
{
    public static class Ar3DMachine
    {
        //Only Support Windows
        public static byte[] LoadBitmapFromFile(string bitmapFile, out int width, out int height)
        {
            Bitmap bitmap = new Bitmap(bitmapFile);
            width = bitmap.Width;
            height = bitmap.Height;
            BitmapData data = bitmap.LockBits(new Rectangle(0, 0, width, height),
                ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
            byte[] result = new byte[width * height * 4];
            Marshal.Copy(data.Scan0, result, 0, result.Length);
            bitmap.UnlockBits(data);
            return result;
        }

        public static long StaticScaleFactor = 1000;
        //public static ArFloatVector3 MultiplyTransformMatrix(ArFloatVector3 position, ArFloatMatrix44 transformMatrix)
        //{
        //    ArFloatVector4 v4 = transformMatrix * new ArFloatVector4(position[0], position[1], position[2], 1);
        //    return new ArFloatVector3(v4[0], v4[1], v4[2]);
        //}

        public static ArFloatMatrix44 ProduceTransformMatrix(ArIntVector3 translateVector, ArFloatVector3 rotateVector, ArFloatVector3 scaleVector)
        {
            ArFloatMatrix44 result = ArFloatMatrix44.One;
            //Standard Scale
            result[0, 0] = result[0, 0] / StaticScaleFactor;
            result[1, 1] = result[1, 1] / StaticScaleFactor;
            result[2, 2] = result[2, 2] / StaticScaleFactor;
            ////Scale
            result[0, 0] = result[0, 0] * scaleVector[0];
            result[1, 1] = result[1, 1] * scaleVector[1];
            result[2, 2] = result[2, 2] * scaleVector[2];
            //Rotate            
            float cos = (float)Math.Cos(rotateVector[0]);
            float sin = (float)Math.Sin(rotateVector[0]);
            result *= new ArFloatMatrix44(
                cos, sin * -1, 0, 0,
                sin, cos, 0, 0,
                0, 0, 1, 0,
                0, 0, 0, 1);
            cos = (float)Math.Cos(rotateVector[1]);
            sin = (float)Math.Sin(rotateVector[1]);
            result *= new ArFloatMatrix44(
                cos, 0, sin * -1, 0,
                0, 1, 0, 0,
                sin, 0, cos, 0,
                0, 0, 0, 1);
            cos = (float)Math.Cos(rotateVector[2]);
            sin = (float)Math.Sin(rotateVector[2]);
            result *= new ArFloatMatrix44(
                1, 0, 0, 0,
                0, cos, sin * -1, 0,
                0, sin, cos, 0,
                0, 0, 0, 1);
            //Translate
            result[0, 3] = result[0, 3] + (float)((double)translateVector[0] / StaticScaleFactor);
            result[1, 3] = result[1, 3] + (float)((double)translateVector[1] / StaticScaleFactor);
            result[2, 3] = result[2, 3] + (float)((double)translateVector[2] / StaticScaleFactor);
            return result;
        }
    }
}
