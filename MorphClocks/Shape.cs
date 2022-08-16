using System;
using System.Drawing;

namespace MorphClocks {
  
  internal class Shape {
    
    private const int PointsCount = 200;

    internal struct Coords3D {
      internal float X { get; set; }
      internal float Y { get; set; }
      internal float Z { get; set; }
    }

    private enum Shapes {
      ShTriangle1,
      ShTriangle2,
      ShTriangle3,
      ShCube,
      ShPyramideTri,
      ShOct,
      ShIco,
      ShSphere1,
      ShSphere2,
      ShEgg,
      ShDodecaedr,
      ShPyramideCut,
      ShCubeCut,
      ShHeadAcke,
      ShTor,
      ShSpiral,
      ShCube2
    }

    private float vectX = (float) 0.00093; // Проекции вектора движения центра отсчета начала координат
    private float vectY = (float) 0.00111;
    private float vectZ = (float) 0.00180; // Horizontal and vertical projections of the vector of moving 3D-center

    private float vectAx = (float) 0.35; // Поворот (pi) фигуры за 1 секунду
    private float vectAy = (float) 0.25; // Rotation (pi) of the figure per 1 second
    private float vectAz = (float) 0.00;

    private int pIndex;

    private float scx, scy, scz; // Moving of the beginning of the coordinates center
    private int scrX = 400; // Абсолютные координаты относительного начала координат
    private int scrY = 300; // Absolute 2D-coordinates of coordinates center

    private float coefX, coefY; // Multiply coefficient for converting relative coordinates to absolute
    private float xa, ya, za; // Rotate angles around the beginning of the coordinates

    private readonly Coords3D[] pCoords1 = new Coords3D[PointsCount];
    private readonly Coords3D[] pCoords2 = new Coords3D[PointsCount];
    private readonly Coords3D[] points = new Coords3D[PointsCount];

    private int lastTickCount;
    private bool doUp;
    private float wait, percent;
    private readonly Random random;

    #region Config

    private readonly bool preview;

    private const float CamZ = 50; //Положение камеры(точки свода лучей) - (0, 0, CamZ) // Z-coordinate of camera - (X=0, Y=0, Z=CamZ)

    private const float ColorZ0 = (float) 1.732; // 3^0.5 Координата для расчета цвета точки // 3^0.5 Coordinate for the calculation of the color of the point

    private const int FogCoef = 62; // Коэффициент тумана / Fog coefficient
    private int waitPer = 2000; // Time of the figure transformation

    internal bool UnSortPoints { get; set; }
    internal bool Move3D { get; set; }
    internal Color BackColor { get; set; }

    #endregion Config

    internal Shape(bool preview, bool move3d, bool mixPoints, Color backColor) {
      this.preview = preview;
      random = new Random();

      BackColor = backColor;
      UnSortPoints = mixPoints;
      Move3D = move3d;
      GetRandomShape(pCoords1);
      CalcPos();
    }

    private void AddPoint(Coords3D[] coordsArr, Coords3D coords) {
      if (0 <= pIndex && pIndex <= PointsCount - 1)
        coordsArr[pIndex] = coords;
      pIndex++;
    }

    private void DupPoint(Coords3D[] coordsArr, int index) {
      if (index <= PointsCount - 1)
        AddPoint(coordsArr, coordsArr[index]);
    }

    private void AddPointsBetween(Coords3D[] coordsArr, int index1, int index2, int num) {
      if (num < 1) return;
      for (var i = 1; i <= num; i++) {
        var coords = new Coords3D {
          X = coordsArr[index1].X +
              (coordsArr[index2].X - coordsArr[index1].X) * i / (num + 1),
          Y = coordsArr[index1].Y +
              (coordsArr[index2].Y - coordsArr[index1].Y) * i / (num + 1),
          Z = coordsArr[index1].Z +
              (coordsArr[index2].Z - coordsArr[index1].Z) * i / (num + 1)
        };
        AddPoint(coordsArr, coords);
      }
    }

    private void AddPointBetween3(Coords3D[] coordsArr, Coords3D coords1, Coords3D coords2, Coords3D coords3) {
      //      1
      //     / \
      //    /   \
      // 2 ------- 3
      //      |
      //   CoordsH

      var coordsH = new Coords3D {
        X = (coords2.X + coords3.X) / 2,
        Y = (coords2.Y + coords3.Y) / 2,
        Z = (coords2.Z + coords3.Z) / 2
      };

      var coords = new Coords3D {
        X = coords1.X + (coordsH.X - coords1.X) * 2 / 3,
        Y = coords1.Y + (coordsH.Y - coords1.Y) * 2 / 3,
        Z = coords1.Z + (coordsH.Z - coords1.Z) * 2 / 3
      };

      AddPoint(coordsArr, coords);
    }

    private Coords3D Xyz(float x, float y, float z) {
      return new Coords3D {X = x, Y = y, Z = z};
    }

    private Coords3D Xyz(double x, double y, double z) {
      return Xyz((float) x, (float) y, (float) z);
    }

    #region FIGURES INITIALIZATION

