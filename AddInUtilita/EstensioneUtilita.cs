using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using ESRI.ArcGIS.Desktop.AddIns;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.ArcMapUI;

using AddInUtilita.Parametri;


namespace AddInUtilita
{
    public class EstensioneUtilita : ESRI.ArcGIS.Desktop.AddIns.Extension
    {
        private static EstensioneUtilita estensioneUtilita = null;
        private static IMxDocument mxdDoc = ArcMap.Application.Document as IMxDocument;
        private static IMap mappa = mxdDoc.FocusMap;

        private static IActiveViewEvents_Event eventiActiveView = null;

        public EstensioneUtilita()
        {
            estensioneUtilita = this;
        }

        protected override void OnStartup()
        {
            try
            {
                ArcMap.Events.OpenDocument += new ESRI.ArcGIS.ArcMapUI.IDocumentEvents_OpenDocumentEventHandler(eventOpenDocument);
            }
            catch
            { }
        }

        private void eventOpenDocument()
        {
            try
            {
                if (this.State == ExtensionState.Enabled)
                {
                    IMxDocument mxdDoc = ArcMap.Application.Document as IMxDocument;
                    IMap mappa = mxdDoc.FocusMap;

                    ComboboxLayer.svuotaTutto();

                    for (uint i = 0; i < mappa.LayerCount; i++)
                    {
                        ComboboxLayer.AggiungiElemento(mappa.Layer[(int)i].Name);
                    }
                }
            }
            catch
            { }
        }

        protected override void OnShutdown()
        {
            try
            {
                this.SganciaEventi();
            }
            catch
            { }
        }

        protected override bool OnSetState(ESRI.ArcGIS.Desktop.AddIns.ExtensionState state)
        {
            try
            {
                this.State = state;

                if (state == ExtensionState.Enabled)
                {
                    ParametriGenerali.VarieEventuali.StatoEstensione = true;
                    AgganciaEventi();
                }

                else if (state == ExtensionState.Disabled)
                {
                    ParametriGenerali.VarieEventuali.StatoEstensione = false;
                    SganciaEventi();
                }
            }
            catch
            { }

            return base.OnSetState(state);
        }

        private void AgganciaEventi()
        {
            try
            {
                eventiActiveView = mappa as IActiveViewEvents_Event;
                eventiActiveView.ItemAdded += new IActiveViewEvents_ItemAddedEventHandler(popolaComboboxFeatureLayer);
                eventiActiveView.ItemDeleted += new IActiveViewEvents_ItemDeletedEventHandler(popolaComboboxFeatureLayer);
            }
            catch
            { throw; }
        }

        private void SganciaEventi()
        {
            try
            {
                eventiActiveView = mappa as IActiveViewEvents_Event;
                eventiActiveView.ItemAdded -= new IActiveViewEvents_ItemAddedEventHandler(popolaComboboxFeatureLayer);
                eventiActiveView.ItemDeleted -= new IActiveViewEvents_ItemDeletedEventHandler(popolaComboboxFeatureLayer);
                eventiActiveView = null;
            }
            catch
            { throw; }
        }

        private void popolaComboboxFeatureLayer(object item)
        {
            try
            {
                if (this.State == ExtensionState.Enabled)
                {
                    IMxDocument mxdDoc = ArcMap.Application.Document as IMxDocument;
                    IMap mappa = mxdDoc.FocusMap;

                    ComboboxLayer.svuotaTutto();

                    for (uint i = 0; i < mappa.LayerCount; i++)
                    {
                        ComboboxLayer.AggiungiElemento(mappa.Layer[(int)i].Name);
                    }
                }
            }
            catch
            { }
        }
    }
}
