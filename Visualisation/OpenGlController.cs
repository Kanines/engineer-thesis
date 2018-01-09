using System;
using System.Drawing;
using LASLibrary;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;

namespace Visualisation
{
    class OpenGlController
    {
        public LasPoint.ClassificationType[,] subsetClass;
        public const int divisionCountX = 400;
        public const int divisionCountY = 400;
        const float StartEyeZ = 1000f;
        public static bool drawQuads;
        public static bool isClassified = false;
        List<Vector3> tempList;        
        Vector3[] Points;
        Vector3[] Colors;
        LasFile file;
        int PointsCount;
        readonly int PointsVBO;
        readonly int ColorsVBO;
        int VAO;
        int UniPerspective, UniLookAt;
        readonly float Aspect;
        Shaders Shader;
        Camera camera;
        
        public LasFile File
        {
            get { return file; }
            set
            {
                file = value;
                PointsCount = file.LasPointDataRecords.Count;
                float maxEyeX = (float) file.LasHeader.MaxX;
                float maxEyeY = (float) file.LasHeader.MaxY;
                float minEyeX = (float) file.LasHeader.MinX;
                float minEyeY = (float) file.LasHeader.MinY;
                float eyeX = minEyeX + (maxEyeX - minEyeX)*0.5f;
                float eyeY = minEyeY + (maxEyeY - minEyeY)*0.5f;

                camera = new Camera(new Vector3(eyeX, StartEyeZ, -eyeY));
            }
        }

        public Camera CameraProp
        {
            get { return camera; }
        }

        public OpenGlController(int w, int h)
        {
            Aspect = (float) w/h;
            
            GL.GenBuffers(1, out PointsVBO);
            GL.GenBuffers(1, out ColorsVBO);
            LoadOpenGl();
        }
      
