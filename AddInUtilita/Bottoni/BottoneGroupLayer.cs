using System;
using System.Windows.Forms;

using ESRI.ArcGIS.Framework;
using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;



namespace AddInUtilita
{
    public class BottoneGroupLayer : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public BottoneGroupLayer()
        {
        }

        protected override void OnClick()
        {
            try
            {
                IDocument processoArcMap = ArcMap.Application.Document;
                IMxDocument mxdoc = processoArcMap as IMxDocument;
                IMap map = mxdoc.FocusMap;
                IContentsView2 contentsView = mxdoc.get_ContentsView(0) as IContentsView2;
                contentsView.RemoveFromSelectedItems(null);

                IEnumLayer enumLayers = map.Layers;

                int numeroLayer = map.LayerCount;

                //List<int> listaIntNumeriGroupLayer = new List<int>();

                ILayer layer = null;

                layer = enumLayers.Next();

                while (layer != null)
                {
                    string nomeLayer = layer.Name;

                    if (layer is ICompositeLayer || layer is IGroupLayer)
                    {
                        IGroupLayer groupLayer = layer as IGroupLayer;
                        ICompositeLayer compositeLayer = groupLayer as ICompositeLayer;

                        // if (compositeLayer.Count > 0)
                        //{
                        //    MessageBox.Show(String.Format("Il layer {0} e' un Group Layer!", layer.Name), "Attenzione", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        //}

                        contentsView.AddToSelectedItems(layer);
                        contentsView.Refresh(null);
                    }

                    layer = enumLayers.Next();                    
                }

            }

            catch (Exception errore)
            {
                MessageBox.Show(errore.Message, "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

        }

        protected override void OnUpdate()
        {
        }
    }
}
