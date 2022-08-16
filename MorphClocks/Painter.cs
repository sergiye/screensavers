using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;

namespace MorphClocks {
  
  internal class Painter {
    
    private class Snowflake {
      internal float X { get; set; }
      internal float Y { get; set; }
      internal int R { get; set; } //radius
      internal int Depth { get; set; } //painting depth
      internal float D { get; set; } //density
    }

    private readonly bool previewMode;
    
    private readonly Font textFont;
    private readonly Pen linesPen;
    private readonly HatchBrush flakesBrush;
    
    private readonly bool drawCircle;
    private readonly List<Shape> shapes;
    private readonly Random random = new Random();

    private readonly bool colorRandomizer;
    private Color TextColor { get; set; }
    private Color BackColor { get; set; }

    //snowflake particles
    private readonly int flakesCount; //max particles
    private readonly List<Snowflake> particles = new List<Snowflake>();

    internal Painter(Rectangle rect, bool previewMode = false) {
      
      this.previewMode = previewMode;

      var size = rect.Width > rect.Height ? rect.Height / 18 : rect.Width / 18;
      textFont = CheckFontExists(AppSettings.Instance.FontName, size, previewMode ? FontStyle.Regular : FontStyle.Bold);
      linesPen = new Pen(AppSettings.Instance.LineColor, 3);
      flakesBrush = new HatchBrush(HatchStyle.LargeConfetti, Color.AliceBlue);
      // flakesBrush = new SolidBrush(Color.AliceBlue);

      TextColor = AppSettings.Instance.TextColor;
      BackColor = AppSettings.Instance.BackColor;

      //if (TextColor.Equals(BackColor) || TextColor.GetBrightness() < 0.1)
      if (TextColor.Equals(BackColor) && BackColor.IsTransparent()) {
        colorRandomizer = true;
      }

      drawCircle = AppSettings.Instance.DrawCircle;

      shapes = new List<Shape>();
      for (var i = 0; i < 1; i++)
        shapes.Add(new Shape(this.previewMode, AppSettings.Instance.Move3D, AppSettings.Instance.MixPoint,
          AppSettings.Instance.BackColor));

      if (SnowFlakesEnabled) {
        flakesCount = random.Next(25, 100);
        PrepareSnowFlakes(rect);
      }
    }

    private bool SnowFlakesEnabled => !previewMode && (colorRandomizer || (BackColor.GetBrightness() > 0.9) && (DateTime.Now.Month < 3 || DateTime.Now.Month > 11));

    private static List<Color> GetStaticPropertyBag() {
      const BindingFlags flags = BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

      var map = new List<Color>();
      foreach (var prop in typeof(Color).GetProperties(flags)) {
        var c = (Color) prop.GetValue(null, null);
        map.Add(c);
      }

      return map;
    }

    private readonly List<Color> colors = GetStaticPropertyBag();
    private static DateTime cChangedTime = DateTime.Now;
    private static int cIdx = 1;

    private Color ColorRandomizer() {
      if (cChangedTime.AddSeconds(3) < DateTime.Now) {
        do {
          cIdx = random.Next(1, colors.Count - 1);
        } while (colors[cIdx].GetBrightness().Equals(BackColor.GetBrightness()));

        cChangedTime = DateTime.Now;
      }

      return colors[cIdx];
    }

    private void PrepareSnowFlakes(Rectangle rect) {
      for (var i = 0; i < flakesCount; i++) {
        particles.Add(new Snowflake {
          X = (float) (random.NextDouble() * rect.Width), //x-coordinate
          Y = (float) (random.NextDouble() * rect.Height), //y-coordinate
          R = random.Next(1, 15), //radius
          Depth = random.Next(3, 5),
          D = (float) (random.NextDouble() * flakesCount) //density
        });
      }
    }

    private static void DrawSnowflake(Graphics gr, Snowflake flake) {
      var initiator = new List<PointF>();
      var height = 0.75f * flake.R;
      var width = (float) (height / Math.Sqrt(3.0) * 2);
      var y3 = flake.Y + height;
      var y1 = y3 - height;
      var x3 = flake.X + (float) flake.R / 2;
      var x1 = flake.X;
      var x2 = x1 + width;
      initiator.Add(new PointF(x1, y1));
      initiator.Add(new PointF(x2, y1));
      initiator.Add(new PointF(x3, y3));
      initiator.Add(new PointF(x1, y1));

      // Draw the snowflake.
      for (var i = 1; i < initiator.Count; i++) {
        var p1 = initiator[i - 1];
        var p2 = initiator[i];

        var dx = p2.X - p1.X;
        var dy = p2.Y - p1.Y;
        var length = (float) Math.Sqrt(dx * dx + dy * dy);
        var theta = (float) Math.Atan2(dy, dx);
        DrawSnowflakeEdge(gr, flake.Depth, ref p1, theta, length);
      }
    }

