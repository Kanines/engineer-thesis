using System;
using Encog.ML.Data.Basic;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog;
using Encog.ML.Train;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using LASLibrary;
using System.Windows.Forms;
using Encog.Util;
using System.Diagnostics;
using System.Threading.Tasks;

namespace LidarClassification
{
    public class EncogNeuralNetworkSlow
    {

        Random _rnd = new Random();
        public BasicNetwork Network;
        public double LearningError = 0;
        public double ClassificationError = 0;
        int inputNumber = 6;
        int regressionCount = 100;
        int regressionRange = 2;
        private void init(int layers = 4, int neuronsPerLayer = 15)
        {
            Network = new BasicNetwork();
            Network.AddLayer(new BasicLayer(null, true, inputNumber));
            for (int i = 0; i < layers; i++)
                Network.AddLayer(new BasicLayer(new ActivationElliott(), true, neuronsPerLayer));
            Network.AddLayer(new BasicLayer(new ActivationSoftMax(), false, 6));
            Network.Structure.FinalizeStructure();
            Network.Reset();
        }
        private EncogNeuralNetworkSlow()
        {
            init();
        }
        public EncogNeuralNetworkSlow(LasFile file)
        {
            var sw = Stopwatch.StartNew();
            int count = 300000;
            LasPointDataRecords points = file.LasPointDataRecords;
            double[][] input = new double[count][];
            double[][] ideal = new double[count][];
            int waterCount = 0;
            int groundCount = 0;
            int lowCount = 0;
            int mediumCount = 0;
            int highCount = 0;
            int buildingCount = 0;
            for (int i = 0; i < count; i++)
            {
                int rndNumber;
                while (true)
                {
                    rndNumber = _rnd.Next(0, points.Count - 1);
                    if (points[rndNumber].Classification == LasPoint.ClassificationType.Water)
                    {
                        waterCount++;
                        if (waterCount - 25 < count / 6)
                            break;
                    }
                    else if (points[rndNumber].Classification == LasPoint.ClassificationType.Ground)
                    {
                        groundCount++;
                        if (groundCount - 25 < count / 6)
                            break;
                    }
                    else if (points[rndNumber].Classification == LasPoint.ClassificationType.Building)
                    {
                        buildingCount++;
                        if (buildingCount - 25 < count / 6)
                            break;
                    }
                    else if (points[rndNumber].Classification == LasPoint.ClassificationType.LowVegetation)
                    {
                        lowCount++;
                        if (lowCount - 25 < count / 6)
                            break;
                    }
                    else if (points[rndNumber].Classification == LasPoint.ClassificationType.MediumVegetation)
                    {
                        mediumCount++;
                        if (mediumCount - 25 < count / 6)
                            break;
                    }
                    else if (points[rndNumber].Classification == LasPoint.ClassificationType.HighVegetation)
                    {
                        highCount++;
                        if (highCount - 25 < count / 6)
                            break;
                    }
                    if (highCount > 5 * count)
                        highCount = 0;
                    if (buildingCount > 5 * count)
                        buildingCount = 0;
                    if (lowCount > 5 * count)
                        lowCount = 0;
                    if (mediumCount > 5 * count)
                        mediumCount = 0;
                    if (waterCount > 5 * count)
                        waterCount = 0;
                    if (groundCount > 5 * count)
                        groundCount = 0;

                }
                if (i % 1000 == 0)
                    Console.WriteLine("Selected point: " + i + "/" + count);
                //double[] regression = LinearRegression.ComputeRegressionNumerics(file, points[rndNumber], regressionCount, regressionRange);
                OpenTK.Vector3 abc = LinearRegression.ComputeRegressionPoint(file, points[rndNumber], regressionCount, regressionRange);
                LasPoint3Short point = (LasPoint3Short)points[rndNumber];
                double distanceFromPlane = Utills.DistanceFromPlane(point, abc);
                double green = point.Green - (point.Red + point.Blue) / 2;
                input[i] = new double[] {green, file.LasHeader.ScaleZ(point.Z), point.Intensity, abc.X, abc.Y, abc.Z, distanceFromPlane };
                ideal[i] = Utills.ClassToVector(point.Classification);
            }
            inputNumber = input[0].Length;
            init();

            IMLDataSet trainingSet = new BasicMLDataSet(input, ideal);
            IMLTrain train = new ResilientPropagation(Network, trainingSet);
            int epoch = 1;
            do
            {
                train.Iteration();
                Console.WriteLine("Train error: " + train.Error + ", iteration: " + epoch);
                epoch++;
            } while (epoch < 1000);
            LearningError = train.Error;
            train.FinishTraining();
            sw.Stop();
            Console.WriteLine("Czas trwania [" + sw.Elapsed.TotalSeconds.ToString() + "s]");
        }

        public LasPoint.ClassificationType[] Classify(LasFile file, int count = 0)
        {
            var sw = Stopwatch.StartNew();
            LasPointDataRecords points = file.LasPointDataRecords;
            if (count == 0 || count > points.Count)
                count = points.Count;
            LasPoint.ClassificationType[] output = new LasPoint.ClassificationType[count];
            Statistics stats = new Statistics();
            stats.Count = count;
            OpenTK.Vector3[] abc = new OpenTK.Vector3[count];
            Parallel.For(0, count, (i) =>
            {
                abc[i] = LinearRegression.ComputeRegressionPoint(file, points[i], regressionCount, regressionRange);
                if (i % 1000 == 0)
                    Console.WriteLine(i);
            });
            for (int i = 0; i < count; i++)
            {
                //double[] regression = LinearRegression.ComputeRegressionNumerics(file, points[i], regressionCount, regressionRange);
                LasPoint3Short point = (LasPoint3Short)points[i];
                //OpenTK.Vector3 abc = LinearRegression.ComputeRegressionPoint(file, points[i], regressionCount, regressionRange);
                double distanceFromPlane = Utills.DistanceFromPlane(point, abc[i]);
                double green = point.Green - (point.Red + point.Blue) / 2;
                IMLData classed = Network.Compute(new BasicMLData(new double[] {green, file.LasHeader.ScaleZ(point.Z), point.Intensity,
                    abc[i].X, abc[i].Y, abc[i].Z, distanceFromPlane }));
                output[i] = Utills.QuickClassess[classed.IndexOfMax()];
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
        ~EncogNeuralNetworkSlow()
        {
            EncogFramework.Instance.Shutdown();
        }
        public void Serialize()
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "enns files (*.enns)|*.enns";
                dialog.FilterIndex = 2;
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SerializeObject.Save(dialog.FileName, Network);
                }
            }
        }

        public static EncogNeuralNetworkSlow DeSerialize()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "enns files (*.enns)|*.enns";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                var ENNetwork = new EncogNeuralNetworkSlow();
                ENNetwork.Network = (BasicNetwork)SerializeObject.Load(fileDialog.FileName);
                return ENNetwork;
            }
            return null;
        }
    }
}
