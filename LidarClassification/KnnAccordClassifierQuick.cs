using Accord.MachineLearning;
using LASLibrary;
using System;
using System.Collections.Generic;
using OpenTK;
using static LASLibrary.LasPoint;
using System.Diagnostics;
using System.Linq;

namespace LidarClassification
{
    [Serializable]
    public class KnnAccordClassifierQuick
    {
        KNearestNeighbors knn;
        int k;

        public KnnAccordClassifierQuick(int k)
        {
            this.k = k;
        }

        public void TeachGroups(double[][] inputs, int[] outputs)
        {
            if (knn != null) return;
            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine("Teaching KNN...");
            knn = new KNearestNeighbors(k, Utills.ClassificationClasses.Length, inputs, outputs);
            sw.Stop();
            Console.WriteLine("Teaching KNN completed [" + sw.Elapsed.TotalSeconds.ToString() + "s]");
        }

        public void Reset()
        {
            knn = null;
        }

        public bool CanClassify
        {
            get { return knn != null; }
        }

        public ClassificationType[,] Classify(LasFile file, int divCountX, int divCountY)
        {
            Stopwatch swTotal = Stopwatch.StartNew();
            Stopwatch sw = Stopwatch.StartNew();
            Console.WriteLine("Preparing testing dataset...");
            LasPointDataRecords points = file.LasPointDataRecords;
            ClassificationType[,] output = new ClassificationType[divCountX, divCountY];
            SubgroupOfPoints[,] values = Utills.GroupPoints(file, divCountX, divCountY);
            Statistics stats = new Statistics();
            stats.Count = divCountX * divCountY;
            sw.Stop();
            Console.WriteLine("Preparing testing dataset completed [" + sw.Elapsed.TotalSeconds.ToString() + "s]");
            Stopwatch sw2 = Stopwatch.StartNew();
            Console.WriteLine("Classification in progress...");

            int noiseCount = 0;

            for (int i = 0; i < divCountX; i++)
            {
                for (int j = 0; j < divCountY; j++)
                {
                    if (values[i, j].classIndex == 7)
                    {
                        output[i, j] = ClassificationType.Noise;
                        noiseCount++;
                    }
                    else
                    {
                        double avgHeight = values[i, j].avgHeight;
                        double avgIntensity = values[i, j].avgIntensity;
                        double avgDistance = values[i, j].avgDistance;
                        OpenTK.Vector3 slopeVector = values[i, j].slopeVector;

                        output[i, j] = Utills.ClassificationClasses[knn.Compute(new double[] {
                            avgDistance, avgHeight, avgIntensity,  slopeVector[0],
                            slopeVector[1], slopeVector[2] })];

                        ClassificationType ct;
                        if (!Utills.QuickClassess.TryGetValue(values[i, j].classIndex, out ct))
                            continue;
                        if (output[i, j] != ct)
                        {
                            stats.ClassErrors[(int)ct]++;
                        }
                        stats.PredictionMatrix[(int)output[i, j], (int)ct]++;
                        stats.PredictionMatrix[(int)ct, (int)output[i, j]]++;
                        stats.ClassCount[(int)output[i, j]]++;
                        stats.ClassRealCount[(int)ct]++;
                    }
                }
                //Console.WriteLine(i);
            }
            Console.Write(stats.ToString());
            sw2.Stop();
            Console.WriteLine("Classification completed [" + sw2.Elapsed.TotalSeconds.ToString() + "s]");
            swTotal.Stop();
            Console.WriteLine("Total time: [" + swTotal.Elapsed.TotalSeconds.ToString() + "s]");
            Console.WriteLine("Noise count: " + noiseCount.ToString());
            stats.SaveMatrixAsCSV();
            return output;
        }

        public static Tuple<double[][], int[]> MakeInputOutputs(List<SubgroupOfPoints> values)
        {
            Console.WriteLine("Preparing training dataset... ");
            Stopwatch sw = Stopwatch.StartNew();

            int count = values.Count;
            int[] outputs = new int[count];
            double[][] inputs = new double[count][];

            for (int i = 0; i < count; i++)
            {
                SubgroupOfPoints value = values[i];
          
                // ClassificationType.Ground
                if (value.classIndex == 4)
                {
                    inputs[i] = new double[] { value.avgDistance, value.avgHeight, value.avgIntensity, value.slopeVector.X, value.slopeVector.Y, value.slopeVector.Z };
                    outputs[i] = 0;
                }
                // ClassificationType.HighVegetation
                else if (value.classIndex == 2)
                {
                    inputs[i] = new double[] { value.avgDistance, value.avgHeight, value.avgIntensity, value.slopeVector.X, value.slopeVector.Y, value.slopeVector.Z };
                    outputs[i] = 1;
                }

                // ClassificationType.Building
                else if (value.classIndex == 0)
                {
                    inputs[i] = new double[] { value.avgDistance, value.avgHeight, value.avgIntensity, value.slopeVector.X, value.slopeVector.Y, value.slopeVector.Z };
                    outputs[i] = 2;
                }
                // ClassificationType.MediumVegetation
                else if (value.classIndex == 1)
                {
                    inputs[i] = new double[] { value.avgDistance, value.avgHeight, value.avgIntensity, value.slopeVector.X, value.slopeVector.Y, value.slopeVector.Z };
                    outputs[i] = 3;
                }               
                // ClassificationType.LowVegetation
                else if (value.classIndex == 3)
                {
                    inputs[i] = new double[] { value.avgDistance, value.avgHeight, value.avgIntensity, value.slopeVector.X, value.slopeVector.Y, value.slopeVector.Z };
                    outputs[i] = 4;
                }               
                // ClassificationType.Water
                else if (value.classIndex == 5)
                {
                    inputs[i] = new double[] { value.avgDistance, value.avgHeight, value.avgIntensity, value.slopeVector.X, value.slopeVector.Y, value.slopeVector.Z };
                    outputs[i] = 5;
                }
            }
            sw.Stop();
            Console.WriteLine("Preparing training dataset completed [" + sw.Elapsed.TotalSeconds.ToString() + "s]");

            return new Tuple<double[][], int[]>(inputs, outputs);
        }   
    }
}
