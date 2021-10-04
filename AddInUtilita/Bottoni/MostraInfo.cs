using System.Collections.Generic;
using System.Windows.Forms;

using ESRI.ArcGIS.ArcMapUI;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geodatabase;


namespace AddInUtilita
{
    public class MostraInfo : ESRI.ArcGIS.Desktop.AddIns.Button
    {
        public MostraInfo()
        {
        }

        protected override void OnClick()
        {
            IMxDocument mxd_corrente = ArcMap.Application.Document as IMxDocument;

            IMap dataframe_attivo = mxd_corrente.FocusMap; // Creo la variabile Focus Map (dataframe attivo! .activeDataFrame in arcpy) che ha come metodo il rimuovi tutti i layer

            if (dataframe_attivo.LayerCount == 0) // Conto il numero di layer presenti
            {
                MessageBox.Show("Non ci sono layer nel Dataframe attivo!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            else
            {
                if (mxd_corrente.SelectedItem is IFeatureLayer2 || mxd_corrente.SelectedItem is ITable)
                {
                    if (mxd_corrente.SelectedItem is IRasterLayer)
                    {
                        MessageBox.Show("Hai selezionato un Raster Layer!!\nSelezionare un Feature Layer.", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    else
                    {
                        ITable tab_selezionata_utente = mxd_corrente.SelectedItem as ITable;
                        reportSchema(tab_selezionata_utente);
                    }
                }
            }
        }

        void reportSchema(ITable tabella_input)
        {
            /* Creo lista di tutte le istanze per la mia classe, ATTENZIONE NON E' COME PYTHON!!
               UNA LISTA DI OGGETTI FEATURE CLASS PUO' CONTENERE SOLAMENTE OGGETTI FEATURE CLASS!!!
             */
            List<ElencaAttributiFC> lista_per_classe = new List<ElencaAttributiFC>(); // creo lista che può contiene gli attributi

            IFields2 attributi = tabella_input.Fields as IFields2; // creo oggetto attributi dai campi della tabella 
            // di input

            for (int i = 0; i < attributi.FieldCount; i++)
            {
                IField2 attributo = attributi.Field[i] as IField2;

                ElencaAttributiFC istanzaClasseDescAttributi = new ElencaAttributiFC(i + 1);
                istanzaClasseDescAttributi.Nome = attributo.Name;
                istanzaClasseDescAttributi.Alias = attributo.AliasName;
                istanzaClasseDescAttributi.Is_Nullable = attributo.IsNullable;
                istanzaClasseDescAttributi.Is_Required = attributo.Required;
                istanzaClasseDescAttributi.Tipologia = attributo.Type;
                istanzaClasseDescAttributi.Lunghezza = attributo.Length;

                /* Popolo ora la lista List<ElencaAttributiFC> con tutte le informazioni ricavate e memorizzate
                   nella mia classe custom */

                lista_per_classe.Add(istanzaClasseDescAttributi);
            }

            IDataset dsTable = tabella_input as IDataset; 
            MiaForm form = new MiaForm(); 
            form.dgv.DataSource = lista_per_classe; // Assegno alla lista della form la lista appena riempita
            form.Text = "Schema attributi di: " + dsTable.Name;
            System.Media.SystemSounds.Beep.Play();
            form.ShowDialog();
        }

        protected override void OnUpdate()
        {
            Enabled = ArcMap.Application != null;
        }
    }
}