    private void InitTriangle1(Coords3D[] coordsArr) {
      for (var n = 0; n <= PointsCount / 3; n++) {
        var ang = (float) n / PointsCount * 3 * Math.PI * 2;
        // pi*2 - full round
        // n/PointsCount - % of then round
        // *_* - (div _) how much points at one time
        var z = Math.Sin(2 * ang);
        AddPoint(coordsArr, Xyz(Math.Sin(ang), Math.Cos(ang), z));
        AddPoint(coordsArr, Xyz(Math.Cos(ang), z, Math.Sin(ang)));
        AddPoint(coordsArr, Xyz(z, Math.Sin(ang), Math.Cos(ang)));
      }
    }

    private void InitTriangle2(Coords3D[] coordsArr) {
      for (var n = 0; n <= PointsCount / 2; n++) {
        var ang = (float) n / PointsCount * 2 * Math.PI * 2;
        // pi*2 - full round
        // n/PointsCount - % of then round
        // *_* - how much points at one time
        var z = Math.Sin(2 * ang);
        AddPoint(coordsArr, Xyz(Math.Sin(ang) * Math.Sqrt(1 - z), Math.Cos(ang) * Math.Sqrt(1 + z), z));
        AddPoint(coordsArr,
          Xyz(Math.Sin(ang + Math.PI / 2) * Math.Sqrt(1 - z), Math.Cos(ang + Math.PI / 2) * Math.Sqrt(1 + z),
            z));
      }
    }

    private void InitTriangle3(Coords3D[] coordsArr) {
      for (var n = 0; n <= PointsCount / 2; n++) {
        var ang = (float) n / PointsCount * 4 * Math.PI * 2; // pi*2 - full round
        // n/PointsCount - % of then round
        // *_* - how much points at one time
        var z = Math.Sin(2 * ang);
        AddPoint(coordsArr, Xyz(Math.Sin(ang) * Math.Sqrt(1 - z), Math.Cos(ang) * Math.Sqrt(1 + z), z));
        AddPoint(coordsArr,
          Xyz(Math.Sin(ang + Math.PI / 2) * Math.Sqrt(1 - z), Math.Cos(ang + Math.PI / 2) * Math.Sqrt(1 + z),
            z));
        AddPoint(coordsArr, Xyz(Math.Sin(ang) * Math.Sqrt(1 + z), Math.Cos(ang) * Math.Sqrt(1 - z), z));
        AddPoint(coordsArr,
          Xyz(Math.Sin(ang + Math.PI / 2) * Math.Sqrt(1 + z), Math.Cos(ang + Math.PI / 2) * Math.Sqrt(1 - z),
            z));
      }
    }

    private void InitPyramideTri(Coords3D[] coordsArr) {
      AddPoint(coordsArr, Xyz(1, 1, 1)); // 1
      AddPoint(coordsArr, Xyz(-1, -1, 1)); // 2
      AddPoint(coordsArr, Xyz(1, -1, -1)); // 3
      AddPoint(coordsArr, Xyz(-1, 1, -1)); // 4

      AddPoint(coordsArr, Xyz(1, 1, 1)); // 1
      AddPoint(coordsArr, Xyz(-1, -1, 1)); // 2
      AddPoint(coordsArr, Xyz(1, -1, -1)); // 3
      AddPoint(coordsArr, Xyz(-1, 1, -1)); // 4

      AddPointsBetween(coordsArr, 0, 1, 32);
      AddPointsBetween(coordsArr, 1, 2, 32);
      AddPointsBetween(coordsArr, 2, 3, 32);
      AddPointsBetween(coordsArr, 0, 2, 32);
      AddPointsBetween(coordsArr, 0, 3, 32);
      AddPointsBetween(coordsArr, 1, 3, 32);
    }

    private void InitCube(Coords3D[] coordsArr) {
      AddPoint(coordsArr, Xyz(1, 1, 1)); // 0
      AddPoint(coordsArr, Xyz(-1, 1, 1)); // 1
      AddPoint(coordsArr, Xyz(1, -1, 1)); // 2
      AddPoint(coordsArr, Xyz(1, 1, -1)); // 3
      AddPoint(coordsArr, Xyz(-1, -1, 1)); // 4
      AddPoint(coordsArr, Xyz(1, -1, -1)); // 5
      AddPoint(coordsArr, Xyz(-1, 1, -1)); // 6
      AddPoint(coordsArr, Xyz(-1, -1, -1)); // 7

      AddPointsBetween(coordsArr, 0, 1, 16);
      AddPointsBetween(coordsArr, 0, 2, 16);
      AddPointsBetween(coordsArr, 0, 3, 16);
      AddPointsBetween(coordsArr, 1, 4, 16);
      AddPointsBetween(coordsArr, 1, 6, 16);
      AddPointsBetween(coordsArr, 2, 4, 16);
      AddPointsBetween(coordsArr, 2, 5, 16);
      AddPointsBetween(coordsArr, 3, 5, 16);
      AddPointsBetween(coordsArr, 3, 6, 16);
      AddPointsBetween(coordsArr, 4, 7, 16);
      AddPointsBetween(coordsArr, 5, 7, 16);
      AddPointsBetween(coordsArr, 6, 7, 16);
    }

