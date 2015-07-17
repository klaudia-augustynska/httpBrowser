using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows;



namespace Projekt
{
    /// <summary>
    /// Wszystkie funkcje potrzebne do uzyskania połączenia ze stroną internetową
    /// </summary>
    public class Polaczenie
    {
        /// <summary>
        /// Adres strony w formie string
        /// </summary>
        protected string adresStrony { get; set; }

        /// <summary>
        /// Adres strony w formie obiektu Uri
        /// </summary>
        protected Uri adresUri { get; set; }

        /// <summary>
        /// Domyślny konstruktor
        /// </summary>
        public Polaczenie() { 
        
        }

        /// <summary>
        /// Tworzy połączenie z zadaną stroną internetową
        /// </summary>
        /// <param name="a">Adres strony internetowej, z którą chcemy się połączyć</param>
        public Polaczenie(string a)
        {
            adresStrony = a;

            Uri uri;

            try
            {
                // Próbuje utworzyć adres URI
                Uri.TryCreate(adresStrony, UriKind.Absolute, out uri);
                adresUri = uri;                    
            }
            catch (Exception ex)
            {
                MessageBox.Show("Połączenie(): " + ex.Message);
            }

        }

        /// <summary>
        /// Sprawdza, czy komputer ma połączenie z internetem
        /// </summary>
        /// <returns>True - jeśli jest połączenie, false - jeśli nie ma połączenia</returns>
        public bool SprawdzPolaczenieInternetowe()
        {
            try
            {
                // Sprawdź, czy dostępne są połączenia sieciowe
                if (!NetworkInterface.GetIsNetworkAvailable())
                    return false;

                // Dla każdej znalezionej konfiguracji
                foreach (NetworkInterface ni in NetworkInterface.GetAllNetworkInterfaces())
                {
                    // Pomiń interfejsy, które:
                    if (
                        // a) są niegotowe do użycia
                        (ni.OperationalStatus != OperationalStatus.Up) ||

                        // b) są wirtualnymi interfejsami (loopback, tunele)
                        (ni.NetworkInterfaceType == NetworkInterfaceType.Loopback) ||
                        (ni.NetworkInterfaceType == NetworkInterfaceType.Tunnel) ||

                        // c) są od VirtualBoxa
                        (ni.Description.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0) ||
                        (ni.Name.IndexOf("virtual", StringComparison.OrdinalIgnoreCase) >= 0))
                        
                        continue;

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SprawdźPołączenieInternetowe: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Sprawdza, czy istnieje połączenie z serwerem na którym jest zadana strona internetowa.
        /// </summary>
        /// <returns>True - jeśli można połączyć się z serwerem, false - jeśli nie można (prawdopodobnie strona nie istnieje)</returns>
        public bool SprawdzPolaczenieZSerwerem()
        {
            Ping ping = new Ping();

            try
            {
                // Jeśli uda się wysłać ping do hosta
                if (ping.Send(adresUri.Host).Status == IPStatus.Success)
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("SprawdźPołączenieZSerwerem: " + ex.Message);
                return false;
            }
        }

    }

}
