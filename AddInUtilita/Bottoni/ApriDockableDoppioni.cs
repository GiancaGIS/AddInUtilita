using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;


namespace AddInUtilita
{
    public class ApriDockableDoppioni : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        #region parte commentata non pi utile
        //DockableFeatureDuplicate.AddinImpl classeDockableDoppioni =
        //   ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID<DockableFeatureDuplicate.AddinImpl>(ThisAddIn.IDs.DockableFeatureDuplicate);
        #endregion

        // Parte di codice per mostrare la Dockable Window
        UID dockableDoppioniID = new UIDClass(); // UID = Unique Identifier Object.
        IDockableWindow DockableDoppioni = null;

        protected override void OnClick()
        {
            DialogResult avvisoUtente = MessageBox.Show("La funzionalita' di ricerca doppioni e' ancora instabile!\n\nProcedere?", "ATTENZIONE!", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (avvisoUtente == DialogResult.Yes)
            {
                dockableDoppioniID.Value = ThisAddIn.IDs.DockableFeatureDuplicate; // Lo valorizzo con l'identificativo della Dockable
                DockableDoppioni = ArcMap.DockableWindowManager.GetDockableWindow(dockableDoppioniID);

                DockableDoppioni.Show(true);
            }
        }
    }
}
