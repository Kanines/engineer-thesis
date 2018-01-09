using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using LASLibrary;
using LidarClassification;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using PixelFormat = System.Drawing.Imaging.PixelFormat;
using System.IO;
using Encog.Util;
using Encog.Neural.Networks;
using OpenTK;

namespace Visualisation
{
    public partial class Form1 : Form
    {
        bool Loaded;
        bool LeftButtonMouse;
        bool RightButtonMouse;
        bool viewChangeProj;
        bool viewChangeCol;
        OpenGlController Controller;
        Point LeftMouseDownLocation;
        Point RightMouseDownLocation;
        Form2 InfoFileForm;

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();

        EncogNeuralNetwork ENNetwork;
        EncogNeuralNetworkQuick ENNetworkQ;
        EncogNeuralNetworkSlow ENNetworkS;
        KnnAccordClassifier KNNClassifier;
        KnnAccordClassifierQuick KNNClassifierQuick;
        KnnAccordClassifierSlow KNNClassifierSlow;

        public static Dictionary<LasPoint.ClassificationType, Color> ClassificationToColor = new Dictionary
            <LasPoint.ClassificationType, Color>
        {
            {LasPoint.ClassificationType.Ground, Color.FromArgb(102, 45, 0)},
            {LasPoint.ClassificationType.LowVegetation, Color.FromArgb(128, 128, 0)},
            {LasPoint.ClassificationType.MediumVegetation, Color.FromArgb(102, 255, 102)},
            {LasPoint.ClassificationType.HighVegetation, Color.FromArgb(0, 102, 0)},
            {LasPoint.ClassificationType.Building, Color.FromArgb(80, 80, 80)},
            {LasPoint.ClassificationType.Noise, Color.FromArgb(255, 10, 10)},
            {LasPoint.ClassificationType.Water, Color.FromArgb(0, 0, 204)},
            {LasPoint.ClassificationType.Other, Color.FromArgb(0, 191, 255)},
            {LasPoint.ClassificationType.Created, Color.FromArgb(192, 192, 192)},
            {LasPoint.ClassificationType.Unclassified, Color.FromArgb(192, 192, 192)},
            {LasPoint.ClassificationType.MassPoint, Color.FromArgb(192, 192, 192)},
            {LasPoint.ClassificationType.OverlapPoints, Color.FromArgb(192, 192, 192)}
        };

        public Form1()
        {
            InitializeComponent();
            //Controller.File = new LASFile("N-34-50-C-c-4-2-2-1.las");
            //LoadFileInfo();
            pictureBox1.BackColor = ClassificationToColor[LasPoint.ClassificationType.Ground];
            pictureBox2.BackColor = ClassificationToColor[LasPoint.ClassificationType.LowVegetation];
            pictureBox3.BackColor = ClassificationToColor[LasPoint.ClassificationType.MediumVegetation];
            pictureBox4.BackColor = ClassificationToColor[LasPoint.ClassificationType.HighVegetation];
            pictureBox5.BackColor = ClassificationToColor[LasPoint.ClassificationType.Building];
            pictureBox6.BackColor = ClassificationToColor[LasPoint.ClassificationType.Noise];
            pictureBox7.BackColor = ClassificationToColor[LasPoint.ClassificationType.Water];
            pictureBox8.BackColor = ClassificationToColor[LasPoint.ClassificationType.Other];
            ColorModeBox.SelectedIndex = 1;
            ProjectionBox.SelectedIndex = 0;

            LegendBox.Hide();
            KlasyfikatorGroupBox.Hide();
            AllocConsole();
        }

