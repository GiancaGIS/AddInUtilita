using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Windows.Forms;


namespace AddInUtilita
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class DockableWinStatistics : UserControl
    {
        public DockableWinStatistics(object hook)
        {
            InitializeComponent();
            this.Hook = hook;
        }

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }

        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private DockableWinStatistics m_windowUI;

            public AddinImpl()
            {
            }

            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new DockableWinStatistics(this.Hook);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        private void btnPopulateLayerList_Click(object sender, EventArgs e)
        {
            IDocument processoArcMap = ArcMap.Application.Document;
            IMxDocument mxdoc = (IMxDocument)processoArcMap;
            IMap map = mxdoc.FocusMap;

            // Per prima cosa svuoto la lista dei layer prima di ripopolarla!
            lstLayer.Items.Clear();
            comboBoxAttrNumerici.Tag = null;

            // Vado alla ricerca dei layer nel dataframe attivo...
            IEnumLayer enumTuttiLayer = map.Layers;
            ILayer layer = enumTuttiLayer.Next();

            while (layer != null)
            {
                if (layer is IFeatureLayer2)
                {
                    if (layer.Visible)
                    {
                        lstLayer.Items.Add(layer.Name);
                    }
                }
                layer = enumTuttiLayer.Next();
            }
        }

        private void lstLayer_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                comboBoxAttrNumerici.Items.Clear(); // Svuoto combobox attributi numerici!
                string strLayerSelezionato = String.Empty;

                if (lstLayer.Items.Count > 0)
                {
                    strLayerSelezionato = lstLayer.SelectedItem.ToString();
                }

                if (!(strLayerSelezionato == String.Empty))
                {
                    // A questo punto ricavo l'oggetto layer avente il nome pari all'elemento selezionato nella lista...
                    IDocument processoArcMap = ArcMap.Application.Document;
                    IMxDocument mxdoc = (IMxDocument)processoArcMap;
                    IMap map = mxdoc.FocusMap;

                    // Creo oggetto enum con tutti i layer presenti nel dataframe attivo
                    IEnumLayer enumTuttiLayer = map.Layers;
                    ILayer layer = enumTuttiLayer.Next();
                    IFeatureLayer2 featureLayer = null;

                    while (layer != null)
                    {
                        if (layer.Name.ToLower() == strLayerSelezionato.ToLower())
                        {
                            featureLayer = (IFeatureLayer2)layer; // Salvo come oggetto feature layer quel layer avente lo stesso nome di quello scelto nella lista
                        }
                        layer = enumTuttiLayer.Next();
                    }

                    // A questo punto vado alla ricerca degli attributi numerici della feature class associata al layer scelto...
                    IFeatureClass FCLayerScelto = featureLayer.FeatureClass;

                    for (uint i = 0; i < FCLayerScelto.Fields.FieldCount; i++)
                    {
                        // Accedo all'oggetto IField
                        IField attributo = FCLayerScelto.Fields.Field[ (int)i ];
                        if (attributo.Type != esriFieldType.esriFieldTypeOID &&
                            attributo.Type != esriFieldType.esriFieldTypeGlobalID &&
                            attributo.Type != esriFieldType.esriFieldTypeGUID &&
                            attributo.Type == esriFieldType.esriFieldTypeDouble ||
                            attributo.Type == esriFieldType.esriFieldTypeInteger ||
                            attributo.Type == esriFieldType.esriFieldTypeSmallInteger ||
                            attributo.Type == esriFieldType.esriFieldTypeSingle)
                        {
                            comboBoxAttrNumerici.Items.Add(attributo.Name);
                        }

                        comboBoxAttrNumerici.Tag = (featureLayer as ILayer2).Name;
                    }
                }
            }

            catch (Exception ex)
            {
                if (ex.Message.ToLower() == "object reference not set to an instance of an object.")
                {
                    System.Windows.Forms.MessageBox.Show("Non hai selezionato alcun Feature Layer della lista!", "ATTENZIONE", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                else
                {
                    System.Windows.Forms.MessageBox.Show(ex.Message, "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

        }

        private void btnCalculate_Click(object sender, EventArgs e)
        {
            string strAttributoScelto = String.Empty;

            if (comboBoxAttrNumerici.SelectedIndex > 0)
            {
                strAttributoScelto = comboBoxAttrNumerici.SelectedItem.ToString();
            }

            if (!(strAttributoScelto == String.Empty))
            {
                if (comboBoxAttrNumerici.Tag == null)
                {
                    lblReport.Text = String.Empty;
                    return;
                }

                string featureLayerName = comboBoxAttrNumerici.Tag.ToString();

                IDocument processoArcMap = ArcMap.Application.Document;
                IMxDocument mxdoc = (IMxDocument)processoArcMap;
                IMap map = mxdoc.FocusMap;

                // Creo oggetto enum con tutti i layer presenti nel dataframe attivo
                IEnumLayer enumTuttiLayer = map.Layers;
                ILayer layer = enumTuttiLayer.Next();
                IFeatureLayer2 featureLayer = null;

                while (layer != null)
                {
                    if (layer.Name == featureLayerName)
                    {
                        featureLayer = layer as IFeatureLayer2;
                    }

                    layer = enumTuttiLayer.Next();
                }

                if (featureLayer == null)
                {
                    return;
                }
                else
                {
                    IFeatureClass FC = featureLayer.FeatureClass;
                    // Creo oggetto cursore di sola lettura sulla feature class dietro al layer selezionato
                    IFeatureCursor featureCursor = FC.Search(null, true); // Uso il parametro true per il riciclo della memoria, essendo il cursore in sola lettura

                    // Procedo col calcolo delle statistiche
                    IDataStatistics statisticheDati = new DataStatistics();

                    statisticheDati.Field = strAttributoScelto; // Scelgo l'attributo sulla quale eseguire la statistica
                    statisticheDati.Cursor = (ICursor)featureCursor; // Vuole l'oggetto ICursor, pertanto casto il mio feature cursor

                    IStatisticsResults SR = statisticheDati.Statistics; // accedo agli oggetti risultati

                    lblReport.Text = string.Format("Numero geometrie: {0}\n", SR.Count);
                    lblReport.Text += string.Format("Valore minimo: {0:#.00}\n", SR.Minimum);
                    lblReport.Text += string.Format("Valore massimo: {0:#.00}\n", SR.Maximum);
                    lblReport.Text += string.Format("Somma cumulata: {0:#.00}\n", SR.Sum);
                    lblReport.Text += string.Format("Valore medio: {0:#.00}\n", SR.Mean);
                    lblReport.Text += string.Format("Deviazione standard: {0:#.00}\n", SR.StandardDeviation);
                }
            }
        }
    }
}
