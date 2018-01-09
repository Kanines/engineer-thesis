using System;
using System.Linq;
using LASLibrary;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using OpenTK;
using MathNet.Numerics.LinearRegression;

namespace LidarClassification
{
    public static class LinearRegression
    {
        public static Vector3 ComputeRegressionPoint(LasFile file, LasPoint point, int count, int radiusSector)
        {
            var neighbours = file.LasPointDataRecords.GetNeighbours(point, count, radiusSector);

            double[,] matrix = new double[3, 3];
            double[,] vector = new double[1, 3];

            foreach (var item in neighbours)
            {
                matrix[0, 0] += item.X * item.X;
                matrix[0, 1] += item.X * item.Y;
                matrix[0, 2] += item.X;
                matrix[1, 0] += item.X * item.Y;
                matrix[1, 1] += item.Y * item.Y;
                matrix[1, 2] += item.Y;
                matrix[2, 0] += item.X;
                matrix[2, 1] += item.Y;

                vector[0, 0] += item.X * item.Z;
                vector[0, 1] += item.Y * item.Z;
                vector[0, 2] += item.Z;
            }
            matrix[2, 2] = neighbours.Count;


            Matrix<double> a = DenseMatrix.OfArray(matrix);
            Matrix<double> b = DenseMatrix.OfArray(vector);
            a = a.Inverse();

            Matrix<double> x = b.Multiply(a);

            var tempArray = x.ToArray();

            Vector3 abc = new Vector3(Convert.ToSingle(tempArray[0, 0]), Convert.ToSingle(tempArray[0, 1]), Convert.ToSingle(tempArray[0, 2]));

            return abc;
        }

        public static double[] ComputeRegressionNumerics(LasFile file, LasPoint point, int count, int radiusSector)
        {
            var neighbours = file.LasPointDataRecords.GetNeighbours(point, count, radiusSector);
            double[][] xy = new double[neighbours.Count][];
            double[] z = new double[neighbours.Count];

            for(int i = 0; i < neighbours.Count; i++)
            {
                xy[i] = new double[] { neighbours[i].X, neighbours[i].Y };
                z[i] = neighbours[i].Z;
            }
            double[] p = MultipleRegression.QR(xy, z);
            
            return p;
        }
    }
}
