using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Display;

namespace AddInUtilita
{
    public class InserisciFrecciaNord : ESRI.ArcGIS.Desktop.AddIns.Tool
    {
        public InserisciFrecciaNord()
        {
            this.Cursor = Cursors.Cross;
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
        }

        protected override void  OnMouseDown(ESRI.ArcGIS.Desktop.AddIns.Tool.MouseEventArgs arg)
        {
            try
            {
                IMxDocument mxdoc = ArcMap.Application.Document as IMxDocument;
                IActiveView activeView = mxdoc.PageLayout as IActiveView;
                IGraphicsContainer gc = mxdoc.PageLayout as IGraphicsContainer;

                IGraphicsContainer graphicsContainer = mxdoc.PageLayout as IGraphicsContainer;
                graphicsContainer.Reset();

                // Devo per prima cosa eliminare le freccie del Nord se già esistenti nel Layout View!
                IElement element = graphicsContainer.Next();
                while (element != null)
                {
                    if (element is IMapSurroundFrame)
                    {
                        IMapSurroundFrame MSF = element as IMapSurroundFrame;
                        if (MSF.MapSurround is INorthArrow)
                        {
                            gc.DeleteElement(element);
                        }
                    }
                    element = graphicsContainer.Next();
                }

                IPoint point = activeView.ScreenDisplay.DisplayTransformation.ToMapPoint(arg.X, arg.Y);
                IEnvelope envelope = new Envelope() as IEnvelope;

                envelope.XMin = point.X;
                envelope.YMin = point.Y;
                envelope.Width = 5;
                envelope.Height = 5;

                IStyleGallery styleGallery = mxdoc.StyleGallery;
                IEnumStyleGalleryItem enumStyleGallery = styleGallery.get_Items("North Arrows", "ESRI.STYLE", "Default");

                IStyleGalleryItem northArrowStyle = enumStyleGallery.Next();
                while (northArrowStyle != null)
                {
                    if (northArrowStyle.Name == "ESRI North 3")
                    {
                        break;
                    }
                    northArrowStyle = enumStyleGallery.Next();
                }

                INorthArrow northArrow = northArrowStyle.Item as INorthArrow;
                northArrow.Map = mxdoc.FocusMap;

                IMapSurroundFrame pMSFrame = new MapSurroundFrame() as IMapSurroundFrame;
                pMSFrame.MapSurround = northArrow;
                IElement MSElement = pMSFrame as IElement;
                MSElement.Geometry = envelope as IGeometry;

                gc.AddElement(MSElement, 0);
                mxdoc.ActiveView.PartialRefresh(esriViewDrawPhase.esriViewGraphics, null, null);
            }

            catch (Exception errore)
            {
                MessageBox.Show(errore.Message, "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

}
