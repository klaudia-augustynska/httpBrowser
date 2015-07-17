using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;



namespace Projekt
{
    /// <summary>
    /// Klasa opisująca plik znaleziony przez program
    /// </summary>
    public class Plik 
    {
        /// <summary>
        /// Nazwa pliku bez rozszerzenia
        /// </summary>
        public string Nazwa { get; set; }

        /// <summary>
        /// Adres hosta
        /// </summary>
        public string Domena { get; set; }

        /// <summary>
        /// Rozszerzenie pliku, np. jpg
        /// </summary>
        public string Typ { get; set; }

        /// <summary>
        /// Rozmiar pliku
        /// </summary>
        public string Rozmiar { get; set; }

        /// <summary>
        /// Link bezpośredni do pliku, umożliwiający pobranie go
        /// </summary>
        public string Pobierz { get; set; }

        /// <summary>
        /// Funkcja uzupełnia pole "Rozmiar" w pliku 
        /// oraz zwraca przy okazji informację, 
        /// czy ten plik istnieje.
        /// </summary>
        /// <returns>True jeśli plik istnieje, false jeśli nie istnieje.</returns>
        public bool PobierzRozmiar()
        {
            try
            {
                // Utwórz zapytanie HTTP do zadanej strony
                WebRequest zapytanie = WebRequest.Create(this.Pobierz);

                // Wybierz zapytanie HEAD, gdyż tam jest info o rozmiarze pliku
                zapytanie.Method = "HEAD";

                using (WebResponse odpowiedz = zapytanie.GetResponse())
                {
                    int rozmiarPliku;

                    // jeśli można odczytać rozmiar pliku
                    if (int.TryParse(odpowiedz.Headers.Get("Content-Length"), out rozmiarPliku))
                    {

                        // Przelicz rozmiar pliku na czytelne jednostki
                        PrzeliczRozmiar(rozmiarPliku);
                        return true;

                    }
                }
            }
            catch (Exception ex)
            {
                return false;
            }

            return false;
        }

        /// <summary>
        /// Funkcja przelicza rozmiar pliku zadany w bajtach
        /// na bardziej przyjazne jednostki
        /// i zapisuje wynik do właściwości Rozmiar
        /// </summary>
        /// <param name="rozmiarPliku">Rozmiar pliku w bajtach</param>
        private void PrzeliczRozmiar(int rozmiarPliku)
        {

            if (rozmiarPliku > 1048576)
            {
                rozmiarPliku /= 1048576;
                this.Rozmiar = rozmiarPliku + " MB";
            }
            else if (rozmiarPliku > 1024)
            {
                rozmiarPliku /= 1024;
                this.Rozmiar = rozmiarPliku + " KB";
            }
            else
            {
                this.Rozmiar = rozmiarPliku + " B";
            }

        }

    }

}
