using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Display;


namespace AddInUtilita
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class DockableFeatureDuplicate : UserControl
    {
        static IStepProgressor pStepProgressor = null; // Ocio che la rendo static!!

        public DockableFeatureDuplicate(object hook)
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
            private DockableFeatureDuplicate m_windowUI;

            public AddinImpl()
            {
            }

            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new DockableFeatureDuplicate(this.Hook);
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

            // Vado alla ricerca dei layer nel dataframe attivo...
            IEnumLayer enumTuttiLayer = map.Layers;
            ILayer layer = enumTuttiLayer.Next();

            while (layer != null)
            {
                if (layer is IFeatureLayer2)
                {
                    if (layer.Visible &&
                        (
                        (((IFeatureLayer2)layer).FeatureClass).ShapeType == esriGeometryType.esriGeometryPolygon
                         || (((IFeatureLayer2)layer).FeatureClass).ShapeType == esriGeometryType.esriGeometryPoint
                         || (((IFeatureLayer2)layer).FeatureClass).ShapeType == esriGeometryType.esriGeometryPolyline
                         )
                        )
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
            int numeroRecord = -9999;
            IProgressDialog2 miaProgress = MostraProgressIndicator("Avanzamento", "Ricerca delle geometrie doppione...", numeroRecord, 1, false, true);

            try
            {
                string nomeFeatureLayerSelez = lstLayer.SelectedItem.ToString();

                // Scansiono ArcMap alla ricerca dell'oggetto selezionato...
                IMxDocument mxdDoc = ArcMap.Application.Document as IMxDocument;
                IMap map = mxdDoc.FocusMap;
                IEnumLayer enumLayers = map.Layers;
                ILayer2 layer = enumLayers.Next() as ILayer2;
                IFeatureLayer2 fLayer = null;
                IFeatureClass fcClass = null;

                while (layer != null)
                {
                    if (layer.Name.ToLower() == nomeFeatureLayerSelez.ToLower() && layer is IFeatureLayer2)
                    {
                        fLayer = layer as IFeatureLayer2;
                        fcClass = fLayer.FeatureClass;
                        numeroRecord = fcClass.FeatureCount(null);
                    }

                    layer = enumLayers.Next() as ILayer2;
                }

                if (fLayer == null)
                {
                    return; 
                }

                // Ricavo lo shape type del feature layer selezionato...
                if (fLayer.ShapeType == esriGeometryType.esriGeometryPolygon)
                {
                    List<string> ListaOutput = new List<string>();
                    IPolygon4 pol1 = null;
                    IPolygon4 pol2 = null;

                    ListaOutput = RicercaDoppioniPoligonali(fcClass, miaProgress, pol1, pol2);

                    if (ListaOutput[1].ToUpper() == "TRUE")
                    {
                        lblReport.Text = ListaOutput[0];
                    }
                    else
                    {
                        lblReport.Text = "Nessun doppione trovato!";
                    }

                    miaProgress.HideDialog();
                }

                else if (fLayer.ShapeType == esriGeometryType.esriGeometryPoint)
                {
                    List<string> ListaOutput = new List<string>();
                    IPoint pol1 = null;
                    IPoint pol2 = null;

                    if (checkBoxSelezione.CheckState == CheckState.Checked)
                    {
                        //ListaOutput = RicercaDoppioniPuntuali(fcLayer, miaProgress, pol1, pol2, true);
                    }
                    else
                    {
                        ListaOutput = RicercaDoppioniPuntuali(fcClass, miaProgress, pol1, pol2);
                    }

                    if (ListaOutput[1].ToUpper() == "TRUE")
                    {
                        lblReport.Text = ListaOutput[0];
                    }
                    else
                    {
                        lblReport.Text = "Nessun doppione trovato!";
                    }

                    miaProgress.HideDialog();
                }

                else if (fLayer.ShapeType == esriGeometryType.esriGeometryPolyline)
                {
                    List<string> ListaOutput = new List<string>();
                    IPolyline6 pol1 = null;
                    IPolyline6 pol2 = null;

                    if (checkBoxSelezione.CheckState == CheckState.Checked)
                    {
                        ListaOutput = RicercaDoppioniPolilinee(fLayer, miaProgress, pol1, pol2, true);
                    }

                    else
                    {
                        ListaOutput = RicercaDoppioniPolilinee(fcClass, miaProgress, pol1, pol2);
                    }

                    if (ListaOutput[1].ToUpper() == "TRUE")
                    {
                        lblReport.Text = ListaOutput[0];
                    }
                    else
                    {
                        lblReport.Text = "Nessun doppione trovato!";
                    }

                    miaProgress.HideDialog();
                }
            }

            catch (Exception errore)
            {
                MessageBox.Show(errore.Message, "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Error);
                miaProgress.HideDialog();
            }
        }

        static List<string> RicercaDoppioniPoligonali(IFeatureClass fcLayer, IProgressDialog2 miaProgress, IPolygon4 poligono1=null, IPolygon4 poligono2=null)
        {
            bool blnDoppioneTrovato = false;
            string striMessaggioAvviso = String.Empty;
            List<string> listaOutput = new List<string>();

            int OIDPoligono1 = -9999;
            int OIDPoligono2 = -9999;

            // Creo l'oggetto relazione geometrica...
            IRelationalOperator2 relazioneOperatore;

            IFeatureCursor cursoreLettura = fcLayer.Search(null, false);

            IFeature fPassoCursore = cursoreLettura.NextFeature();

            while (fPassoCursore != null)
            {
                OIDPoligono1 = fPassoCursore.OID;
                poligono1 = fPassoCursore.ShapeCopy as IPolygon4;

                IFeatureCursor cursoreLettura2 = fcLayer.Search(null, false);
                IFeature fPassoCursore2 = cursoreLettura.NextFeature();

                while (fPassoCursore2 != null)
                {
                    OIDPoligono2 = fPassoCursore2.OID;
                    poligono2 = fPassoCursore2.ShapeCopy as IPolygon4;

                    relazioneOperatore = poligono1 as IRelationalOperator2;

                    if (relazioneOperatore.Equals(poligono2) == true && OIDPoligono1 != OIDPoligono2)
                    {
                        blnDoppioneTrovato = true;

                        if (!(striMessaggioAvviso.Contains(String.Format("\nObjectID {0} e {1}", OIDPoligono1, OIDPoligono2))))
                        {
                            striMessaggioAvviso += String.Format("\nObjectID {0} e {1}", OIDPoligono1, OIDPoligono2);
                        }

                    }

                    fPassoCursore2 = cursoreLettura2.NextFeature();
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(cursoreLettura2);

                fPassoCursore = cursoreLettura.NextFeature();
                pStepProgressor.Step();
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(cursoreLettura);

            if (blnDoppioneTrovato)
            {
                listaOutput.Add(striMessaggioAvviso);
                listaOutput.Add(blnDoppioneTrovato.ToString());
            }

            else
            {
                listaOutput.Add(String.Empty);
                listaOutput.Add(blnDoppioneTrovato.ToString());
            }

            return listaOutput;
        }

        static List<string> RicercaDoppioniPuntuali(IFeatureClass fcClass, IProgressDialog2 miaProgress, IPoint punto1 = null, IPoint punto2 = null)
        {
            bool blnDoppioneTrovato = false;
            string striMessaggioAvviso = String.Empty;
            List<string> listaOutput = new List<string>();

            int OIDPunto1 = -9999;
            int OIDPunto2 = -9999;

            // Creo l'oggetto relazione geometrica...
            IRelationalOperator2 relazioneOperatore;

            IFeatureCursor cursoreLettura = fcClass.Search(null, false);

            IFeature fPassoCursore = cursoreLettura.NextFeature();

            while (fPassoCursore != null)
            {
                OIDPunto1 = fPassoCursore.OID;
                punto1 = fPassoCursore.ShapeCopy as IPoint;

                IFeatureCursor cursoreLettura2 = fcClass.Search(null, false);
                IFeature fPassoCursore2 = cursoreLettura.NextFeature();

                while (fPassoCursore2 != null)
                {
                    OIDPunto2 = fPassoCursore2.OID;
                    punto2 = fPassoCursore2.ShapeCopy as IPoint;

                    relazioneOperatore = punto1 as IRelationalOperator2;

                    if (relazioneOperatore.Equals(punto2) == true && OIDPunto1 != OIDPunto2)
                    {
                        blnDoppioneTrovato = true;

                        if (!(striMessaggioAvviso.Contains(String.Format("\nObjectID {0} e {1}", OIDPunto1, OIDPunto2))))
                        {
                            striMessaggioAvviso += String.Format("\nObjectID {0} e {1}", OIDPunto1, OIDPunto2);
                        }

                    }

                    fPassoCursore2 = cursoreLettura2.NextFeature();
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(cursoreLettura2);

                fPassoCursore = cursoreLettura.NextFeature();
                pStepProgressor.Step();
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(cursoreLettura);

            if (blnDoppioneTrovato)
            {
                listaOutput.Add(striMessaggioAvviso);
                listaOutput.Add(blnDoppioneTrovato.ToString());
            }

            else
            {
                listaOutput.Add(String.Empty);
                listaOutput.Add(blnDoppioneTrovato.ToString());
            }

            return listaOutput;
        }

        static List<string> RicercaDoppioniPolilinee(IFeatureClass fcClass, IProgressDialog2 miaProgress, IPolyline6 polilinea1 = null, IPolyline6 polilinea2 = null)
        {
            bool blnDoppioneTrovato = false;
            string striMessaggioAvviso = String.Empty;
            List<string> listaOutput = new List<string>();

            int OIDPunto1 = -9999;
            int OIDPunto2 = -9999;

            // Creo l'oggetto relazione geometrica...
            IRelationalOperator2 relazioneOperatore;

            IFeatureCursor cursoreLettura = fcClass.Search(null, false);

            IFeature fPassoCursore = cursoreLettura.NextFeature();

            while (fPassoCursore != null)
            {
                OIDPunto1 = fPassoCursore.OID;
                polilinea1 = fPassoCursore.ShapeCopy as IPolyline6;

                IFeatureCursor cursoreLettura2 = fcClass.Search(null, false);
                IFeature fPassoCursore2 = cursoreLettura.NextFeature();

                while (fPassoCursore2 != null)
                {
                    OIDPunto2 = fPassoCursore2.OID;
                    polilinea2 = fPassoCursore2.ShapeCopy as IPolyline6;

                    relazioneOperatore = polilinea1 as IRelationalOperator2;

                    if (relazioneOperatore.Equals(polilinea2) == true && OIDPunto1 != OIDPunto2)
                    {
                        blnDoppioneTrovato = true;

                        if (!(striMessaggioAvviso.Contains(String.Format("\nObjectID {0} e {1}", OIDPunto1, OIDPunto2))))
                        {
                            striMessaggioAvviso += String.Format("\nObjectID {0} e {1}", OIDPunto1, OIDPunto2);
                        }

                    }

                    fPassoCursore2 = cursoreLettura2.NextFeature();
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(cursoreLettura2);

                fPassoCursore = cursoreLettura.NextFeature();
                pStepProgressor.Step();
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(cursoreLettura);

            if (blnDoppioneTrovato)
            {
                listaOutput.Add(striMessaggioAvviso);
                listaOutput.Add(blnDoppioneTrovato.ToString());
            }

            else
            {
                listaOutput.Add(String.Empty);
                listaOutput.Add(blnDoppioneTrovato.ToString());
            }

            return listaOutput;
        }

        static List<string> RicercaDoppioniPolilinee(IFeatureLayer2 fLayer, IProgressDialog2 miaProgress, IPolyline6 polilinea1 = null, IPolyline6 polilinea2 = null, bool blnSoloSelezionati = true)
        {
            bool blnDoppioneTrovato = false;
            string striMessaggioAvviso = String.Empty;
            List<string> listaOutput = new List<string>();

            int OIDPunto1 = -9999;
            int OIDPunto2 = -9999;

            IFeatureClass fClass = fLayer.FeatureClass;

            // Creo l'oggetto relazione geometrica...
            IRelationalOperator2 relazioneOperatore;
            IFeatureSelection fSelezione = fLayer as IFeatureSelection;
            ISelectionSet2 selectionSet = fSelezione.SelectionSet as ISelectionSet2;

            ICursor cursore = null;

            selectionSet.Search(null, false, out cursore);

            IFeatureCursor cursoreLettura = cursore as IFeatureCursor;

            IFeature fPassoCursore = cursoreLettura.NextFeature();

            while (fPassoCursore != null)
            {
                OIDPunto1 = fPassoCursore.OID;

                polilinea1 = fPassoCursore.ShapeCopy as IPolyline6;

                IFeatureCursor cursoreLettura2 = fClass.Search(null, false);
                IFeature fPassoCursore2 = cursoreLettura.NextFeature();

                while (fPassoCursore2 != null)
                {
                    OIDPunto2 = fPassoCursore2.OID;
                    polilinea2 = fPassoCursore2.ShapeCopy as IPolyline6;

                    relazioneOperatore = polilinea1 as IRelationalOperator2;

                    if (relazioneOperatore.Equals(polilinea2) == true && OIDPunto1 != OIDPunto2)
                    {
                        blnDoppioneTrovato = true;

                        if (!(striMessaggioAvviso.Contains(String.Format("\nObjectID {0} e {1}", OIDPunto1, OIDPunto2))))
                        {
                            striMessaggioAvviso += String.Format("\nObjectID {0} e {1}", OIDPunto1, OIDPunto2);
                        }

                    }

                    fPassoCursore2 = cursoreLettura2.NextFeature();
                }

                System.Runtime.InteropServices.Marshal.ReleaseComObject(cursoreLettura2);

                fPassoCursore = cursoreLettura.NextFeature();
                pStepProgressor.Step();
            }

            System.Runtime.InteropServices.Marshal.ReleaseComObject(cursoreLettura);

            if (blnDoppioneTrovato)
            {
                listaOutput.Add(striMessaggioAvviso);
                listaOutput.Add(blnDoppioneTrovato.ToString());
            }

            else
            {
                listaOutput.Add(String.Empty);
                listaOutput.Add(blnDoppioneTrovato.ToString());
            }

            return listaOutput;
        }

        /// <summary>
        /// Funzione che genera una prograss bar già compilata!
        /// </summary>
        /// <param name="strTitle">Titolo nella barra in alto</param>
        /// <param name="strDescrizione">Descrizione</param>
        /// <param name="iMax">Max valore di avanzamento barra</param>
        /// <param name="iStepValue">Passo di avanzamento</param>
        /// <param name="blnPermettiCancel">Passare true per permettere la cancellazione</param>
        /// <returns></returns>
        private IProgressDialog2 MostraProgressIndicator(string strTitle, string strDescrizione, int iMax, int iStepValue, bool blnPermettiCancel, bool mostraAvanzamento)
        {
            IProgressDialogFactory pProgressDlgFact = null;
            IProgressDialog2 pProgressDialog = null;
            ITrackCancel pTrackCancel = null;

            // Mostro la progress dialog
            pTrackCancel = new CancelTracker();
            pProgressDlgFact = new ProgressDialogFactoryClass();
            pProgressDialog = (IProgressDialog2)pProgressDlgFact.Create(pTrackCancel, 0);
            pProgressDialog.CancelEnabled = blnPermettiCancel;
            pProgressDialog.Title = strTitle;
            pProgressDialog.Description = strDescrizione;
            pProgressDialog.Animation = esriProgressAnimationTypes.esriProgressSpiral;

            if (mostraAvanzamento)
            {
                // Imposto le proprietà del Step Progressor
                pStepProgressor = (IStepProgressor)pProgressDialog;
                pStepProgressor.MinRange = 0;
                pStepProgressor.MaxRange = iMax;
                pStepProgressor.StepValue = iStepValue;
            }

            return pProgressDialog;
        }

        private void vScrollBar1_Scroll(object sender, ScrollEventArgs e)
        {

        }
    }
}