    private void InitPlayingCube(Coords3D[] coordsArr) {
      for (var i = 0; i <= 15; i++) {
        var ang = (float) i / 16 * 2 * Math.PI;
        AddPoint(coordsArr, Xyz(1, 0.75 * Math.Cos(ang), 0.75 * Math.Sin(ang)));
        AddPoint(coordsArr, Xyz(-1, 0.75 * Math.Cos(ang), 0.75 * Math.Sin(ang)));
        AddPoint(coordsArr, Xyz(0.75 * Math.Cos(ang), 1, 0.75 * Math.Sin(ang)));
        AddPoint(coordsArr, Xyz(0.75 * Math.Cos(ang), -1, 0.75 * Math.Sin(ang)));
        AddPoint(coordsArr, Xyz(0.75 * Math.Cos(ang), 0.75 * Math.Sin(ang), 1));
        AddPoint(coordsArr, Xyz(0.75 * Math.Cos(ang), 0.75 * Math.Sin(ang), -1));
      }

      for (var i = 0; i <= 11; i++) {
        var ang = i / 12 * 2 * Math.PI;
        AddPoint(coordsArr, Xyz(0.875, 0.875 * Math.Cos(ang), 0.875 * Math.Sin(ang)));
        AddPoint(coordsArr, Xyz(-0.875, 0.875 * Math.Cos(ang), 0.875 * Math.Sin(ang)));
        AddPoint(coordsArr, Xyz(0.875 * Math.Cos(ang), 0.875, 0.875 * Math.Sin(ang)));
        AddPoint(coordsArr, Xyz(0.875 * Math.Cos(ang), -0.875, 0.875 * Math.Sin(ang)));
        AddPoint(coordsArr, Xyz(0.875 * Math.Cos(ang), 0.875 * Math.Sin(ang), 0.875)); // 7/8
        AddPoint(coordsArr, Xyz(0.875 * Math.Cos(ang), 0.875 * Math.Sin(ang), -0.875));
      }

      AddPoint(coordsArr, Xyz(0.725, 0.725, 0.725));
      AddPoint(coordsArr, Xyz(-0.725, 0.725, 0.725));
      AddPoint(coordsArr, Xyz(0.725, -0.725, 0.725));
      AddPoint(coordsArr, Xyz(0.725, 0.725, -0.725));
      AddPoint(coordsArr, Xyz(-0.725, -0.725, 0.725));
      AddPoint(coordsArr, Xyz(0.725, -0.725, -0.725));
      AddPoint(coordsArr, Xyz(-0.725, 0.725, -0.725));
      AddPoint(coordsArr, Xyz(-0.725, -0.725, -0.725));

      AddPoint(coordsArr, Xyz(0, 0, 1));

      AddPoint(coordsArr, Xyz(0.25, 1, 0.25));
      AddPoint(coordsArr, Xyz(-0.25, 1, -0.25));

      AddPoint(coordsArr, Xyz(-1, -0.25, 0.25));
      AddPoint(coordsArr, Xyz(-1, 0, 0));
      AddPoint(coordsArr, Xyz(-1, 0.25, -0.25));

      AddPoint(coordsArr, Xyz(0.25, 0.25, -1));
      AddPoint(coordsArr, Xyz(-0.25, 0.25, -1));
      AddPoint(coordsArr, Xyz(0.25, -0.25, -1));
      AddPoint(coordsArr, Xyz(-0.25, -0.25, -1));

      AddPoint(coordsArr, Xyz(1, 0.25, 0.25));
      AddPoint(coordsArr, Xyz(1, -0.25, 0.25));
      AddPoint(coordsArr, Xyz(1, 0.25, -0.25));
      AddPoint(coordsArr, Xyz(1, -0.25, -0.25));
      AddPoint(coordsArr, Xyz(1, 0, 0));

      AddPoint(coordsArr, Xyz(0, -1, 0.4));
      AddPoint(coordsArr, Xyz(-0.2, -1, 0.2));
      AddPoint(coordsArr, Xyz(-0.4, -1, 0));
      AddPoint(coordsArr, Xyz(0.4, -1, 0));
      AddPoint(coordsArr, Xyz(0.2, -1, -0.2));
      AddPoint(coordsArr, Xyz(0, -1, -0.4));

      for (var i = 0; i <= 2; i++)
        DupPoint(coordsArr, i);
    }

    private void InitOctaedr(Coords3D[] coordsArr) {
      AddPoint(coordsArr, Xyz(0, 0, 1)); // 2
      AddPoint(coordsArr, Xyz(1, 0, 0)); // 0
      AddPoint(coordsArr, Xyz(0, 1, 0)); // 1
      AddPoint(coordsArr, Xyz(-1, 0, 0)); // 3
      AddPoint(coordsArr, Xyz(0, -1, 0)); // 4
      AddPoint(coordsArr, Xyz(0, 0, -1)); // 5

      AddPoint(coordsArr, Xyz(0, 0, 1));
      AddPoint(coordsArr, Xyz(0, 0, -1));

      AddPointsBetween(coordsArr, 0, 1, 16);
      AddPointsBetween(coordsArr, 0, 2, 16);
      AddPointsBetween(coordsArr, 0, 3, 16);
      AddPointsBetween(coordsArr, 0, 4, 16);
      AddPointsBetween(coordsArr, 1, 2, 16);
      AddPointsBetween(coordsArr, 2, 3, 16);

      AddPointsBetween(coordsArr, 3, 4, 16);
      AddPointsBetween(coordsArr, 4, 1, 16);
      AddPointsBetween(coordsArr, 5, 1, 16);
      AddPointsBetween(coordsArr, 5, 2, 16);
      AddPointsBetween(coordsArr, 5, 3, 16);
      AddPointsBetween(coordsArr, 5, 4, 16);
    }

