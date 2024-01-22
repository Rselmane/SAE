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
        DispatcherTimer timer = new DispatcherTimer();
        private bool jouer = false;
        public Init()
        {
            InitializeComponent();
            musiqueSelection.Open(new
             Uri(AppDomain.CurrentDomain.BaseDirectory + "Sons/selection1.wav"));

            // Commence la lecture
            musiqueSelection.Play();
            timer.Interval = TimeSpan.FromSeconds(1);  // Vérifier la position toutes les secondes
            timer.Tick += Timer_Tick;
            timer.Start();

        }

        private void btJouer_Click(object sender, RoutedEventArgs e)
        {
            jouer = true;
            DialogResult = true;
            timer.Stop();
            musiqueSelection.Stop();
            ((MainWindow)Application.Current.MainWindow).diffculte = cb_diffculte.SelectedIndex; 
        }
        private void Timer_Tick(object sender, EventArgs e)
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
            MessageBox.Show("Developper : Rayan Selmane TD3 ", "Fin de partie", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
