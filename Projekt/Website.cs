using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace Projekt
{
    /// <summary>
    /// Klasa opisująca stronę internetową jako kod źródłowy oraz zestaw adresów plików, które w nim wystąpiły.
    /// </summary>
    public class Website : Polaczenie
    {
        /// <summary>
        /// Kod źródłowy strony
        /// </summary>
        public string Kod { get; private set; }

        /// <summary>
        /// Lista wszystkich linków na danej stronie
        /// </summary>
        public MatchCollection Matches { get; private set; }

        /// <summary>
        /// Tworzy obiekt strony o zadanym adresie
        /// oraz pobiera aktualne dane nt. tej strony
        /// (jej kod oraz listę odnalezionych linków)
        /// </summary>
        /// <param name="adres">Adres strony internetowej</param>
        public Website(string adres)
        {
            adresStrony = adres;
            Uri uri;

            try
            {
                // Uzupełnij pole adresUri
                if (Uri.TryCreate(adresStrony, UriKind.Absolute, out uri))
                    adresUri = uri;
                else
                    throw new ApplicationException
                        ("Nie można utworzyć adresu URI");

                // Sprawdź warunki konieczne
                if (this.SprawdzPolaczenieInternetowe() == false)
                    throw new ApplicationException
                        ("Brak połączenia z internetem");

                if (this.SprawdzPolaczenieZSerwerem() == false)
                    throw new ApplicationException
                        ("Brak połączenia ze stroną");

                // Jeśli wszystko OK to uzupełnij informacje o stronie
                this.PobierzKod();
                this.PobierzAdresy();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Website(): " + ex.Message);
            }
        }

        /// <summary>
        /// Pobiera kod HTML zadanej strony
        /// </summary>
        /// <returns>True - jeśli udało się pobrać, false - jeśli się nie udało</returns>
        private bool PobierzKod()
        {
            bool jestSuper = false;

            try
            {
                // Zrób zapytanie HTTP o kod strony
                WebRequest request = WebRequest.Create(adresUri);
                request.Method = "GET";
                WebResponse response = request.GetResponse();

                try
                {
                    // Uzyskaj strumień
                    Stream stream = response.GetResponseStream();

                    // Uruchom StreamReader do tego strumienia
                    StreamReader reader = new StreamReader(stream);

                    try 
                    {
                        // Przeczytaj kod
                        Kod = reader.ReadToEnd();
                        jestSuper = true;
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("PobierzKod-read:" + ex.Message);
                    }
                    finally {
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("PobierzKod-stream:" + ex.Message);
                }
                finally {
                    response.Close();
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("PobierzKod-response:" + ex.Message);
            }

            return jestSuper;
        }

        /// <summary>
        /// Pobiera listę wszystkich adresów plików, 
        /// które pojawiły się na stronie.
        /// </summary>
        private void PobierzAdresy()
        {
            try
            {
                // Wyrażenie regularne opisujące poprawny adres URL
                string pattern = @"(http://)[A-Za-z0-9\-\.]+\.[A-Za-z]{2,3}(:[a-zA-Z0-9]*)?/?([a-zA-Z0-9\-\._\?\,\'/\\\+&amp;%\$#\=~])*[^\.\,\)\(\s\u0022/]";
                
                // Załaduj wyrażenie regularne
                Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);

                // Utwórz listę wyników
                Matches = regex.Matches(this.Kod);
            }
            catch (Exception ex)
            {
                MessageBox.Show("PobierzAdresy:" + ex.Message);
            }
            
        }
    }

}