    private int Gm9(int num) {
      var result = num;
      while (result > 9)
        result -= 10;
      return result;
    }

    private void InitIcosaedr(Coords3D[] coordsArr) {
      for (var n = 0; n <= 4; n++) //0-9
      {
        var ang = (float) n / 5 * 2 * Math.PI; // 5 divisions
        AddPoint(coordsArr, Xyz(Math.Sin(ang), Math.Cos(ang), 0.5));
        AddPoint(coordsArr, Xyz(Math.Sin(ang + Math.PI / 5), Math.Cos(ang + Math.PI / 5), -0.5));
      }

      AddPoint(coordsArr, Xyz(0, 0, Math.Sqrt(5) / 2)); // 10
      AddPoint(coordsArr, Xyz(0, 0, -Math.Sqrt(5) / 2)); // 11

      for (var n = 0; n <= 9; n++) {
        AddPointsBetween(coordsArr, n, Gm9(n + 1), 6);
        AddPointsBetween(coordsArr, n, Gm9(n + 2), 6);
        AddPointsBetween(coordsArr, n, 10 + (n % 2), 6);
      }

      for (var n = 0; n <= 7; n++)
        DupPoint(coordsArr, n);
    }

    private void InitDodecaedr(Coords3D[] coordsArr) {
      var icoPoints = new Coords3D[12];
      for (var n = 0; n <= 4; n++) //0-9
      {
        var ang = (float) n / 5 * 2 * Math.PI; // 5 divisions
        icoPoints[2 * n] = Xyz(Math.Sin(ang), Math.Cos(ang), 0.5);
        icoPoints[2 * n + 1] = Xyz(Math.Sin(ang + Math.PI / 5), Math.Cos(ang + Math.PI / 5), -0.5);
      }

      icoPoints[10] = Xyz(0, 0, Math.Sqrt(5) / 2); // 10
      icoPoints[11] = Xyz(0, 0, -Math.Sqrt(5) / 2); // 11

      for (var n = 0; n <= 9; n++)
        AddPointBetween3(coordsArr, icoPoints[n], icoPoints[Gm9(n + 1)], icoPoints[Gm9(n + 2)]);

      for (var n = 0; n <= 4; n++) {
        AddPointBetween3(coordsArr, icoPoints[10], icoPoints[2 * n], icoPoints[Gm9(2 * n + 2)]);
        AddPointBetween3(coordsArr, icoPoints[11], icoPoints[2 * n + 1], icoPoints[Gm9(2 * n + 3)]);
      }

      for (var n = 0; n <= 9; n++)
        AddPointsBetween(coordsArr, n, Gm9(n + 1), 6);

      for (var n = 0; n <= 4; n++) {
        AddPointsBetween(coordsArr, 2 * n + 10, Gm9(2 * n + 2) + 10, 6);
        AddPointsBetween(coordsArr, 2 * n + 11, Gm9(2 * n + 2) + 11, 6);
      }

      for (var n = 0; n <= 9; n++)
        AddPointsBetween(coordsArr, n, n + 10, 6);
    }

    private void InitPyramideCut(Coords3D[] coordsArr) {
      AddPoint(coordsArr, Xyz(0.33, 0.33, 1)); // 0
      AddPoint(coordsArr, Xyz(1, 0.33, 0.33)); // 1
      AddPoint(coordsArr, Xyz(0.33, 1, 0.33)); // 2

      AddPoint(coordsArr, Xyz(1, -0.33, -0.33)); // 3
      AddPoint(coordsArr, Xyz(0.33, -1, -0.33)); // 4
      AddPoint(coordsArr, Xyz(0.33, -0.33, -1)); // 5

      AddPoint(coordsArr, Xyz(-0.33, -1, 0.33)); // 6
      AddPoint(coordsArr, Xyz(-1, -0.33, 0.33)); // 7
      AddPoint(coordsArr, Xyz(-0.33, -0.33, 1)); // 8

      AddPoint(coordsArr, Xyz(-1, 0.33, -0.33)); // 9
      AddPoint(coordsArr, Xyz(-0.33, 1, -0.33)); // 10
      AddPoint(coordsArr, Xyz(-0.33, 0.33, -1)); // 11

      for (var i = 0; i <= 3; i++) {
        AddPointsBetween(coordsArr, i * 3 + 0, i * 3 + 1, 10);
        AddPointsBetween(coordsArr, i * 3 + 1, i * 3 + 2, 10);
        AddPointsBetween(coordsArr, i * 3 + 0, i * 3 + 2, 10);
      }

      AddPointsBetween(coordsArr, 0, 8, 10);
      AddPointsBetween(coordsArr, 1, 3, 10);
      AddPointsBetween(coordsArr, 2, 10, 10);
      AddPointsBetween(coordsArr, 4, 6, 10);
      AddPointsBetween(coordsArr, 7, 9, 10);
      AddPointsBetween(coordsArr, 5, 11, 10);

      DupPoint(coordsArr, 0);
      DupPoint(coordsArr, 1);
      DupPoint(coordsArr, 3);
      DupPoint(coordsArr, 4);
      DupPoint(coordsArr, 6);
      DupPoint(coordsArr, 7);
      DupPoint(coordsArr, 9);
      DupPoint(coordsArr, 10);
    }

