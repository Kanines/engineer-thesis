using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using LASLibrary;
using OpenTK;
using System.Diagnostics;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using Encog.ML.Data;

namespace LidarClassification
{
    public static class Utills
    {
        public static LasPoint.ClassificationType[] ClassificationClasses = {
            LasPoint.ClassificationType.Ground,
            LasPoint.ClassificationType.HighVegetation,
            LasPoint.ClassificationType.Building,
            LasPoint.ClassificationType.MediumVegetation,
            LasPoint.ClassificationType.LowVegetation,     
            LasPoint.ClassificationType.Water,
            LasPoint.ClassificationType.Other
        };

        public static List<LasPoint> GenerateClassArray(LasPointDataRecords points, double percentToTrain)
        {
            Console.WriteLine("Generationg Class Arrays in progress");

            return points.GetRandomPointsByClass(ClassificationClasses, percentToTrain);
        }

        public static List<Vector3> GenerateVectorArray(List<LasPoint> classPointsRanged, LasFile file)
        {
            List<Vector3> vectors = new List<Vector3>();

            Console.WriteLine("Generationg Vector Arrays in progress");

            int pointNo = 0;
            foreach (LasPoint point in classPointsRanged)
            {
                pointNo += 1;
                vectors.Add(LinearRegression.ComputeRegressionPoint(file, point, 100, 2));
                Console.WriteLine("Computing Vectors " + pointNo + "/" + classPointsRanged.Count);          
            }

            return vectors;
        }

        public static void SerializeToFile<T>(T obj, string filename)
        {
            using (FileStream fileStream = File.Create(filename))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(fileStream, obj);
            }
        }

