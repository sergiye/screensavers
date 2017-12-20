using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;

namespace MorphClocks
{
    public class Painter
    {
        private readonly bool _previewMode;
        private readonly string _fontName;
        private readonly bool _drawCircle;
        private readonly bool _backTimer;
        private readonly int _workEnd;
        private readonly Color _textColor;
        private readonly Color _linesColor;
        private readonly List<Shape> _shapes;

        public Painter(string fontName, Color textColor, Color linesColor, bool backTimer = false, int workEnd = 0, bool drawCircle = false, bool previewMode = false)
        {
            _previewMode = previewMode;
            _fontName = fontName;
            _textColor = textColor;
            _linesColor = linesColor;
            _backTimer = backTimer;
            _workEnd = workEnd;
            _drawCircle = drawCircle;

            _shapes = new List<Shape>();
            for (var i = 0; i < 1; i++)
                _shapes.Add(new Shape(_previewMode, AppSettings.Instance.Move3D, AppSettings.Instance.MixPoint));
        }

        public void UpdateDisplay(Graphics graphics, Rectangle rect)
        {
            //setting the color palette
            var nowTime = DateTime.Now;
            var workEnd = 0;
            var backColor = workEnd > 0 && nowTime.Hour >= workEnd ? Color.DarkRed : Color.Black;
            using (var backBrush = new SolidBrush(backColor))
                graphics.FillRectangle(backBrush, rect);
            foreach (var shape in _shapes)
            {
                shape.BackColor = backColor;
                shape.DrawScreen(graphics, rect);
            }
            // drawing clocks and figure
            DrawTimer(graphics, rect, 0, 0, nowTime);
        }

        private static string TimeToStr(DateTime aTime, bool backTimer, int workEnd)
        {
            if (!backTimer) return aTime.ToString("HH:mm:ss");
            var secsNow = Math.Abs(aTime.Hour * 3600 + aTime.Minute * 60 + aTime.Second - workEnd * 3600);
            var ss = secsNow % 60;
            secsNow = secsNow - ss;
            var hh = secsNow / 3600;
            secsNow = secsNow - hh * 3600;
            var mm = secsNow / 60;
            return String.Format("{0}:{1}:{2:00}", hh, mm, ss);
        }

        private static bool CheckFontExists(string fontName)
        {
            var fontsCollection = new InstalledFontCollection();
            foreach (var fontFamiliy in fontsCollection.Families)
            {
                if (fontFamiliy.Name == fontName) return true;
            }
            return false;
//            using (Font fontTester = new Font(fontName, fontSize, FontStyle.Regular, GraphicsUnit.Pixel))
//                return fontTester.Name == fontName;
        }