    private void InitCubeCut(Coords3D[] coordsArr) {
      AddPoint(coordsArr, Xyz(1, 0.4, 1)); // 0
      AddPoint(coordsArr, Xyz(0.4, 1, 1)); // 1
      AddPoint(coordsArr, Xyz(-0.4, 1, 1)); // 2
      AddPoint(coordsArr, Xyz(-1, 0.4, 1)); // 3
      AddPoint(coordsArr, Xyz(-1, -0.4, 1)); // 4
      AddPoint(coordsArr, Xyz(-0.4, -1, 1)); // 5
      AddPoint(coordsArr, Xyz(0.4, -1, 1)); // 6
      AddPoint(coordsArr, Xyz(1, -0.4, 1)); // 7
      AddPoint(coordsArr, Xyz(1, 1, 0.4)); // 8
      AddPoint(coordsArr, Xyz(1, 1, -0.4)); // 9
      AddPoint(coordsArr, Xyz(0.4, 1, -1)); // 10
      AddPoint(coordsArr, Xyz(-0.4, 1, -1)); // 11
      AddPoint(coordsArr, Xyz(-1, 1, -0.4)); // 12
      AddPoint(coordsArr, Xyz(-1, 1, 0.4)); // 13
      AddPoint(coordsArr, Xyz(1, -1, 0.4)); // 14
      AddPoint(coordsArr, Xyz(1, -1, -0.4)); // 15
      AddPoint(coordsArr, Xyz(1, -0.4, -1)); // 16
      AddPoint(coordsArr, Xyz(1, 0.4, -1)); // 17
      AddPoint(coordsArr, Xyz(-1, 0.4, -1)); // 18
      AddPoint(coordsArr, Xyz(-1, -0.4, -1)); // 19
      AddPoint(coordsArr, Xyz(-0.4, -1, -1)); // 20
      AddPoint(coordsArr, Xyz(0.4, -1, -1)); // 21
      AddPoint(coordsArr, Xyz(-1, -1, 0.4)); // 22
      AddPoint(coordsArr, Xyz(-1, -1, -0.4)); // 23

      AddPointsBetween(coordsArr, 0, 1, 4);
      AddPointsBetween(coordsArr, 1, 8, 4);
      AddPointsBetween(coordsArr, 8, 0, 4);
      AddPointsBetween(coordsArr, 2, 3, 4);
      AddPointsBetween(coordsArr, 3, 13, 4);
      AddPointsBetween(coordsArr, 13, 2, 4);
      AddPointsBetween(coordsArr, 4, 5, 4);
      AddPointsBetween(coordsArr, 5, 22, 4);
      AddPointsBetween(coordsArr, 22, 4, 4);
      AddPointsBetween(coordsArr, 6, 7, 4);
      AddPointsBetween(coordsArr, 7, 14, 4);
      AddPointsBetween(coordsArr, 14, 6, 4);
      AddPointsBetween(coordsArr, 11, 12, 4);
      AddPointsBetween(coordsArr, 12, 18, 4);
      AddPointsBetween(coordsArr, 18, 11, 4);
      AddPointsBetween(coordsArr, 19, 23, 4);
      AddPointsBetween(coordsArr, 23, 20, 4);
      AddPointsBetween(coordsArr, 20, 19, 4);
      AddPointsBetween(coordsArr, 15, 16, 4);
      AddPointsBetween(coordsArr, 16, 21, 4);
      AddPointsBetween(coordsArr, 21, 15, 4);
      AddPointsBetween(coordsArr, 9, 17, 4);
      AddPointsBetween(coordsArr, 17, 10, 4);
      AddPointsBetween(coordsArr, 10, 9, 4);

      AddPointsBetween(coordsArr, 1, 2, 5);
      AddPointsBetween(coordsArr, 5, 6, 5);
      AddPointsBetween(coordsArr, 20, 21, 5);
      AddPointsBetween(coordsArr, 10, 11, 5);
      AddPointsBetween(coordsArr, 3, 4, 5);
      AddPointsBetween(coordsArr, 7, 0, 5);
      AddPointsBetween(coordsArr, 16, 17, 5);
      AddPointsBetween(coordsArr, 18, 19, 5);
      AddPointsBetween(coordsArr, 12, 13, 5);
      AddPointsBetween(coordsArr, 22, 23, 5);
      AddPointsBetween(coordsArr, 14, 15, 5);
      AddPointsBetween(coordsArr, 8, 9, 5);

      for (var i = 0; i <= 19; i++)
        DupPoint(coordsArr, i);
    }

