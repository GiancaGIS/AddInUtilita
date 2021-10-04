using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;

using AddInUtilita.GraficoBarre;


namespace AddInUtilita
{
    public class CreaGraficoBarre : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public CreaGraficoBarre()
        {
        }

        protected override void OnClick()
        {
            frmBarchart graficoBarre = new frmBarchart();

            graficoBarre.NomeFeatureLayer = ComboboxLayer.nomeFeatureLayerSelezionato();
            graficoBarre.NomeAttributo = ComboBoxAttr.AttributoSelezionato();

            try
            {
                graficoBarre.ShowDialog();
            }

            catch (Exception errore)
            {
                MessageBox.Show(errore.Message, "ERRORE", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        protected override void OnUpdate()
        {
            if (Parametri.ParametriGenerali.VarieEventuali.StatoEstensione == true
                && ComboBoxAttr.AttributoSelezionato() != null
                && ComboboxLayer.nomeFeatureLayerSelezionato() != null)
            {
                this.Enabled = true;
            }
        }
    }
}
