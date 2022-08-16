using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MorphClocks {
  
  public partial class MainForm : Form {
    
    #region Preview API's

    [DllImport("user32.dll")]
    static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

    [DllImport("user32.dll")]
    static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", SetLastError = true)]
    static extern int GetWindowLong(IntPtr hWnd, int nIndex);

    [DllImport("user32.dll")]
    static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

    [DllImport("user32.dll")]
    public static extern bool LockWorkStation();
    
    #endregion

    readonly bool isPreviewMode;
    private Painter painter;
    private readonly Timer timer;

    #region Constructors

    public MainForm() {
      InitializeComponent();
      StartPosition = FormStartPosition.Manual;
      ShowInTaskbar = false;
      //Application.Idle += (sender, args) => Refresh();
      timer = new Timer { Interval = 30 };
      timer.Tick += (sender, args) => Invalidate();
    }

    //This constructor is passed the bounds this form is to show in
    //It is used when in normal mode
    public MainForm(Rectangle bounds) : this() {
      this.Bounds = bounds;
      //hide the cursor
      // Cursor.Hide();
    }

    //This constructor is the handle to the select screen saver dialog preview window
    //It is used when in preview mode (/p)
    public MainForm(IntPtr previewHandle) : this() {
      //set the preview window as the parent of this window
      SetParent(Handle, previewHandle);

      //make this a child window, so when the select screen saver dialog closes, this will also close
      SetWindowLong(Handle, -20, new IntPtr(GetWindowLong(Handle, -20) | 0x00000080));

      //set our window's size to the size of our window's new parent
      GetClientRect(previewHandle, out var parentRect);
      Size = parentRect.Size;

      //set our location at (0, 0)
      Location = new Point(0, 0);

      isPreviewMode = true;
    }

    #endregion

    #region GUI

    private void MainForm_Shown(object sender, EventArgs e) {
      
      if (!AppSettings.Instance.BackColor.IsTransparent())
        BackColor = AppSettings.Instance.BackColor;
      
      painter = new Painter(ClientRectangle, AppSettings.Instance.FontName, AppSettings.Instance.FontSize, AppSettings.Instance.TextColor,
        AppSettings.Instance.BackColor, AppSettings.Instance.LineColor, AppSettings.Instance.DrawCircle, isPreviewMode);
      //if (!IsPreviewMode) //we don't want all those effects for just a preview
      {
        Invalidate();
      }
      var localPath = Application.StartupPath;
      
      // int handle = (int)this.Handle;
      // var play = new MPlayer(handle, MplayerBackends.Direct3D, localpath + @"\mplayer\mplayer.exe");
      // play.Play(localpath + @"\video.mp4");
      // play.Mute();
      // play.VideoExited += (o, ev) => { play.Play(localpath + @"\video.mp4");};
      // play.CurrentPosition += (o, ev) => { };
      // play.SetSize(this.Width, this.Height);
      
      var backgroundPath = localPath + "/background.gif";
      if (File.Exists(backgroundPath))
        try {
          pictureBox.Image = Image.FromFile(backgroundPath);
        }
        catch (Exception) {
          pictureBox.Image = null;
        }
      timer.Enabled = true;
    }

    private void MainForm_Paint(object sender, PaintEventArgs e) {
      painter.UpdateDisplay(e.Graphics, e.ClipRectangle);
    }

    #endregion

    #region User Input

    private void MainForm_KeyDown(object sender, KeyEventArgs e) {
      if (isPreviewMode) return;
      Exit();
    }

    private void MainForm_Click(object sender, EventArgs e) {
      if (isPreviewMode) return;
      Exit();
    }

    //start off OriginalLoction with an X and Y of int.MaxValue, because
    //it is impossible for the cursor to be at that position. That way, we
    //know if this variable has been set yet.
    private Point originalLocation = new Point(int.MaxValue, int.MaxValue);

    private void MainForm_MouseMove(object sender, MouseEventArgs e) {
      if (isPreviewMode) return;
      //see if originallocat5ion has been set
      if (originalLocation.X == int.MaxValue & originalLocation.Y == int.MaxValue) {
        originalLocation = e.Location;
      }

      //see if the mouse has moved more than 20 pixels in any direction. If it has, close the application.
      if (Math.Abs(e.X - originalLocation.X) > 20 | Math.Abs(e.Y - originalLocation.Y) > 20) {
        Exit();
      }
    }

    private void Exit() {
      if (AppSettings.Instance.LockOnExit)
        LockWorkStation();
      Application.Exit();
    }
    
    #endregion
  }
}