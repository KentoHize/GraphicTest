using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace GraphicLibrary.Items
{
    public class Ar3DArea
    {
        public Ar3DModel[]? Models { get; set; }
        public ArFloatVector4 BackgroudColor { get; set; }
        public ArIntVector3 TranslateTransform { get; set; }
        public ArFloatVector3 RotateTransform { get; set; }
        public ArFloatVector3 ScaleTransform { get; set; } = ArFloatVector3.One;
        public Ar3DArea(List<Ar3DModel> models)
            : this(models.ToArray())
        { }

        public Ar3DArea(Ar3DModel[] models)
        {
            Models = models;
        }

        public Ar3DArea()
        {
            
        }
    }
}
