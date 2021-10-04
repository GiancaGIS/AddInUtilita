using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using System;
using System.Windows.Forms;


namespace AddInUtilita
{
    public class ToolPosizionaGraphics : ESRI.ArcGIS.Desktop.AddIns.Tool
    {
        public ToolPosizionaGraphics()
        {
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnMouseDown(MouseEventArgs arg)
        {
            try
            {
                IMxDocument mxdoc = ArcMap.Application.Document as IMxDocument;
                IMap mappa = mxdoc.FocusMap;

                // Ricavo le coordinate del punto cliccato sull'activeview dall'utente
                IActiveView activeView = mappa as IActiveView;
                IScreenDisplay3 screenDisplay = activeView.ScreenDisplay as IScreenDisplay3;
                IPoint punto = screenDisplay.DisplayTransformation.ToMapPoint(arg.X, arg.Y);

                // Mi occupo ora di scegliere i colori...
                IRgbColor spazioColore = new RgbColor();
                Random randomico = new Random();
                spazioColore.Blue = randomico.Next(255);
                spazioColore.Red = randomico.Next(255);
                spazioColore.Green = randomico.Next(255);

                // Mi occupo ora della simbologia...
                ISimpleMarkerSymbol markerSymbol = new SimpleMarkerSymbol();
                markerSymbol.Size = 23;
                markerSymbol.Style = esriSimpleMarkerStyle.esriSMSDiamond;
                markerSymbol.Color = spazioColore;

                // Istanzio l'oggetto Marker Element necessario per la creazione dell'oggetto Element...
                IMarkerElement markerElement = new MarkerElement() as IMarkerElement;
                markerElement.Symbol = markerSymbol;

                // Creo e valorizzo l'oggetto Element necessario per l'aggiunta dall'oggetto Graphics...
                IElement elemento = markerElement as IElement;
                elemento.Geometry = punto as IGeometry;

                // Procedo ora al disegno del Graphics
                IGraphicsContainer contenitoreGraphics = mappa as IGraphicsContainer;
                contenitoreGraphics.AddElement(elemento, 0);
                activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

            }

            catch (Exception errore)
            {
                MessageBox.Show(errore.Message, "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
