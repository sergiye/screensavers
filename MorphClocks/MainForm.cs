using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace MorphClocks
{
    public partial class MainForm : Form
    {        
        #region Preview API's

        [DllImport("user32.dll")]
        static extern IntPtr SetParent(IntPtr hWndChild, IntPtr hWndNewParent);

        [DllImport("user32.dll")]
        static extern int SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

        [DllImport("user32.dll", SetLastError = true)]
        static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        static extern bool GetClientRect(IntPtr hWnd, out Rectangle lpRect);

        #endregion

        readonly bool IsPreviewMode;
        private Painter painter;
        private readonly Timer _timer;

        #region Constructors

        public MainForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.Manual;
//            Application.Idle += (sender, args) => Refresh();
            _timer = new Timer();
            _timer.Tick += (sender, args) => Refresh();
            _timer.Interval = 30;
        }

        //This constructor is passed the bounds this form is to show in
        //It is used when in normal mode
        public MainForm(Rectangle Bounds): this()
        {
            this.Bounds = Bounds;
            //hide the cursor
//            Cursor.Hide();
        }

        //This constructor is the handle to the select screen saver dialog preview window
        //It is used when in preview mode (/p)
        public MainForm(IntPtr PreviewHandle) : this()
        {
            //set the preview window as the parent of this window
            SetParent(Handle, PreviewHandle);

            //make this a child window, so when the select screen saver dialog closes, this will also close
            SetWindowLong(Handle, -16, new IntPtr(GetWindowLong(Handle, -16) | 0x40000000));

            //set our window's size to the size of our window's new parent
            Rectangle ParentRect;
            GetClientRect(PreviewHandle, out ParentRect);
            Size = ParentRect.Size;

            //set our location at (0, 0)
            Location = new Point(0, 0);

            IsPreviewMode = true;
        }

        #endregion

        #region GUI

        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (!AppSettings.Instance.BackColor.IsTransparent())
                BackColor = AppSettings.Instance.BackColor;
            painter = new Painter(ClientRectangle, AppSettings.Instance.FontName, AppSettings.Instance.TextColor, 
                AppSettings.Instance.BackColor, AppSettings.Instance.LineColor, AppSettings.Instance.BackTimer, 
                AppSettings.Instance.WorkEnd, AppSettings.Instance.DrawCircle, IsPreviewMode);
            //if (!IsPreviewMode) //we don't want all those effects for just a preview
            {
                Refresh();
            }
//            int handle = (int)this.Handle;
            var localpath = Application.StartupPath;
//            var _play = new MPlayer(handle, MplayerBackends.Direct3D, localpath + @"\mplayer\mplayer.exe");
//            _play.Play(localpath + @"\video.mp4");
//            _play.Mute();
            //_play.VideoExited += (o, ev) => { _play.Play(localpath + @"\video.mp4");};
            //_play.CurrentPosition += (o, ev) => { };
            //_play.SetSize(this.Width, this.Height);
            try
            {
                pictureBox.Image = Image.FromFile(localpath + "/image.gif");
            }
            catch (Exception)
            {
                //ignore
            }
            _timer.Enabled = true;
        }

        private void MainForm_Paint(object sender, PaintEventArgs e)
        {
            var r = e.ClipRectangle;//ClientRectangle;
            using (var bitmap = new Bitmap(r.Width, r.Height))
            {
                //using (var graphics = this.CreateGraphics())
                using (var graphics = Graphics.FromImage(bitmap))
                {
                    painter.UpdateDisplay(graphics, r);
                    e.Graphics.DrawImageUnscaled(bitmap, 0, 0, bitmap.Width, bitmap.Height);
                }
            }
        }

        #endregion

        #region User Input

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (IsPreviewMode) return;
            Application.Exit();
        }

        private void MainForm_Click(object sender, EventArgs e)
        {
            if (IsPreviewMode) return;
            Application.Exit();
        }

        //start off OriginalLoction with an X and Y of int.MaxValue, because
        //it is impossible for the cursor to be at that position. That way, we
        //know if this variable has been set yet.
        Point OriginalLocation = new Point(int.MaxValue, int.MaxValue);

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (IsPreviewMode) return;
            //see if originallocat5ion has been set
            if (OriginalLocation.X == int.MaxValue & OriginalLocation.Y == int.MaxValue)
            {
                OriginalLocation = e.Location;
            }
            //see if the mouse has moved more than 20 pixels in any direction. If it has, close the application.
            if (Math.Abs(e.X - OriginalLocation.X) > 20 | Math.Abs(e.Y - OriginalLocation.Y) > 20)
            {
                Application.Exit();
            }
        }

        #endregion
    }
}