        public void LoadLinearRegressionQuads3D()
        {
            if (file == null) return;

            drawQuads = true;
            double scaleZ, minX, minY, ratioX, ratioY;
            ComputeQuadsData(out scaleZ, out minX, out minY, out ratioX, out ratioY);

            const int MATRIX_ROWS = 3;
            const int MATRIX_COLUMNS = 3;
            const int VECTOR_LENGTH = 3;

            double[,] matrix = new double[MATRIX_ROWS, MATRIX_COLUMNS];
            double[,] vector = new double[1, VECTOR_LENGTH];

            Vector3[,] results = new Vector3[divisionCountX, divisionCountY];

            List<Vector3>[,] pieces = new List<Vector3>[divisionCountX, divisionCountY];
            for (int i = 0; i < divisionCountX; i++)
                for (int j = 0; j < divisionCountY; j++)
                    pieces[i, j] = new List<Vector3>();

            for (int i = 0; i < PointsCount; i++)
            {
                var x = (float)file.LasHeader.ScaleX(file.LasPointDataRecords[i].X);
                var y = (float)file.LasHeader.ScaleY(file.LasPointDataRecords[i].Y);
                var z = (float)file.LasHeader.ScaleZ(file.LasPointDataRecords[i].Z);

                var xReal = (float)file.LasPointDataRecords[i].X;
                var yReal = (float)file.LasPointDataRecords[i].Y;
                var zReal = (float)file.LasPointDataRecords[i].Z;

                Vector3 coordinates = new Vector3(xReal, yReal, zReal);

                int arrayX = (int)((xReal - minX) / ratioX);
                int arrayY = (int)((yReal - minY) / ratioY);

                if ((arrayX < divisionCountX) && (arrayY < divisionCountY))
                    pieces[arrayX, arrayY].Add(coordinates);
            }

            for (int i = 0; i < divisionCountX; i++)
            {
                for (int j = 0; j < divisionCountY; j++)
                {
                    for (int r = 0; r < MATRIX_ROWS; r++)
                        for (int c = 0; c < MATRIX_COLUMNS; c++)
                            matrix[r, c] = 0;

                    for (int v = 0; v < VECTOR_LENGTH; v++)
                        vector[0, v] = 0;

                    List<Vector3> points = pieces[i, j];
                    int pointsCount = points.Count;

                    for (int k = 0; k < pointsCount; k++)
                    {
                        matrix[0, 0] += (points[k].X * points[k].X);
                        matrix[0, 1] += (points[k].X * points[k].Y);
                        matrix[0, 2] += points[k].X;
                        matrix[1, 0] += (points[k].X * points[k].Y);
                        matrix[1, 1] += (points[k].Y * points[k].Y);
                        matrix[1, 2] += points[k].Y;
                        matrix[2, 0] += points[k].X;
                        matrix[2, 1] += points[k].Y;

                        vector[0, 0] += (points[k].X * points[k].Z);
                        vector[0, 1] += (points[k].Y * points[k].Z);
                        vector[0, 2] += points[k].Z;
                    }
                    matrix[2, 2] = pointsCount;

                    Matrix<double> A = DenseMatrix.OfArray(matrix);
                    Matrix<double> b = DenseMatrix.OfArray(vector);
                    Matrix<double> x;

                    A = A.Inverse();
                    x = b.Multiply(A);

                    double[,] myarray;
                    myarray = x.ToArray();

                    Vector3 abc = new Vector3(Convert.ToSingle(myarray[0, 0]), Convert.ToSingle(myarray[0, 1]), Convert.ToSingle(myarray[0, 2]));
                    results[i, j].Add(abc);

                }
            }
            // ax + by + cz + d = 0
            double d = Math.Sqrt((minX * minX) + (minY * minY)) * scaleZ;

            for (int i = 0; i < divisionCountX; i++)
            {
                for (int j = 0; j < divisionCountY; j++)
                {
                    double x1, y1, x2, y2, x3, y3, x4, y4;
                    ComputeQuadsCoordinates(minX, minY, ratioX, ratioY, i, j, out x1, out y1, out x2, out y2, out x3, out y3, out x4, out y4);

                    Vector3 abc = results[i, j];

                    double a = abc.X;
                    double b = abc.Y;
                    double c = abc.Z;

                    double z1 = ((-a / c) * x1) + ((-b / c) * y1);
                    double z2 = ((-a / c) * x2) + ((-b / c) * y2);
                    double z3 = ((-a / c) * x3) + ((-b / c) * y3);
                    double z4 = ((-a / c) * x4) + ((-b / c) * y4);

                    tempList.Add(new Vector3((float)file.LasHeader.ScaleX(Convert.ToSingle(x1)), (float)file.LasHeader.ScaleZ(Convert.ToSingle(z1)), (float)file.LasHeader.ScaleY(Convert.ToSingle(-y1))));
                    tempList.Add(new Vector3((float)file.LasHeader.ScaleX(Convert.ToSingle(x2)), (float)file.LasHeader.ScaleZ(Convert.ToSingle(z2)), (float)file.LasHeader.ScaleY(Convert.ToSingle(-y2))));
                    tempList.Add(new Vector3((float)file.LasHeader.ScaleX(Convert.ToSingle(x4)), (float)file.LasHeader.ScaleZ(Convert.ToSingle(z4)), (float)file.LasHeader.ScaleY(Convert.ToSingle(-y4))));
                    tempList.Add(new Vector3((float)file.LasHeader.ScaleX(Convert.ToSingle(x3)), (float)file.LasHeader.ScaleZ(Convert.ToSingle(z3)), (float)file.LasHeader.ScaleY(Convert.ToSingle(-y3))));
                }
            }
            Points = tempList.ToArray();
        }
       
        public void LoadQuads2D()
        {
            if (file == null) return;

            drawQuads = true;
            double scaleZ, minX, minY, ratioX, ratioY;
            ComputeQuadsData(out scaleZ, out minX, out minY, out ratioX, out ratioY);
                 
            for (int i = 0; i < divisionCountX; i++)
            {
                for (int j = 0; j < divisionCountY; j++)
                {
                    double x1, y1, x2, y2, x3, y3, x4, y4;
                    ComputeQuadsCoordinates(minX, minY, ratioX, ratioY, i, j, out x1, out y1, out x2, out y2, out x3, out y3, out x4, out y4);

                    tempList.Add(new Vector3((float)file.LasHeader.ScaleX(Convert.ToSingle(x1)), (float)0, (float)file.LasHeader.ScaleY(Convert.ToSingle(-y1))));
                    tempList.Add(new Vector3((float)file.LasHeader.ScaleX(Convert.ToSingle(x2)), (float)0, (float)file.LasHeader.ScaleY(Convert.ToSingle(-y2))));
                    tempList.Add(new Vector3((float)file.LasHeader.ScaleX(Convert.ToSingle(x4)), (float)0, (float)file.LasHeader.ScaleY(Convert.ToSingle(-y4))));
                    tempList.Add(new Vector3((float)file.LasHeader.ScaleX(Convert.ToSingle(x3)), (float)0, (float)file.LasHeader.ScaleY(Convert.ToSingle(-y3))));
                }
            }
            Points = tempList.ToArray();
        }

