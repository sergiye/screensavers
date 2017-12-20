using System;
using System.Linq;
using System.Windows.Forms;

namespace MorphClocks
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0)
            {
                switch (args[0].ToLower().Trim().Substring(0, 2))
                {
                    case "/c":
                        //inform the user no options can be set in this screen saver
                        Application.Run(new SettingsForm());
//                        MessageBox.Show("This screen saver has no options that you can set",
//                            "Information",
//                            MessageBoxButtons.OK,
//                            MessageBoxIcon.Information);
                        return;
                    case "/p":
                        //show the screen saver preview
                        Application.Run(new MainForm(new IntPtr(long.Parse(args[1])))); //args[1] is the handle to the preview window
                        return;
                }
            }
            //run the screen saver
            ShowScreensaver();
            Application.Run();
        }

        //will show the screen saver
        static void ShowScreensaver()
        {
            //var screensaver = new MainForm(Screen.AllScreens[1].Bounds);
            var screensaver = new MainForm(Screen.PrimaryScreen.Bounds);
            screensaver.Show();

            //loops through all the computer's screens (monitors)
//            foreach (var screen in Screen.AllScreens)
//            {
//                //creates a form just for that screen and passes it the bounds of that screen
//                var screensaver = new MainForm(screen.Bounds);
//                screensaver.Show();
//            }
        }
    }
}
