using Encog.Engine.Network.Activation;
using Encog.ML.Data;
using Encog.ML.Data.Basic;
using Encog.ML.Train;
using Encog.Neural.Networks;
using Encog.Neural.Networks.Layers;
using Encog.Neural.Networks.Training.Propagation.Resilient;
using Encog.Util;
using Encog.Util.Normalize;
using LASLibrary;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows.Forms;
using static LASLibrary.LasPoint;

namespace LidarClassification
{
    public class EncogNeuralNetworkQuick
    {
        //Encog.Neural.Pattern.BoltzmannPattern pattern = new BoltzmannPattern();
        Random _rnd = new Random();
        public BasicNetwork Network;
        public double LearningError = 0;
        public double ClassificationError = 0;
        int inputNumber = 6;
        int _divisionCountX;
        int _divisionCountY;
        DataNormalization norm = new DataNormalization();
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
        private EncogNeuralNetworkQuick()
        {
            init();
        }

        public EncogNeuralNetworkQuick(LasFile file, int divisionCountX, int divisionCountY)
        {
            var sw = Stopwatch.StartNew();
            _divisionCountX = divisionCountX;
            _divisionCountY = divisionCountY;

            var groupPointList = Utills.GroupPointsList(file, _divisionCountX, _divisionCountY);
            int count = groupPointList.Count / 5;
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
                ClassificationType simpleClass;
                while (true)
                {
                    rndNumber = _rnd.Next(0, groupPointList.Count - 1);
                    if (!Utills.QuickClassess.TryGetValue(groupPointList[rndNumber].classIndex, out simpleClass))
                        continue;
                    if (simpleClass == LasPoint.ClassificationType.Water)
                    {
                        waterCount++;
                        if (waterCount - 25 < count / 6)
                            break;
                    }
                    else if (simpleClass == LasPoint.ClassificationType.Ground)
                    {
                        groundCount++;
                        if (groundCount - 25 < count / 6)
                            break;
                    }
                    else if (simpleClass == LasPoint.ClassificationType.Building)
                    {
                        buildingCount++;
                        if (buildingCount - 25 < count / 6)
                            break;
                    }
                    else if (simpleClass == LasPoint.ClassificationType.LowVegetation)
                    {
                        lowCount++;
                        if (lowCount - 25 < count / 6)
                            break;
                    }
                    else if (simpleClass == LasPoint.ClassificationType.MediumVegetation)
                    {
                        mediumCount++;
                        if (mediumCount - 25 < count / 6)
                            break;
                    }
                    else if (simpleClass == LasPoint.ClassificationType.HighVegetation)
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

                double avgHeight = groupPointList[rndNumber].avgHeight;
                double avgIntensity = groupPointList[rndNumber].avgIntensity;
                double avgDistance = groupPointList[rndNumber].avgDistance;
                OpenTK.Vector3 slopeVector = groupPointList[rndNumber].slopeVector;
                input[i] = new double[] { avgDistance, avgHeight, avgIntensity, slopeVector[0], slopeVector[1], slopeVector[2] };
                ideal[i] = Utills.ClassToVector(simpleClass);
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

        public ClassificationType[,] Classify(LasFile file)
        {
            var sw = Stopwatch.StartNew();
            ClassificationType[,] output = new ClassificationType[_divisionCountX, _divisionCountY];
            SubgroupOfPoints[,] values = Utills.GroupPoints(file, _divisionCountX, _divisionCountY);
            Statistics stats = new Statistics();
            stats.Count = _divisionCountX * _divisionCountY;
            for (int i = 0; i < _divisionCountX; i++)
            {
                for (int j = 0; j < _divisionCountY; j++)
                {
                    double avgHeight = values[i, j].avgHeight;
                    double avgIntensity = values[i, j].avgIntensity;
                    double avgDistance = values[i, j].avgDistance;
                    OpenTK.Vector3 slopeVector = values[i, j].slopeVector;
                    IMLData classed = Network.Compute(new BasicMLData(
                        new double[] { avgDistance, avgHeight, avgIntensity, slopeVector[0], slopeVector[1], slopeVector[2] }));
                    output[i, j] = Utills.QuickClassess[classed.IndexOfMax()];
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
                Console.WriteLine(i);
            }
            Console.Write(stats.ToString());
            sw.Stop();
            Console.WriteLine("Czas trwania [" + sw.Elapsed.TotalSeconds.ToString() + "s]");
            stats.SaveMatrixAsCSV();
            return output;
        }

        public void Serialize()
        {
            using (SaveFileDialog dialog = new SaveFileDialog())
            {
                dialog.Filter = "ennq files (*.ennq)|*.ennq";
                dialog.FilterIndex = 2;
                dialog.RestoreDirectory = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    SerializeObject.Save(dialog.FileName, Network);
                }
            }
        }

        public static EncogNeuralNetworkQuick DeSerialize(int divisionCountX, int divisionCountY)
        {
            var fileDialog = new OpenFileDialog();
            fileDialog.Filter = "ennq files (*.ennq)|*.ennq";
            if (fileDialog.ShowDialog() == DialogResult.OK)
            {
                var ENNetwork = new EncogNeuralNetworkQuick();
                ENNetwork.Network = (BasicNetwork)SerializeObject.Load(fileDialog.FileName);
                ENNetwork._divisionCountX = divisionCountX;
                ENNetwork._divisionCountY = divisionCountY;
                return ENNetwork;
            }
            return null;
        }
    }
}
