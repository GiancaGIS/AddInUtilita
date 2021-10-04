using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;


namespace AddInUtilita
{
    public class ApriDockableStatistiche : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        #region parte commentata non pi utile
        //DockableWinStatistics.AddinImpl classeDockableStatistiche =
        //   ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID<DockableWinStatistics.AddinImpl>(ThisAddIn.IDs.DockableWinStatistics);
        #endregion

        // Parte di codice per mostrare la Dockable Window
        UID dockableStatisticheID = new UIDClass(); // UID = Unique Identifier Object.
        IDockableWindow DockableStatistiche = null;

        protected override void OnClick()
        {
            dockableStatisticheID.Value = ThisAddIn.IDs.DockableWinStatistics; // Lo valorizzo con l'identificativo della Dockable
            DockableStatistiche = ArcMap.DockableWindowManager.GetDockableWindow(dockableStatisticheID);

            DockableStatistiche.Show(true);
         
        }
    }
}
