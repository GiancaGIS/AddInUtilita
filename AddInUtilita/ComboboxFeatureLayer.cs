using System;

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;


namespace AddInUtilita
{
    public class ComboboxLayer : ESRI.ArcGIS.Desktop.AddIns.ComboBox
    {
        private static ComboboxLayer comboboxLayer = null;
        private static string layerSelezionato = String.Empty;
        private static IFeatureLayer2 fLayer = null;

        public ComboboxLayer()
        {
            comboboxLayer = this;
        }

        protected override void OnUpdate()
        {
            this.Enabled = Parametri.ParametriGenerali.VarieEventuali.StatoEstensione;
        }

        protected override void OnSelChange(int cookie)
        {
            ComboBoxAttr.SvuotaTutto();
            layerSelezionato = this.GetItem(cookie).Caption;

            IMxDocument mxdoc = ArcMap.Document as IMxDocument;
            IMap map = mxdoc.FocusMap;

            for (uint i = 0; i < map.LayerCount; i++)
            {
                ILayer2 layer = map.Layer[(int)i] as ILayer2;

                if (layer is IFeatureLayer2 && layer.Name.ToUpper() == layerSelezionato.ToUpper())
                {
                    fLayer = layer as IFeatureLayer2;
                    IFeatureClass fClass = fLayer.FeatureClass;

                    for (int j = 0; j < fClass.Fields.FieldCount; j++)
                    {
                        switch (fClass.Fields.Field[j].Type)
                        {
                            case esriFieldType.esriFieldTypeDouble:
                            case esriFieldType.esriFieldTypeInteger:
                            case esriFieldType.esriFieldTypeSingle:
                            case esriFieldType.esriFieldTypeSmallInteger:
                                ComboBoxAttr.AggiungiElemento(fClass.Fields.Field[j].Name);
                                break;
                        }
                    }
                }
            }
        }

        internal static void svuotaTutto()
        {
            layerSelezionato = null;
            comboboxLayer.Clear();
        }

        internal static void AggiungiElemento(string featureLayerName)
        {
            comboboxLayer.Add(featureLayerName);
        }

        internal static string nomeFeatureLayerSelezionato()
        {
            return layerSelezionato;
        }

        internal static ILayer2 oggettoILayerSelezionato()
        {
            return (ILayer2)fLayer;
        }
    }
}