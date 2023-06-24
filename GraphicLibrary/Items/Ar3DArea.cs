namespace GraphicLibrary.Items
{
    public class Ar3DArea
    {
        public Ar3DModelGroup[]? ModelGroups { get; set; }
        public ArFloatVector4 BackgroudColor { get; set; }
        public ArIntVector3 TranslateTransform { get; set; }
        public ArFloatVector3 RotateTransform { get; set; }
        public ArFloatVector3 ScaleTransform { get; set; } = ArFloatVector3.One;
        public Ar3DArea(List<Ar3DModelGroup> modelGroups)
            : this(modelGroups.ToArray())
        { }

        public Ar3DArea(Ar3DModelGroup[] modelGroups)
        {
            ModelGroups = modelGroups;
        }

        public Ar3DArea()
        {

        }
    }
}