        public static T DeserilizeFromFile<T>(string filename)
        {
            T obj;
            using (FileStream fileStream = File.OpenRead(filename))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                obj = (T)binaryFormatter.Deserialize(fileStream);
            }
            return obj;
        }

        public static bool IsSignificant(LasPoint.ClassificationType clType)
        {
            switch(clType)
            {
                case LasPoint.ClassificationType.Ground:
                case LasPoint.ClassificationType.LowVegetation:
                case LasPoint.ClassificationType.MediumVegetation:
                case LasPoint.ClassificationType.HighVegetation:
                case LasPoint.ClassificationType.Building:
                case LasPoint.ClassificationType.Water:
                    return true;
            }
            return false;
        }

        public static Dictionary<int, LasPoint.ClassificationType> QuickClassess = new Dictionary
        <int, LasPoint.ClassificationType>
        {
            {0,  LasPoint.ClassificationType.Building},
            {1,  LasPoint.ClassificationType.MediumVegetation},
            {2,  LasPoint.ClassificationType.HighVegetation},
            {3,  LasPoint.ClassificationType.LowVegetation},
            {4,  LasPoint.ClassificationType.Ground},
            {5,  LasPoint.ClassificationType.Water},
        };
        public static Dictionary<LasPoint.ClassificationType, int> QuickClassessReversed = QuickClassess.ToDictionary(kp => kp.Value, kp => kp.Key);

        public static List<SubgroupOfPoints> GroupPointsList(LasFile file, int divisionCountX, int divisionCountY)
        {
            Console.WriteLine("Preparing input dataset... ");
            Stopwatch sw = Stopwatch.StartNew();

            List<SubgroupOfPoints> results = new List<SubgroupOfPoints>();

            bool enoughPoints;
            float percentOfClassTypeThreshold;
            int PointsCount;
            double minX, minY, ratioX, ratioY;
            double[,] matrix, vector;
            List<LasPoint>[,] pieces;
            PrepareGroupPointsData(file, divisionCountX, divisionCountY, out enoughPoints, out percentOfClassTypeThreshold, out PointsCount, out minX, out minY, out ratioX, out ratioY, out matrix, out vector, out pieces);

            for (int i = 0; i < PointsCount; i++)
            {
                var xReal = (float)file.LasPointDataRecords[i].X;
                var yReal = (float)file.LasPointDataRecords[i].Y;

                int arrayX = (int)((xReal - minX) / ratioX);
                int arrayY = (int)((yReal - minY) / ratioY);

                if ((arrayX < divisionCountX) && (arrayY < divisionCountY))
                    pieces[arrayX, arrayY].Add(file.LasPointDataRecords[i]);
            }


            for (int i = 0; i < divisionCountX; i++)
            {
                for (int j = 0; j < divisionCountY; j++)
                {
                    List<LasPoint> points;
                    int pointsCount;
                    double avgIntensity, avgHeight;
                    int[] groupClass;
                    CountGroupPointsParams(out enoughPoints, matrix, vector, pieces, i, j, out points, out pointsCount, out avgIntensity, out avgHeight, out groupClass);

                    int classIndex = 6;

                    if (pointsCount >= 12)
                    {
                        if (groupClass[0] >= (percentOfClassTypeThreshold * pointsCount))
                            classIndex = 0;
                        else if (groupClass[1] >= (percentOfClassTypeThreshold * pointsCount))
                            classIndex = 1;
                        else if (groupClass[2] >= (percentOfClassTypeThreshold * pointsCount))
                            classIndex = 2;
                        else if (groupClass[3] >= (percentOfClassTypeThreshold * pointsCount))
                            classIndex = 3;
                        else if (groupClass[4] >= (percentOfClassTypeThreshold * pointsCount))
                            classIndex = 4;
                        else if (groupClass[5] >= (percentOfClassTypeThreshold * pointsCount))
                            classIndex = 5;
                    }
                    else
                    {
                        enoughPoints = false;
                    }

                    if (enoughPoints)
                    {
                        SubgroupOfPoints group = CountSumOfDistancesFromPlane(matrix, vector, points, pointsCount, ref avgIntensity, ref avgHeight, classIndex);
                        results.Add(group);
                    }
                }
            }

            int[] classCount = new int[8];
            float[] percentOfClassInputs = { 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f, 0.25f };

            foreach (SubgroupOfPoints group in results)
            {
                if (group.classIndex == 0)
                    classCount[0]++;
                else if (group.classIndex == 1)
                    classCount[1]++;
                else if (group.classIndex == 2)
                    classCount[2]++;
                else if (group.classIndex == 3)
                    classCount[3]++;
                else if (group.classIndex == 4)
                    classCount[4]++;
                else if (group.classIndex == 5)
                    classCount[5]++;
                else if (group.classIndex == 6)
                    classCount[6]++;
            }

            results.Sort((a, b) => a.classIndex.CompareTo(b.classIndex));
            List<SubgroupOfPoints> properResults = new List<SubgroupOfPoints>();

            properResults.AddRange(results.GetRange(0, (int)(classCount[0] * percentOfClassInputs[0])));
            properResults.AddRange(results.GetRange(classCount[0], (int)(classCount[1] * percentOfClassInputs[1])));
            properResults.AddRange(results.GetRange(classCount[0] + classCount[1], (int)(classCount[2] * percentOfClassInputs[2])));
            properResults.AddRange(results.GetRange(classCount[0] + classCount[1] + classCount[2], (int)(classCount[3] * percentOfClassInputs[3])));
            properResults.AddRange(results.GetRange(classCount[0] + classCount[1] + classCount[2] + classCount[3], (int)(classCount[4] * percentOfClassInputs[4])));
            properResults.AddRange(results.GetRange(classCount[0] + classCount[1] + classCount[2] + classCount[3] + classCount[4], (int)(classCount[5] * percentOfClassInputs[5])));

            sw.Stop();
            Console.WriteLine("Preparing input dataset completed [" + sw.Elapsed.TotalSeconds.ToString() + "s]");

            return properResults;
        }

        public static SubgroupOfPoints[,] GroupPoints(LasFile file, int divisionCountX, int divisionCountY)
        {
            SubgroupOfPoints[,] results = new SubgroupOfPoints[divisionCountX, divisionCountY];
            bool enoughPoints;
            float percentOfClassTypeThreshold;
            int PointsCount;
            double minX, minY, ratioX, ratioY;
            double[,] matrix, vector;
            List<LasPoint>[,] pieces;
            PrepareGroupPointsData(file, divisionCountX, divisionCountY, out enoughPoints, out percentOfClassTypeThreshold, out PointsCount, out minX, out minY, out ratioX, out ratioY, out matrix, out vector, out pieces);
           
            for (int i = 0; i < PointsCount; i++)
            {
                var xReal = (float)file.LasPointDataRecords[i].X;
                var yReal = (float)file.LasPointDataRecords[i].Y;

                int arrayX = (int)((xReal - minX) / ratioX);
                int arrayY = (int)((yReal - minY) / ratioY);

                if ((arrayX < divisionCountX) && (arrayY < divisionCountY))
                {
                    pieces[arrayX, arrayY].Add(file.LasPointDataRecords[i]);
                }
            }

            for (int i = 0; i < divisionCountX; i++)
            {
                for (int j = 0; j < divisionCountY; j++)
                {
                    List<LasPoint> points;
                    int pointsCount;
                    double avgIntensity, avgHeight;
                    int[] groupClass;
                    CountGroupPointsParams(out enoughPoints, matrix, vector, pieces, i, j, out points, out pointsCount, out avgIntensity, out avgHeight, out groupClass);

                    int classIndex = 6;
                    if (pointsCount < 12)
                    {
                        results[i, j] = new SubgroupOfPoints(7);
                        enoughPoints = false;
                    }
                    else
                    {
                        if (groupClass[0] >= (percentOfClassTypeThreshold * pointsCount))
                            classIndex = 0;
                        else if (groupClass[1] >= (percentOfClassTypeThreshold * pointsCount))
                            classIndex = 1;
                        else if (groupClass[2] >= (percentOfClassTypeThreshold * pointsCount))
                            classIndex = 2;
                        else if (groupClass[3] >= (percentOfClassTypeThreshold * pointsCount))
                            classIndex = 3;
                        else if (groupClass[4] >= (percentOfClassTypeThreshold * pointsCount))
                            classIndex = 4;
                        else if (groupClass[5] >= (percentOfClassTypeThreshold * pointsCount))
                            classIndex = 5;
                    }

                    if (enoughPoints)
                    {
                        SubgroupOfPoints group = CountSumOfDistancesFromPlane(matrix, vector, points, pointsCount, ref avgIntensity, ref avgHeight, classIndex);
                        results[i, j] = group;
                    }
                }
            }
            return results;
        }

        private static void CountGroupPointsParams(out bool enoughPoints, double[,] matrix, double[,] vector, List<LasPoint>[,] pieces, int i, int j, out List<LasPoint> points, out int pointsCount, out double avgIntensity, out double avgHeight, out int[] groupClass)
        {
            enoughPoints = true;

            for (int r = 0; r < 3; r++)
                for (int c = 0; c < 3; c++)
                    matrix[r, c] = 0;

            for (int v = 0; v < 3; v++)
                vector[0, v] = 0;

            points = pieces[i, j];
            pointsCount = points.Count;
            avgIntensity = 0;
            avgHeight = 0;
            groupClass = new int[7];
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

                avgIntensity += points[k].Intensity;
                avgHeight += points[k].Z;

                if (points[k].Classification == LasPoint.ClassificationType.Building)
                    groupClass[0]++;
                else if (points[k].Classification == LasPoint.ClassificationType.MediumVegetation)
                    groupClass[1]++;
                else if (points[k].Classification == LasPoint.ClassificationType.HighVegetation)
                    groupClass[2]++;
                else if (points[k].Classification == LasPoint.ClassificationType.LowVegetation)
                    groupClass[3]++;
                else if (points[k].Classification == LasPoint.ClassificationType.Ground)
                    groupClass[4]++;
                else if (points[k].Classification == LasPoint.ClassificationType.Water)
                    groupClass[5]++;
                else
                {
                    groupClass[6]++;
                }
            }
        }

        private static void PrepareGroupPointsData(LasFile file, int divisionCountX, int divisionCountY, out bool enoughPoints, out float percentOfClassTypeThreshold, out int PointsCount, out double minX, out double minY, out double ratioX, out double ratioY, out double[,] matrix, out double[,] vector, out List<LasPoint>[,] pieces)
        {
            enoughPoints = true;
            percentOfClassTypeThreshold = 0.07f;
            PointsCount = file.LasPointDataRecords.Count;
            Vector3[] Points;
            Points = new Vector3[4 * divisionCountX * divisionCountY];

            var scaleX = file.LasHeader.ScaleFactorX;
            var scaleY = file.LasHeader.ScaleFactorY;
            var scaleZ = file.LasHeader.ScaleFactorZ;

            minX = (float)(file.LasHeader.MinX / scaleX);
            minY = (float)(file.LasHeader.MinY / scaleY);
            double maxX = (float)(file.LasHeader.MaxX / scaleX);
            double maxY = (float)(file.LasHeader.MaxY / scaleY);
            double minZ = (float)(file.LasHeader.MinZ / scaleZ);

            double diffX = maxX - minX;
            double diffY = maxY - minY;

            ratioX = diffX / divisionCountX;
            ratioY = diffY / divisionCountY;
            matrix = new double[3, 3];
            vector = new double[1, 3];

            pieces = new List<LasPoint>[divisionCountX, divisionCountY];
            for (int i = 0; i < divisionCountX; i++)
                for (int j = 0; j < divisionCountY; j++)
                    pieces[i, j] = new List<LasPoint>();
        }

        private static SubgroupOfPoints CountSumOfDistancesFromPlane(double[,] matrix, double[,] vector, List<LasPoint> points, int pointsCount, ref double avgIntensity, ref double avgHeight, int classIndex)
        {
            avgIntensity /= pointsCount;
            avgHeight /= pointsCount;

            matrix[2, 2] = pointsCount;

            Matrix<double> A = DenseMatrix.OfArray(matrix);
            Matrix<double> b = DenseMatrix.OfArray(vector);
            Matrix<double> x;

            A = A.Inverse();
            x = b.Multiply(A);

            double[,] myarray;
            myarray = x.ToArray();

            Vector3 abc = new Vector3(Convert.ToSingle(myarray[0, 0]), Convert.ToSingle(myarray[0, 1]), Convert.ToSingle(myarray[0, 2]));
            double d = 0;

            for (int k = 0; k < pointsCount; k++)
                d += Math.Abs((abc.X * points[k].X) + (abc.Y * points[k].Y) + (abc.Z * points[k].Z)) / Math.Sqrt((abc.X * abc.X) + (abc.Y * abc.Y) + (abc.Z * abc.Z));

            d = d / pointsCount;

            SubgroupOfPoints group = new SubgroupOfPoints(classIndex, Convert.ToSingle(d), Convert.ToSingle(avgIntensity), abc, Convert.ToSingle(avgHeight));
            return group;
        }

        public static double MaxColumn(this double[][] input, int column)
        {
            double max = double.MinValue;
            for(int i = 0; i < input.Length; i++)
            {
                if (input[i][column] > max)
                    max = input[i][column];
            }
            return max;
        }

        public static double MinColumn(this double[][] input, int column)
        {
            double min = double.MaxValue;
            for (int i = 0; i < input.Length; i++)
            {
                if (input[i][column] < min)
                    min = input[i][column];
            }
            return min;
        }

        public static double DistanceFromPlane(LasPoint point, Vector3 abc)
        {
            return ((abc.X * point.X) + (abc.Y * point.Y) + (abc.Z * point.Z)) / ((abc.X * abc.X) + (abc.Y * abc.Y) + (abc.Z * abc.Z));
        }

        public static int IndexOfMax(this IMLData arr)
        {
            int index = 0;
            for(int i = 0; i < arr.Count; i++)
            {
                if(arr[i] > arr[index])
                {
                    index = i;
                }
            }
            return index;
        }

        public static double[] ClassToVector(LasPoint.ClassificationType classType)
        {
            double[] ideal = new double[] { 0, 0, 0, 0, 0, 0 };
            if (classType == LasPoint.ClassificationType.Building)
                ideal[0] = 1;
            else if (classType == LasPoint.ClassificationType.MediumVegetation)
                ideal[1] = 1;
            else if (classType == LasPoint.ClassificationType.HighVegetation)
                ideal[2] = 1;
            else if (classType == LasPoint.ClassificationType.LowVegetation)
                ideal[3] = 1;
            else if (classType == LasPoint.ClassificationType.Ground)
                ideal[4] = 1;
            else if (classType == LasPoint.ClassificationType.Water)
                ideal[5] = 1;
            return ideal;
        }
    }
}
