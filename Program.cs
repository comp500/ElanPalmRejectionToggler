namespace ElanPalmRejectionToggler
{
    internal static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            if (args.Length > 0 && args[0] == "restartDevice")
            {
                try
                {
                    PalmRejectionHandler.RestartDevice();
                }
                catch(Exception)
                {
                    Environment.Exit(-1);
                }

                Environment.Exit(100);
                return;
            }

            ApplicationConfiguration.Initialize();
            Application.Run(new TrayApplicationContext());
        }
    }
}