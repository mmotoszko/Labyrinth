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
    /// Interaction logic for WindowInfo.xaml
    /// </summary>
    /// 

    public partial class WindowInfo : Window
    {
        public WindowInfo()
        {
            InitializeComponent();
        }

        private void CloseWindow(object sender, EventArgs e)
        {
            Keyboard.Focus(Owner);
            Close();
        }
    }
}
