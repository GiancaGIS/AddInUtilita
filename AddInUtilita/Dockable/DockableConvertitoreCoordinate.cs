using ESRI.ArcGIS.Geometry;
using System;
using System.Windows.Forms;

namespace AddInUtilita
{
    /// <summary>
    /// Designer class of the dockable window add-in. It contains user interfaces that
    /// make up the dockable window.
    /// </summary>
    public partial class DockableConvertitoreCoordinate : UserControl
    {
        private int _EPSG_WGS84 = 4326;
        private ISpatialReferenceFactory3 _srFactory = (ISpatialReferenceFactory3)new SpatialReferenceEnvironment();
        private ISpatialReference3 _geographicSR = null;

        public DockableConvertitoreCoordinate(object hook)
        {
            InitializeComponent();
            this.Hook = hook;
            _geographicSR = _srFactory.CreateSpatialReference(_EPSG_WGS84) as ISpatialReference3;
        }

        /// <summary>
        /// Host object of the dockable window
        /// </summary>
        private object Hook
        {
            get;
            set;
        }


        public static class distanzeClass
        {
            /// <summary>
            /// Dato un angolo in gradi, restituisco il medesimo angolo in radianti. 
            /// </summary>
            /// <param name="angle">Angolo in gradi (360°)</param>
            /// <returns></returns>
            public static double ConvertiInRadianti(double angle)
            {
                return (Math.PI / 180) * angle;
            }

            /// <summary>
            /// Formula Matematica di Haversine per calcolare la distanza metrica, fra due punti angolari sul globo terrestre
            /// </summary>
            /// <param name="lat1">Latitudine Primo Punto</param>
            /// <param name="long1">Longitudine Primo Punto</param>
            /// <param name="lat2">Latitudine Secondo Punto</param>
            /// <param name="long2">Longitudine Secondo Punto</param>
            /// <returns>Ritorna la distanza metrica</returns>
            public static double DistanzaDa(double lat1, double long1, double lat2, double long2)
            {
                double raggioTerra = 6371000; //metri
                double deltaLat = ConvertiInRadianti(lat2 - lat1);
                double deltaLong = ConvertiInRadianti(long2 - long1);
                double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                            Math.Cos(ConvertiInRadianti(lat1)) * Math.Cos(ConvertiInRadianti(lat2)) *
                            Math.Sin(deltaLong / 2) * Math.Sin(deltaLong / 2);

                double dist = Math.Round(2 * raggioTerra * Math.Asin(Math.Sqrt(a)), 2);

                return dist; //metri
            }
        }


        /// <summary>
        /// Implementation class of the dockable window add-in. It is responsible for 
        /// creating and disposing the user interface class of the dockable window.
        /// </summary>
        public class AddinImpl : ESRI.ArcGIS.Desktop.AddIns.DockableWindow
        {
            private DockableConvertitoreCoordinate m_windowUI;

            public AddinImpl()
            {
            }

            protected override IntPtr OnCreateChild()
            {
                m_windowUI = new DockableConvertitoreCoordinate(this.Hook);
                return m_windowUI.Handle;
            }

            protected override void Dispose(bool disposing)
            {
                if (m_windowUI != null)
                    m_windowUI.Dispose(disposing);

                base.Dispose(disposing);
            }

        }

