using System;
using System.Drawing;

namespace MorphClocks
{
    public class Shape
    {
        private const int PointsCount = 200;

        public struct Coords3D
        {
            public double X { get; set; }
            public double Y { get; set; }
            public double Z { get; set; }
        }

        private enum Shapes
        {
            shTriangle1,
            shTriangle2,
            shTriangle3,
            shCube,
            shPyramideTri,
            shOct,
            shIco,
            shSphere1,
            shSphere2,
            shEgg,
            shDodecaedr,
            shPyramideCut,
            shCubeCut,
            shHeadAcke,
            shTor,
            shSpiral,
            shCube2
        }

        private double VectX = 0.00093; // Проекции вектора движения центра отсчета начала координат
        private double VectY = 0.00111;
        private double VectZ = 0.00180; // Horizontal and vertical projections of the vector of moving 3D-center

        private double VectAX = 0.35; // Поворот (pi) фигуры за 1 секунду
        private double VectAY = 0.25; // Rotation (pi) of the figure per 1 second
        private double VectAZ = 0.00;

        private readonly Random _random;

        private int _pIndex;

        private double _scx; // Относительное смещение начала координат
        private double _scy; // Moving of the beginning of the coordinates center
        private double _scz;

        private int _scrX = 400; // Абсолютные координаты относительного начала координат
        private int _scrY = 300; // Absolute 2D-coordinates of coordinates center

        private double _coefX; // Коэффициент умножения - перевод относительных
        private double _coefY; // координат в абсолютные / Multiply coefficient for counting absolute coordinates
        private double _xa, _ya, _za; // Углы поворота вокруг начала координат // Rotate angles around the beginning of the coordinates

        private readonly Coords3D[] _pCoords1 = new Coords3D[PointsCount];
        private readonly Coords3D[] _pCoords2 = new Coords3D[PointsCount];
        private readonly Coords3D[] _points = new Coords3D[PointsCount];

        private int LastTickCount;

        private readonly bool _preview;

        private bool DoUp;
        private double Wait, Percent;

        #region Config

        private const double CamZ = 10;        // Положение камеры(точки свода лучей) - (0, 0, CamZ) // Z-coordinate of camera - (X=0, Y=0, Z=CamZ)
        private const double ColorZ0 = 1.732;  // 3^0.5 Координата для расчета цвета точки // 3^0.5 Coordinate for the calculation of the color of the point
        private const int FogCoef = 62;     // Коэффициент тумана / Fog coefficient
        private int WaitPer = 2000; // Time of the figure transformation

        public bool ShowFPS { get; set; }
        readonly bool UnSortPoints;
        readonly bool Move3D;

        public Color BackColor { get; set; }

        public Shape(bool preview, bool move3d, bool mixPoints)
        {
            _preview = preview;
            _random = new Random();

            BackColor = Color.Black;
            UnSortPoints = mixPoints;
            Move3D = move3d;
            InitShape(_pCoords1);
            CalcPos();
        }

        #endregion Config

        private void AddPoint(Coords3D[] CoordsArr, Coords3D Coords)
        {
            if(0<=_pIndex && _pIndex<=PointsCount-1) 
                CoordsArr[_pIndex] = Coords;
            _pIndex++;
        }

        private void DupPoint(Coords3D[] CoordsArr, int Index)
        {
            if (Index <= PointsCount - 1)
                AddPoint(CoordsArr, CoordsArr[Index]);
        }

