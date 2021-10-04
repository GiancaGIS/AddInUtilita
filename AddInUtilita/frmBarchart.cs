using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ZedGraph;

using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Desktop;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.esriSystem;

using AddInUtilita;

namespace AddInUtilita.GraficoBarre
{
    public partial class frmBarchart : Form
    {
        public frmBarchart()
        {
            InitializeComponent();
        }


        public string NomeAttributo { get; set; }
        public string NomeFeatureLayer { set; get; }
       

        private void chartFormResize()
        {
            zgc.Location = new Point(10, 10);
            zgc.Size = new Size(ClientRectangle.Width - 20, ClientRectangle.Height - 20);
        }

        private void CreateBarchart(ZedGraphControl zgc)
        {

            GraphPane pane = zgc.GraphPane;
            pane.Fill = new Fill(Color.MistyRose);
            pane.Title.Text = string.Format("Bar Chart for {0} Field of {1}", this.NomeAttributo, this.NomeFeatureLayer);
            pane.XAxis.Title.Text = "Features";
            pane.YAxis.Title.Text = "Values for " + this.NomeAttributo;
            pane.XAxis.Type = AxisType.Text;

            string[] nameOfFeatures = null;
            double[] valuesOfFeatures = null;
            FillDataArrays(out nameOfFeatures, out valuesOfFeatures);

            BarItem bar = pane.AddBar(this.NomeAttributo, null, valuesOfFeatures, Color.Red);
            pane.XAxis.Scale.TextLabels = nameOfFeatures;
            pane.XAxis.MajorGrid.IsVisible = true;
            pane.XAxis.MajorGrid.PenWidth = 2f;
            pane.YAxis.MajorGrid.IsVisible = true;

            pane.Legend.IsVisible = false;
            pane.Chart.Fill = new Fill(Color.White, Color.Blue, 45f);
            //draw the chart
            zgc.AxisChange();
        }

        private void FillDataArrays(out string[] nameOfFeatures, out double[] valuesOfFeatures)
        {
            try
            {
                IMxDocument mxdoc = ArcMap.Application.Document as IMxDocument;
                IMap map = mxdoc.FocusMap;
                IFeatureLayer fLayer = null;
                for (int j = 0; j < map.LayerCount; j++)
                {
                    if (map.Layer[j].Name == this.NomeFeatureLayer && map.Layer[j] is IFeatureLayer)
                    {
                        fLayer = map.Layer[j] as IFeatureLayer;
                        break;
                    }
                }
                if (fLayer == null)
                {
                    nameOfFeatures = null; valuesOfFeatures = null;
                    return;
                }

                IFeatureClass fClass = fLayer.FeatureClass;
                int idxNameField = fClass.Fields.FindField(fLayer.DisplayField);
                int idxValueField = fClass.Fields.FindField(this.NomeAttributo);

                int numberOfFeatures = fClass.FeatureCount(null);
                nameOfFeatures = new string[numberOfFeatures];
                valuesOfFeatures = new double[numberOfFeatures];

                IFeatureCursor fCursor = fClass.Search(null, true);
                IFeature feature = fCursor.NextFeature();
                int i = 0;

                while (feature != null)
                {
                    if (feature.Value[idxNameField] != null)
                    {
                        nameOfFeatures[i] = Convert.ToString(feature.Value[idxNameField]);
                    }

                    else
                    {
                        nameOfFeatures[i] = "";
                    }

                    if (feature.Value[idxValueField] != null)
                    {
                        valuesOfFeatures[i] = Convert.ToDouble(feature.Value[idxValueField]);
                    }

                    else
                    {
                        valuesOfFeatures[i] = 0;
                    }

                    i++;
                    feature = fCursor.NextFeature();
                }
                //releasing the Cursor object
                System.Runtime.InteropServices.Marshal.ReleaseComObject(fCursor);
            }
            catch (Exception ex)
            {
                nameOfFeatures = null; valuesOfFeatures = null;
                MessageBox.Show(ex.Message);
            }
        }

        private void frmBarchart_Load(object sender, EventArgs e)
        {
            CreateBarchart(zgc);
            chartFormResize();
        }

        private void frmBarchart_Resize(object sender, EventArgs e)
        {
            chartFormResize();
        }
    }
}
