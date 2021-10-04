using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.CartoUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.ArcMapUI;


namespace AddInUtilita
{
    public class ToolSmartInfoOggetto : ESRI.ArcGIS.Desktop.AddIns.Tool
    {
        public ToolSmartInfoOggetto()
        {
            this.Cursor = Cursors.Help;
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnMouseDown(MouseEventArgs arg)
        {
            IMxDocument mxdDoc = ArcMap.Application.Document as IMxDocument;
            IActiveView activeView = mxdDoc.ActiveView;

            try
            {
                smartIdentify(activeView, arg.X, arg.Y);
            }

            catch (Exception errore)
            {
                MessageBox.Show(errore.Message, "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Esegui una smart identify!
        /// </summary>
        /// <param name="activeView"></param>
        /// <param name="xCoord"></param>
        /// <param name="yCoord"></param>
        public void smartIdentify(IActiveView activeView, int xCoord, int yCoord)
        {
            if (activeView == null)
            {
                return;
            }

            IMap map = activeView.FocusMap;

            IIdentifyDialog identifyDialog = new IdentifyDialog();
            identifyDialog.Map = map;

            // Clear the dialog on each mouse click
            identifyDialog.ClearLayers();
            IScreenDisplay screenDisplay = activeView.ScreenDisplay;

            IDisplay display = screenDisplay; // Implicit Cast
            identifyDialog.Display = display;

            IIdentifyDialogProps identifyDialogProps = (IIdentifyDialogProps)identifyDialog; // Explicit Cast
            IEnumLayer enumLayer = identifyDialogProps.Layers;
            enumLayer.Reset();

            ILayer layer = enumLayer.Next();

            while (layer != null)
            {
                identifyDialog.AddLayerIdentifyPoint(layer, xCoord, yCoord);

                layer = enumLayer.Next();
            }

            identifyDialog.Show();
        }
    }

}