        private void AddPointsBetween(Coords3D[] coordsArr, int index1, int index2, int num)
        {
            if (num == -1) return;
            for (var i = 1; i <= num; i++)
            {
                var coords = new Coords3D
                             {
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

        private void AddPointBetween3(Coords3D[] CoordsArr, Coords3D Coords1, Coords3D Coords2, Coords3D Coords3)
        {
            //      1
            //     / \
            //    /   \
            // 2 ------- 3
            //      |
            //   CoordsH

            var coordsH = new Coords3D
                          {
                              X = (Coords2.X + Coords3.X) / 2,
                              Y = (Coords2.Y + Coords3.Y) / 2,
                              Z = (Coords2.Z + Coords3.Z) / 2
                          };

            var coords = new Coords3D
                         {
                             X = Coords1.X + (coordsH.X - Coords1.X) * 2 / 3,
                             Y = Coords1.Y + (coordsH.Y - Coords1.Y) * 2 / 3,
                             Z = Coords1.Z + (coordsH.Z - Coords1.Z) * 2 / 3
                         };

            AddPoint(CoordsArr, coords);
        }

        private Coords3D XYZ(double X, double Y, double Z)
        {
            return new Coords3D { X = X, Y = Y, Z = Z };
        }

        #region FIGURES INITIALIZATION

        private void InitTriangle1(Coords3D[] coordsArr) // кривая 1/ curve 1
        {
            for (var n = 0; n <= PointsCount / 3; n++)
            {
                var ang = (double)n / PointsCount * 3 * Math.PI * 2;
                // pi*2 - полная окружность / full round
                // n/PointsCount - % круга / of then round
                // *_* - сколько точек за раз (div _) / how much points at one time
                var z = Math.Sin(2 * ang);
                AddPoint(coordsArr, XYZ(Math.Sin(ang), Math.Cos(ang), z));
                AddPoint(coordsArr, XYZ(Math.Cos(ang), z, Math.Sin(ang)));
                AddPoint(coordsArr, XYZ(z, Math.Sin(ang), Math.Cos(ang)));
            }
        }

        private void InitTriangle2(Coords3D[] CoordsArr) // кривая 2 / curve 2
        {
            for (var n = 0; n <= PointsCount / 2; n++)
            {
                var ang = (double)n / PointsCount * 2 * Math.PI * 2; 
                // pi*2 - полная окружность / full round
                // n/PointsCount - % круга / of then round
                // *_* - сколько точек за раз (div _) / how much points at one time
                var z = Math.Sin(2 * ang);
                AddPoint(CoordsArr, XYZ(Math.Sin(ang) * Math.Sqrt(1 - z), Math.Cos(ang) * Math.Sqrt(1 + z), z));
                AddPoint(CoordsArr, XYZ(Math.Sin(ang + Math.PI / 2) * Math.Sqrt(1 - z), Math.Cos(ang + Math.PI / 2) * Math.Sqrt(1 + z), z));
            }
        }

        private void InitTriangle3(Coords3D[] CoordsArr) // кривая 3 / curve 3
        {
            for (var n = 0; n <= PointsCount / 2; n++)
            {
                var ang = (double)n / PointsCount * 4 * Math.PI * 2; // pi*2 - полная окружность / full round
                // n/PointsCount - % круга / of then round
                // *_* - сколько точек за раз (div _) / how much points at one time
                var z = Math.Sin(2 * ang);
                AddPoint(CoordsArr, XYZ(Math.Sin(ang) * Math.Sqrt(1 - z), Math.Cos(ang) * Math.Sqrt(1 + z), z));
                AddPoint(CoordsArr, XYZ(Math.Sin(ang + Math.PI / 2) * Math.Sqrt(1 - z), Math.Cos(ang + Math.PI / 2) * Math.Sqrt(1 + z), z));
                AddPoint(CoordsArr, XYZ(Math.Sin(ang) * Math.Sqrt(1 + z), Math.Cos(ang) * Math.Sqrt(1 - z), z));
                AddPoint(CoordsArr, XYZ(Math.Sin(ang + Math.PI / 2) * Math.Sqrt(1 + z), Math.Cos(ang + Math.PI / 2) * Math.Sqrt(1 - z), z));
            }
        }

        private void InitPyramideTri(Coords3D[] CoordsArr) // тетраэдр / tetraedr
        {
            AddPoint(CoordsArr, XYZ(1, 1, 1));    // 1
            AddPoint(CoordsArr, XYZ(-1, -1, 1)); // 2
            AddPoint(CoordsArr, XYZ(1, -1, -1)); // 3
            AddPoint(CoordsArr, XYZ(-1, 1, -1));  // 4

            AddPoint(CoordsArr, XYZ(1, 1, 1));    // 1
            AddPoint(CoordsArr, XYZ(-1, -1, 1)); // 2
            AddPoint(CoordsArr, XYZ(1, -1, -1)); // 3
            AddPoint(CoordsArr, XYZ(-1, 1, -1));  // 4

            AddPointsBetween(CoordsArr, 0, 1, 32);
            AddPointsBetween(CoordsArr, 1, 2, 32);
            AddPointsBetween(CoordsArr, 2, 3, 32);
            AddPointsBetween(CoordsArr, 0, 2, 32);
            AddPointsBetween(CoordsArr, 0, 3, 32);
            AddPointsBetween(CoordsArr, 1, 3, 32);
        }

        private void InitCube(Coords3D[] CoordsArr) // гексаэдр, куб / cube
        {
            AddPoint(CoordsArr, XYZ(1, 1, 1)); // 0
            AddPoint(CoordsArr, XYZ(-1, 1, 1)); // 1
            AddPoint(CoordsArr, XYZ(1, -1, 1)); // 2
            AddPoint(CoordsArr, XYZ(1, 1, -1)); // 3
            AddPoint(CoordsArr, XYZ(-1, -1, 1)); // 4
            AddPoint(CoordsArr, XYZ(1, -1, -1)); // 5
            AddPoint(CoordsArr, XYZ(-1, 1, -1)); // 6
            AddPoint(CoordsArr, XYZ(-1, -1, -1)); // 7

            AddPointsBetween(CoordsArr, 0, 1, 16);
            AddPointsBetween(CoordsArr, 0, 2, 16);
            AddPointsBetween(CoordsArr, 0, 3, 16);
            AddPointsBetween(CoordsArr, 1, 4, 16);
            AddPointsBetween(CoordsArr, 1, 6, 16);
            AddPointsBetween(CoordsArr, 2, 4, 16);
            AddPointsBetween(CoordsArr, 2, 5, 16);
            AddPointsBetween(CoordsArr, 3, 5, 16);
            AddPointsBetween(CoordsArr, 3, 6, 16);
            AddPointsBetween(CoordsArr, 4, 7, 16);
            AddPointsBetween(CoordsArr, 5, 7, 16);
            AddPointsBetween(CoordsArr, 6, 7, 16);
        }

        private void InitPlayingCube(Coords3D[] CoordsArr) // Игральный кубик / Play cube
        {
            for (var i = 0; i <= 15; i++)
            {
                var ang = (double) i / 16 * 2 * Math.PI;
                AddPoint(CoordsArr, XYZ(1, 0.75 * Math.Cos(ang), 0.75 * Math.Sin(ang)));
                AddPoint(CoordsArr, XYZ(-1, 0.75 * Math.Cos(ang), 0.75 * Math.Sin(ang)));
                AddPoint(CoordsArr, XYZ(0.75 * Math.Cos(ang), 1, 0.75 * Math.Sin(ang)));
                AddPoint(CoordsArr, XYZ(0.75 * Math.Cos(ang), -1, 0.75 * Math.Sin(ang)));
                AddPoint(CoordsArr, XYZ(0.75 * Math.Cos(ang), 0.75 * Math.Sin(ang), 1));
                AddPoint(CoordsArr, XYZ(0.75 * Math.Cos(ang), 0.75 * Math.Sin(ang), -1));
            }

            for (var i = 0; i <= 11; i++)
            {
                var ang = i / 12 * 2 * Math.PI;
                AddPoint(CoordsArr, XYZ(0.875, 0.875 * Math.Cos(ang), 0.875 * Math.Sin(ang)));
                AddPoint(CoordsArr, XYZ(-0.875, 0.875 * Math.Cos(ang), 0.875 * Math.Sin(ang)));
                AddPoint(CoordsArr, XYZ(0.875 * Math.Cos(ang), 0.875, 0.875 * Math.Sin(ang)));
                AddPoint(CoordsArr, XYZ(0.875 * Math.Cos(ang), -0.875, 0.875 * Math.Sin(ang)));
                AddPoint(CoordsArr, XYZ(0.875 * Math.Cos(ang), 0.875 * Math.Sin(ang), 0.875)); // 7/8
                AddPoint(CoordsArr, XYZ(0.875 * Math.Cos(ang), 0.875 * Math.Sin(ang), -0.875));
            }

            AddPoint(CoordsArr, XYZ(0.725, 0.725, 0.725));
            AddPoint(CoordsArr, XYZ(-0.725, 0.725, 0.725));
            AddPoint(CoordsArr, XYZ(0.725, -0.725, 0.725));
            AddPoint(CoordsArr, XYZ(0.725, 0.725, -0.725));
            AddPoint(CoordsArr, XYZ(-0.725, -0.725, 0.725));
            AddPoint(CoordsArr, XYZ(0.725, -0.725, -0.725));
            AddPoint(CoordsArr, XYZ(-0.725, 0.725, -0.725));
            AddPoint(CoordsArr, XYZ(-0.725, -0.725, -0.725));

            AddPoint(CoordsArr, XYZ(0, 0, 1));

            AddPoint(CoordsArr, XYZ(0.25, 1, 0.25));
            AddPoint(CoordsArr, XYZ(-0.25, 1, -0.25));

            AddPoint(CoordsArr, XYZ(-1, -0.25, 0.25));
            AddPoint(CoordsArr, XYZ(-1, 0, 0));
            AddPoint(CoordsArr, XYZ(-1, 0.25, -0.25));

            AddPoint(CoordsArr, XYZ(0.25, 0.25, -1));
            AddPoint(CoordsArr, XYZ(-0.25, 0.25, -1));
            AddPoint(CoordsArr, XYZ(0.25, -0.25, -1));
            AddPoint(CoordsArr, XYZ(-0.25, -0.25, -1));

            AddPoint(CoordsArr, XYZ(1, 0.25, 0.25));
            AddPoint(CoordsArr, XYZ(1, -0.25, 0.25));
            AddPoint(CoordsArr, XYZ(1, 0.25, -0.25));
            AddPoint(CoordsArr, XYZ(1, -0.25, -0.25));
            AddPoint(CoordsArr, XYZ(1, 0, 0));

            AddPoint(CoordsArr, XYZ(0, -1, 0.4));
            AddPoint(CoordsArr, XYZ(-0.2, -1, 0.2));
            AddPoint(CoordsArr, XYZ(-0.4, -1, 0));
            AddPoint(CoordsArr, XYZ(0.4, -1, 0));
            AddPoint(CoordsArr, XYZ(0.2, -1, -0.2));
            AddPoint(CoordsArr, XYZ(0, -1, -0.4));

            for (var i = 0; i<=2; i++)
                DupPoint(CoordsArr, i);
        }

        private void InitOctaedr(Coords3D[] CoordsArr) // октаэдр / octaedr
        {
            AddPoint(CoordsArr, XYZ(0, 0, 1)); // 2
            AddPoint(CoordsArr, XYZ(1, 0, 0)); // 0
            AddPoint(CoordsArr, XYZ(0, 1, 0)); // 1
            AddPoint(CoordsArr, XYZ(-1, 0, 0)); // 3
            AddPoint(CoordsArr, XYZ(0, -1, 0)); // 4
            AddPoint(CoordsArr, XYZ(0, 0, -1)); // 5

            AddPoint(CoordsArr, XYZ(0, 0, 1));
            AddPoint(CoordsArr, XYZ(0, 0, -1));

            AddPointsBetween(CoordsArr, 0, 1, 16);
            AddPointsBetween(CoordsArr, 0, 2, 16);
            AddPointsBetween(CoordsArr, 0, 3, 16);
            AddPointsBetween(CoordsArr, 0, 4, 16);
            AddPointsBetween(CoordsArr, 1, 2, 16);
            AddPointsBetween(CoordsArr, 2, 3, 16);

            AddPointsBetween(CoordsArr, 3, 4, 16);
            AddPointsBetween(CoordsArr, 4, 1, 16);
            AddPointsBetween(CoordsArr, 5, 1, 16);
            AddPointsBetween(CoordsArr, 5, 2, 16);
            AddPointsBetween(CoordsArr, 5, 3, 16);
            AddPointsBetween(CoordsArr, 5, 4, 16);
        }

        private int GM9(int num)
        {
            var result = num;
            while (result > 9)
                result -= 10;
            return result;
        }

        private void InitIcosaedr(Coords3D[] CoordsArr) // икосаэдр / icosaedr
        {
            for (var n = 0; n <= 4; n++) //0-9
            {
                var ang = (double) n / 5 * 2 * Math.PI; // 5 делений / 5 divisions
                AddPoint(CoordsArr, XYZ(Math.Sin(ang), Math.Cos(ang), 0.5));
                AddPoint(CoordsArr, XYZ(Math.Sin(ang + Math.PI / 5), Math.Cos(ang + Math.PI / 5), -0.5));
            }

            AddPoint(CoordsArr, XYZ(0, 0, Math.Sqrt(5) / 2));  // 10
            AddPoint(CoordsArr, XYZ(0, 0, -Math.Sqrt(5) / 2)); // 11

            for (var n = 0; n <= 9; n++)
            {
                AddPointsBetween(CoordsArr, n, GM9(n + 1), 6);
                AddPointsBetween(CoordsArr, n, GM9(n + 2), 6);
                AddPointsBetween(CoordsArr, n, 10 + (n % 2), 6);
            }
            for (var n = 0; n <= 7; n++)
                DupPoint(CoordsArr, n);
        }

        private void InitDodecaedr(Coords3D[] CoordsArr) // додекаэдр / dodecaedr
        {
            var IcoPoints = new Coords3D[12];
            for (var n = 0; n <= 4; n++) //0-9
            {
                var ang = (double) n / 5 * 2 * Math.PI; // 5 делений / 5 divisions
                IcoPoints[2 * n] = XYZ(Math.Sin(ang), Math.Cos(ang), 0.5);
                IcoPoints[2 * n + 1] = XYZ(Math.Sin(ang + Math.PI / 5), Math.Cos(ang + Math.PI / 5), -0.5);
            }

            IcoPoints[10] = XYZ(0, 0, Math.Sqrt(5) / 2);  // 10
            IcoPoints[11] = XYZ(0, 0, -Math.Sqrt(5) / 2); // 11

            for (var n = 0; n <= 9; n++)
                AddPointBetween3(CoordsArr, IcoPoints[n], IcoPoints[GM9(n + 1)], IcoPoints[GM9(n + 2)]);

            for (var n = 0; n <= 4; n++)
            {
                AddPointBetween3(CoordsArr, IcoPoints[10], IcoPoints[2 * n], IcoPoints[GM9(2 * n + 2)]);
                AddPointBetween3(CoordsArr, IcoPoints[11], IcoPoints[2 * n + 1], IcoPoints[GM9(2 * n + 3)]);
            }

            for (var n = 0; n <= 9; n++)
                AddPointsBetween(CoordsArr, n, GM9(n + 1), 6);

            for (var n = 0; n <= 4; n++)
            {
                AddPointsBetween(CoordsArr, 2 * n + 10, GM9(2 * n + 2) + 10, 6);
                AddPointsBetween(CoordsArr, 2 * n + 11, GM9(2 * n + 2) + 11, 6);
            }

            for (var n = 0; n <= 9; n++)
                AddPointsBetween(CoordsArr, n, n + 10, 6);
        }

        private void InitPyramideCut(Coords3D[] CoordsArr)
        {
            AddPoint(CoordsArr, XYZ(0.33, 0.33, 1));   // 0
            AddPoint(CoordsArr, XYZ(1, 0.33, 0.33));   // 1
            AddPoint(CoordsArr, XYZ(0.33, 1, 0.33));   // 2

            AddPoint(CoordsArr, XYZ(1, -0.33, -0.33)); // 3
            AddPoint(CoordsArr, XYZ(0.33, -1, -0.33)); // 4
            AddPoint(CoordsArr, XYZ(0.33, -0.33, -1)); // 5

            AddPoint(CoordsArr, XYZ(-0.33, -1, 0.33)); // 6
            AddPoint(CoordsArr, XYZ(-1, -0.33, 0.33)); // 7
            AddPoint(CoordsArr, XYZ(-0.33, -0.33, 1)); // 8

            AddPoint(CoordsArr, XYZ(-1, 0.33, -0.33)); // 9
            AddPoint(CoordsArr, XYZ(-0.33, 1, -0.33)); // 10
            AddPoint(CoordsArr, XYZ(-0.33, 0.33, -1)); // 11

            for (var i = 0; i <= 3; i++)
            {
                AddPointsBetween(CoordsArr, i * 3 + 0, i * 3 + 1, 10);
                AddPointsBetween(CoordsArr, i * 3 + 1, i * 3 + 2, 10);
                AddPointsBetween(CoordsArr, i * 3 + 0, i * 3 + 2, 10);
            }

            AddPointsBetween(CoordsArr, 0, 8, 10);
            AddPointsBetween(CoordsArr, 1, 3, 10);
            AddPointsBetween(CoordsArr, 2, 10, 10);
            AddPointsBetween(CoordsArr, 4, 6, 10);
            AddPointsBetween(CoordsArr, 7, 9, 10);
            AddPointsBetween(CoordsArr, 5, 11, 10);

            DupPoint(CoordsArr, 0);
            DupPoint(CoordsArr, 1);
            DupPoint(CoordsArr, 3);
            DupPoint(CoordsArr, 4);
            DupPoint(CoordsArr, 6);
            DupPoint(CoordsArr, 7);
            DupPoint(CoordsArr, 9);
            DupPoint(CoordsArr, 10);
        }

        private void InitCubeCut(Coords3D[] CoordsArr)
        {
            AddPoint(CoordsArr, XYZ(1, 0.4, 1));   // 0
            AddPoint(CoordsArr, XYZ(0.4, 1, 1));   // 1
            AddPoint(CoordsArr, XYZ(-0.4, 1, 1));  // 2
            AddPoint(CoordsArr, XYZ(-1, 0.4, 1));  // 3
            AddPoint(CoordsArr, XYZ(-1, -0.4, 1)); // 4
            AddPoint(CoordsArr, XYZ(-0.4, -1, 1)); // 5
            AddPoint(CoordsArr, XYZ(0.4, -1, 1));  // 6
            AddPoint(CoordsArr, XYZ(1, -0.4, 1));  // 7
            AddPoint(CoordsArr, XYZ(1, 1, 0.4));    // 8
            AddPoint(CoordsArr, XYZ(1, 1, -0.4));   // 9
            AddPoint(CoordsArr, XYZ(0.4, 1, -1));   // 10
            AddPoint(CoordsArr, XYZ(-0.4, 1, -1));  // 11
            AddPoint(CoordsArr, XYZ(-1, 1, -0.4));  // 12
            AddPoint(CoordsArr, XYZ(-1, 1, 0.4));   // 13
            AddPoint(CoordsArr, XYZ(1, -1, 0.4));  // 14
            AddPoint(CoordsArr, XYZ(1, -1, -0.4)); // 15
            AddPoint(CoordsArr, XYZ(1, -0.4, -1)); // 16
            AddPoint(CoordsArr, XYZ(1, 0.4, -1));  // 17
            AddPoint(CoordsArr, XYZ(-1, 0.4, -1));   // 18
            AddPoint(CoordsArr, XYZ(-1, -0.4, -1));  // 19
            AddPoint(CoordsArr, XYZ(-0.4, -1, -1));  // 20
            AddPoint(CoordsArr, XYZ(0.4, -1, -1));   // 21
            AddPoint(CoordsArr, XYZ(-1, -1, 0.4));  // 22
            AddPoint(CoordsArr, XYZ(-1, -1, -0.4)); // 23

            AddPointsBetween(CoordsArr, 0, 1, 4);
            AddPointsBetween(CoordsArr, 1, 8, 4);
            AddPointsBetween(CoordsArr, 8, 0, 4);
            AddPointsBetween(CoordsArr, 2, 3, 4);
            AddPointsBetween(CoordsArr, 3, 13, 4);
            AddPointsBetween(CoordsArr, 13, 2, 4);
            AddPointsBetween(CoordsArr, 4, 5, 4);
            AddPointsBetween(CoordsArr, 5, 22, 4);
            AddPointsBetween(CoordsArr, 22, 4, 4);
            AddPointsBetween(CoordsArr, 6, 7, 4);
            AddPointsBetween(CoordsArr, 7, 14, 4);
            AddPointsBetween(CoordsArr, 14, 6, 4);
            AddPointsBetween(CoordsArr, 11, 12, 4);
            AddPointsBetween(CoordsArr, 12, 18, 4);
            AddPointsBetween(CoordsArr, 18, 11, 4);
            AddPointsBetween(CoordsArr, 19, 23, 4);
            AddPointsBetween(CoordsArr, 23, 20, 4);
            AddPointsBetween(CoordsArr, 20, 19, 4);
            AddPointsBetween(CoordsArr, 15, 16, 4);
            AddPointsBetween(CoordsArr, 16, 21, 4);
            AddPointsBetween(CoordsArr, 21, 15, 4);
            AddPointsBetween(CoordsArr, 9, 17, 4);
            AddPointsBetween(CoordsArr, 17, 10, 4);
            AddPointsBetween(CoordsArr, 10, 9, 4);

            AddPointsBetween(CoordsArr, 1, 2, 5);
            AddPointsBetween(CoordsArr, 5, 6, 5);
            AddPointsBetween(CoordsArr, 20, 21, 5);
            AddPointsBetween(CoordsArr, 10, 11, 5);
            AddPointsBetween(CoordsArr, 3, 4, 5);
            AddPointsBetween(CoordsArr, 7, 0, 5);
            AddPointsBetween(CoordsArr, 16, 17, 5);
            AddPointsBetween(CoordsArr, 18, 19, 5);
            AddPointsBetween(CoordsArr, 12, 13, 5);
            AddPointsBetween(CoordsArr, 22, 23, 5);
            AddPointsBetween(CoordsArr, 14, 15, 5);
            AddPointsBetween(CoordsArr, 8, 9, 5);

            for (var i = 0; i<=19; i++)
                DupPoint(CoordsArr, i);
        }

        private void InitHeadAcke(Coords3D[] CoordsArr)
        {
            AddPoint(CoordsArr, XYZ(1, 0.4, 0.2));    // 0
            AddPoint(CoordsArr, XYZ(-1, 0.4, 0.2));   // 1
            AddPoint(CoordsArr, XYZ(-1, -0.4, 0.2));  // 2
            AddPoint(CoordsArr, XYZ(1, -0.4, 0.2));   // 3
            AddPoint(CoordsArr, XYZ(1, 0.4, -0.2));   // 4
            AddPoint(CoordsArr, XYZ(-1, 0.4, -0.2));  // 5
            AddPoint(CoordsArr, XYZ(-1, -0.4, -0.2)); // 6
            AddPoint(CoordsArr, XYZ(1, -0.4, -0.2));  // 7
            AddPoint(CoordsArr, XYZ(0.4, 0.2, 1));     // 8
            AddPoint(CoordsArr, XYZ(0.4, 0.2, -1));    // 9
            AddPoint(CoordsArr, XYZ(-0.4, 0.2, -1));   // 10
            AddPoint(CoordsArr, XYZ(-0.4, 0.2, 1));    // 11
            AddPoint(CoordsArr, XYZ(0.4, -0.2, 1));    // 12
            AddPoint(CoordsArr, XYZ(0.4, -0.2, -1));   // 13
            AddPoint(CoordsArr, XYZ(-0.4, -0.2, -1));  // 14
            AddPoint(CoordsArr, XYZ(-0.4, -0.2, 1));   // 15
            AddPoint(CoordsArr, XYZ(0.2, 1, 0.4));    // 16
            AddPoint(CoordsArr, XYZ(0.2, -1, 0.4));   // 17
            AddPoint(CoordsArr, XYZ(0.2, -1, -0.4));  // 18
            AddPoint(CoordsArr, XYZ(0.2, 1, -0.4));   // 19
            AddPoint(CoordsArr, XYZ(-0.2, 1, 0.4));   // 20
            AddPoint(CoordsArr, XYZ(-0.2, -1, 0.4));  // 21
            AddPoint(CoordsArr, XYZ(-0.2, -1, -0.4)); // 22
            AddPoint(CoordsArr, XYZ(-0.2, 1, -0.4));  // 23

            for (var i = 0; i <= 5; i++)
            {
                AddPointsBetween(CoordsArr, 4 * i + 0, 4 * i + 1, 8);
                AddPointsBetween(CoordsArr, 4 * i + 1, 4 * i + 2, 4);
                AddPointsBetween(CoordsArr, 4 * i + 2, 4 * i + 3, 8);
                AddPointsBetween(CoordsArr, 4 * i + 3, 4 * i + 0, 4);
            }
            for (var i = 0; i <= 2; i++)
            {
                AddPointsBetween(CoordsArr, 8 * i + 0, 8 * i + 4, 2);
                AddPointsBetween(CoordsArr, 8 * i + 1, 8 * i + 5, 2);
                AddPointsBetween(CoordsArr, 8 * i + 2, 8 * i + 6, 2);
                AddPointsBetween(CoordsArr, 8 * i + 3, 8 * i + 7, 2);
            }
            for (var i = 0; i <= 7; i++)
                DupPoint(CoordsArr, i);
        }

        private void InitSphere1(Coords3D[] CoordsArr)
        {
            for (var nang = -9; nang <= 10; nang++)
            {
                var anga = (nang - 0.5) / 19 * Math.PI;
                var z = Math.Sin(anga);
                for (var nokr = 0; nokr <= 9; nokr++)
                {
                    var ango = (double)nokr / 10 * Math.PI * 2;
                    AddPoint(CoordsArr, XYZ(Math.Sin(ango) * Math.Sqrt(1 - z * z), Math.Cos(ango) * Math.Sqrt(1 - z * z), z));
                }
            }
        }

        private void InitSphere2(Coords3D[] CoordsArr)
        {
            for (var nang = -4; nang <= 5; nang++)
            {
                var anga = (nang - 0.5) / 10 * Math.PI;
                var z = Math.Sin(anga);
                for (var nokr = 0; nokr <= 19; nokr++)
                {
                    var ango = (double)nokr / 20 * Math.PI * 2;
                    AddPoint(CoordsArr, XYZ(Math.Sin(ango) * Math.Sqrt(1 - z * z), Math.Cos(ango) * Math.Sqrt(1 - z * z), z));
                }
            }
        }

        private void InitEgg(Coords3D[] CoordsArr) // Яйцо / Egg
        {
            for (var nsl = -10; nsl <= 9; nsl++)
            {
                double z;
                double R;
                if (nsl <= -4)
                {
                    var angs = (double)(nsl + 4) / 6 * Math.PI / 2;
                    z = Math.Sin(angs) / 2 - 0.5;
                    R = Math.Cos(angs);
                }
                else
                {
                    var angs = (double)(nsl + 4) / 14 * Math.PI / 2;
                    z = Math.Sin(angs) * 1.5 - 0.5;
                    R = Math.Cos(angs);
                }
                for (var nokr = 0; nokr <= 9; nokr++)
                {
                    var ango = (double)nokr / 10 * Math.PI * 2;
                    AddPoint(CoordsArr, XYZ(Math.Sin(ango) * R / 1.5, Math.Cos(ango) * R / 1.5, z));
                }
            }
        }

        private void InitTor(Coords3D[] CoordsArr) // тор / torus
        {
            for (var n = 0; n <= (PointsCount / 10) - 1; n++)
            {
                var ang = (double) n / PointsCount * 10 * 2 * Math.PI;
                for (var k = 0; k <= 9; k++)
                {
                    var r = 1 + 0.33 * Math.Cos((double) k / 10 * 2 * Math.PI);
                    var z = 0.33 * Math.Sin((double) k / 10 * 2 * Math.PI);
                    var x = r * Math.Cos(ang);
                    var y = r * Math.Sin(ang);
                    AddPoint(CoordsArr, XYZ(x, y, z));
                }
            }
        }

        private void InitSpiral(Coords3D[] CoordsArr) // кривая 2 / curve 2
        {
            for (var n = 0; n <= PointsCount - 1; n++)
            {
                var angm = (double) n / PointsCount * 2 * Math.PI;
                var ang = angm * 16;
                var z = 0.33 * Math.Sin(ang);
                var r = 1 + 0.33 * Math.Cos(ang);
                var x = r * Math.Cos(angm);
                var y = r * Math.Sin(angm);
                AddPoint(CoordsArr, XYZ(x, y, z));
            }
        }

        private void UnSort(Coords3D[] CoordsArr)
        {
            for (var i = 1; i <= 1024; i++)
            {
                var k = _random.Next(PointsCount);
                var l = _random.Next(PointsCount);
                var temp = CoordsArr[k];
                CoordsArr[k] = CoordsArr[l];
                CoordsArr[l] = temp;
            }
        }

        private void CalcPos()
        {
            for (var n = 0; n <= PointsCount - 1; n++)
            {
                _points[n].X = _pCoords1[n].X + (_pCoords2[n].X - _pCoords1[n].X) * Percent / 100;
                _points[n].Y = _pCoords1[n].Y + (_pCoords2[n].Y - _pCoords1[n].Y) * Percent / 100;
                _points[n].Z = _pCoords1[n].Z + (_pCoords2[n].Z - _pCoords1[n].Z) * Percent / 100;
            }
        }

        private void InitShape(Coords3D[] coordsArr)
        {
            _pIndex = 0;

            //check all shaper registered
            var possibleShapes = (Shapes[]) Enum.GetValues(typeof(Shapes));
            var shapeInd = _random.Next(Int32.MaxValue) % possibleShapes.Length;

            switch (possibleShapes[shapeInd])
            {
                case Shapes.shTriangle1:
                    InitTriangle1(coordsArr);
                    break;
                case Shapes.shTriangle2:
                    InitTriangle2(coordsArr);
                    break;
                case Shapes.shTriangle3:
                    InitTriangle3(coordsArr);
                    break;
                case Shapes.shCube:
                    InitCube(coordsArr);
                    break;
                case Shapes.shPyramideTri:
                    InitPyramideTri(coordsArr);
                    break;
                case Shapes.shOct:
                    InitOctaedr(coordsArr);
                    break;
                case Shapes.shIco:
                    InitIcosaedr(coordsArr);
                    break;
                case Shapes.shSphere1:
                    InitSphere1(coordsArr);
                    break;
                case Shapes.shSphere2:
                    InitSphere2(coordsArr);
                    break;
                case Shapes.shEgg:
                    InitEgg(coordsArr);
                    break;
                case Shapes.shDodecaedr:
                    InitDodecaedr(coordsArr);
                    break;
                case Shapes.shPyramideCut:
                    InitPyramideCut(coordsArr);
                    break;
                case Shapes.shCubeCut:
                    InitCubeCut(coordsArr);
                    break;
                case Shapes.shHeadAcke:
                    InitHeadAcke(coordsArr);
                    break;
                case Shapes.shTor:
                    InitTor(coordsArr);
                    break;
                case Shapes.shSpiral:
                    InitSpiral(coordsArr);
                    break;
                case Shapes.shCube2:
                    InitPlayingCube(coordsArr);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            if (UnSortPoints)
                UnSort(coordsArr); // перемешать точки / mix points
        }

        #endregion FIGURES INITIALIZATION

        #region Screen Saver's engine

        private void DrawPoint(Graphics graphics, Point point, Color color)
        {
            using (var pen = new Pen(color, _preview ? 1 : 2))
            {
                graphics.DrawEllipse(pen, point.X, point.Y, _preview ? 1 : 3, _preview ? 1 : 3);
            }
        }

        private Point GetCoords2D(Coords3D coords3D) 
        {
            var result = new Point();
            var zNorm = 1 - (coords3D.Z + _scz) / CamZ;
            if (zNorm != 0)
            {
                result.X = (int) Math.Round((coords3D.X + _scx) / zNorm * _coefX) + _scrX;
                result.Y = (int) Math.Round((coords3D.Y + _scy) / zNorm * _coefY) + _scrY;
            }
            return result;
        }

        private Coords3D Rotate3D(Coords3D coords3D)
        {
            var result = new Coords3D();
            double sina;
            double cosa;
            if (_xa != 0)
            {
                sina = Math.Sin(_xa);
                cosa = Math.Cos(_xa);
                result.X = coords3D.X;
                result.Y = coords3D.Y * cosa - coords3D.Z * sina;
                result.Z = coords3D.Y * sina + coords3D.Z * cosa;

                coords3D.X = result.X;
                coords3D.Y = result.Y;
                coords3D.Z = result.Z;
            }

            if (_ya != 0)
            {
                sina = Math.Sin(_ya);
                cosa = Math.Cos(_ya);

                result.X = coords3D.X * cosa + coords3D.Z * sina;
                result.Y = coords3D.Y;
                result.Z = -coords3D.X * sina + coords3D.Z * cosa;

                coords3D.X = result.X;
                coords3D.Y = result.Y;
                coords3D.Z = result.Z;
            }

            if (_za != 0)
            {
                sina = Math.Sin(_za);
                cosa = Math.Cos(_za);
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

        private Color GetColor(Coords3D coords3D)
        {
            var len = Math.Sqrt(Math.Pow(coords3D.X - 0, 2) + Math.Pow(coords3D.Y - 0, 2) + Math.Pow(coords3D.Z - ColorZ0, 2));
            var gr = (int) Math.Abs(Math.Truncate((BackColor.ToArgb() & 0xFFFFFF) - len * FogCoef)); //todo: check!
            //  Gr := Trunc(255-Len*FogCoef);
            //  If Gr<0 then Gr := 0;
            return Color.FromArgb(gr, gr, gr);    // Translation RGB to the hue of gray
        }

        #endregion

        public void DrawScreen(Graphics graphics, Rectangle rect) //procedure of screen drawing
        {
            _scrX = rect.Right / 2;
            _scrY = rect.Bottom / 2;
            _coefX = (double)rect.Right / 8;
            _coefY = (double)rect.Bottom / 6;

            const int MinTimeDelta = 10;
            const int MaxTimeDelta = 200;

            var TimeDelta = Environment.TickCount - LastTickCount;
            if (TimeDelta > MaxTimeDelta)
                TimeDelta = MaxTimeDelta;
            if (TimeDelta<MinTimeDelta)
                TimeDelta = MinTimeDelta;
            LastTickCount = Environment.TickCount;

            if (Wait > 0)
                Wait = Wait - TimeDelta;
            else
            {
                if (DoUp)
                {
                    Percent = Percent + (double)TimeDelta / 10;
                    if (Percent >= 100)
                    {
                        Percent = 100;
                        DoUp = false;
                        Wait = WaitPer;
                        InitShape(_pCoords1);
                    }
                }
                else
                {
                    Percent = Percent - (double)TimeDelta / 10;
                    if (Percent <= 0)
                    {
                        Percent = 0;
                        DoUp = true;
                        Wait = WaitPer;
                        InitShape(_pCoords2);
                    }
                }
                CalcPos();
            }

            _xa = _xa + TimeDelta * Math.PI * VectAX / 1000;
            _ya = _ya + TimeDelta * Math.PI * VectAY / 1000;
            _za = _za - TimeDelta * Math.PI * VectAZ / 1000;

            _scx = _scx + VectX * TimeDelta;
            if (_scx > 3.5 - _scz / 2.5 || (_scx > 2.75 && !Move3D))
            {
                VectX = -Math.Abs(VectX);
                VectAY = -Math.Abs(_random.NextDouble() / 3 + 0.25);
            }
            if(_scx<-3.5+_scz/2.5 || _scx<-2.75 && !Move3D)
            {
                VectX = Math.Abs(VectX);
                VectAY = Math.Abs(_random.NextDouble() / 3 + 0.25);
            }

            _scy = _scy+VectY* TimeDelta;
            if(_scy>3-_scz/3 || _scy>1.8 && !Move3D)
            {
                VectY = -Math.Abs(VectY);
                VectAX = -Math.Abs(_random.NextDouble() / 3 + 0.25);
            }
            if (_scy < -3 + _scz / 3 || _scy < -1.8 && !Move3D)
            {
                VectY = Math.Abs(VectY);
                VectAX = Math.Abs(_random.NextDouble() / 3 + 0.25);
            }

            if (Move3D)
            {
                _scz = _scz+VectZ* TimeDelta;
                if (_scz > 4)
                {
                    VectZ = -Math.Abs(VectZ);
                    VectAX = -Math.Abs(_random.NextDouble() / 3 + 0.25);
                    VectAY = -Math.Abs(_random.NextDouble() / 3 + 0.25);
                }
                if (_scz < -10)
                {
                    VectZ = Math.Abs(VectZ);
                    VectAX = Math.Abs(_random.NextDouble() / 3 + 0.25);
                    VectAY = Math.Abs(_random.NextDouble() / 3 + 0.25);
                }
            }

            for (var n = 0; n <= PointsCount - 1; n++)
            {
                var point = Rotate3D(_points[n]);
                var color = GetColor(point);
                DrawPoint(graphics, GetCoords2D(point), color);
            }
        }
    }
}
