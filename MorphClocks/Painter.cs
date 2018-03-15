using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;

namespace MorphClocks
{
    internal class Painter
    {
        private class Snowflake
        {
            internal float X { get; set; }
            internal float Y { get; set; }
            internal int R { get; set; } //radius
            internal int Depth { get; set; } //painting depth
            internal float D { get; set; } //density
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
        internal Color TextColor { get; set; }
        internal Color BackColor { get; set; }

        //snowflake particles
        private readonly int _flakesCount; //max particles
        private readonly List<Snowflake> _particles = new List<Snowflake>();

        internal Painter(Rectangle rect, string fontName, Color textColor, Color backColor, Color linesColor, bool backTimer = false, int workEnd = 0, 
            bool drawCircle = false, bool previewMode = false)
        {
            _random = new Random();

            _previewMode = previewMode;
            _fontName = fontName;
            TextColor = textColor;
            BackColor = backColor;
            //if (TextColor.Equals(BackColor) || TextColor.GetBrightness() < 0.1)
            if (TextColor.Equals(BackColor) || BackColor.IsTransparent())
            {
                _colorRandomizer = true;
            }
            _linesColor = linesColor;
            _backTimer = backTimer;
            _workEnd = workEnd;
            _drawCircle = drawCircle;

            _shapes = new List<Shape>();
            for (var i = 0; i < 1; i++)
                _shapes.Add(new Shape(_previewMode, AppSettings.Instance.Move3D, AppSettings.Instance.MixPoint, AppSettings.Instance.BackColor));

            if (SnowFlakesEnabled)
            {
                _flakesCount = _random.Next(25, 100);
                PrepareSnowFlakes(rect);
            }
        }

        private bool SnowFlakesEnabled
        {
            get { return !_previewMode && (DateTime.Now.Month < 3 || DateTime.Now.Month > 11) && (_colorRandomizer || BackColor.GetBrightness() > 0.9); }
        }

        internal static List<Color> GetStaticPropertyBag()
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
        internal Color ColorRandomizer()
        {
            if (_cChangedTime.AddSeconds(3) < DateTime.Now)
            {
                do
                {
                    _cIdx = _random.Next(1, _colors.Count - 1);
                }
                while (_colors[_cIdx].GetBrightness().Equals(BackColor.GetBrightness()));
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
                                   R = _random.Next(1, 15), //radius
                                   Depth = _random.Next(3, 5),
                                   D = (float)(_random.NextDouble() * _flakesCount) //density
                               });
            }
        }

        private static void DrawSnowflake(Graphics gr, Snowflake flake)
        {
            var Initiator = new List<PointF>();
            float height = 0.75f * flake.R;
            var width = (float)(height / Math.Sqrt(3.0) * 2);
            var y3 = flake.Y + height;
            var y1 = y3 - height;
            var x3 = flake.X + (float)flake.R / 2;
            var x1 = flake.X;
            var x2 = x1 + width;
            Initiator.Add(new PointF(x1, y1));
            Initiator.Add(new PointF(x2, y1));
            Initiator.Add(new PointF(x3, y3));
            Initiator.Add(new PointF(x1, y1));

            // Draw the snowflake.
            for (var i = 1; i < Initiator.Count; i++)
            {
                var p1 = Initiator[i - 1];
                var p2 = Initiator[i];

                var dx = p2.X - p1.X;
                var dy = p2.Y - p1.Y;
                var length = (float)Math.Sqrt(dx * dx + dy * dy);
                var theta = (float)Math.Atan2(dy, dx);
                DrawSnowflakeEdge(gr, flake.Depth, ref p1, theta, length);
            }
        }

        // Recursively draw a snowflake edge starting at
        // (x1, y1) in direction theta and distance dist.
        // Leave the coordinates of the endpoint in
        // (x1, y1).
        private static void DrawSnowflakeEdge(Graphics gr, int depth,
            ref PointF p1, float theta, float dist)
        {
            const float ScaleFactor = 1 / 3f; // Make subsegments 1/3 size.
            var GeneratorDTheta = new List<float>();
            var pi_over_3 = (float)(Math.PI / 3f);
            GeneratorDTheta.Add(0);                 // Draw in the original direction.
            GeneratorDTheta.Add(-pi_over_3);        // Turn -60 degrees.
            GeneratorDTheta.Add(2 * pi_over_3);     // Turn 120 degrees.
            GeneratorDTheta.Add(-pi_over_3);        // Turn -60 degrees.

            if (depth == 0)
            {
                var p2 = new PointF(
                    (float)(p1.X + dist * Math.Cos(theta)),
                    (float)(p1.Y + dist * Math.Sin(theta)));
                gr.DrawLine(Pens.Blue, p1, p2);
                p1 = p2;
                return;
            }

            // Recursively draw the edge.
            dist *= ScaleFactor;
            for (var i = 0; i < GeneratorDTheta.Count; i++)
            {
                theta += GeneratorDTheta[i];
                DrawSnowflakeEdge(gr, depth - 1, ref p1, theta, dist);
            }
        }

        //Lets draw the flakes
        private void DrawFlakes(Graphics graphics, Rectangle rect)
        {
            foreach (var p in _particles)
            {
                //DrawSnowflake(graphics, p);

                using (var brush = new HatchBrush(HatchStyle.LargeConfetti, Color.AliceBlue))
//                using (var brush = new SolidBrush(Color.AliceBlue))
                {
                    graphics.FillEllipse(brush, p.X, p.Y, p.R, p.R);
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
                p.Y += (float)(Math.Cos(_snowFlakeAngle + p.D) + 1 + (float)p.R / 2);
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

        internal void UpdateDisplay(Graphics graphics, Rectangle rect)
        {
            if (_colorRandomizer)
                TextColor = ColorRandomizer();
            //setting the color palette
            var nowTime = DateTime.Now;
            //var backColor = _workEnd > 0 && nowTime.Hour >= _workEnd ? Color.DarkRed : Color.Black;
//            using (var backBrush = new SolidBrush(BackColor))
//                graphics.FillRectangle(backBrush, rect);
            foreach (var shape in _shapes)
            {
                shape.BackColor = BackColor;
                shape.DrawScreen(graphics, rect);
            }
            // drawing clocks and figure
            if (SnowFlakesEnabled)
                DrawFlakes(graphics, rect);
            DrawTimer(graphics, rect, 0, 0, nowTime);
        }

        private static string TimeToStr(DateTime aTime, bool backTimer, int workEnd)
        {
            if (!backTimer) return aTime.ToString("HH:mm:ss");
            var secsTotal = Math.Abs(aTime.Hour * 3600 + aTime.Minute * 60 + aTime.Second - workEnd * 3600);
            var ss = secsTotal % 60;
            secsTotal = secsTotal - ss;
            var hh = secsTotal / 3600;
            secsTotal = secsTotal - hh * 3600;
            var mm = secsTotal / 60;
            return String.Format("{0:00}:{1:00}:{2:00}", hh, mm, ss);
        }

        private static Font CheckFontExists(string fontName, float size, FontStyle style)
        {
            var fontTester = new Font(fontName, size, style, GraphicsUnit.Pixel);
            return fontTester;//.Name == fontName ? fontTester 
                //: new Font(FontFamily.GenericSerif, size, style);
        }

        internal void DrawTimer(Graphics graphics, Rectangle r, long aLeft, long aTop, DateTime nowTime)
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
                    using (var textFont = CheckFontExists(_fontName, size, _previewMode ? FontStyle.Regular : FontStyle.Bold))
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
