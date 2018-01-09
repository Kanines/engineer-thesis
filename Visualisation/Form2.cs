using System.Text;
using System.Windows.Forms;
using LASLibrary;

namespace Visualisation
{
    public partial class Form2 : Form
    {
        public Form2(LasHeader lasHeader)
        {
            InitializeComponent();
            Text = "InfoBox - " + lasHeader.LasFile.Filename;
            StringBuilder strBuilder = new StringBuilder();
            strBuilder.AppendLine(string.Format("Version LAS File: {0}.{1}", lasHeader.VersionMajor,
                lasHeader.VersionMinor));
            strBuilder.AppendLine("System Identifier: " + lasHeader.SystemIdentifier);
            strBuilder.AppendLine("Generating Software: " + lasHeader.GeneratingSoftware);
            strBuilder.AppendLine(string.Format("Number of point record : {0}", lasHeader.NumberOfPointRecords));
            strBuilder.AppendLine(string.Format("Point record format : Format {0}", (int) lasHeader.PointDataFormatId));
            strBuilder.AppendLine(string.Format("Point record length : {0}", lasHeader.PointDataRecordLength));
            strBuilder.AppendLine("Number of points by return: ");
            for (int i = 0; i < lasHeader.NumberOfPointsByReturn.Length; i++)
            {
                strBuilder.AppendLine(string.Format("\tReturn {0} - {1}", i, lasHeader.NumberOfPointsByReturn[i]));
            }
            strBuilder.AppendLine(string.Format("MinX, MaxX - {0}, {1}", lasHeader.MinX, lasHeader.MaxX));
            strBuilder.AppendLine(string.Format("MinY, MaxY - {0}, {1}", lasHeader.MinY, lasHeader.MaxY));
            strBuilder.AppendLine(string.Format("MinZ, MaxZ - {0}, {1}", lasHeader.MinZ, lasHeader.MaxZ));
            strBuilder.AppendLine(string.Format("Scale Factor X - {0}", lasHeader.ScaleFactorX));
            strBuilder.AppendLine(string.Format("Scale Factor Y - {0}", lasHeader.ScaleFactorY));
            strBuilder.AppendLine(string.Format("Scale Factor Z - {0}", lasHeader.ScaleFactorZ));
            strBuilder.AppendLine(string.Format("Offset X - {0}", lasHeader.OffsetX));
            strBuilder.AppendLine(string.Format("Offset Y - {0}", lasHeader.OffsetY));
            strBuilder.AppendLine(string.Format("Offset Z - {0}", lasHeader.OffsetZ));

            infoTextBox.Text += strBuilder;
        }

        public sealed override string Text
        {
            get { return base.Text; }
            set { base.Text = value; }
        }
    }
}