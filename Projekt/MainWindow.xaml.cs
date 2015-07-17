using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;




namespace Projekt
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Lista plików do wypełnienia kontrolki
        /// </summary>
        private ListaPlikow listaPlikow = new ListaPlikow();


        /// <summary>
        /// Konstruktor domyślny.
        /// Tworzy okno programu.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();  
        }

        void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            listViewWyniki.ItemsSource = listaPlikow;
        }

        private async void buttonZnajdz_Click(object sender, RoutedEventArgs e)
        {
            buttonZnajdz.IsEnabled = false;
            // Pobierz asynchronicznie listę wyników
            ListaPlikow lista = await Rob(adresStrony.Text);

            // Usuń stare wyniki.
            listaPlikow.Clear();

            // Komunikat o ewentualnym braku wyników
            if (lista.Count == 0)
                MessageBox.Show("Brak rezultatów");
            else
            {
                // Uaktywnij przycisk pobierania wszystkich
                pobierzWszystkie.IsEnabled = true;

                // Skopiuj wynik do listy wyświetlanej przez program
                foreach (Plik p in lista)
                    listaPlikow.Add(p);
            }

            buttonZnajdz.IsEnabled = true;
        }

        private async void adresStrony_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Return)
            {
                buttonZnajdz.IsEnabled = false;
                // Pobierz asynchronicznie listę wyników
                ListaPlikow lista = await Rob(adresStrony.Text);

                // Usuń stare wyniki.
                listaPlikow.Clear();

                // Komunikat o ewentualnym braku wyników
                if (lista.Count == 0)
                    MessageBox.Show("Brak rezultatów");
                else
                {
                    // Uaktywnij przycisk pobierania wszystkich
                    pobierzWszystkie.IsEnabled = true;

                    // Skopiuj wynik do listy wyświetlanej przez program
                    foreach (Plik p in lista)
                        listaPlikow.Add(p);
                }
                buttonZnajdz.IsEnabled = true;
            }
        }
        
        private void adresStrony_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                // Pobierz info o regułach walidacji pola
                BindingExpression adresStronyBe = adresStrony.GetBindingExpression(TextBox.TextProperty);

                // Sprawdź czy zawartość pola odpowiada zdefiniowanym regułom
                adresStronyBe.UpdateSource();

                // Jeśli jest to poprawny adres, uaktywnij przycisk
                if (adresStronyBe.HasError)
                {
                    buttonZnajdz.IsEnabled = false;
                }
                else
                {
                    buttonZnajdz.IsEnabled = true;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Textchanged: " + ex.Message);
            }
            


        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // Zidentyfikuj o który plik chodzi
            Button button = sender as Button;
            Plik plik = button.DataContext as Plik;

            SaveFileDialog d = new SaveFileDialog()
            {
                OverwritePrompt = true,
                FileName = plik.Nazwa,
                DefaultExt = plik.Typ,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            // Zapisz plik w wybranym miejscu
            if (d.ShowDialog() == true)
            {
                // Webclient - wysyłanie oraz pobieranie danych
                // korzystając z adresu URI
                WebClient webClient = new WebClient();

                webClient.DownloadFileAsync(new Uri(plik.Pobierz), d.FileName);
            }
        }

        private void pobierzWszystkie_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog d = new SaveFileDialog()
            {
                FileName = listaPlikow[0].Nazwa,
                DefaultExt = listaPlikow[0].Typ,
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };

            // Webclient - wysyłanie oraz pobieranie danych
            // korzystając z adresu URI
            WebClient webClient;

            // Wybierz lokalizację i pobierz pliki
            if (d.ShowDialog() == true)
            {
                // index ostatniego wystąpienia znaku "\"
                // aby uzyskać ścieżkę do katalogu
                int a = d.FileName.LastIndexOf('\\');

                // bieżący folder
                string folder = d.FileName.Substring(0, a+1);
                string path;

                foreach (Plik p in listaPlikow)
                {
                    // utwórz ścieżkę dla pliku
                    path = folder + p.Nazwa + "." + p.Typ;

                    // jeśli plik istnieje - 
                    // dopisuj kolejne numerki tak długo,
                    // aż będzie dobra nazwa
                    for (int i = 0; File.Exists(path); ++i)
                        path = folder + p.Nazwa + "(" + i.ToString() + ")." + p.Typ;
                    
                    try
                    {
                        webClient = new WebClient();
                        webClient.DownloadFileAsync(new Uri(p.Pobierz), path);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Problem podczas pobierania plików: "+ex.Message);
                    }
                }
            }

        }

        /// <summary>
        /// Przywraca program do pierwotnej postaci,
        /// pozwala na użycie go do wielu zapytań.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void wyczysc_Click(object sender, RoutedEventArgs e)
        {
            listaPlikow.Clear();
            adresStrony.Text = "";
            pobierzWszystkie.IsEnabled = false;
            buttonZnajdz.IsEnabled = true;
        }

        /// <summary>
        /// Zadanie uruchomienia głównych funkcji programu
        /// </summary>
        private Task<ListaPlikow> Rob(string adresStrony)
        {

            // Zwróć asynchronicznie listę plików
            return Task<ListaPlikow>.Factory.StartNew(() =>
            {
                ListaPlikow lista = new ListaPlikow();

                try
                {
                    // Przetwórz daną stronę internetową
                    Website website = new Website(adresStrony);

                    // Dodaj znalezione adresy plików do listy
                    lista.Dodaj(website.Matches);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Rób: "+ex.Message);
                }

                return lista;
            });

        }

    }

}