    // Recursively draw a snowflake edge starting at
    // (x1, y1) in direction theta and distance dist.
    // Leave the coordinates of the endpoint in
    // (x1, y1).
    private static void DrawSnowflakeEdge(Graphics gr, int depth,
      ref PointF p1, float theta, float dist) {
      const float scaleFactor = 1 / 3f; // Make subsegments 1/3 size.
      var generatorDTheta = new List<float>();
      var piOver3 = (float) (Math.PI / 3f);
      generatorDTheta.Add(0); // Draw in the original direction.
      generatorDTheta.Add(-piOver3); // Turn -60 degrees.
      generatorDTheta.Add(2 * piOver3); // Turn 120 degrees.
      generatorDTheta.Add(-piOver3); // Turn -60 degrees.

      if (depth == 0) {
        var p2 = new PointF(
          (float) (p1.X + dist * Math.Cos(theta)),
          (float) (p1.Y + dist * Math.Sin(theta)));
        gr.DrawLine(Pens.Blue, p1, p2);
        p1 = p2;
        return;
      }

      // Recursively draw the edge.
      dist *= scaleFactor;
      for (var i = 0; i < generatorDTheta.Count; i++) {
        theta += generatorDTheta[i];
        DrawSnowflakeEdge(gr, depth - 1, ref p1, theta, dist);
      }
    }

    //Lets draw the flakes
    private void DrawFlakes(Graphics graphics, Rectangle rect) {
      foreach (var p in particles) {
        //DrawSnowflake(graphics, p);
        graphics.FillEllipse(flakesBrush, p.X, p.Y, p.R, p.R);
      }

      UpdateFlakes(rect);
    }

    //Function to move the snowflakes
    //angle will be an ongoing incremental flag. Sin and Cos functions will be applied to it to create vertical and horizontal movements of the flakes
    double snowFlakeAngle;

    private void UpdateFlakes(Rectangle rect) {
      snowFlakeAngle += 0.01;
      for (var i = 0; i < flakesCount; i++) {
        var p = particles[i];
        //Updating X and Y coordinates
        //We will add 1 to the cos function to prevent negative values which will lead flakes to move upwards
        //Every particle has its own density which can be used to make the downward movement different for each flake
        //Lets make it more random by adding in the radius
        p.Y += (float) (Math.Cos(snowFlakeAngle + p.D) + 1 + (float) p.R / 2);
        p.X += (float) Math.Sin(snowFlakeAngle) * 2;

        //Sending flakes back from the top when it exits
        //Lets make it a bit more organic and let flakes enter from the left and right also.
        if (p.X > rect.Width + 5 || p.X < -5 || p.Y > rect.Height) {
          if (i % 3 > 0) //66.67% of the flakes
          {
            particles[i] = new Snowflake {X = (float) (random.NextDouble() * rect.Width), Y = -10, R = p.R, D = p.D};
          }
          else {
            //If the flake is exitting from the right
            if (Math.Sin(snowFlakeAngle) > 0) {
              //Enter from the left
              particles[i] = new Snowflake
                {X = -5, Y = (float) (random.NextDouble() * rect.Height), R = p.R, D = p.D};
            }
            else {
              //Enter from the right
              particles[i] = new Snowflake
                {X = rect.Width + 5, Y = (float) (random.NextDouble() * rect.Height), R = p.R, D = p.D};
            }
          }
        }
      }
    }

    internal void UpdateDisplay(Graphics graphics, Rectangle rect) {
      if (colorRandomizer)
        TextColor = ColorRandomizer();
      //setting the color palette
      // graphics.Clear(BackColor);
      foreach (var shape in shapes)
        shape.DrawScreen(graphics, rect);

      // drawing clocks and figure
      if (SnowFlakesEnabled)
        DrawFlakes(graphics, rect);
      DrawTimer(graphics, rect);
    }

    private static Font CheckFontExists(string fontName, float size, FontStyle style) {
      try {
        return  new Font(fontName, size, style, GraphicsUnit.Pixel);
      }
      catch (Exception) {
        return new Font(FontFamily.GenericSansSerif, size, style);
      }
    }

