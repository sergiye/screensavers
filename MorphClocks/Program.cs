using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace MorphClocks {
  
  static class Program {
    
    [STAThread]
    static void Main(string[] args) {
    
      Application.EnableVisualStyles();
      Application.SetCompatibleTextRenderingDefault(false);
      if (args.Length > 0) {
        switch (args[0].ToLower().Trim().Substring(0, 2)) {
          case "/c":
            Application.Run(new SettingsForm());
            return;
          case "/p":
            Application.Run(
              new MainForm(new IntPtr(long.Parse(args[1])))); //args[1] is the handle to the preview window
            return;
        }
      }

      ShowScreensaver();
      Application.Run();
    }

    private static void ShowScreensaver() {
      if (Debugger.IsAttached || AppSettings.Instance.PrimaryDisplayOnly) {
        var screensaver = new MainForm(Screen.PrimaryScreen.Bounds);
        screensaver.Show();
      }
      else {
        //loops through all the computer's screens (monitors)
        foreach (var screen in Screen.AllScreens) {
          new MainForm(screen.Bounds).Show();
        }
      }
    }
  }
}