    private void InitHeadAcke(Coords3D[] coordsArr) {
      AddPoint(coordsArr, Xyz(1, 0.4, 0.2)); // 0
      AddPoint(coordsArr, Xyz(-1, 0.4, 0.2)); // 1
      AddPoint(coordsArr, Xyz(-1, -0.4, 0.2)); // 2
      AddPoint(coordsArr, Xyz(1, -0.4, 0.2)); // 3
      AddPoint(coordsArr, Xyz(1, 0.4, -0.2)); // 4
      AddPoint(coordsArr, Xyz(-1, 0.4, -0.2)); // 5
      AddPoint(coordsArr, Xyz(-1, -0.4, -0.2)); // 6
      AddPoint(coordsArr, Xyz(1, -0.4, -0.2)); // 7
      AddPoint(coordsArr, Xyz(0.4, 0.2, 1)); // 8
      AddPoint(coordsArr, Xyz(0.4, 0.2, -1)); // 9
      AddPoint(coordsArr, Xyz(-0.4, 0.2, -1)); // 10
      AddPoint(coordsArr, Xyz(-0.4, 0.2, 1)); // 11
      AddPoint(coordsArr, Xyz(0.4, -0.2, 1)); // 12
      AddPoint(coordsArr, Xyz(0.4, -0.2, -1)); // 13
      AddPoint(coordsArr, Xyz(-0.4, -0.2, -1)); // 14
      AddPoint(coordsArr, Xyz(-0.4, -0.2, 1)); // 15
      AddPoint(coordsArr, Xyz(0.2, 1, 0.4)); // 16
      AddPoint(coordsArr, Xyz(0.2, -1, 0.4)); // 17
      AddPoint(coordsArr, Xyz(0.2, -1, -0.4)); // 18
      AddPoint(coordsArr, Xyz(0.2, 1, -0.4)); // 19
      AddPoint(coordsArr, Xyz(-0.2, 1, 0.4)); // 20
      AddPoint(coordsArr, Xyz(-0.2, -1, 0.4)); // 21
      AddPoint(coordsArr, Xyz(-0.2, -1, -0.4)); // 22
      AddPoint(coordsArr, Xyz(-0.2, 1, -0.4)); // 23

      for (var i = 0; i <= 5; i++) {
        AddPointsBetween(coordsArr, 4 * i + 0, 4 * i + 1, 8);
        AddPointsBetween(coordsArr, 4 * i + 1, 4 * i + 2, 4);
        AddPointsBetween(coordsArr, 4 * i + 2, 4 * i + 3, 8);
        AddPointsBetween(coordsArr, 4 * i + 3, 4 * i + 0, 4);
      }

      for (var i = 0; i <= 2; i++) {
        AddPointsBetween(coordsArr, 8 * i + 0, 8 * i + 4, 2);
        AddPointsBetween(coordsArr, 8 * i + 1, 8 * i + 5, 2);
        AddPointsBetween(coordsArr, 8 * i + 2, 8 * i + 6, 2);
        AddPointsBetween(coordsArr, 8 * i + 3, 8 * i + 7, 2);
      }

      for (var i = 0; i <= 7; i++)
        DupPoint(coordsArr, i);
    }

    private void InitSphere1(Coords3D[] coordsArr) {
      for (var nang = -9; nang <= 10; nang++) {
        var anga = (nang - 0.5) / 19 * Math.PI;
        var z = Math.Sin(anga);
        for (var nokr = 0; nokr <= 9; nokr++) {
          var ango = (float) nokr / 10 * Math.PI * 2;
          AddPoint(coordsArr,
            Xyz(Math.Sin(ango) * Math.Sqrt(1 - z * z), Math.Cos(ango) * Math.Sqrt(1 - z * z), z));
        }
      }
    }

    private void InitSphere2(Coords3D[] coordsArr) {
      for (var nang = -4; nang <= 5; nang++) {
        var anga = (nang - 0.5) / 10 * Math.PI;
        var z = Math.Sin(anga);
        for (var nokr = 0; nokr <= 19; nokr++) {
          var ango = (float) nokr / 20 * Math.PI * 2;
          AddPoint(coordsArr,
            Xyz(Math.Sin(ango) * Math.Sqrt(1 - z * z), Math.Cos(ango) * Math.Sqrt(1 - z * z), z));
        }
      }
    }

    private void InitEgg(Coords3D[] coordsArr) {
      for (var nsl = -10; nsl <= 9; nsl++) {
        float z;
        float r;
        if (nsl <= -4) {
          var angs = (float) (nsl + 4) / 6 * Math.PI / 2;
          z = (float) (Math.Sin(angs) / 2 - 0.5);
          r = (float) Math.Cos(angs);
        }
        else {
          var angs = (float) (nsl + 4) / 14 * Math.PI / 2;
          z = (float) (Math.Sin(angs) * 1.5 - 0.5);
          r = (float) Math.Cos(angs);
        }

        for (var nokr = 0; nokr <= 9; nokr++) {
          var ango = (float) nokr / 10 * Math.PI * 2;
          AddPoint(coordsArr, Xyz(Math.Sin(ango) * r / 1.5, Math.Cos(ango) * r / 1.5, z));
        }
      }
    }

