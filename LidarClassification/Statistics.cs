using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LASLibrary.LasPoint;

namespace LidarClassification
{
    class Statistics
    {
        public int Count;
        public static int NumberOfClasses = Enum.GetValues(typeof(ClassificationType)).Cast<int>().Max();
        public int[] ClassCount = new int[NumberOfClasses];
        public int[] ClassErrors = new int[NumberOfClasses];
        public int[] ClassRealCount = new int[NumberOfClasses];
        public int[,] PredictionMatrix = new int[NumberOfClasses, NumberOfClasses];
        public override string ToString()
        {
            string str = string.Empty;
            for(int i = 0; i < ClassCount.Length; i++)
            {
                if (ClassCount[i] == 0)
                    continue;
                str += ((ClassificationType)i).ToString() + " - sklasyfikowano " + ClassCount[i] + " z " + ClassRealCount[i] + ", błędnie sklasyfikowano "
                    + ClassErrors[i] + "\n";
            }

            double error = ClassErrors.Sum() / (double)Count;
            str += "Procent błędów: " + error * 100.0 + "% \n";
            str += GetMatrix();
            return str;
        }
        public void SaveMatrixAsCSV()
        {
            File.WriteAllText("matrix.csv", GetMatrix());
        }
        private string GetMatrix()
        {
            string str = " ,Ziemia,Niska roślinność, Średnia roślinność, Wysoka roślinność,Budynek,Woda\n";
            for (int i = 0; i < NumberOfClasses; i++)
            {
                if (Utills.IsSignificant((ClassificationType)i))
                {
                    if ((ClassificationType)i == ClassificationType.Ground)
                        str += "Ziemia,";
                    else if ((ClassificationType)i == ClassificationType.LowVegetation)
                        str += "Niska roślinność,";
                    else if ((ClassificationType)i == ClassificationType.MediumVegetation)
                        str += "Średnia roślinność,";
                    else if ((ClassificationType)i == ClassificationType.HighVegetation)
                        str += "Wysoka roślinność,";
                    else if ((ClassificationType)i == ClassificationType.Building)
                        str += "Budynek,";
                    else if ((ClassificationType)i == ClassificationType.Water)
                        str += "Woda,";

                }
                for (int j = 0; j < NumberOfClasses; j++)
                {
                    if (Utills.IsSignificant((ClassificationType)i) && Utills.IsSignificant((ClassificationType)j))
                        str += PredictionMatrix[i, j] + ", ";
                }

                if (Utills.IsSignificant((ClassificationType)i))
                {
                    str = str.Remove(str.Length - 2);
                    str += "\n";
                }
            }
            return str;
        }
    }
}
