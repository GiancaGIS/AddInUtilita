using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.Geodatabase;
using System;
using System.Collections.Generic;
using System.Windows.Forms;


namespace AddInUtilita
{
    public class BottoneSintesiSelezione : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public BottoneSintesiSelezione()
        {
        }

        public static IDocument processoArcMap = ArcMap.Application.Document;
        public static IMxDocument mxdDoc = processoArcMap as IMxDocument;
        public static IMap mappa = mxdDoc.FocusMap;
        public static IActiveView activeView = mappa as IActiveView;
        
        public static Dictionary<string, int> dizLayerSelezione = new Dictionary<string, int>(); // Dizionario che conterrà info sulle tal layer, e quanti oggetti selezionati ha

        protected override void OnClick()
        {
            try
            {
                if (mappa.SelectionCount == 0)
                {
                    MessageBox.Show("Non ci sono oggetti selezionati!", "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }

                else
                {
                    IFeatureSelection fSelection = null;
                    IEnumLayer enumLayer = mappa.Layers;
                    ILayer2 layer = enumLayer.Next() as ILayer2;

                    while (layer != null)
                    {
                        if (layer is IFeatureLayer)
                        {
                            IFeatureLayer2 fLayer = layer as IFeatureLayer2;
                            fSelection = fLayer as IFeatureSelection;
                            ISelectionSet2 selectioSet = fSelection.SelectionSet as ISelectionSet2;
                            int intOggettiSelezionati = selectioSet.Count;
                            dizLayerSelezione.Add(layer.Name, intOggettiSelezionati);
                        }

                        layer = enumLayer.Next() as ILayer2;
                    }

                    string messaggio = "Hai selezionato questi oggetti:";

                    foreach (KeyValuePair<string, int> kvp in dizLayerSelezione)
                    {
                        messaggio = messaggio + "\n\n" + String.Format("Layer: {0}\nOggetti selezionati: {1}", kvp.Key, kvp.Value.ToString());
                    }

                    MessageBox.Show(messaggio, "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    dizLayerSelezione.Clear();
                }

            }

            catch (Exception errore)
            {
                MessageBox.Show(errore.Message, "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnUpdate()
        {
        }
    }
}
