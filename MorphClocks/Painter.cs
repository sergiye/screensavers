using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Reflection;

namespace MorphClocks
{
    public class Painter
    {
        private class Snowflake
        {
            public float X { get; set; }
            public float Y { get; set; }
            public float R { get; set; }
            public float D { get; set; }
        }

        private readonly bool _previewMode;
        private readonly string _fontName;
        private readonly bool _drawCircle;
        private readonly bool _backTimer;
        private readonly int _workEnd;
        private readonly Color _linesColor;
        private readonly List<Shape> _shapes;
        private readonly Random _random;

        private readonly bool _colorRandomizer;
        public Color TextColor { get; set; }

        //snowflake particles
        private readonly int _flakesCount; //max particles
        private readonly List<Snowflake> _particles = new List<Snowflake>();

        public Painter(Rectangle rect, string fontName, Color textColor, Color linesColor, bool backTimer = false, int workEnd = 0, 
            bool drawCircle = false, bool previewMode = false)
        {
            _random = new Random();

            _previewMode = previewMode;
            _fontName = fontName;
            TextColor = textColor;
            if (textColor == Color.Black || textColor.GetBrightness() < 0.1)
                _colorRandomizer = true;
            _linesColor = linesColor;
            _backTimer = backTimer;
            _workEnd = workEnd;
            _drawCircle = drawCircle;

            _shapes = new List<Shape>();
            for (var i = 0; i < 1; i++)
                _shapes.Add(new Shape(_previewMode, AppSettings.Instance.Move3D, AppSettings.Instance.MixPoint));

            if (!_previewMode && _colorRandomizer)
            {
                _flakesCount = _random.Next(25, 100);
                PrepareSnowFlakes(rect);
            }
        }

        public static List<Color> GetStaticPropertyBag()
        {
            const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

            var map = new List<Color>();
            foreach (var prop in typeof(Color).GetProperties(flags))
            {
                var c = (Color)prop.GetValue(null, null);
                map.Add(c);
            }
            return map;
        }
        private readonly List<Color> _colors = GetStaticPropertyBag();
        private static DateTime _cChangedTime = DateTime.Now;
        private static int _cIdx = 1;
        public Color ColorRandomizer()
        {
            if (_cChangedTime.AddSeconds(3) < DateTime.Now)
            {
                do
                {
                    _cIdx = _random.Next(1, _colors.Count - 1);
                }
                while (_colors[_cIdx].GetBrightness() < 0.1);
                _cChangedTime = DateTime.Now;
            }
            return _colors[_cIdx];
        }

        private void PrepareSnowFlakes(Rectangle rect)
        {
            for (var i = 0; i < _flakesCount; i++)
            {
                _particles.Add(new Snowflake
                               {
                                   X = (float)(_random.NextDouble() * rect.Width), //x-coordinate
                                   Y = (float)(_random.NextDouble() * rect.Height), //y-coordinate
                                   R = (float)(_random.NextDouble() * 4 + 1), //radius
                                   D = (float)(_random.NextDouble() * _flakesCount) //density
                               });
            }
        }

        //Lets draw the flakes
        private void DrawFlakes(Graphics graphics, Rectangle rect)
        {
            foreach (var p in _particles)
            {
                using (var pen = new Pen(Color.AliceBlue, 3))
                {
                    graphics.DrawEllipse(pen, p.X, p.Y, p.R, p.R);
                }
            }
            UpdateFlakes(rect);
        }

        //Function to move the snowflakes
        //angle will be an ongoing incremental flag. Sin and Cos functions will be applied to it to create vertical and horizontal movements of the flakes
        double _snowFlakeAngle;
        private void UpdateFlakes(Rectangle rect)
        {
            _snowFlakeAngle += 0.01;
            for (var i = 0; i < _flakesCount; i++)
            {
                var p = _particles[i];
                //Updating X and Y coordinates
                //We will add 1 to the cos function to prevent negative values which will lead flakes to move upwards
                //Every particle has its own density which can be used to make the downward movement different for each flake
                //Lets make it more random by adding in the radius
                p.Y += (float)(Math.Cos(_snowFlakeAngle + p.D) + 1 + p.R / 2);
                p.X += (float)Math.Sin(_snowFlakeAngle) * 2;

                //Sending flakes back from the top when it exits
                //Lets make it a bit more organic and let flakes enter from the left and right also.
                if (p.X > rect.Width + 5 || p.X < -5 || p.Y > rect.Height)
                {
                    if (i % 3 > 0) //66.67% of the flakes
                    {
                        _particles[i] = new Snowflake{ X = (float) (_random.NextDouble() * rect.Width), Y = -10, R = p.R, D = p.D};
                    }
                    else
                    {
                        //If the flake is exitting from the right
                        if (Math.Sin(_snowFlakeAngle) > 0)
                        {
                            //Enter from the left
                            _particles[i] = new Snowflake{ X = -5, Y = (float) (_random.NextDouble() * rect.Height), R = p.R, D = p.D};
                        }
                        else
                        {
                            //Enter from the right
                            _particles[i] = new Snowflake{ X = rect.Width + 5, Y = (float) (_random.NextDouble() * rect.Height), R = p.R, D = p.D};
                        }
                    }
                }
            }
        }

        public void UpdateDisplay(Graphics graphics, Rectangle rect)
        {
            if (_colorRandomizer)
                TextColor = ColorRandomizer();
            //setting the color palette
            var nowTime = DateTime.Now;
            var backColor = _workEnd > 0 && nowTime.Hour >= _workEnd ? Color.DarkRed : Color.Black;
            using (var backBrush = new SolidBrush(backColor))
                graphics.FillRectangle(backBrush, rect);
            foreach (var shape in _shapes)
            {
                shape.BackColor = backColor;
                shape.DrawScreen(graphics, rect);
            }
            // drawing clocks and figure
            if (!_previewMode && _colorRandomizer)
                DrawFlakes(graphics, rect);
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
                using (var textBrush = new SolidBrush(TextColor))
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