    private void InitTor(Coords3D[] coordsArr) {
      for (var n = 0; n <= (PointsCount / 10) - 1; n++) {
        var ang = (float) n / PointsCount * 10 * 2 * Math.PI;
        for (var k = 0; k <= 9; k++) {
          var r = 1 + 0.33 * Math.Cos((float) k / 10 * 2 * Math.PI);
          var z = 0.33 * Math.Sin((float) k / 10 * 2 * Math.PI);
          var x = r * Math.Cos(ang);
          var y = r * Math.Sin(ang);
          AddPoint(coordsArr, Xyz(x, y, z));
        }
      }
    }

    private void InitSpiral(Coords3D[] coordsArr) {
      for (var n = 0; n <= PointsCount - 1; n++) {
        var angm = (float) n / PointsCount * 2 * Math.PI;
        var ang = angm * 16;
        var z = 0.33 * Math.Sin(ang);
        var r = 1 + 0.33 * Math.Cos(ang);
        var x = r * Math.Cos(angm);
        var y = r * Math.Sin(angm);
        AddPoint(coordsArr, Xyz(x, y, z));
      }
    }

    private void UnSort(Coords3D[] coordsArr) {
      for (var i = 1; i <= 1024; i++) {
        var k = random.Next(PointsCount);
        var l = random.Next(PointsCount);
        var temp = coordsArr[k];
        coordsArr[k] = coordsArr[l];
        coordsArr[l] = temp;
      }
    }

    private void CalcPos() {
      for (var n = 0; n <= PointsCount - 1; n++) {
        points[n].X = pCoords1[n].X + (pCoords2[n].X - pCoords1[n].X) * percent / 100;
        points[n].Y = pCoords1[n].Y + (pCoords2[n].Y - pCoords1[n].Y) * percent / 100;
        points[n].Z = pCoords1[n].Z + (pCoords2[n].Z - pCoords1[n].Z) * percent / 100;
      }
    }

    private void GetRandomShape(Coords3D[] coordsArr) {
      pIndex = 0;

      //check all shaper registered
      var possibleShapes = (Shapes[]) Enum.GetValues(typeof(Shapes));
      var shapeInd = random.Next(possibleShapes.Length - 1);

      switch (possibleShapes[shapeInd]) {
        case Shapes.ShTriangle1:
          InitTriangle1(coordsArr);
          break;
        case Shapes.ShTriangle2:
          InitTriangle2(coordsArr);
          break;
        case Shapes.ShTriangle3:
          InitTriangle3(coordsArr);
          break;
        case Shapes.ShCube:
          InitCube(coordsArr);
          break;
        case Shapes.ShPyramideTri:
          InitPyramideTri(coordsArr);
          break;
        case Shapes.ShOct:
          InitOctaedr(coordsArr);
          break;
        case Shapes.ShIco:
          InitIcosaedr(coordsArr);
          break;
        case Shapes.ShSphere1:
          InitSphere1(coordsArr);
          break;
        case Shapes.ShSphere2:
          InitSphere2(coordsArr);
          break;
        case Shapes.ShEgg:
          InitEgg(coordsArr);
          break;
        case Shapes.ShDodecaedr:
          InitDodecaedr(coordsArr);
          break;
        case Shapes.ShPyramideCut:
          InitPyramideCut(coordsArr);
          break;
        case Shapes.ShCubeCut:
          InitCubeCut(coordsArr);
          break;
        case Shapes.ShHeadAcke:
          InitHeadAcke(coordsArr);
          break;
        case Shapes.ShTor:
          InitTor(coordsArr);
          break;
        case Shapes.ShSpiral:
          InitSpiral(coordsArr);
          break;
        case Shapes.ShCube2:
          InitPlayingCube(coordsArr);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }

      if (UnSortPoints)
        UnSort(coordsArr); //mix points
    }

    #endregion FIGURES INITIALIZATION

    #region Screen Saver's engine

    private void DrawPoint(Graphics graphics, Point point, Color color) {
      using (var pen = new Pen(color, preview ? 1 : 2)) {
        graphics.DrawEllipse(pen, point.X, point.Y, preview ? 1 : 3, preview ? 1 : 3);
      }
    }

    private Point GetCoords2D(Coords3D coords3D) {
      var result = new Point();
      var zNorm = 1 - (coords3D.Z + scz) / CamZ;
      if (zNorm != 0) {
        result.X = (int) ((coords3D.X + scx) / zNorm * coefX) + scrX;
        result.Y = (int) ((coords3D.Y + scy) / zNorm * coefY) + scrY;
      }

      return result;
    }

