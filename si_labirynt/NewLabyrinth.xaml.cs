using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace si_labirynt
{
    /// <summary>
    /// Interaction logic for NewLabyrinth.xaml
    /// </summary>
    public partial class NewLabyrinth : Window
    {
        public int columns { get; set; }
        public int rows { get; set; }
        public bool random { get; set; }

        public NewLabyrinth()
        {
            InitializeComponent();
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            int temp;
            int.TryParse(txtColumns.Text, out temp);
            columns = temp;
            int.TryParse(txtRows.Text, out temp);
            rows = temp;
            random = (bool)chkRand.IsChecked;

            DialogResult = true;
            Close();
        }

        private void OnAnuluj(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }
    }
}
