using System;
using System.Windows.Forms;

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.esriSystem;


namespace AddInUtilita
{
    public class SelezionaOggettoInMappaDinamicamente : ESRI.ArcGIS.Desktop.AddIns.Tool
    {
        private static int oid = -1;
        private static int newOID = -1;

        public SelezionaOggettoInMappaDinamicamente()
        {
            this.Cursor = Cursors.Cross;
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnMouseDown(MouseEventArgs arg)
        {
            try
            {
                eseguiSelezione(arg.X, arg.Y);
            }
            catch (Exception errore)
            {
                MessageBox.Show(errore.Message);
            }
        }

        protected override void OnMouseMove(MouseEventArgs arg)
        {
            try
            {
                eseguiSelezione(arg.X, arg.Y);
            }
            catch (Exception errore)
            {
                MessageBox.Show(errore.Message);
            }
        }

        protected void eseguiSelezione(int xCoord, int yCoord)
        {
            try
            {
                IMxDocument mxDocument = ArcMap.Application.Document as IMxDocument;
                IActiveView activeView = mxDocument.ActiveView;
                //IEnvelope2 envelopeCorrente = activeView.Extent as IEnvelope2;
                //activeView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);

                IMap map = mxDocument.FocusMap;

                IPoint identifyPoint = activeView.ScreenDisplay.DisplayTransformation.ToMapPoint(xCoord, yCoord);

                IBufferConstruction operatoreBuffer = new BufferConstruction();

                IGeometry5 geometriaBuffer = operatoreBuffer.Buffer(identifyPoint, 5) as IGeometry5;

                IArea area = (IPolygon)geometriaBuffer as IArea;
                double dblArea = area.Area;

                IEnumLayer enumeratoreLayer = map.Layers;

                ILayer layer = enumeratoreLayer.Next();

                string nomeLayer = string.Empty;

                while (layer != null)
                {
                    nomeLayer = layer.Name;

                    if (layer.Visible)
                    {
                        IFeatureLayer featureLayer = layer as IFeatureLayer;

                        IIdentify identifyLayer = layer as IIdentify;
                        IArray array = identifyLayer.Identify(geometriaBuffer);

                        IFeature feature = null;

                        if (array != null)
                        {
                            object obj = array.get_Element(0);
                            IFeatureIdentifyObj fobj = obj as IFeatureIdentifyObj;
                            IRowIdentifyObject irow = fobj as IRowIdentifyObject;
                            feature = irow.Row as IFeature;
                            newOID = feature.OID;
                        }

                        try
                        {
                            if (newOID > -1 && oid != newOID)
                            {
                                evidenziaFeatureDinamicamente(featureLayer, newOID, activeView);
                                oid = feature.OID; // Solo ora che ho evidenziato, aggiorno il oid per il confronto
                            }
                        }

                        catch
                        {
                            throw; // rigetto l'errore al catch che sta "sopra"!
                        }
                    }

                    layer = enumeratoreLayer.Next();
                }
            }
            catch (Exception errore)
            {
                MessageBox.Show(errore.Message, "ERRORE!", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void evidenziaFeatureDinamicamente(IFeatureLayer layer, int featureOID, IActiveView activeView)
        {
            IScreenDisplay3 screenDisplay = activeView.ScreenDisplay as IScreenDisplay3;

            IFeatureClass featureClass = layer.FeatureClass;
            IFeature feature = featureClass.GetFeature(featureOID);
            IGeometry5 geometriaFeature = feature.ShapeCopy as IGeometry5;

            if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPolyline)
            {
                IPolyline6 poliLinea = geometriaFeature as IPolyline6;
            }
            else if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
            {
                IPolygon5 poligono = geometriaFeature as IPolygon5;
            }
            else if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPoint)
            {
                IPoint punto = geometriaFeature as IPoint;
            }
            
            IBufferConstruction operatoreBuffer = new BufferConstruction();

            IGeometry5 geometriaBuffer = operatoreBuffer.Buffer(geometriaFeature, 3) as IGeometry5;

            //ITopologicalOperator5 operatoreTopologico = geometriaFeature as ITopologicalOperator5;
            //IGeometry5 geometriaBuffer = operatoreTopologico.Buffer(1000) as IGeometry5;

            IRgbColor spazioColoreRGB = new RgbColor();
            Random random = new Random();
            spazioColoreRGB.Green = random.Next(0, 255);
            spazioColoreRGB.Blue = random.Next(0, 255);
            spazioColoreRGB.Red = random.Next(0, 255);

            ISimpleLineSymbol simboLinea = new SimpleLineSymbol();
            simboLinea.Width = 5;
            simboLinea.Style = esriSimpleLineStyle.esriSLSInsideFrame;

            ISimpleFillSymbol simbologiaLinea = new SimpleFillSymbol();

            simbologiaLinea.Color = spazioColoreRGB;
            simbologiaLinea.Style = esriSimpleFillStyle.esriSFSDiagonalCross;

            // Definisco l'oggetto ISymbol
            ISymbol simbolo = simboLinea as ISymbol;

            IArea area = (IPolygon)geometriaBuffer as IArea;

            double dblArea = area.Area;

            // Disegno nello screen display
            screenDisplay.StartDrawing(screenDisplay.hDC, (short)esriScreenCache.esriNoScreenCache);
            screenDisplay.SetSymbol(simbolo);
            //screenDisplay.DrawRectangle((IEnvelope2)geometriaBuffer);
            //screenDisplay.DrawPolygon(geometriaBuffer);
            screenDisplay.DrawPolyline(geometriaBuffer);
            screenDisplay.FinishDrawing();

            #region Parte commentata non più utile
            //ISpatialFilter spatialFilter = new SpatialFilterClass();
            //spatialFilter.Geometry = geometry;
            //spatialFilter.GeometryField = featureClass.ShapeFieldName;
            //spatialFilter.SpatialRel = esriSpatialRelEnum.esriSpatialRelIntersects;
            //spatialFilter.WhereClause = "OBJECTID = " + featureOid;

            //IFeatureSelection featureSelection = layer as IFeatureSelection;

            //if (featureSelection != null)
            //{
            //    featureSelection.SelectFeatures(spatialFilter, esriSelectionResultEnum.esriSelectionResultAdd,false); //multiple selections  
            //    //featureSelection.SelectFeatures(spatialFilter, esriSelectionResultEnum.esriSelectionResultNew, false); //single selection  
            //    activeView.Refresh();
            //}
            #endregion
        }

    }

}