        private void BottoneConverti_Click(object sender, EventArgs e)
        {
            try
            {
                if (Codice_EPSG.Text.Length == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Non hai specificato il codice EPSG del SR planare!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    if (x_input.Text.Length == 0 || y_input.Text.Length == 0)
                    {
                        System.Windows.Forms.MessageBox.Show("Le coordinate inserite non sono valide!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }

                    else
                    {
                        int EPSG_sistema_planare = Convert.ToInt32(Codice_EPSG.Text);

                        ISpatialReference3 ProjectedSR = _srFactory.CreateSpatialReference(EPSG_sistema_planare) as ISpatialReference3;

                        IPoint punto = new ESRI.ArcGIS.Geometry.Point
                        {
                            SpatialReference = ProjectedSR
                        };
                        punto.PutCoords(Convert.ToDouble(x_input.Text.Replace(".", ",")), Convert.ToDouble(y_input.Text.Replace(".", ",")));

                        try
                        {
                            punto.Project(_geographicSR);

                            double LONG = punto.X;
                            double LAT = punto.Y;

                            System.Media.SystemSounds.Beep.Play();

                            LONGITUDINE.Text = Convert.ToString(LONG);
                            LATITUDINE.Text = Convert.ToString(LAT);

                            // Copio le info in clipboard...
                            Clipboard.SetText(string.Format("LONG: {0}\nLAT: {1}", LONGITUDINE.Text, LATITUDINE.Text));
                        }

                        catch
                        {
                            System.Windows.Forms.MessageBox.Show("Le coordinate inserite non possono essere convertite in quanto invalide!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
            }
        }

        private void BottoneInvesione_Click(object sender, EventArgs e)
        {
            // Uguale all'altro bottone, ma completamente SPECULARE nel comportamento
            if (Codice_EPSG.Text.Length == 0)
            {
                System.Windows.Forms.MessageBox.Show("Non hai specificato il codice EPSG del SR planare!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                if (LONGITUDINE.Text.Length == 0 || LATITUDINE.Text.Length == 0)
                {
                    System.Windows.Forms.MessageBox.Show("Le coordinate inserite non sono valide!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                else
                {
                    int EPSG_WGS84 = 4326;
                    int EPSG_sistema_planare = Convert.ToInt32(Codice_EPSG.Text.Replace(".", ","));

                    ISpatialReferenceFactory3 srFactory = new SpatialReferenceEnvironment() as ISpatialReferenceFactory3;
                    ISpatialReference3 ProjectedSR = srFactory.CreateSpatialReference(EPSG_sistema_planare) as ISpatialReference3;
                    ISpatialReference3 GeographicSR = srFactory.CreateSpatialReference(EPSG_WGS84) as ISpatialReference3;

                    IPoint punto = new ESRI.ArcGIS.Geometry.Point
                    {
                        SpatialReference = GeographicSR
                    };
                    punto.PutCoords(Convert.ToDouble(LONGITUDINE.Text.Replace(".", ",")), Convert.ToDouble(LATITUDINE.Text.Replace(".", ",")));

                    try
                    {
                        punto.Project(ProjectedSR);

                        double X_PROIETT = Math.Round(punto.X, 3);
                        double Y_PROIETT = Math.Round(punto.Y, 3);

                        System.Media.SystemSounds.Beep.Play();

                        x_input.Text = Convert.ToString(X_PROIETT);
                        y_input.Text = Convert.ToString(Y_PROIETT);

                        // Copio le info in clipboard...
                        Clipboard.SetText(String.Format("X: {0}\nY: {1}", x_input.Text, y_input.Text));
                    }

                    catch
                    {
                        System.Windows.Forms.MessageBox.Show("Le coordinate inserite non possono essere convertite in quanto invalide!", "Errore", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void buttonePulitore_Click(object sender, EventArgs e)
        {
            x_input.Clear();
            y_input.Clear();
            LATITUDINE.Clear();
            LONGITUDINE.Clear();
        }

        private void calcolaDistanzaMetrica_Click(object sender, EventArgs e)
        {
            if (latiPunto1.Text != String.Empty
                && longiPunto1.Text != String.Empty
                && latiPunto2.Text != String.Empty
                && longiPunto2.Text != String.Empty)
            {
                try
                {
                    double dblDistanza = distanzeClass.DistanzaDa(Convert.ToDouble(latiPunto1.Text.Replace(".", ",")), Convert.ToDouble(longiPunto1.Text.Replace(".", ",")), Convert.ToDouble(latiPunto2.Text.Replace(".", ",")), Convert.ToDouble(longiPunto2.Text.Replace(".", ",")));
                    distanzaMetrica.Text = dblDistanza.ToString();
                }

                catch (Exception errore)
                {
                    MessageBox.Show(errore.Message);
                }
            }

            else

            { return; }
        }

        private void svuotaTutto_Click(object sender, EventArgs e)
        {
            longiPunto1.Clear();
            latiPunto1.Clear();
            longiPunto2.Clear();
            latiPunto2.Clear();
            distanzaMetrica.Clear();
        }
    }
}
