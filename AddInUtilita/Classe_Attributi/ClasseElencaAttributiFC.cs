using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using ESRI.ArcGIS.Geodatabase;


/* Creo ora una classe dedicata al mostrare tutti gli attributi di un Feature Layer selezionato
 */

namespace AddInUtilita
{
    class ElencaAttributiFC // Questa classe NON è statica, va pertanto istanziata!
    {
        public int Num
        { get; set; } // Get & Set --> Proprietà di lettura e scrittura

        public string Nome
        { get; set; }

        public string Alias
        { get; set; }

        public esriFieldType Tipologia // oggetto tipologia di un attributo di un Feature Layer
        { get; set; }

        public int Lunghezza
        { get; set; }

        public bool Is_Required
        { get; set; }

        public bool Is_Nullable
        { get; set; }

        /// <summary>
        /// Costruttore della classe
        /// </summary>
        /// <param name="intAttributiInput"></param>
        public ElencaAttributiFC(int intAttributiInput) 
        { this.Num = intAttributiInput; }
    }
}
