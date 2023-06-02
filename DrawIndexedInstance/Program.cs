using GraphicLibrary;

namespace DrawIndexedInstance
{
    internal static class Program
    {
        internal static bool Exit { get; set; }
        internal static DateTime time { get; set; }
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            MainForm mf = new MainForm();
            mf.Width = 1200;
            mf.Height = 1000;
            mf.Show();

            Application.Run(mf);
            //time = DateTime.Now;
            //while(!Exit)
            //{
            //    Application.DoEvents();
            //    Thread.Sleep(20);
            //    if(DateTime.Now.AddMilliseconds(-33) > time)
            //    {
            //        time = DateTime.Now;
            //        mf.Invalidate();
            //    }   
            //}
            
            mf.Dispose();            
        }
    }
}