using Accord.MachineLearning;
using Accord.Math;
using LASLibrary;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LidarClassification
{
    [Serializable]
    public class KnnAccordClassifierSlow
    {
        static int regressionCount = 100;
        static int regressionRange = 2;
        KNearestNeighbors knn;
        int k;

        public KnnAccordClassifierSlow(int k)
        {
            this.k = k;
        }

        public void Teach(double[][] inputs, int[] outputs)
        {
            if (knn != null) return;
            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine("Teaching KNN...");
            knn = new KNearestNeighbors(k, Utills.ClassificationClasses.Length, inputs, outputs);
            sw.Stop();
            Console.WriteLine("Teaching KNN completed [" + sw.Elapsed.TotalSeconds.ToString() + "s]");
        }

        public LasPoint.ClassificationType[] Classify(LasFile file)
        {
            var sw = Stopwatch.StartNew();
            LasPointDataRecords points = file.LasPointDataRecords;
            int pointsCount = points.Count();
            LasPoint.ClassificationType[] output = new LasPoint.ClassificationType[pointsCount];         
            Statistics stats = new Statistics();
            stats.Count = pointsCount;
            OpenTK.Vector3[] slopeVector = new OpenTK.Vector3[pointsCount];

            Parallel.For(0, pointsCount, (i) =>
            {
                slopeVector[i] = LinearRegression.ComputeRegressionPoint(file, points[i], regressionCount, regressionRange);
                if (i % 1000 == 0)
                    Console.WriteLine("ComputeRegression "  + i);
            });

            for (int i = 0; i < pointsCount; i++)
            {
                LasPoint3Short point = (LasPoint3Short)points[i];
                double distanceFromPlane = Utills.DistanceFromPlane(point, slopeVector[i]);
                double green = point.Green - (point.Red + point.Blue) / 2;

                output[i] = Utills.ClassificationClasses[knn.Compute(new double[] { green,
                    file.LasHeader.ScaleZ(point.Z), point.Intensity, slopeVector[i].X,
                    slopeVector[i].Y, slopeVector[i].Z, distanceFromPlane })];

                if (output[i] != points[i].Classification)
                {
                    stats.ClassErrors[(int)points[i].Classification]++;
                }
                stats.ClassCount[(int)output[i]]++;
                stats.ClassRealCount[(int)points[i].Classification]++;
                stats.PredictionMatrix[(int)points[i].Classification, (int)output[i]]++;
                if (i % 1000 == 0)
                    Console.WriteLine(i);
            }
            Console.Write(stats.ToString());
            sw.Stop();
            Console.WriteLine("Czas trwania [" + sw.Elapsed.TotalSeconds.ToString() + "s]");
            stats.SaveMatrixAsCSV();
            return output;
        }

        public void Reset()
        {
            knn = null;
        }

        public static Tuple<double[], int> GetInputOutputFromClassificationType(LasPoint point, LasFile file)
        {
            var abc = LinearRegression.ComputeRegressionPoint(file, point, regressionCount, regressionRange);
            double distanceFromPlane = Utills.DistanceFromPlane(point, abc);
            LasPoint3Short pointShort = (LasPoint3Short)point;
            double green = pointShort.Green - (pointShort.Red + pointShort.Blue) / 2;
            var input = new double[] { green, file.LasHeader.ScaleZ(point.Z), point.Intensity, abc.X, abc.Y, abc.Z, distanceFromPlane };
            int output;

            switch (point.Classification)
            {
                case LasPoint.ClassificationType.Ground:
                    output = 0;
                    break;
                case LasPoint.ClassificationType.HighVegetation:
                    output = 1;
                    break;
                case LasPoint.ClassificationType.Building:
                    output = 2;
                    break;
                case LasPoint.ClassificationType.MediumVegetation:
                    output = 3;
                    break;
                case LasPoint.ClassificationType.LowVegetation:
                    output = 4;
                    break;
                case LasPoint.ClassificationType.Water:
                    output = 5;
                    break;
                default:
                    input = new double[] { -1, -1, -1, -1, -1, -1, -1 };
                    output = 6;
                    break;
            }
            return Tuple.Create(input, output);
        }

        public static Tuple<double[][], int[]> MakeInputOutputs(List<LasPoint> points, LasFile file)
        {
            int count = points.Count;
            int[] outputs = new int[count];
            double[][] inputs = new double[count][];


            Parallel.For(0, count, (i) =>
            {
                LasPoint point = points[i];

                Console.WriteLine("Preparing Teaching Dataset " + i + "/" + count);
                var inout = GetInputOutputFromClassificationType(point, file);
                inputs[i] = inout.Item1;
                outputs[i] = inout.Item2;
            });

            return new Tuple<double[][], int[]>(inputs, outputs);
        }

        public bool CanClassify
        {
            get { return knn != null; }
        }           
    }
}
