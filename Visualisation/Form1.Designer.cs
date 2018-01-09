using System.ComponentModel;
using System.Windows.Forms;
using OpenTK;

namespace Visualisation
{
    partial class Form1
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.GlControl1 = new OpenTK.GLControl();
            this.NumberOfPointsLabel = new System.Windows.Forms.Label();
            this.FormatLabel = new System.Windows.Forms.Label();
            this.ColorModeBox = new System.Windows.Forms.ComboBox();
            this.ColorsDescriptionLabel = new System.Windows.Forms.Label();
            this.ProjectionLabel = new System.Windows.Forms.Label();
            this.ProjectionBox = new System.Windows.Forms.ComboBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.plikToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.wczytajToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.eksportObrazuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.właściwościToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.informacjeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.sterowanieToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.KlasyfikatorGroupBox = new System.Windows.Forms.GroupBox();
            this.pointsToClassifyLabel = new System.Windows.Forms.Label();
            this.pointsToClassify = new System.Windows.Forms.TextBox();
            this.deserializeNetwork = new System.Windows.Forms.Button();
            this.serializeNetwork = new System.Windows.Forms.Button();
            this.KlasyfikatorComboBox = new System.Windows.Forms.ComboBox();
            this.label11 = new System.Windows.Forms.Label();
            this.trainButton = new System.Windows.Forms.Button();
            this.classifyButton = new System.Windows.Forms.Button();
            this.viewButton = new System.Windows.Forms.Button();
            this.centerButton = new System.Windows.Forms.Button();
            this.pointsFormatLabel = new System.Windows.Forms.Label();
            this.LegendBox = new System.Windows.Forms.GroupBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox8 = new System.Windows.Forms.PictureBox();
            this.pictureBox7 = new System.Windows.Forms.PictureBox();
            this.pictureBox6 = new System.Windows.Forms.PictureBox();
            this.pictureBox5 = new System.Windows.Forms.PictureBox();
            this.pictureBox4 = new System.Windows.Forms.PictureBox();
            this.pictureBox3 = new System.Windows.Forms.PictureBox();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.pictureBox10 = new System.Windows.Forms.PictureBox();
            this.pictureBox9 = new System.Windows.Forms.PictureBox();
            this.openGlControllerBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.menuStrip1.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.KlasyfikatorGroupBox.SuspendLayout();
            this.LegendBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.openGlControllerBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // GlControl1
            // 
            this.GlControl1.BackColor = System.Drawing.Color.Black;
            this.GlControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.GlControl1.Location = new System.Drawing.Point(0, 24);
            this.GlControl1.Name = "GlControl1";
            this.GlControl1.Size = new System.Drawing.Size(1276, 627);
            this.GlControl1.TabIndex = 0;
            this.GlControl1.VSync = false;
            this.GlControl1.Paint += new System.Windows.Forms.PaintEventHandler(this.GlControl1_Paint);
            this.GlControl1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.GlControl1_MouseDown);
            this.GlControl1.MouseMove += new System.Windows.Forms.MouseEventHandler(this.GlControl1_MouseMove);
            this.GlControl1.MouseUp += new System.Windows.Forms.MouseEventHandler(this.GlControl1_MouseUp);
            // 
            // NumberOfPointsLabel
            // 
            this.NumberOfPointsLabel.AutoSize = true;
            this.NumberOfPointsLabel.Location = new System.Drawing.Point(6, 16);
            this.NumberOfPointsLabel.Name = "NumberOfPointsLabel";
            this.NumberOfPointsLabel.Size = new System.Drawing.Size(57, 13);
            this.NumberOfPointsLabel.TabIndex = 1;
            this.NumberOfPointsLabel.Text = "0 punktów";
            // 
            // FormatLabel
            // 
            this.FormatLabel.AutoSize = true;
            this.FormatLabel.Location = new System.Drawing.Point(6, 29);
            this.FormatLabel.Name = "FormatLabel";
            this.FormatLabel.Size = new System.Drawing.Size(86, 13);
            this.FormatLabel.TabIndex = 2;
            this.FormatLabel.Text = "Format punktów:";
            // 
            // ColorModeBox
            // 
            this.ColorModeBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ColorModeBox.FormattingEnabled = true;
            this.ColorModeBox.Items.AddRange(new object[] {
            "wysokość",
            "prawdziwe kolory",
            "klasa",
            "intensywność",
            "własna klasyfikacja"});
            this.ColorModeBox.Location = new System.Drawing.Point(9, 58);
            this.ColorModeBox.Name = "ColorModeBox";
            this.ColorModeBox.Size = new System.Drawing.Size(121, 21);
            this.ColorModeBox.TabIndex = 3;
            this.ColorModeBox.SelectedIndexChanged += new System.EventHandler(this.ColorModeBox_SelectedIndexChanged);
            // 
            // ColorsDescriptionLabel
            // 
            this.ColorsDescriptionLabel.AutoSize = true;
            this.ColorsDescriptionLabel.Location = new System.Drawing.Point(6, 42);
            this.ColorsDescriptionLabel.Name = "ColorsDescriptionLabel";
            this.ColorsDescriptionLabel.Size = new System.Drawing.Size(83, 13);
            this.ColorsDescriptionLabel.TabIndex = 4;
            this.ColorsDescriptionLabel.Text = "Rodzaj kolorów:";
            // 
            // ProjectionLabel
            // 
            this.ProjectionLabel.AutoSize = true;
            this.ProjectionLabel.Location = new System.Drawing.Point(6, 82);
            this.ProjectionLabel.Name = "ProjectionLabel";
            this.ProjectionLabel.Size = new System.Drawing.Size(54, 13);
            this.ProjectionLabel.TabIndex = 5;
            this.ProjectionLabel.Text = "Projekcja:";
            // 
            // ProjectionBox
            // 
            this.ProjectionBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.ProjectionBox.FormattingEnabled = true;
            this.ProjectionBox.Items.AddRange(new object[] {
            "2D",
            "3D"});
            this.ProjectionBox.Location = new System.Drawing.Point(9, 98);
            this.ProjectionBox.Name = "ProjectionBox";
            this.ProjectionBox.Size = new System.Drawing.Size(121, 21);
            this.ProjectionBox.TabIndex = 6;
            this.ProjectionBox.SelectedIndexChanged += new System.EventHandler(this.ProjectionBox_SelectedIndexChanged);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.plikToolStripMenuItem,
            this.informacjeToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1276, 24);
            this.menuStrip1.TabIndex = 7;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // plikToolStripMenuItem
            // 
            this.plikToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.wczytajToolStripMenuItem,
            this.eksportObrazuToolStripMenuItem,
            this.właściwościToolStripMenuItem});
            this.plikToolStripMenuItem.Name = "plikToolStripMenuItem";
            this.plikToolStripMenuItem.Size = new System.Drawing.Size(38, 20);
            this.plikToolStripMenuItem.Text = "Plik";
            // 
            // wczytajToolStripMenuItem
            // 
            this.wczytajToolStripMenuItem.Name = "wczytajToolStripMenuItem";
            this.wczytajToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.wczytajToolStripMenuItem.Text = "Wczytaj";
            this.wczytajToolStripMenuItem.Click += new System.EventHandler(this.wczytajToolStripMenuItem_Click);
            // 
            // eksportObrazuToolStripMenuItem
            // 
            this.eksportObrazuToolStripMenuItem.Name = "eksportObrazuToolStripMenuItem";
            this.eksportObrazuToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.eksportObrazuToolStripMenuItem.Text = "Eksport obrazu";
            this.eksportObrazuToolStripMenuItem.Click += new System.EventHandler(this.eksportObrazuToolStripMenuItem_Click);
            // 
            // właściwościToolStripMenuItem
            // 
            this.właściwościToolStripMenuItem.Name = "właściwościToolStripMenuItem";
            this.właściwościToolStripMenuItem.Size = new System.Drawing.Size(152, 22);
            this.właściwościToolStripMenuItem.Text = "Właściwości";
            this.właściwościToolStripMenuItem.Click += new System.EventHandler(this.właściwościToolStripMenuItem_Click);
            // 
            // informacjeToolStripMenuItem
            // 
            this.informacjeToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.sterowanieToolStripMenuItem});
            this.informacjeToolStripMenuItem.Name = "informacjeToolStripMenuItem";
            this.informacjeToolStripMenuItem.Size = new System.Drawing.Size(76, 20);
            this.informacjeToolStripMenuItem.Text = "Informacje";
            // 
            // sterowanieToolStripMenuItem
            // 
            this.sterowanieToolStripMenuItem.Name = "sterowanieToolStripMenuItem";
            this.sterowanieToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.sterowanieToolStripMenuItem.Text = "Sterowanie";
            this.sterowanieToolStripMenuItem.Click += new System.EventHandler(this.sterowanieToolStripMenuItem_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.KlasyfikatorGroupBox);
            this.groupBox1.Controls.Add(this.viewButton);
            this.groupBox1.Controls.Add(this.centerButton);
            this.groupBox1.Controls.Add(this.pointsFormatLabel);
            this.groupBox1.Controls.Add(this.LegendBox);
            this.groupBox1.Controls.Add(this.NumberOfPointsLabel);
            this.groupBox1.Controls.Add(this.ProjectionBox);
            this.groupBox1.Controls.Add(this.FormatLabel);
            this.groupBox1.Controls.Add(this.ProjectionLabel);
            this.groupBox1.Controls.Add(this.ColorsDescriptionLabel);
            this.groupBox1.Controls.Add(this.ColorModeBox);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Right;
            this.groupBox1.Location = new System.Drawing.Point(1076, 24);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(200, 627);
            this.groupBox1.TabIndex = 8;
            this.groupBox1.TabStop = false;
            // 
            // KlasyfikatorGroupBox
            // 
            this.KlasyfikatorGroupBox.Controls.Add(this.pointsToClassifyLabel);
            this.KlasyfikatorGroupBox.Controls.Add(this.pointsToClassify);
            this.KlasyfikatorGroupBox.Controls.Add(this.deserializeNetwork);
            this.KlasyfikatorGroupBox.Controls.Add(this.serializeNetwork);
            this.KlasyfikatorGroupBox.Controls.Add(this.KlasyfikatorComboBox);
            this.KlasyfikatorGroupBox.Controls.Add(this.label11);
            this.KlasyfikatorGroupBox.Controls.Add(this.trainButton);
            this.KlasyfikatorGroupBox.Controls.Add(this.classifyButton);
            this.KlasyfikatorGroupBox.Location = new System.Drawing.Point(9, 315);
            this.KlasyfikatorGroupBox.Name = "KlasyfikatorGroupBox";
            this.KlasyfikatorGroupBox.Size = new System.Drawing.Size(185, 220);
            this.KlasyfikatorGroupBox.TabIndex = 15;
            this.KlasyfikatorGroupBox.TabStop = false;
            this.KlasyfikatorGroupBox.Text = "Klasyfikacja";
            // 
            // pointsToClassifyLabel
            // 
            this.pointsToClassifyLabel.AutoSize = true;
            this.pointsToClassifyLabel.Enabled = false;
            this.pointsToClassifyLabel.Location = new System.Drawing.Point(6, 90);
            this.pointsToClassifyLabel.Name = "pointsToClassifyLabel";
            this.pointsToClassifyLabel.Size = new System.Drawing.Size(168, 26);
            this.pointsToClassifyLabel.TabIndex = 20;
            this.pointsToClassifyLabel.Text = "Ilość punktów do sklasyfikowania,\r\n 0 - wszystkie.";
            // 
            // pointsToClassify
            // 
            this.pointsToClassify.Enabled = false;
            this.pointsToClassify.Location = new System.Drawing.Point(9, 119);
            this.pointsToClassify.Name = "pointsToClassify";
            this.pointsToClassify.Size = new System.Drawing.Size(88, 20);
            this.pointsToClassify.TabIndex = 19;
            // 
            // deserializeNetwork
            // 
            this.deserializeNetwork.Location = new System.Drawing.Point(104, 148);
            this.deserializeNetwork.Name = "deserializeNetwork";
            this.deserializeNetwork.Size = new System.Drawing.Size(75, 23);
            this.deserializeNetwork.TabIndex = 18;
            this.deserializeNetwork.Text = "Wczytaj sieć";
            this.deserializeNetwork.UseVisualStyleBackColor = true;
            this.deserializeNetwork.Click += new System.EventHandler(this.deserializeNetwork_Click);
            // 
            // serializeNetwork
            // 
            this.serializeNetwork.Location = new System.Drawing.Point(9, 148);
            this.serializeNetwork.Name = "serializeNetwork";
            this.serializeNetwork.Size = new System.Drawing.Size(75, 23);
            this.serializeNetwork.TabIndex = 17;
            this.serializeNetwork.Text = "Zapisz sieć";
            this.serializeNetwork.UseVisualStyleBackColor = true;
            this.serializeNetwork.Click += new System.EventHandler(this.serializeNetwork_Click);
            // 
            // KlasyfikatorComboBox
            // 
            this.KlasyfikatorComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.KlasyfikatorComboBox.FormattingEnabled = true;
            this.KlasyfikatorComboBox.Items.AddRange(new object[] {
            "KNN",
            "Sieć neuronowa",
            "KNN-szybkie",
            "Sieć neuronowa-szybkie",
            "KNN-wolne",
            "Sieć neuronowa-wolne"});
            this.KlasyfikatorComboBox.Location = new System.Drawing.Point(7, 37);
            this.KlasyfikatorComboBox.Name = "KlasyfikatorComboBox";
            this.KlasyfikatorComboBox.Size = new System.Drawing.Size(172, 21);
            this.KlasyfikatorComboBox.TabIndex = 16;
            this.KlasyfikatorComboBox.SelectedIndexChanged += new System.EventHandler(this.KlasyfikatorComboBox_SelectedIndexChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(6, 21);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(61, 13);
            this.label11.TabIndex = 15;
            this.label11.Text = "Klasyfikator";
            // 
            // trainButton
            // 
            this.trainButton.Location = new System.Drawing.Point(7, 64);
            this.trainButton.Name = "trainButton";
            this.trainButton.Size = new System.Drawing.Size(75, 23);
            this.trainButton.TabIndex = 11;
            this.trainButton.Text = "Trenuj";
            this.trainButton.UseVisualStyleBackColor = true;
            this.trainButton.Click += new System.EventHandler(this.TrainButtonClick);
            // 
            // classifyButton
            // 
            this.classifyButton.Location = new System.Drawing.Point(104, 64);
            this.classifyButton.Name = "classifyButton";
            this.classifyButton.Size = new System.Drawing.Size(75, 23);
            this.classifyButton.TabIndex = 12;
            this.classifyButton.Text = "Klasyfikuj";
            this.classifyButton.UseVisualStyleBackColor = true;
            this.classifyButton.Click += new System.EventHandler(this.ClassifyButtonClick);
            // 
            // viewButton
            // 
            this.viewButton.Location = new System.Drawing.Point(9, 126);
            this.viewButton.Name = "viewButton";
            this.viewButton.Size = new System.Drawing.Size(75, 23);
            this.viewButton.TabIndex = 10;
            this.viewButton.Text = "Wyświetl";
            this.viewButton.UseVisualStyleBackColor = true;
            this.viewButton.Click += new System.EventHandler(this.viewButton_Click);
            // 
            // centerButton
            // 
            this.centerButton.Location = new System.Drawing.Point(113, 125);
            this.centerButton.Name = "centerButton";
            this.centerButton.Size = new System.Drawing.Size(75, 23);
            this.centerButton.TabIndex = 9;
            this.centerButton.Text = "Wyśrodkuj";
            this.centerButton.UseVisualStyleBackColor = true;
            this.centerButton.Click += new System.EventHandler(this.centerButton_Click);
            // 
            // pointsFormatLabel
            // 
            this.pointsFormatLabel.AutoSize = true;
            this.pointsFormatLabel.Location = new System.Drawing.Point(90, 29);
            this.pointsFormatLabel.MaximumSize = new System.Drawing.Size(80, 0);
            this.pointsFormatLabel.MinimumSize = new System.Drawing.Size(20, 0);
            this.pointsFormatLabel.Name = "pointsFormatLabel";
            this.pointsFormatLabel.Size = new System.Drawing.Size(20, 13);
            this.pointsFormatLabel.TabIndex = 8;
            // 
            // LegendBox
            // 
            this.LegendBox.Controls.Add(this.label8);
            this.LegendBox.Controls.Add(this.label7);
            this.LegendBox.Controls.Add(this.label6);
            this.LegendBox.Controls.Add(this.label5);
            this.LegendBox.Controls.Add(this.label4);
            this.LegendBox.Controls.Add(this.label3);
            this.LegendBox.Controls.Add(this.label2);
            this.LegendBox.Controls.Add(this.label1);
            this.LegendBox.Controls.Add(this.pictureBox8);
            this.LegendBox.Controls.Add(this.pictureBox7);
            this.LegendBox.Controls.Add(this.pictureBox6);
            this.LegendBox.Controls.Add(this.pictureBox5);
            this.LegendBox.Controls.Add(this.pictureBox4);
            this.LegendBox.Controls.Add(this.pictureBox3);
            this.LegendBox.Controls.Add(this.pictureBox2);
            this.LegendBox.Controls.Add(this.pictureBox1);
            this.LegendBox.Location = new System.Drawing.Point(9, 155);
            this.LegendBox.Name = "LegendBox";
            this.LegendBox.Size = new System.Drawing.Size(185, 154);
            this.LegendBox.TabIndex = 7;
            this.LegendBox.TabStop = false;
            this.LegendBox.Text = "Legenda";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(24, 132);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(27, 13);
            this.label8.TabIndex = 15;
            this.label8.Text = "inne";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(23, 116);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(33, 13);
            this.label7.TabIndex = 14;
            this.label7.Text = "woda";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 100);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(31, 13);
            this.label6.TabIndex = 13;
            this.label6.Text = "szum";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(23, 84);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(48, 13);
            this.label5.TabIndex = 12;
            this.label5.Text = "budynek";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(23, 65);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 13);
            this.label4.TabIndex = 11;
            this.label4.Text = "wysoka roślinność";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 49);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(91, 13);
            this.label3.TabIndex = 10;
            this.label3.Text = "średnia roślinność";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 13);
            this.label2.TabIndex = 9;
            this.label2.Text = "niska roślinność";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.label1.Location = new System.Drawing.Point(23, 17);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(36, 13);
            this.label1.TabIndex = 8;
            this.label1.Text = "ziemia";
            // 
            // pictureBox8
            // 
            this.pictureBox8.Location = new System.Drawing.Point(7, 132);
            this.pictureBox8.Name = "pictureBox8";
            this.pictureBox8.Size = new System.Drawing.Size(10, 10);
            this.pictureBox8.TabIndex = 7;
            this.pictureBox8.TabStop = false;
            // 
            // pictureBox7
            // 
            this.pictureBox7.Location = new System.Drawing.Point(7, 116);
            this.pictureBox7.Name = "pictureBox7";
            this.pictureBox7.Size = new System.Drawing.Size(10, 10);
            this.pictureBox7.TabIndex = 6;
            this.pictureBox7.TabStop = false;
            // 
            // pictureBox6
            // 
            this.pictureBox6.Location = new System.Drawing.Point(7, 100);
            this.pictureBox6.Name = "pictureBox6";
            this.pictureBox6.Size = new System.Drawing.Size(10, 10);
            this.pictureBox6.TabIndex = 5;
            this.pictureBox6.TabStop = false;
            // 
            // pictureBox5
            // 
            this.pictureBox5.Location = new System.Drawing.Point(7, 84);
            this.pictureBox5.Name = "pictureBox5";
            this.pictureBox5.Size = new System.Drawing.Size(10, 10);
            this.pictureBox5.TabIndex = 4;
            this.pictureBox5.TabStop = false;
            // 
            // pictureBox4
            // 
            this.pictureBox4.Location = new System.Drawing.Point(7, 68);
            this.pictureBox4.Name = "pictureBox4";
            this.pictureBox4.Size = new System.Drawing.Size(10, 10);
            this.pictureBox4.TabIndex = 3;
            this.pictureBox4.TabStop = false;
            // 
            // pictureBox3
            // 
            this.pictureBox3.Location = new System.Drawing.Point(7, 52);
            this.pictureBox3.Name = "pictureBox3";
            this.pictureBox3.Size = new System.Drawing.Size(10, 10);
            this.pictureBox3.TabIndex = 2;
            this.pictureBox3.TabStop = false;
            // 
            // pictureBox2
            // 
            this.pictureBox2.Location = new System.Drawing.Point(7, 36);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(10, 10);
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
            this.pictureBox1.Location = new System.Drawing.Point(7, 20);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(10, 10);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(24, 33);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(27, 13);
            this.label10.TabIndex = 15;
            this.label10.Text = "inne obiekty";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(24, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(27, 13);
            this.label9.TabIndex = 15;
            this.label9.Text = "ziemia";
            // 
            // pictureBox10
            // 
            this.pictureBox10.Location = new System.Drawing.Point(7, 36);
            this.pictureBox10.Name = "pictureBox10";
            this.pictureBox10.Size = new System.Drawing.Size(10, 10);
            this.pictureBox10.TabIndex = 7;
            this.pictureBox10.TabStop = false;
            // 
            // pictureBox9
            // 
            this.pictureBox9.Location = new System.Drawing.Point(7, 20);
            this.pictureBox9.Name = "pictureBox9";
            this.pictureBox9.Size = new System.Drawing.Size(10, 10);
            this.pictureBox9.TabIndex = 7;
            this.pictureBox9.TabStop = false;
            // 
            // openGlControllerBindingSource
            // 
            this.openGlControllerBindingSource.DataSource = typeof(Visualisation.OpenGlController);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1276, 651);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.GlControl1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "Form1";
            this.Text = "Visualisation";
            this.Load += new System.EventHandler(this.Form1_Load_1);
            this.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.Form1_MouseWheel);
            this.Resize += new System.EventHandler(this.Form1_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.KlasyfikatorGroupBox.ResumeLayout(false);
            this.KlasyfikatorGroupBox.PerformLayout();
            this.LegendBox.ResumeLayout(false);
            this.LegendBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox8)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox7)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox6)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox5)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox4)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox3)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox10)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox9)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.openGlControllerBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private GLControl GlControl1;
        private Label NumberOfPointsLabel;
        private Label FormatLabel;
        private ComboBox ColorModeBox;
        private Label ColorsDescriptionLabel;
        private Label ProjectionLabel;
        private ComboBox ProjectionBox;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem plikToolStripMenuItem;
        private ToolStripMenuItem wczytajToolStripMenuItem;
        private ToolStripMenuItem informacjeToolStripMenuItem;
        private ToolStripMenuItem sterowanieToolStripMenuItem;
        private GroupBox groupBox1;
        private GroupBox LegendBox;
        private PictureBox pictureBox1;
        private PictureBox pictureBox8;
        private PictureBox pictureBox7;
        private PictureBox pictureBox6;
        private PictureBox pictureBox5;
        private PictureBox pictureBox4;
        private PictureBox pictureBox3;
        private PictureBox pictureBox2;
        private PictureBox pictureBox9;
        private PictureBox pictureBox10;
        private Label label2;
        private Label label1;
        private Label label8;
        private Label label7;
        private Label label6;
        private Label label5;
        private Label label4;
        private Label label3;
        private Label label9;
        private Label label10;
        private Label pointsFormatLabel;
        private Button centerButton;
        private ToolStripMenuItem właściwościToolStripMenuItem;
        private BindingSource openGlControllerBindingSource;
        private ToolStripMenuItem eksportObrazuToolStripMenuItem;
        private Button viewButton;
        private Button classifyButton;
        private Button trainButton;
        private GroupBox KlasyfikatorGroupBox;
        private ComboBox KlasyfikatorComboBox;
        private Label label11;
        private Button serializeNetwork;
        private Button deserializeNetwork;
        private Label pointsToClassifyLabel;
        private TextBox pointsToClassify;
    }
}

