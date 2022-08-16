using System;
using System.Windows.Forms;

namespace Bsod {
  
  static class Program {
    
    [STAThread]
    static void Main(string[] args) {
      
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      if (args.Length > 0) {
        switch (args[0].ToLower().Trim().Substring(0, 2)) {
          case "/c":
            //inform the user no options can be set in this screen saver
            MessageBox.Show("This screensaver has no options that you can set", Application.ProductName, 
              MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
          case "/p":
            //show the screen saver preview, args[1] is the handle to the preview window
            Application.Run(new MainForm(new IntPtr(long.Parse(args[1]))));
            return;
        }
      }
      
      ShowScreensaver();
      Application.Run();
    }


    private static void ShowScreensaver() {
      foreach (var screen in Screen.AllScreens) {
        new MainForm(screen.Bounds).Show();
      }
    }
  }
}