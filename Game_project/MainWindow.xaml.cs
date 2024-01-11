using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Game_project
{
    public partial class MainWindow : Window
    {
        private int[,] matrice = new int[40, 40];
        private Random rand = new Random();
        private int randX;
        private int randY;
        private double pourcentageZeroEtDeux;

        public MainWindow()
        {
            InitializeComponent();
            Init main = new Init();
            main.ShowDialog();
            CreationMatrice(matrice);
            CreationJoueur(matrice);
            CreationChemin(matrice);
            CreationCheminAlternatif(matrice);
            AffichageMatrice(matrice);
            CreateShapes();
        }

        private void CreationMatrice(int[,] tab)
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
            Console.WriteLine($"Player x:{randX} y:{randY}\n");
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
                                if (mat[i, j] == 3 || mat[i,j] == 4)
                                {
                                    continue;
                                }
                                else if (mat[i, j] == 1)
                                {
                                    ChoixChemin(mat, ref i, ref j);
                                }
                                if (h > mat.GetLength(0) / 2 - 1)
                                {
                                    mat[i, j] = 3;
                                    fin = true;
                                    Console.WriteLine($"Sortie x:{i} y:{j}");
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
            pourcentageZeroEtDeux = (nb2 + nb0) * 100 / ((mat.GetLength(0) + 1) * (mat.GetLength(1) + 1));
        }

        private void CreationCheminAlternatif(int[,] mat)
        {
            CalculPZED(mat, ref pourcentageZeroEtDeux);
            while (pourcentageZeroEtDeux > 50)
            {
                int i = rand.Next(1, mat.GetLength(0) - 1);
                int j = rand.Next(1, mat.GetLength(1) - 1);

                if (mat[i, j] == 0)
                {
                    mat[i, j] = 5;
                    CalculPZED(mat, ref pourcentageZeroEtDeux);
                }
            }
        }

        private void ChoixChemin(int[,] mat, ref int x, ref int y)
        {
            int randY = 0;
            int randX = 0;
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
                mat[x + randX, y + randY] = 3; // Utilisez 3 pour représenter le chemin alternatif
            }
            y = y + randY * 2;
            x = x + randX * 2;
        }


        private static void AffichageMatrice(int[,] mat)
        {
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                Console.Write("(");
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    Console.Write($"{mat[i, j]},");
                }
                Console.WriteLine(")");
            }
        }
        private void CreateShapes()
        {
            // Parcourez la matrice et créez les formes en fonction des valeurs
            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                for (int j = 0; j < matrice.GetLength(1); j++)
                {
                    Shape shape = null;

                    switch (matrice[i, j])
                    {
                        case 0: // Remplacez par un rectangle blanc
                            shape = new Rectangle
                            {
                                Width = 20,
                                Height = 20,
                                Fill = Brushes.Black
                            };
                            break;
                        case 1: // Remplacez par un rectangle blanc
                            shape = new Rectangle
                            {
                                Width = 20,
                                Height = 20,
                                Fill = Brushes.White
                            };
                            break;
                        case 2: // Remplacez par un rectangle noir
                            shape = new Rectangle
                            {
                                Width = 20,
                                Height = 20,
                                Fill = Brushes.Black
                            };
                            break;
                        case 3: // Remplacez par un carré jaune
                            shape = new Rectangle
                            {
                                Width = 20,
                                Height = 20,
                                Fill = Brushes.Yellow
                            };
                            break;
                        case 4: // Remplacez par un rectangle rouge
                            shape = new Rectangle
                            {
                                Width = 20,
                                Height = 20,
                                Fill = Brushes.Red
                            };
                            break;
                        case 5: // Remplacez par un rectangle blanc
                            shape = new Rectangle
                            {
                                Width = 20,
                                Height = 20,
                                Fill = Brushes.White
                            };
                            break;
                        default:
                            break;
                    }

                    if (shape != null)
                    {
                        // Positionnez la forme dans le Canvas
                        Canvas.SetLeft(shape, j * 20); // Ajustez la position X
                        Canvas.SetTop(shape, i * 20); // Ajustez la position Y

                        // Ajoutez la forme au Canvas
                        LabyrinthCanvas.Children.Add(shape);
                    }
                }
            }
        }
                }
}
       
        
            
        
    

    

