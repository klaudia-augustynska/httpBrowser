using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;



namespace Projekt
{

    /// <summary>
    /// Lista plików znalezionych przez program.
    /// </summary>
    public class ListaPlikow : ObservableCollection<Plik>
    {
        public ListaPlikow()
            : base()
        {
        }

        /// <summary>
        /// Umożliwia dodanie do listy plików listy adresów pozyskanej 
        /// za pomocą wyrażenia regularnego
        /// </summary>
        /// <param name="mm">Adresy pozyskane wyrażeniem regularnym</param>
        public void Dodaj(MatchCollection mm)
        {
            // Wyrażenie pozyskujące hosta
            string pattern = @"http://(?<host>[A-Za-z0-9\-\.]+)(?<reszta>.*)";
            Regex regexHost = new Regex(pattern, RegexOptions.IgnoreCase);

            // Wyrażenie pozyskujące nazwę i typ pliku
            pattern = @"(/(?<nazwa>[a-zA-Z0-9\-\._\?\,\'\\\+&amp;%\$#\=~]+)\.(?<typ>[a-z]+))$";
            Regex regexNazwa = new Regex(pattern, RegexOptions.IgnoreCase);
            
            // Dla każdego adresu w kolekcji...
            foreach (Match m in mm)
            {
                // Pozyskaj hosta
                Match match2 = regexHost.Match(m.Value);
                GroupCollection groups2 = match2.Groups;

                // Pozyskaj nazwę i typ
                Match match3 = regexNazwa.Match(groups2["reszta"].Value);
                GroupCollection groups3 = match3.Groups;

                // Jeśli nie ma rozszerzenia, to pomiń
                if (groups3["typ"].Value.Length == 0) continue;

                // Utwórz plik
                Plik p = new Plik()
                {
                    Nazwa = groups3["nazwa"].Value,
                    Domena = groups2["host"].Value,
                    Typ = groups3["typ"].Value,
                    Pobierz = m.Value
                };

                // Jeśli można pobrać rozmiar pliku, 
                // to znaczy, że istnieje i można go dodać do listy
                if (p.PobierzRozmiar())
                    this.Add(p);

            }
        }

    }
}