    private Coords3D Rotate3D(Coords3D coords3D) {
      var result = new Coords3D();
      float sina;
      float cosa;
      if (xa != 0) {
        sina = (float) Math.Sin(xa);
        cosa = (float) Math.Cos(xa);
        result.X = coords3D.X;
        result.Y = coords3D.Y * cosa - coords3D.Z * sina;
        result.Z = coords3D.Y * sina + coords3D.Z * cosa;

        coords3D.X = result.X;
        coords3D.Y = result.Y;
        coords3D.Z = result.Z;
      }

      if (ya != 0) {
        sina = (float) Math.Sin(ya);
        cosa = (float) Math.Cos(ya);

        result.X = coords3D.X * cosa + coords3D.Z * sina;
        result.Y = coords3D.Y;
        result.Z = -coords3D.X * sina + coords3D.Z * cosa;

        coords3D.X = result.X;
        coords3D.Y = result.Y;
        coords3D.Z = result.Z;
      }

      if (za != 0) {
        sina = (float) Math.Sin(za);
        cosa = (float) Math.Cos(za);
        result.X = coords3D.X * cosa - coords3D.Y * sina;
        result.Y = coords3D.X * sina + coords3D.Y * cosa;
        result.Z = coords3D.Z;

        coords3D.X = result.X;
        coords3D.Y = result.Y;
        coords3D.Z = result.Z;
      }

      result.X = coords3D.X;
      result.Y = coords3D.Y;
      result.Z = coords3D.Z;

      return result;
    }

    private Color GetColor(Coords3D coords3D) {
      var len = Math.Sqrt(Math.Pow(coords3D.X - 0, 2) + Math.Pow(coords3D.Y - 0, 2) +
                          Math.Pow(coords3D.Z - ColorZ0, 2));
      var gr = (int) Math.Abs((BackColor.ToArgb() & 0xFFFFFF) - len * FogCoef) & 0xFF;
      //  Gr := Trunc(255-Len*FogCoef);
      //  If Gr<0 then Gr := 0;
      return Color.FromArgb(gr, gr, gr); // Translation RGB to the hue of gray
    }

    #endregion

    internal void DrawScreen(Graphics graphics, Rectangle rect) {
      scrX = rect.Right / 2;
      scrY = rect.Bottom / 2;
      coefX = (float) rect.Right / 8;
      coefY = (float) rect.Bottom / 6;

      const int minTimeDelta = 10;
      const int maxTimeDelta = 200;

      var timeDelta = Environment.TickCount - lastTickCount;
      if (timeDelta > maxTimeDelta)
        timeDelta = maxTimeDelta;
      if (timeDelta < minTimeDelta)
        timeDelta = minTimeDelta;
      lastTickCount = Environment.TickCount;

      if (wait > 0)
        wait = wait - timeDelta;
      else {
        if (doUp) {
          percent = percent + (float) timeDelta / 10;
          if (percent >= 100) {
            percent = 100;
            doUp = false;
            wait = waitPer;
            GetRandomShape(pCoords1);
          }
        }
        else {
          percent = percent - (float) timeDelta / 10;
          if (percent <= 0) {
            percent = 0;
            doUp = true;
            wait = waitPer;
            GetRandomShape(pCoords2);
          }
        }

        CalcPos();
      }

      xa = (float) (xa + timeDelta * Math.PI * vectAx / 1000);
      ya = (float) (ya + timeDelta * Math.PI * vectAy / 1000);
      za = (float) (za - timeDelta * Math.PI * vectAz / 1000);

      scx = scx + vectX * timeDelta;
      if (scx > 3.5 - scz / 2.5 || (scx > 2.75 && !Move3D)) {
        vectX = -Math.Abs(vectX);
        vectAy = (float) -Math.Abs(random.NextDouble() / 3 + 0.25);
      }

      if (scx < -3.5 + scz / 2.5 || scx < -2.75 && !Move3D) {
        vectX = Math.Abs(vectX);
        vectAy = (float) Math.Abs(random.NextDouble() / 3 + 0.25);
      }

      scy = scy + vectY * timeDelta;
      if (scy > 3 - scz / 3 || scy > 1.8 && !Move3D) {
        vectY = -Math.Abs(vectY);
        vectAx = (float) -Math.Abs(random.NextDouble() / 3 + 0.25);
      }

      if (scy < -3 + scz / 3 || scy < -1.8 && !Move3D) {
        vectY = Math.Abs(vectY);
        vectAx = (float) Math.Abs(random.NextDouble() / 3 + 0.25);
      }

      if (Move3D) {
        scz = scz + vectZ * timeDelta;
        if (scz > 4) {
          vectZ = -Math.Abs(vectZ);
          vectAx = (float) -Math.Abs(random.NextDouble() / 3 + 0.25);
          vectAy = (float) -Math.Abs(random.NextDouble() / 3 + 0.25);
        }

        if (scz < -10) {
          vectZ = Math.Abs(vectZ);
          vectAx = (float) Math.Abs(random.NextDouble() / 3 + 0.25);
          vectAy = (float) Math.Abs(random.NextDouble() / 3 + 0.25);
        }
      }

      for (var n = 0; n <= PointsCount - 1; n++) {
        var point = Rotate3D(points[n]);
        var color = GetColor(point);
        DrawPoint(graphics, GetCoords2D(point), color);
      }
    }
  }
}