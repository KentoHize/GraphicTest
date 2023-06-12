namespace D3D11on12
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            OtherForm mf = new OtherForm
            {
                Width = 1200,
                Height = 1000,
                Text = "D3D11 on 12",
                StartPosition = FormStartPosition.CenterScreen
            };
            Application.Run(mf);
        }
    }
}