        private static void ComputeQuadsCoordinates(double minX, double minY, double ratioX, double ratioY, int i, int j, out double x1, out double y1, out double x2, out double y2, out double x3, out double y3, out double x4, out double y4)
        {
            x1 = (i * ratioX) + minX;
            y1 = (j * ratioY) + minY;
            x2 = ((i + 1) * ratioX) + minX;
            y2 = (j * ratioY) + minY;
            x3 = (i * ratioX) + minX;
            y3 = ((j + 1) * ratioY) + minY;
            x4 = ((i + 1) * ratioX) + minX;
            y4 = ((j + 1) * ratioY) + minY;
        }

        private void ComputeQuadsData(out double scaleZ, out double minX, out double minY, out double ratioX, out double ratioY)
        {           
            Points = new Vector3[4 * divisionCountX * divisionCountY];

            tempList = new List<Vector3>();

            var scaleX = file.LasHeader.ScaleFactorX;
            var scaleY = file.LasHeader.ScaleFactorY;
            scaleZ = file.LasHeader.ScaleFactorZ;
            minX = (float)(file.LasHeader.MinX / scaleX);
            minY = (float)(file.LasHeader.MinY / scaleY);
            double maxX = (float)(file.LasHeader.MaxX / scaleX);
            double maxY = (float)(file.LasHeader.MaxY / scaleY);
            double diffX = maxX - minX;
            double diffY = maxY - minY;
            ratioX = diffX / divisionCountX;
            ratioY = diffY / divisionCountY;
        }

        public void LoadPoints2D()
        {
            if (file == null) return;

            if (drawQuads)
            {
                Points = new Vector3[4 * divisionCountX * divisionCountY];
                LoadQuads2D();
                Points = tempList.ToArray();
            }
            else
            {
                Points = new Vector3[PointsCount];
                for (int i = 0; i < PointsCount; i++)
                {
                    var x = (float)file.LasHeader.ScaleX(file.LasPointDataRecords[i].X);
                    var y = (float)file.LasHeader.ScaleY(file.LasPointDataRecords[i].Y);
                    Points[i] = new Vector3(x, 0, -y);
                }
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, PointsVBO);
            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(Points.Length*Vector3.SizeInBytes),
                Points, BufferUsageHint.StaticDraw);
        }

