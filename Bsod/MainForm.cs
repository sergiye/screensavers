using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Bsod
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

        #region Constructors

        public MainForm()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.Manual;
        }

        //This constructor is passed the bounds this form is to show in
        //It is used when in normal mode
        public MainForm(Rectangle Bounds): this()
        {
            this.Bounds = Bounds;
            //hide the cursor
            Cursor.Hide();
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

        //sets up the fake BSOD
        private void MainForm_Shown(object sender, EventArgs e)
        {
            if (!IsPreviewMode) //we don't want all those effects for just a preview
            {
                Refresh();
                //keep the screen black for one second to simulate the changing of screen resolution
                System.Threading.Thread.Sleep(1000);
            }
            //change the back color to a lovely BSOD blue
            BackColor = Color.FromArgb(0, 0, 130);
            //make the background image a fake BSOD generated by the GenerateBSOD() method.
            //the image is only 640x480, but it will be streatched to fit the whole screen like a real BSOD.
            BackgroundImage = GenerateBSOD();
        }

        //generates a BSOD bitmap with a random error and a random file name gathered from the "Errors" class
        private Bitmap GenerateBSOD()
        {
            //create the bitmap and graphics
            var BSOD = new Bitmap(640, 480);
            var BSODGraphics = Graphics.FromImage(BSOD);
            //make the image BSOD blue
            BSODGraphics.FillRectangle(new SolidBrush(Color.FromArgb(0, 0, 130)), new Rectangle(0, 0, 640, 480));
            //create the BSOD text
            var Error = Errors.GetRandomError();
            var File = Errors.GetRandomFile();
            var BSODText = "\r\n" + BSODBodyText.Header + " " + File + "\r\n\r\n" + Error + "\r\n\r\n" + BSODBodyText.Middle + File + BSODBodyText.End;
            //turn off any text smoothing (text smoothing would make it look really fake)
            BSODGraphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit ;
            //draw the text (FYI Lucida Console is the font used in real BSOD's)
            BSODGraphics.DrawString(BSODText, new Font("Lucida Console", 10, FontStyle.Regular), Brushes.White, new PointF(0, 0));
            //we are done with BSODGraphics
            BSODGraphics.Dispose();
            //create a new image the size of the window and some graphics for it
            var Scaled = new Bitmap(Width, Height);
            var ScaledGraphics = Graphics.FromImage(Scaled);
            if (IsPreviewMode)
            {
                //we want high quality resizing for preview so it will actually show up clearly
                ScaledGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            }
            else
            {
                //we want low quality resizing for full screen to make it more authentic
                ScaledGraphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.Low;
            }
            //draw the BSOD image on the Scaled image so that it is enlarged
            ScaledGraphics.DrawImage(BSOD, new Rectangle(0, 0, Width, Height));
            return Scaled;
        }

        #endregion

        #region User Input

        private void MainForm_KeyDown(object sender, KeyEventArgs e)
        {
            if (!IsPreviewMode) //disable exit functions for preview
            {
                Application.Exit();
            }
        }

        private void MainForm_Click(object sender, EventArgs e)
        {
            if (!IsPreviewMode) //disable exit functions for preview
            {
                Application.Exit();
            }
        }

        //start off OriginalLoction with an X and Y of int.MaxValue, because
        //it is impossible for the cursor to be at that position. That way, we
        //know if this variable has been set yet.
        Point OriginalLocation = new Point(int.MaxValue, int.MaxValue);

        private void MainForm_MouseMove(object sender, MouseEventArgs e)
        {
            if (!IsPreviewMode) //disable exit functions for preview
            {
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
        }

        #endregion
    }
}
