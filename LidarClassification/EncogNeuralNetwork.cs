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

namespace LidarClassification
{
    public class EncogNeuralNetwork
    {
        Random _rnd = new Random();
        public BasicNetwork Network;
        public double LearningError = 0;
        public double ClassificationError = 0;
        int inputNumber = 3;
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
        public EncogNeuralNetwork()
        {
            init();
        }
        public EncogNeuralNetwork(LasFile file)
        {
            Stopwatch sw = Stopwatch.StartNew();
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
            for(int i = 0; i < count; i++)
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
                if (i % 100 == 0)
                    Console.WriteLine(i);
                LasPoint3Short point = (LasPoint3Short)points[rndNumber];
                double green = point.Green - (point.Red + point.Blue) / 2;
                input[i] = new double[] { file.LasHeader.ScaleZ(point.Z), point.Intensity, green };
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
                Console.WriteLine(train.Error + " | " + epoch);
                epoch++;
            } while (epoch < 1000);
            LearningError = train.Error;
            train.FinishTraining();
            sw.Stop();
            Console.WriteLine("Czas trwania [" + sw.Elapsed.TotalSeconds.ToString() + "s]");
        }

        public LasPoint.ClassificationType[] Classify(LasFile file)
        {
            Stopwatch sw = Stopwatch.StartNew();
            LasPointDataRecords points = file.LasPointDataRecords;
            LasPoint.ClassificationType[] output = new LasPoint.ClassificationType[points.Count];
            Statistics stats = new Statistics();
            stats.Count = points.Count;
            for(int i = 0; i < points.Count; i++)
            {
                LasPoint3Short point = (LasPoint3Short)points[i];
                double green = point.Green - (point.Red + point.Blue) / 2;
                IMLData classed = Network.Compute(
                    new BasicMLData(new double[] { file.LasHeader.ScaleZ(point.Z), point.Intensity, green }));
                output[i] = Utills.QuickClassess[classed.IndexOfMax()];
                if (output[i] != points[i].Classification)
                {
                    stats.ClassErrors[(int)points[i].Classification]++;
                }
                stats.PredictionMatrix[(int)points[i].Classification, (int)output[i]]++;
                stats.ClassCount[(int)output[i]]++;
                stats.ClassRealCount[(int)points[i].Classification]++;
                if (i % 1000 == 0)
                    Console.WriteLine(i);
            }
            Console.Write(stats.ToString());
            sw.Stop();
            Console.WriteLine("Czas trwania [" + sw.Elapsed.TotalSeconds.ToString() + "s]");
            stats.SaveMatrixAsCSV();
            return output;
        }
        ~EncogNeuralNetwork()
        {
            EncogFramework.Instance.Shutdown();
        }

        public void Serialize()
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "enn files (*.enn)|*.enn";
                dialog.FilterIndex = 2;
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SerializeObject.Save(dialog.FileName, Network);
                }
            }
        }

        public static EncogNeuralNetwork DeSerialize()
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "enn files (*.enn)|*.enn";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                var ENNetwork = new EncogNeuralNetwork();
                ENNetwork.Network = (BasicNetwork)SerializeObject.Load(fileDialog.FileName);
                return ENNetwork;
            }
            return null;
        }
    }
}