        public void DrawTimer(Graphics graphics, Rectangle r, long aLeft, long aTop, DateTime nowTime)
        {
            long x = (r.Right-r.Left) / 2;
            long y = (r.Bottom-r.Top) / 2;
            var size = x > y ? y / 9 : x / 9;

            x = x + aLeft;
            y = y + aTop;

            //clock paint
            using (var linesPen = new Pen(_linesColor, 1))
            {
                if (_drawCircle)
                {
                    graphics.DrawEllipse(linesPen, aLeft, aTop, aLeft + r.Right - r.Left, aTop + r.Bottom - r.Top);
                }

                //Label and Timer paint
                using (var textBrush = new SolidBrush(_textColor))
                {
                    using (var textFont = CheckFontExists(_fontName)
                        ? new Font(_fontName, size, _previewMode ? FontStyle.Regular : FontStyle.Bold)
                        : new Font(FontFamily.GenericSerif, size, _previewMode ? FontStyle.Regular : FontStyle.Bold))
                    {
                        //var textFont = _modernFont ? new Font("Segoe Script", size, FontStyle.Bold) : new Font(FontFamily.GenericSerif, size, FontStyle.Bold);

                        //var text = "Morph Clock";
                        var text = nowTime.ToString("MMM-dd dddd");
                        var textSize = graphics.MeasureString(text, textFont);
                        graphics.DrawString(text, textFont, textBrush,
                            new PointF(x - textSize.Width / 2, (float) (r.Bottom - r.Top) / 7 + textSize.Height / 2));
                        text = TimeToStr(nowTime, _backTimer, _workEnd);
                        textSize = graphics.MeasureString(text, textFont);
                        graphics.DrawString(text, textFont, textBrush,
                            new PointF(x - textSize.Width / 2,
                                (float) (r.Bottom - r.Top) * 5 / 7 + textSize.Height / 2));

                        //clock face
                        for (var i = 1; i < 13; i++)
                        {
                            text = i.ToString("##");
                            textSize = graphics.MeasureString(text, textFont);
                            graphics.DrawString(text, textFont, textBrush, new PointF(
                                (float) (x - textSize.Width / 2 +
                                         (x - aLeft) * 2.7 * Math.Sin(i * 30 * Math.PI / 180) / 3),
                                (float) (y - textSize.Height / 2 -
                                         (y - aTop) * 2.7 * Math.Cos(i * 30 * Math.PI / 180) / 3)));
                        }
                    }
                }
                for (var i = 1; i < 61; i++)
                {
                    if (i % 5 != 0)
                    {
                        graphics.DrawLine(linesPen, 
                            new PointF((float) (x + (x - aLeft - 3) * Math.Sin(i * 6 * Math.PI / 180)),
                                       (float) (y - (y - aTop - 3) * Math.Cos(i * 6 * Math.PI / 180))),
                            new PointF((float) (x + (x - aLeft) / 1.1 * Math.Sin(i * 6 * Math.PI / 180)),
                                       (float) (y - (y - aTop) / 1.1 * Math.Cos(i * 6 * Math.PI / 180))));
                    }
                    else
                    {
                        graphics.DrawLine(linesPen,
                            new PointF((float)(x + (x - aLeft) / 1.5 * Math.Sin(i * 6 * Math.PI / 180)),
                                (float)(y - (y - aTop) / 1.5 * Math.Cos(i * 6 * Math.PI / 180))),
                            new PointF((float)(x + (x - aLeft) / 1.25 * Math.Sin(i * 6 * Math.PI / 180)),
                                (float)(y - (y - aTop) / 1.25 * Math.Cos(i * 6 * Math.PI / 180))));
                    }
                }

                //Arrows paint
                linesPen.Width = _previewMode ? 3 : 7;
                graphics.DrawEllipse(linesPen, x-3, y-3, 6, 6);
                //hour
                graphics.DrawLine(linesPen, new PointF(x, y),
                    new PointF((float)(x + (x - aLeft) / 2.5 * Math.Sin(((float)nowTime.Minute / 60 + nowTime.Hour) * 30 * Math.PI / 180)),
                               (float)(y - (y - aTop) / 2.5 * Math.Cos(((float)nowTime.Minute / 60 + nowTime.Hour) * 30 * Math.PI / 180))));
                //minute
                linesPen.Width = _previewMode ? 2 : 4;
                graphics.DrawLine(linesPen, new PointF(x, y),
                    new PointF((float)(x + (x - aLeft) / 1.5 * Math.Sin(((float)nowTime.Second / 60 + nowTime.Minute) * 6 * Math.PI / 180)),
                        (float)(y - (y - aTop) / 1.5 * Math.Cos(((float)nowTime.Second / 60 + nowTime.Minute) * 6 * Math.PI / 180))));
                //second
                linesPen.Width = 1;
                graphics.DrawLine(linesPen, new PointF(x, y),
                    new PointF((float)(x + (x - aLeft) / 1.1 * Math.Sin(nowTime.Second * 6 * Math.PI / 180)),
                        (float)(y - (y - aTop) / 1.1 * Math.Cos(nowTime.Second * 6 * Math.PI / 180))));
            }
        }
    }
}