        public void LoadPoints3D()
        {
            if (file == null) return;

            if (drawQuads)
            {
                Points = new Vector3[4 * divisionCountX * divisionCountY];
                LoadLinearRegressionQuads3D();
                Points = tempList.ToArray();
            }
            else
            {
                Points = new Vector3[PointsCount];
                for (int i = 0; i < PointsCount; i++)
                {
                    var x = (float)file.LasHeader.ScaleX(file.LasPointDataRecords[i].X);
                    var y = (float)file.LasHeader.ScaleY(file.LasPointDataRecords[i].Y);
                    var z = (float)file.LasHeader.ScaleZ(file.LasPointDataRecords[i].Z);
                    Points[i] = new Vector3(x, z, -y);
                }
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, PointsVBO);
            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(Points.Length*Vector3.SizeInBytes),
                Points, BufferUsageHint.StaticDraw);
        }

        public void LoadColorsByHeight()
        {
            if (file == null) return;

            drawQuads = false;
            Colors = new Vector3[PointsCount];
            float maxZ = (float) file.LasHeader.MaxZ/4;
            for (int i = 0; i < PointsCount; i++)
            {
                var z = (float) file.LasHeader.ScaleZ(file.LasPointDataRecords[i].Z);
                var colorR = z/maxZ;
                var colorG = 1 - z/maxZ;
                Colors[i] = new Vector3(colorR, colorG, 0);
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorsVBO);
            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(Colors.Length*Vector3.SizeInBytes),
                Colors, BufferUsageHint.StaticDraw);
        }

        public void LoadColorsByIntensity()
        {
            if (file == null) return;

            drawQuads = false;
            Colors = new Vector3[PointsCount];
            float ratio = (float) byte.MaxValue/ushort.MaxValue;
            for (int i = 0; i < PointsCount; i++)
            {
                var z = file.LasPointDataRecords[i].Intensity*ratio;
                Colors[i] = new Vector3(z, z, z);
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorsVBO);
            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(Colors.Length*Vector3.SizeInBytes),
                Colors, BufferUsageHint.StaticDraw);
        }

        public void LoadColorsByClassification()
        {
            if (file == null) return;

            drawQuads = false;
            Colors = new Vector3[PointsCount];
            for (int i = 0; i < PointsCount; i++)
                Colors[i] =
                    ColorToVector3(Form1.ClassificationToColor[file.LasPointDataRecords[i].Classification]);

            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorsVBO);
            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(Colors.Length*Vector3.SizeInBytes),
                Colors, BufferUsageHint.StaticDraw);
        }

        public void LoadColorsByClassificationSubgroups()
        {
            drawQuads = true;
            Colors = new Vector3[4 * divisionCountX * divisionCountY];
            for (int i = 0; i < divisionCountX; i++)
            {
                for (int j = 0; j < divisionCountY; j++)
                {
                    Colors[(i * divisionCountX * 4) + (j * 4)] = ColorToVector3(Form1.ClassificationToColor[subsetClass[i, j]]);
                    Colors[(i * divisionCountX * 4) + (j * 4) + 1] = ColorToVector3(Form1.ClassificationToColor[subsetClass[i, j]]);
                    Colors[(i * divisionCountX * 4) + (j * 4) + 2] = ColorToVector3(Form1.ClassificationToColor[subsetClass[i, j]]);
                    Colors[(i * divisionCountX * 4) + (j * 4) + 3] = ColorToVector3(Form1.ClassificationToColor[subsetClass[i, j]]);
                }
            }

            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorsVBO);
            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(Colors.Length * Vector3.SizeInBytes),
                Colors, BufferUsageHint.StaticDraw);
        }

        public void LoadColorsByKnnClassification(LasPoint.ClassificationType[] arr)
        {
            Colors = new Vector3[PointsCount];
            for (int i = 0; i < PointsCount; i++)
            {
                Colors[i] = ColorToVector3(Form1.ClassificationToColor[arr[i]]);
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorsVBO);
            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(Colors.Length * Vector3.SizeInBytes),
                Colors, BufferUsageHint.StaticDraw);
        }

        public void LoadColorsByRealColors()
        {
            if (file == null) return;

            drawQuads = false;
            if (!(file.LasPointDataRecords.Format == PointsFormat.Format2 ||
                  file.LasPointDataRecords.Format == PointsFormat.Format3))
                return;
            Colors = new Vector3[PointsCount];
            for (int i = 0; i < PointsCount; i++)
            {
                var colorR = ((ILasPointWithRgb) file.LasPointDataRecords[i]).Red/65536.0f;
                var colorG = ((ILasPointWithRgb) file.LasPointDataRecords[i]).Green/65536.0f;
                var colorB = ((ILasPointWithRgb) file.LasPointDataRecords[i]).Blue/65536.0f;
                Colors[i] = new Vector3(colorR, colorG, colorB);
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorsVBO);
            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(Colors.Length*Vector3.SizeInBytes),
                Colors, BufferUsageHint.StaticDraw);
        }

        public void LoadColorsFromClassArray(LasPoint.ClassificationType[] arr)
        {
            drawQuads = false;
            Colors = new Vector3[PointsCount];
            for (int i = 0; i < arr.Length; i++)
            {
                Colors[i] = ColorToVector3(Form1.ClassificationToColor[arr[i]]);
            }
            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorsVBO);
            GL.BufferData(BufferTarget.ArrayBuffer,
                new IntPtr(Colors.Length * Vector3.SizeInBytes),
                Colors, BufferUsageHint.StaticDraw);
        }

        public void LoadOpenGl()
        {
            GL.GenVertexArrays(1, out VAO);
            GL.BindVertexArray(VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, PointsVBO);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);
            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorsVBO);
            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vector3.SizeInBytes, 0);

            GL.EnableVertexAttribArray(0);
            GL.EnableVertexAttribArray(1);

            Shader = new Shaders();
            UniPerspective = GL.GetUniformLocation(Shader.ShaderProgramHandle, "perspective");
            UniLookAt = GL.GetUniformLocation(Shader.ShaderProgramHandle, "lookAt");
            GL.ClearColor(Color.Black);
        }

        public void Paint()
        {
            if (file == null)
            {
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.ClearColor(Color.Black);
            }
            else
            {
                Matrix4 perspective = Matrix4.CreatePerspectiveFieldOfView(0.75f, Aspect, 0.1f, 10000f);
                Matrix4 lookat = camera.GetViewMatrix(); // 'Setup camera
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                GL.UniformMatrix4(UniLookAt, false, ref lookat);
                GL.UniformMatrix4(UniPerspective, false, ref perspective);
                GL.BindVertexArray(VAO);

                if(drawQuads)
                    GL.DrawArrays(PrimitiveType.Quads, 0, Points.Length);
                else
                    GL.DrawArrays(PrimitiveType.Points, 0, Points.Length);
            }
        }

        static Vector3 ColorToVector3(Color color)
        {
            return new Vector3(color.R/256f, color.G/256f, color.B/256f);
        }

        public void ChangeViewport(int x, int y, int w, int h)
        {
            GL.Viewport(x, y, w, h);
        }
    }
}