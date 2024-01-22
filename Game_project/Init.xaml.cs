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
using System.Windows.Threading;

namespace Game_project
{
    /// <summary>
    /// Logique d'interaction pour Init.xaml
    /// </summary>
    public partial class Init : Window
    {
        MediaPlayer musiqueSelection = new MediaPlayer();
        DispatcherTimer minMusique = new DispatcherTimer();
        private bool jouer = false;
        public Init()
        {
            InitializeComponent();
            musiqueSelection.Open(new
             Uri(AppDomain.CurrentDomain.BaseDirectory + "Sons/selection1.wav"));

            // Commence la lecture
            musiqueSelection.Play();
            minMusique.Interval = TimeSpan.FromSeconds(1);  // Vérifier la position toutes les secondes
            minMusique.Tick += minMusiqueTick;
            minMusique.Start();

        }

        private void btJouer_Click(object sender, RoutedEventArgs e)
        {
            jouer = true;
            DialogResult = true;
            minMusique.Stop();
            musiqueSelection.Stop();
            ((MainWindow)Application.Current.MainWindow).diffculte = cb_diffculte.SelectedIndex; 
        }
        private void minMusiqueTick(object sender, EventArgs e)
        {
            if (musiqueSelection.Position >= TimeSpan.FromSeconds(192))  // 2 minutes et 20 secondes
            {
                musiqueSelection.Position = TimeSpan.FromSeconds(32);
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            if (jouer == false)
            {
                Application.Current.Shutdown();
            }
           
        }

        private void btCredit_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Developper : Rayan Selmane TD3 ", "Lumi-Labyrinthe", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