        public static void BrakPlikuMessageBox()
        {
            MessageBox.Show("Nie otworzono pliku LAS!", "Uwaga!",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void KoniecTreninguMessageBox()
        {
            MessageBox.Show("Zakończono trening!", "Informacja!",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        public static void KoniecKlasyfikacjiMessageBox()
        {
            MessageBox.Show("Zakończono klasyfikacje!", "Informacja",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        void LoadFileInfo()
        {
            Text = "Visualisation - " + Controller.File.Filename;
            NumberOfPointsLabel.Text = Controller.File.LasPointDataRecords.Count + " punktów";
            pointsFormatLabel.Text = ((int)Controller.File.LasPointDataRecords.Format).ToString();
        }

        void GlControl1_Paint(object sender, PaintEventArgs e)
        {
            if (!Loaded)
                return;
            Controller.Paint();
            GlControl1.SwapBuffers();
        }

        void Form1_Load_1(object sender, EventArgs e)
        {
            // Controller = new Controller(Controller.File);
            Loaded = true;
            Controller = new OpenGlController(GlControl1.Width, GlControl1.Height);
            GlControl1_Paint(null, null);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Right:
                    Controller.CameraProp.Move(10, 0, 0);
                    GlControl1.Invalidate();
                    return true;
                case Keys.Left:
                    Controller.CameraProp.Move(-10, 0, 0);
                    GlControl1.Invalidate();
                    return true;
                case Keys.Up:
                    Controller.CameraProp.Move(0, -10, 0);
                    GlControl1.Invalidate();
                    return true;
                case Keys.Down:
                    Controller.CameraProp.Move(0, 10, 0);
                    GlControl1.Invalidate();
                    return true;
                case Keys.W:
                    Controller.CameraProp.AddRotation(1);
                    GlControl1.Invalidate();
                    return true;
                case Keys.S:
                    Controller.CameraProp.AddRotation(-1);
                    GlControl1.Invalidate();
                    return true;
                case Keys.A:
                    Controller.CameraProp.AddRotation(0, 1);
                    GlControl1.Invalidate();
                    return true;
                case Keys.D:
                    Controller.CameraProp.AddRotation(0, -1);
                    GlControl1.Invalidate();
                    return true;
                case Keys.Z:
                    Controller.CameraProp.Move(0, 0, -10);
                    GlControl1.Invalidate();
                    return true;
                case Keys.X:
                    Controller.CameraProp.Move(0, 0, 10);
                    GlControl1.Invalidate();
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        void wczytajToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Las Files|*.las;";
            DialogResult dr = ofd.ShowDialog();

            if (dr != DialogResult.OK) return;

            Controller.File = new LasFile(ofd.FileName);
            InfoFileForm = new Form2(Controller.File.LasHeader);
            ColorModeBox.SelectedIndex = 1;
            ProjectionBox.SelectedIndex = 0;
            viewChangeProj = false;
            viewChangeCol = false;
            LoadFileInfo();
            OpenGlController.drawQuads = false;
            OpenGlController.isClassified = false;
            Controller.LoadPoints2D();
            Controller.LoadColorsByRealColors();
            Controller.CameraProp.ResetCamera();
            GlControl1_Paint(null, null);
            KlasyfikatorComboBox.SelectedIndex = Settings.Default.KlasyfikatorBoxIndex;
        }

        void sterowanieToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(
                "Przesuwanie kamery strzałkami, przybliżanie klawiszem Z oddalanie X. Obrót kamerą W,S,A,D. Sterowanie możliwe również myszą.",
                "Sterowanie");
        }

        void Form1_Resize(object sender, EventArgs e)
        {
            if (!Loaded)
                return;
            Controller.ChangeViewport(0, 0, GlControl1.Width, GlControl1.Height);
            GlControl1_Paint(null, null);
        }

        void centerButton_Click(object sender, EventArgs e)
        {
            if (!Loaded || Controller.File == null)
                return;
            Controller.CameraProp.ResetCamera();
            GlControl1_Paint(null, null);
        }

        void Form1_MouseWheel(object sender, MouseEventArgs e)
        {
            if (!Loaded || Controller.File == null)
                return;

            Controller.CameraProp.Move(0, 0, -e.Delta * 0.5f);
            GlControl1.Invalidate();
        }

        void GlControl1_MouseDown(object sender, MouseEventArgs e)
        {
            if (!Loaded || Controller.File == null)
                return;
            switch (e.Button)
            {
                case MouseButtons.Left:
                    LeftMouseDownLocation = e.Location;
                    LeftButtonMouse = true;
                    break;
                case MouseButtons.Right:
                    RightMouseDownLocation = e.Location;
                    RightButtonMouse = true;
                    break;
            }
        }

        void GlControl1_MouseMove(object sender, MouseEventArgs e)
        {
            if (!Loaded || Controller.File == null)
                return;
            if (e.Button == MouseButtons.Left && LeftButtonMouse)
            {
                Controller.CameraProp.Move(-(e.X - LeftMouseDownLocation.X) * Controller.CameraProp.Zoom,
                    -(e.Y - LeftMouseDownLocation.Y) * Controller.CameraProp.Zoom, 0);
                GlControl1.Invalidate();
                LeftMouseDownLocation = e.Location;
            }
            else if (e.Button == MouseButtons.Right && RightButtonMouse)
            {
                Controller.CameraProp.AddRotation(y: -(e.X - RightMouseDownLocation.X) * 0.25f,
                    x: -(e.Y - RightMouseDownLocation.Y) * 0.25f);
                GlControl1.Invalidate();
                RightMouseDownLocation = e.Location;
            }
        }

        void GlControl1_MouseUp(object sender, MouseEventArgs e)
        {
            if (!Loaded)
                return;
            if (Controller.File == null)
            {
                BrakPlikuMessageBox();
                return;
            }
            if (e.Button == MouseButtons.Left && LeftButtonMouse)
                LeftButtonMouse = false;
            else if (e.Button == MouseButtons.Right && RightButtonMouse)
                RightButtonMouse = false;
        }

        void właściwościToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!Loaded)
                return;
            if (Controller.File == null)
            {
                BrakPlikuMessageBox();
                return;
            }
            InfoFileForm.Show();
        }

        void eksportObrazuToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (Controller.File == null)
            {
                BrakPlikuMessageBox();
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG image|*.png";
            saveFileDialog.AddExtension = true;
            DialogResult dr = saveFileDialog.ShowDialog();
            if (dr != DialogResult.OK) return;

            Bitmap screen = GrabScreenshot();
            screen.Save(saveFileDialog.FileName, ImageFormat.Png);
        }

        public Bitmap GrabScreenshot()
        {
            if (GraphicsContext.CurrentContext == null)
                throw new GraphicsContextMissingException();

            Bitmap bmp = new Bitmap(GlControl1.Width, GlControl1.Height);
            BitmapData data =
                bmp.LockBits(GlControl1.DisplayRectangle, ImageLockMode.WriteOnly,
                    PixelFormat.Format24bppRgb);
            GL.ReadPixels(0, 0, GlControl1.Width, GlControl1.Height, OpenTK.Graphics.OpenGL.PixelFormat.Bgr, PixelType.UnsignedByte, data.Scan0);
            bmp.UnlockBits(data);

            bmp.RotateFlip(RotateFlipType.RotateNoneFlipY);
            return bmp;
        }

        private void viewButton_Click(object sender, EventArgs e)
        {
            RefreshView();
        }

        private void RefreshView()
        {
            if (Controller.File == null)
            {
                BrakPlikuMessageBox();
                return;
            }

            if (!Loaded || (!viewChangeProj && !viewChangeCol))
                return;

            //if (viewChangeCol)
            //{
                if (ColorModeBox.SelectedItem.Equals("prawdziwe kolory"))
                    Controller.LoadColorsByRealColors();
                else if (ColorModeBox.SelectedItem.Equals("wysokość"))
                    Controller.LoadColorsByHeight();
                else if (ColorModeBox.SelectedItem.Equals("intensywność"))
                    Controller.LoadColorsByIntensity();
                else if (ColorModeBox.SelectedItem.Equals("klasa"))
                    Controller.LoadColorsByClassification();
                else if (ColorModeBox.SelectedItem.Equals("regresja liniowa 3D"))
                    Controller.LoadLinearRegressionQuads3D();
                else if (ColorModeBox.SelectedItem.Equals("własna klasyfikacja") && (KlasyfikatorComboBox.SelectedIndex == 2))
                {
                    if (OpenGlController.isClassified)
                        Controller.LoadColorsByClassificationSubgroups();
                    else
                        MessageBox.Show("Najpierw wykonaj klasyfikację terenu!", "Informacja!",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                //viewChangeCol = false;
            //}
           
            if (ProjectionBox.SelectedItem.Equals("2D"))
                Controller.LoadPoints2D();
            else if (ProjectionBox.SelectedItem.Equals("3D"))
                Controller.LoadPoints3D();
            viewChangeProj = false;
            
            GlControl1_Paint(null, null);
        }

        private void ProjectionBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.ProjectionBoxIndex = ProjectionBox.SelectedIndex;
            viewChangeProj = true;
        }

        private void ColorModeBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.ColorModeBoxIndex = ColorModeBox.SelectedIndex;
            viewChangeCol = true;

            LegendBox.Hide();
            KlasyfikatorGroupBox.Hide();
            if (ColorModeBox.SelectedItem.Equals("klasa"))
            {
                LegendBox.Show();
            }
            else if (ColorModeBox.SelectedItem.Equals("własna klasyfikacja"))
            {
                KlasyfikatorComboBox.SelectedIndex = Settings.Default.KlasyfikatorBoxIndex;
                LegendBox.Show();
                KlasyfikatorGroupBox.Show();
            }
        }

        private async void TrainButtonClick(object sender, EventArgs e)
        {
            if (Controller.File == null)
            {
                BrakPlikuMessageBox();
                return;
            }

            switch (KlasyfikatorComboBox.SelectedIndex)
            {
                //2plik = 0.056 4plik = 0.066 [300k points]
                case 0:
                    DisableChangeButtons();
                    await Task.Run(() =>
                    {
                        KNNClassifier = new KnnAccordClassifier(3);
                        if (!KNNClassifier.CanClassify)
                        {
                            var pointsToTrain = Utills.GenerateClassArray(Controller.File.LasPointDataRecords, 0.066);
                            var inputOutputArrays = KnnAccordClassifier.MakeInputOutputs(pointsToTrain, Controller.File);
                            KNNClassifier.Teach(inputOutputArrays.Item1, inputOutputArrays.Item2);
                        }
                    });
                    EnableChangeButtons();
                    break;
                 case 1:
                    DisableChangeButtons();
                    await Task.Run(() =>
                    {
                        ENNetwork = new EncogNeuralNetwork(Controller.File);
                    });
                    EnableChangeButtons();
                    break;
                case 2:
                    DisableChangeButtons();
                    await Task.Run(() =>
                    {
                        KNNClassifierQuick = new KnnAccordClassifierQuick(3);
                        if (!KNNClassifierQuick.CanClassify)
                        {
                            var inputOutputArrays =
                                KnnAccordClassifierQuick.MakeInputOutputs(Utills.GroupPointsList(
                                        Controller.File, OpenGlController.divisionCountX, OpenGlController.divisionCountY));
                            KNNClassifierQuick.TeachGroups(inputOutputArrays.Item1, inputOutputArrays.Item2);

                        }
                    });
                    EnableChangeButtons();
                    break;
                case 3:
                    DisableChangeButtons();
                    await Task.Run(() =>
                    {
                        ENNetworkQ = new EncogNeuralNetworkQuick(Controller.File, OpenGlController.divisionCountX, OpenGlController.divisionCountY);
                    });
                    EnableChangeButtons();
                    break;
                case 4:
                    DisableChangeButtons();
                    await Task.Run(() =>
                    {
                        KNNClassifierSlow = new KnnAccordClassifierSlow(3);
                        if (!KNNClassifierSlow.CanClassify)
                        {
                            var pointsToTrain = Utills.GenerateClassArray(Controller.File.LasPointDataRecords, 0.10);
                            var inputOutputArrays = KnnAccordClassifierSlow.MakeInputOutputs(pointsToTrain, Controller.File);
                            KNNClassifierSlow.Teach(inputOutputArrays.Item1, inputOutputArrays.Item2);
                        }
                    });
                    EnableChangeButtons();
                    break;
                case 5:
                    DisableChangeButtons();
                    await Task.Run(() =>
                    {
                        ENNetworkS = new EncogNeuralNetworkSlow(Controller.File);
                    });
                    EnableChangeButtons();
                    break;
            }
            KoniecTreninguMessageBox();
        }

        private async void ClassifyButtonClick(object sender, EventArgs e)
        {
            if (Controller.File == null)
            {
                BrakPlikuMessageBox();
                return;
            }
            if (KlasyfikatorComboBox.SelectedIndex == 0 && KNNClassifier != null)
            {
                LasPoint.ClassificationType[] output = null;
                DisableChangeButtons();
                await Task.Run(() =>
                {
                    output = KNNClassifier.Classify(Controller.File);
                });
                Controller.LoadColorsByKnnClassification(output);
                EnableChangeButtons();
                KoniecKlasyfikacjiMessageBox();
            }
            else if (KlasyfikatorComboBox.SelectedIndex == 1 && ENNetwork != null)
            {
                LasPoint.ClassificationType[] output = null;
                DisableChangeButtons();
                await Task.Run(() =>
                {
                    output = ENNetwork.Classify(Controller.File);
                });
                Controller.LoadColorsFromClassArray(output);
                EnableChangeButtons();
                KoniecKlasyfikacjiMessageBox();
            }
            else if (KlasyfikatorComboBox.SelectedIndex == 2 && KNNClassifierQuick != null)
            {
                LasPoint.ClassificationType[,] output = null;
                DisableChangeButtons();
                await Task.Run(() =>
                {
                    output = KNNClassifierQuick.Classify(Controller.File, OpenGlController.divisionCountX, OpenGlController.divisionCountY);
                });
                Controller.subsetClass = output;
                Controller.LoadColorsByClassificationSubgroups();
                EnableChangeButtons();
                OpenGlController.isClassified = true;
                KoniecKlasyfikacjiMessageBox();               
            }
            else if (KlasyfikatorComboBox.SelectedIndex == 3 && ENNetworkQ != null)
            {
                LasPoint.ClassificationType[,] output = null;
                DisableChangeButtons();
                await Task.Run(() =>
                {
                    output = ENNetworkQ.Classify(Controller.File);
                });
                //OpenGlController.isClassified = false;
                Controller.subsetClass = output;
                Controller.LoadColorsByClassificationSubgroups();
                EnableChangeButtons();
                KoniecKlasyfikacjiMessageBox();
            }
            else if (KlasyfikatorComboBox.SelectedIndex == 4 && KNNClassifierSlow != null)
            {
                LasPoint.ClassificationType[] output = null;
                DisableChangeButtons();
                await Task.Run(() =>
                {
                    output = KNNClassifierSlow.Classify(Controller.File);
                });
                Controller.LoadColorsByKnnClassification(output);
                EnableChangeButtons();
                KoniecKlasyfikacjiMessageBox();
            }
            
            else if(KlasyfikatorComboBox.SelectedIndex == 5 && ENNetworkS != null)
            {
                int count = 0;
                if(!int.TryParse(pointsToClassify.Text, out count))
                {
                    if (pointsToClassify.Text.Equals(string.Empty))
                        count = 0;
                    else
                        return;
                }
                LasPoint.ClassificationType[] output = null;
                DisableChangeButtons();
                await Task.Run(() =>
                {
                    output = ENNetworkS.Classify(Controller.File, count);
                });
                Controller.LoadColorsFromClassArray(output);
                EnableChangeButtons();
                KoniecKlasyfikacjiMessageBox();
            }
            else if (KlasyfikatorComboBox.SelectedIndex == 2 && KNNClassifierQuick == null)
            {
                MessageBox.Show("Najpierw wykonaj uczenie klasyfikatora!", "Informacja!",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }
            RefreshView();
            GlControl1_Paint(null, null);
        }

        private void DisableChangeButtons()
        {
            trainButton.Enabled = false;
            classifyButton.Enabled = false;
            viewButton.Enabled = false;
            ColorModeBox.Enabled = false;
            ProjectionBox.Enabled = false;
            KlasyfikatorComboBox.Enabled = false;
            serializeNetwork.Enabled = false;
            deserializeNetwork.Enabled = false;
        }

        private void EnableChangeButtons()
        {
            trainButton.Enabled = true;
            classifyButton.Enabled = true;
            viewButton.Enabled = true;
            ColorModeBox.Enabled = true;
            ProjectionBox.Enabled = true;
            KlasyfikatorComboBox.Enabled = true;

            KlasyfikatorComboBox_SelectedIndexChanged(null, null);
        }

        private void serializeNetwork_Click(object sender, EventArgs e)
        {
            switch (KlasyfikatorComboBox.SelectedIndex)
            {
                case 1:
                    if (ENNetwork != null)
                        ENNetwork.Serialize();
                    break;
                case 3:
                    if (ENNetworkQ != null)
                        ENNetworkQ.Serialize();
                    break;
                case 4:
                    if (ENNetworkS != null)
                        ENNetworkS.Serialize();
                    break;
            }
        }

        private void deserializeNetwork_Click(object sender, EventArgs e)
        {
            switch (KlasyfikatorComboBox.SelectedIndex)
            {
                case 1:
                    ENNetwork = EncogNeuralNetwork.DeSerialize();
                    break;
                case 3:
                    ENNetworkQ = EncogNeuralNetworkQuick.DeSerialize(OpenGlController.divisionCountX, OpenGlController.divisionCountY);
                    break;
                case 4:
                    ENNetworkS = EncogNeuralNetworkSlow.DeSerialize();
                    break;
            }
        }

        private void KlasyfikatorComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            Settings.Default.KlasyfikatorBoxIndex = KlasyfikatorComboBox.SelectedIndex;
            switch (KlasyfikatorComboBox.SelectedIndex)
            {
                case 1:
                case 3:
                case 4:
                    serializeNetwork.Enabled = true;
                    deserializeNetwork.Enabled = true;
                    break;
                default:
                    serializeNetwork.Enabled = false;
                    deserializeNetwork.Enabled = false;
                    break;
            }
            if(KlasyfikatorComboBox.SelectedIndex == 4)
            {
                pointsToClassify.Enabled = true;
                pointsToClassifyLabel.Enabled = true;
            }
            else
            {
                pointsToClassify.Enabled = false;
                pointsToClassifyLabel.Enabled = false;
            }
        }
    }
}