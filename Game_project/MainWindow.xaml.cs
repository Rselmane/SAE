using System;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Game_project
{
    public partial class MainWindow : Window
    {
        private int[,] matrice = new int[20, 20];
        private Random rand = new Random();
        private int randX;
        private int randY;
        private double pourcentageZeroEtDeux;
        private bool sortieEstPlace = false;
        private bool playerEstPlace = false;
        private Point Joueur = new Point();
        private Point positionLaPlusEloignee = new Point();
        private int maxDistance = 0;

        public MainWindow()
        {
            InitializeComponent();
            Init main = new Init();
            main.ShowDialog();

            while (!sortieEstPlace && !playerEstPlace)
            {
                CreationMatrice(matrice);
                CreationJoueur(matrice);
                CreationChemin(matrice);
            }

            CreationCheminAlternatif(matrice);
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

            if (!playerEstPlace)
            {

                 randY = 0;
                 randX = 0;
                randX = rand.Next(mat.GetLength(0) / 4, mat.GetLength(0) / 4 * 3 - 1);
                randY = rand.Next(mat.GetLength(0) / 4, mat.GetLength(0) / 4 * 3 - 1);
                Joueur.X = randX;
                Joueur.Y = randY;

                mat[randX, randY] = 4;
                Console.WriteLine($"Player x:{Joueur.X} y:{Joueur.Y}\n");
                playerEstPlace = true;
            }
            
            
        }

        private void CreationChemin(int[,] mat)
        {
            // Réinitialiser les variables pour le placement de la sortie
            maxDistance = 0;
            positionLaPlusEloignee = new Point(0, 0);

            // Trouve la position du joueur et commencer  l'algorithle DFS
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    if (mat[i, j] == 4) // 4 représente le joueur
                    {
                        ChoixChemin(mat, ref i, ref j);
                        break; // Sort de la boucle une fois que le chemin est créé
                    }
                }
            }

            // Placez la sortie à la position la plus éloignée trouvée par DFS
            if (!sortieEstPlace)
            {
                mat[(int)positionLaPlusEloignee.X, (int)positionLaPlusEloignee.Y] = 3;
                sortieEstPlace = true;
                Console.WriteLine($"Sortie x:{positionLaPlusEloignee.X} y:{positionLaPlusEloignee.Y}");
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

                if (mat[i, j] == 0 && mat[i,j] !=4 && mat[i,j] !=3)
                {
                    mat[i, j] = 5;
                    CalculPZED(mat, ref pourcentageZeroEtDeux);
                }
               
            }
        }
        // Implementation de l'algo  Depth-First Search, DFS
        private void DFS(int[,] mat, int x, int y, int distance)
        {
            if (mat[x, y] != 4)
            {
                mat[x, y] = 1; // Marque la position actuelle comme le chemin principal
            }

            // Met à jour la position la plus éloignée pour la sortie
            if (distance > maxDistance)
            {
                maxDistance = distance;
                positionLaPlusEloignee = new Point(x, y);
            }

            // Directions possibles : haut, bas, gauche, droite
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };
            DirectionsAleatoire(dx, dy);

            for (int i = 0; i < 4; i++)
            {
                int newX = x + dx[i] * 2;
                int newY = y + dy[i] * 2;

                // Vérifiez si la nouvelle position est valide et non visitée
                if (EstValide(mat, newX, newY))
                {
                    // Créez un chemin entre les cases, sauf si ça écrase le joueur
                    if (mat[x + dx[i], y + dy[i]] != 4)
                    {
                        mat[x + dx[i], y + dy[i]] = 1;
                    }
                    DFS(mat, newX, newY, distance + 1);
                }
            }
        }


        private void ChoixChemin(int[,] mat, ref int x, ref int y)
        {
            DFS(mat, x, y, 0);
            mat[x, y] = 4; // Remarque à nouveau la position du joueur 
        }

        private void DFS(int[,] mat, int x, int y)
        {
            // Directions possibles : haut, bas, gauche, droite
            int[] dx = { -1, 1, 0, 0 };
            int[] dy = { 0, 0, -1, 1 };
            DirectionsAleatoire(dx, dy); // Mélange les directions aléatoirement

            for (int i = 0; i < 4; i++)
            {
                int newX = x + dx[i] * 2;
                int newY = y + dy[i] * 2;

                // Vérifie si la nouvelle position est valide et non visitée
                if (EstValide(mat, newX, newY))
                {
                    mat[x + dx[i], y + dy[i]] = 1; // Créer  un chemin entre les cellules
                    mat[newX, newY] = 1;
                    DFS(mat, newX, newY);
                }
            }
        }

        private void DirectionsAleatoire(int[] dx, int[] dy)
        {
            for (int i = 3; i > 0; i--)
            {
                int j = rand.Next(i + 1);
                EchangeValeursMatrice(ref dx[i], ref dx[j]);
                EchangeValeursMatrice(ref dy[i], ref dy[j]);
            }
        }

        private void EchangeValeursMatrice(ref int a, ref int b)
        {
            int temp = a;
            a = b;
            b = temp;
        }

        private bool EstValide(int[,] mat, int x, int y)
        {
            // Vérifiez si x et y est à l'intérieur de la grille, est un mur et pas une bordure
            return x > 0 && x < mat.GetLength(0) - 1 && y > 0 && y < mat.GetLength(1) - 1 && mat[x, y] == 0;
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
       
        
            
        
    

    

