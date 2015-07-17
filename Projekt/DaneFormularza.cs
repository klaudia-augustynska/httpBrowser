using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;




namespace Projekt
{
    /// <summary>
    /// Klasa opisująca zależności między polami formularza
    /// </summary>
    public class DaneFormularza : INotifyPropertyChanged
    {
        private string adresStrony = "klaudynka";

        /// <summary>
        /// Zależności opisujące pole adresu strony
        /// </summary>
        
        public string AdresStrony
        {

            get 
            { 
                return adresStrony; 
            }
            set
            {
                Uri uri;

                if (String.IsNullOrEmpty(value))
                {
                    throw new ApplicationException
                        ("Brak adresu strony");
                }
                else if (Uri.TryCreate(adresStrony, UriKind.Absolute, out uri) == true)
                {
                    throw new ApplicationException
                        ("Niepoprawny adres strony");
                }
                else
                {
                    adresStrony = value;
                    OnPropertyChanged("Name");
                }
            }

        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }

        }

    }
}
