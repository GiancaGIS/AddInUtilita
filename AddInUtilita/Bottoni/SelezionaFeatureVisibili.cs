using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Framework;

namespace AddInUtilita
{
    public class SelezionaFeatureVisibili : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public SelezionaFeatureVisibili()
        {
        }

        protected override void OnClick()
        {
            IDocument processoArcMap = ArcMap.Application.Document;
            IMxDocument mxdDoc = processoArcMap as IMxDocument;
            IMap map = mxdDoc.FocusMap;
            IActiveView activeView = map as IActiveView;

            if (map.LayerCount == 0) // Conto il numero di layer presenti
            {
                System.Windows.Forms.MessageBox.Show("Non ci sono layer nel Dataframe attivo!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else
            {
                // Creo oggetto enum con tutti i layer presenti nel dataframe attivo
                IEnumLayer enumTuttiLayer = map.Layers;
                ILayer layer = enumTuttiLayer.Next();

                ISpatialFilter spatialFilter = new SpatialFilter();
                spatialFilter.Geometry = activeView.Extent;
                spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;

                // Ciclo ora su tutti i layer nel dataframe attivo e vado a selezionare tutti gli oggetti che ricadono nell'extent del Dataframe attivo
                while (layer != null)
                {
                    if (layer is IFeatureLayer2)
                    {
                        if (layer.Visible)
                        {
                            IFeatureLayer2 featureLayer = (IFeatureLayer2)layer; // Casto l'oggetto Layer in Feature Layer

                            // Creo l'oggetto selezione
                            IFeatureSelection featureSelection = (IFeatureSelection)featureLayer; // Casto l'oggetto Feature Layer in Feature Selection
                            featureSelection.SelectFeatures(spatialFilter, esriSelectionResultEnum.esriSelectionResultNew, false);
                        }
                    }

                    layer = enumTuttiLayer.Next();
                }

                mxdDoc.ActiveView.Refresh();
            }
        }
    }
}