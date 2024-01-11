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
        private double pourcentageZeroEtDeux;


        public MainWindow()
        {
           InitializeComponent();
            Init main = new Init();
            main.ShowDialog();
            CreationMatrice(matrice);
            CreationJoueur(matrice) ;
            CreationChemin(matrice);
            CreationCheminAlternatif(matrice);
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
            randX = rand.Next(mat.GetLength(0) / 4, mat.GetLength(0) / 4 * 3 - 1);
            randY = rand.Next(mat.GetLength(0) / 4, mat.GetLength(0) / 4 * 3 - 1);
            mat[randX, randY] = 4;
            joueur.X = randX;
            joueur.Y = randY; 
            Console.WriteLine($"Player x:{joueur.X} y:{joueur.Y}\n");

        }

        private void CreationChemin(int[,] mat)
        {
            bool fin = false;
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    if (mat[i, j] == 4)
                    {
                        ChoixChemin(mat, ref i, ref j);
                        if (mat[i, j] != 3)
                        {
                            for (int h = 1; h < mat.GetLength(0) / 2 + 1; h++)
                            {
                                if (mat[i, j] == 3)
                                {
                                    break;
                                }
                                else if (mat[i, j] == 1)
                                {
                                    ChoixChemin(mat, ref i, ref j);
                                }
                                if (h > mat.GetLength(0) / 2 -1)
                                {
                                    mat[i, j] = 3;
                                    fin = true;
                                    Console.WriteLine($"i:{i} j: {j}");

                                }
                            }
                        }
                    }
                    
                   
                }
                if (fin)
                {

                    break;
                }

            }
        }
        private void CalculPZED(int[,] mat, ref double pourcentageZeroEtDeux)
        {
            double nb2 = 0;
            double nb0 = 0;
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    if (mat[i, j] == 2)
                    {
                        nb2++;
                    }
                    if (mat[i, j] == 0)
                    {
                        nb0++;
                    }
                }
            }
            pourcentageZeroEtDeux = (nb2 + nb0) * 100 /((mat.GetLength(0) + 1) * (mat.GetLength(1) + 1));
        }
        private void CreationCheminAlternatif(int[,] mat)
        {
            int i,j;
            CalculPZED(mat, ref pourcentageZeroEtDeux);
            Console.WriteLine(pourcentageZeroEtDeux);
            while (pourcentageZeroEtDeux > 50)
            {
                for (i = rand.Next(0, mat.GetLength(0) - 1); i < mat.GetLength(0); i++)
                {
                    for (j = rand.Next(0, mat.GetLength(0)- 1); j < mat.GetLength(1); j++)
                    {
                        if (mat[i, j] == 1 || mat[i, j] == 4 || mat[i, j] == 5)
                        {
                            for (int k = 0; k < 10; k++)
                            {
                                if (mat[i, j] == 1 || mat[i, j] == 4 || mat[i, j] == 5)
                                {
                                    ChoixCheminAlternatif(mat, ref i, ref j);
                                }
                            }
                            CalculPZED(mat, ref pourcentageZeroEtDeux);
                            Console.WriteLine(pourcentageZeroEtDeux);
                            AfffichageMatrice(matrice);
                        }
                    }
                }
            }
        }
        private void ChoixCheminAlternatif(int[,] mat, ref int x, ref int y)
        {
            bool sans = true;
            randY = 0;
            randX = 0;
            while (mat[x + randX, y + randY] != 0)
            {
                randX = rand.Next(0, 5);
                switch (randX)
                {
                    case 0:
                        randX = 1;
                        randY = 0;
                        break;
                    case 1:
                        randX = -1;
                        randY = 0;
                        break;
                    case 2:
                        randX = 0;
                        randY = 1;
                        break;
                    case 3:
                        randX = 0;
                        randY = -1;
                        break;
                    case 4:
                        sans = false;
                        randY = 0;
                        randX = 0;
                        break;
                }
            }
            if (sans)
            {
                mat[x + randX, y + randY] = 5;
            }
            y = y + randY;
            x = x + randX;
        }
        private void ChoixChemin(int[,] mat, ref int x, ref int y)
        {
            randY = 0;
            randX = 0;
            while (mat[x + randX, y + randY] != 0)
            {
                randX = rand.Next(0, 4);
                switch (randX)
                {
                    case 0:
                        randX = 1;
                        randY = 0;
                        break;
                    case 1:
                        randX = -1;
                        randY = 0;
                        break;
                    case 2:
                        randX = 0;
                        randY = 1;
                        break;
                    case 3:
                        randX = 0;
                        randY = -1;
                        break;
                }
            }
            mat[x + randX, y + randY] = 1;
            if (mat[x + randX * 2, y + randY * 2] == 0)
            {
                mat[x + randX * 2, y + randY * 2] = 1;
            }
            else if (mat[x + randX + randY, y + randY + randX] == 0)
            {
                mat[x + randX + randY, y + randY + randX] = 1;
            }
            else
            {
                mat[x + randX, y + randY] = 3;
            }
            y = y + randY * 2;
            x = x + randX * 2;
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

