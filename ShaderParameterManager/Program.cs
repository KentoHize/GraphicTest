namespace ShaderParameterManager
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
            Application.Run(new MainForm
            {
                Width = 1200,
                Height = 1000,
                Text = "Shader Parameter Manager",
                StartPosition = FormStartPosition.CenterScreen
            });
        }
    }
}