    private void DrawTimer(Graphics graphics, Rectangle r, long aLeft = 0, long aTop = 0) {
      var x = (r.Right - r.Left) / 2 + aLeft;
      var y = (r.Bottom - r.Top) / 2 + aTop;

      //clock paint
      if (drawCircle) {
        graphics.DrawEllipse(linesPen, aLeft, aTop, aLeft + r.Right - r.Left, aTop + r.Bottom - r.Top);
      }

      var nowTime = DateTime.Now;
      //Label and Timer paint
      using (var textBrush = new SolidBrush(TextColor)) {
        var text = nowTime.ToString("MMM-dd dddd");
        var textSize = graphics.MeasureString(text, textFont);
        graphics.DrawString(text, textFont, textBrush,
          new PointF(x - textSize.Width / 2, (float) (r.Bottom - r.Top) / 7 + textSize.Height / 2));
        text = nowTime.ToString("HH:mm:ss");
        textSize = graphics.MeasureString(text, textFont);
        graphics.DrawString(text, textFont, textBrush,
          new PointF(x - textSize.Width / 2,
            (float) (r.Bottom - r.Top) * 5 / 7 + textSize.Height / 2));

        //clock face
        for (var i = 1; i < 13; i++) {
          text = i.ToString("##");
          textSize = graphics.MeasureString(text, textFont);
          graphics.DrawString(text, textFont, textBrush, new PointF(
            (float) (x - textSize.Width / 2 +
                     (x - aLeft) * 2.8 * Math.Sin(i * 30 * Math.PI / 180) / 3),
            (float) (y - textSize.Height / 2 -
                     (y - aTop) * 2.8 * Math.Cos(i * 30 * Math.PI / 180) / 3)));
        }
      }

      for (var i = 1; i < 61; i++) {
        if (i % 5 != 0) {
          graphics.DrawLine(linesPen,
            new PointF((float) (x + (x - aLeft - 10) * Math.Sin(i * 6 * Math.PI / 180)),
              (float) (y - (y - aTop - 10) * Math.Cos(i * 6 * Math.PI / 180))),
            new PointF((float) (x + (x - aLeft - 10) / 1.03 * Math.Sin(i * 6 * Math.PI / 180)),
              (float) (y - (y - aTop - 10) / 1.03 * Math.Cos(i * 6 * Math.PI / 180))));
        }
        else {
          graphics.DrawLine(linesPen,
            new PointF((float) (x + (x - aLeft) / 1.15 * Math.Sin(i * 6 * Math.PI / 180)),
              (float) (y - (y - aTop) / 1.15 * Math.Cos(i * 6 * Math.PI / 180))),
            new PointF((float) (x + (x - aLeft) / 1.20 * Math.Sin(i * 6 * Math.PI / 180)),
              (float) (y - (y - aTop) / 1.20 * Math.Cos(i * 6 * Math.PI / 180))));
        }
      }

      //Arrows paint
      linesPen.Width = previewMode ? 3 : 7;
      graphics.DrawEllipse(linesPen, x - 3, y - 3, 6, 6);
      //hour
      graphics.DrawLine(linesPen, new PointF(x, y),
        new PointF(
          (float) (x + (x - aLeft) / 2.5 *
            Math.Sin(((float) nowTime.Minute / 60 + nowTime.Hour) * 30 * Math.PI / 180)),
          (float) (y - (y - aTop) / 2.5 *
            Math.Cos(((float) nowTime.Minute / 60 + nowTime.Hour) * 30 * Math.PI / 180))));
      //minute
      linesPen.Width = previewMode ? 2 : 4;
      graphics.DrawLine(linesPen, new PointF(x, y),
        new PointF(
          (float) (x + (x - aLeft) / 1.5 *
            Math.Sin(((float) nowTime.Second / 60 + nowTime.Minute) * 6 * Math.PI / 180)),
          (float) (y - (y - aTop) / 1.5 *
            Math.Cos(((float) nowTime.Second / 60 + nowTime.Minute) * 6 * Math.PI / 180))));
      //second
      linesPen.Width = 1;
      graphics.DrawLine(linesPen, new PointF(x, y),
        new PointF((float) (x + (x - aLeft) / 1.1 * Math.Sin(nowTime.Second * 6 * Math.PI / 180)),
          (float) (y - (y - aTop) / 1.1 * Math.Cos(nowTime.Second * 6 * Math.PI / 180))));
    }
  }
}