using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Framework;

namespace AddInUtilita
{
    public class ApriDockableConvertitoreCoordinate : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        // Inizializzo classe per dockabke, con questo metodo mi permette di passargli i valori
        DockableConvertitoreCoordinate.AddinImpl classe_per_implementare_dockable =
            ESRI.ArcGIS.Desktop.AddIns.AddIn.FromID<DockableConvertitoreCoordinate.AddinImpl>(ThisAddIn.IDs.DockableConvertitoreCoordinate);

        // Parte di codice per mostrare la Dockable Window
        UID dockableConvertitoreID = new UIDClass(); // UID = Unique Identifier Object.
        IDockableWindow DockableConvertitore;


        protected override void OnClick()
        {
            dockableConvertitoreID.Value = ThisAddIn.IDs.DockableConvertitoreCoordinate; // Lo valorizzo con l'identificativo della Dockable
            DockableConvertitore = ArcMap.DockableWindowManager.GetDockableWindow(dockableConvertitoreID);

            DockableConvertitore.Show(true);
        }

        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }

}