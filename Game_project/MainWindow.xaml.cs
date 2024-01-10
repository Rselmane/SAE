using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Game_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int[,] matrice = new int[20, 20];
        private Random rand = new Random();
        private int randX;
        private int randY;
        private int sortieX;
        private int sortieY;
        private int cheminX;
        private int cheminY;
        Point joueur = new Point(); 
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();


        public MainWindow()
        {
           InitializeComponent();
            Init main = new Init();
            main.ShowDialog();
            CreationMatrice(matrice);
            CreationJoueur(matrice) ;
            CreationChemin(matrice);
            AfffichageMatrice(matrice);
            // Création de la boucle de Jeu
           //  dispatcherTimer.Tick += GameEngine; 
            // rafraissement toutes les 16 milliseconds
            //dispatcherTimer.Interval = TimeSpan.FromMilliseconds(16);
            // lancement du timer
            //dispatcherTimer.Start();


        }
  

        private  void CreationMatrice(int[,] tab) 
        {
            
            for (int i = 0; i < tab.GetLength(0); i++)
            {
                for (int j = 0; j < tab.GetLength(1); j++)
                {
                    tab[i, 0] = 2;
                    tab[0, i] = 2;
                    tab[i, tab.GetLength(0) - 1] = 2;
                    tab[tab.GetLength(0) - 1, i] = 2;
                  
                  


                }
               
            }
            
        }
        private void CreationJoueur(int[,] mat)
        {
            randX = rand.Next(1, mat.GetLength(0) - 1);
            randY = rand.Next(1, mat.GetLength(0) - 1);
            mat[randX, randY] = 4;
            joueur.X = randX;
            joueur.Y = randY; 
            Console.WriteLine($"Player x:{joueur.X} y:{joueur.Y}\n");

        }

        private void CreationChemin(int[,] mat)
        {

            
        }

        private static void ChoixChemin(int[,] mat)
        {

        }

            private static void AfffichageMatrice(int[,] mat)
        {
            for (int i = 0; i < mat.GetLength(0); i++)
            {
               
                    Console.Write("(");
                    for (int j = 0; j < mat.GetLength(1); j++)
                        Console.Write($"{mat[i, j]},");
                    Console.WriteLine(")");


                







            }
           

        }
        private void GameEngine(object sender, EventArgs e)
        {


        }

    }

        

}

