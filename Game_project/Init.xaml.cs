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

namespace Game_project
{
    /// <summary>
    /// Logique d'interaction pour Init.xaml
    /// </summary>
    public partial class Init : Window
    {
        public Init()
        {
            InitializeComponent();
        }

        private void btJouer_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
