namespace GraphicLibrary
{
    public class ShaderFileInfo
    {
        public string File { get; set; }
        public string EntryPoint { get; set; }
        public ShaderType Type { get; set; }
        public int VersionT { get; set; } // Version * 1000

        public ShaderFileInfo(string file, ShaderType type, int versionT = 5100, string entryPoint = "")
        {
            File = file;
            Type = type;
            VersionT = versionT;
            EntryPoint = entryPoint == "" ? DefaultEntryName : entryPoint;
        }

        protected string ShaderTypeAbbr()
        {
            string s = Type.ToString();
            return $"{s[0]}{s.LastOrDefault(char.IsUpper)}".ToLower();
        }
        public string Profile
        {
            get
            {
                int v1 = int.Parse(VersionT.ToString()[0].ToString()),
                    v2 = int.Parse(VersionT.ToString()[1].ToString());

                if (Type == ShaderType.RootSignature)
                    return $"rootsig_{v1}_{v2}";
                else
                    return $"{ShaderTypeAbbr()}_{v1}_{v2}";
            }
        }

        public string DefaultEntryName => $"{ShaderTypeAbbr().ToUpper()}Main";
    }
}