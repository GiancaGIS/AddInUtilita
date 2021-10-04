using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;


namespace AddInUtilita
{
    public class RimuoviLayer : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        protected override void OnClick() // NOTA BENE! IL METODO ON CLICK DEVE ESSERE SEMPRE PRESENTE NEI BOTTONI DEGLI ADDIN!
        {
            try
            {
                IMxDocument mxdoc = ArcMap.Application.Document as IMxDocument; // Inizializzo l'oggetto mxd

                IMap map = mxdoc.FocusMap; // Creo la variabile Focus Map (dataframe attivo! .activeDataFrame in arcpy) che ha come metodo il rimuovi tutti i layer

                if (map.LayerCount == 0) // Conto il numero di layer presenti
                {
                    System.Windows.Forms.MessageBox.Show("Non ci sono layer nel Dataframe attivo!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                else
                {

                    map.ClearLayers(); // li rimuovo...
                    IActiveView activeView = map as IActiveView; // Creo la variabile activeView
                    activeView.Refresh();
                    mxdoc.UpdateContents();
                }
            }

            catch (Exception errore)
            {
                System.Windows.Forms.MessageBox.Show("Errore nella rimozione dei layer dal mxd!", "Errore: " + errore.Message, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }
        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }

    }

}
