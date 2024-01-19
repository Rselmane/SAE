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
    /// Logique d'interaction pour GameOver.xaml
    /// </summary>
    public partial class GameOver : Window
    {
        public GameOver()
        {
            InitializeComponent();
        }

        private void bt_menu_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
            MainWindow init = new MainWindow();
            init.ShowDialog();
        }

        private void bt_reesayer_Click(object sender, RoutedEventArgs e)
        {
            MainWindow init = new MainWindow();
            this.Close();
        }
    }
}
