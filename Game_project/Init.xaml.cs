﻿using System;
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
            DialogResult = true;
            timer.Stop();
            musiqueSelection.Stop();
        }
        private void Timer_Tick(object sender, EventArgs e)
        {
            if (musiqueSelection.Position >= TimeSpan.FromSeconds(192))  // 2 minutes et 20 secondes
            {
                musiqueSelection.Position = TimeSpan.FromSeconds(32);
            }
        }
    }
}
