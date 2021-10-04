using System;
using System.Windows.Forms;

using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;


namespace AddInUtilita
{
    public class ToolRubberBandMethod : ESRI.ArcGIS.Desktop.AddIns.Tool
    {
        public ToolRubberBandMethod()
        {
        }

        protected override void OnUpdate()
        {
        }

        /// <summary>
        /// Funzione per disegnare un poligono dato ciò che è 'tracciato' nell'ActiveView
        /// </summary>
        /// <param name="activeView"></param>
        static void disegnaRettangoloRubber(IActiveView activeView)
        {
            // Accedo all'oggetto screen display
            IScreenDisplay3 screenDisplay = activeView.ScreenDisplay as IScreenDisplay3;

            // Definisco lo spazio colore...
            IRgbColor spazioColoreRGB = new RgbColor();
            Random random = new Random();
            spazioColoreRGB.Green = random.Next(0, 255);
            spazioColoreRGB.Blue = random.Next(0, 255);
            spazioColoreRGB.Red = random.Next(0, 255);

            // Definisco le proprietà simboliche...
            ISimpleFillSymbol simpleFillSymbol = new SimpleFillSymbol();
            simpleFillSymbol.Color = spazioColoreRGB;
            simpleFillSymbol.Style = esriSimpleFillStyle.esriSFSDiagonalCross;

            // Definisco l'oggetto ISymbol
            ISymbol simbolo = simpleFillSymbol as ISymbol;

            // Disegno il poligono col metodo Rubber Band
            IRubberBand2 rubberBand = new RubberEnvelope() as IRubberBand2;
            IGeometry5 geometria = rubberBand.TrackNew((IScreenDisplay)screenDisplay, simbolo) as IGeometry5;

            // Disegno nello screen display
            screenDisplay.StartDrawing(screenDisplay.hDC, (short)esriScreenCache.esriNoScreenCache);
            screenDisplay.SetSymbol(simbolo);
            screenDisplay.DrawRectangle((IEnvelope2)geometria); 
            screenDisplay.FinishDrawing();
        }

        /// <summary>
        /// Evento che scatta al premere del Mouse sull'Active View del DataFrame attivo
        /// </summary>
        /// <param name="arg"></param>
        protected override void OnMouseDown(MouseEventArgs arg)
        {
            try
            {
                IDocument processoArcMap = ArcMap.Application.Document;
                IMxDocument mxdDoc = processoArcMap as IMxDocument;
                IMap mappa = mxdDoc.FocusMap;
                IActiveView activeView = mappa as IActiveView;

                disegnaRettangoloRubber(activeView);
            }
            catch (Exception errore)
            {
                MessageBox.Show(errore.Message, "Errore nel disegno del Rettangolo - Rubber Band", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}