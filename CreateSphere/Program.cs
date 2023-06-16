namespace CreateSphere
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
            MainForm mf = new MainForm
            {
                Width = 1200,
                Height = 1000,
                Text = "Create Sphere",
                StartPosition = FormStartPosition.CenterScreen
            };
            Application.Run(mf);
        }
    }
}