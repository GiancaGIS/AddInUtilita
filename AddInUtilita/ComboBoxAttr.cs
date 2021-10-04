using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

using AddInUtilita.Parametri;


namespace AddInUtilita
{
    public class ComboBoxAttr : ESRI.ArcGIS.Desktop.AddIns.ComboBox
    {
        private static ComboBoxAttr comboboxAttr = null;
        private static string attrSelezionato = String.Empty;

        public ComboBoxAttr()
        {
            comboboxAttr = this;
        }

        protected override void OnUpdate()
        {
            this.Enabled = Parametri.ParametriGenerali.VarieEventuali.StatoEstensione;
        }

        protected override void OnSelChange(int cookie)
        {
            if (cookie < 0)
            { return; }

            attrSelezionato = this.GetItem(cookie).Caption;
        }

        //for sharing functionality
        internal static string GetSelectedField()
        {
            return attrSelezionato;
        }

        internal static void AggiungiElemento(string fieldName)
        {
            comboboxAttr.Add(fieldName);
        }

        internal static void SvuotaTutto()
        {
            attrSelezionato = null;
            comboboxAttr.Clear();
        }

        internal static string AttributoSelezionato()
        {
            return attrSelezionato;
        }

    }

}
