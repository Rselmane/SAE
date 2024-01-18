﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Numerics;
using System.Windows;
using System.Windows.Controls;
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
    public partial class MainWindow : Window
    {
        private int[,] matrice = new int[20, 20];
        private Random rand = new Random();
        private int randX;
        private int randY;
        private int randIndex;
        private double pourcentageZeroEtDeux;
        private bool sortieEstPlace = false;
        private bool playerEstPlace = false;
        private bool goLeft, goRight,goUp,goDown,solutionAffiche = false;
        private bool aGagne = false;
        private double playerSpeed = 20;
        private Point Joueur = new Point();
        private Point positionLaPlusEloignee = new Point();
        private Point chemins = new Point();
        private Point positionDepart = new Point();

        private int maxDistance = 0;
        private Rect collisionJoueur = new Rect();
        private List<Point> ListChemins = new List<Point>();
        private List<Ennemies> ListEnnemies = new List<Ennemies>();
        private DispatcherTimer dispatcherTimer = new DispatcherTimer();
        // classe de pinceau d'image que nous utiliserons comme image du joueur appelée skin du joueur
        private ImageBrush background = new ImageBrush();
        private ImageBrush wall = new ImageBrush();
        private ImageBrush murExterieur = new ImageBrush();
        private ImageBrush tbox = new ImageBrush();
        private ImageBrush Player = new ImageBrush();
        private ImageBrush Enemies = new ImageBrush();

        public MainWindow()
        {
            InitializeComponent();
            Init main = new Init();
            main.ShowDialog();
            LabyrinthCanvas.Focus();
            // rafraissement toutes les 16 milliseconds
            dispatcherTimer.Tick += GameEngine; 
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(64);
            // lancement du timer
            dispatcherTimer.Start();

            // chargement de l’image du joueur 
            background.ImageSource = new BitmapImage(new
            Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/Background.jpg"));
            wall.ImageSource = new BitmapImage(new
            Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/wall.jpg"));
            murExterieur.ImageSource = new BitmapImage(new
            Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/wall2.jpg"));
            tbox.ImageSource = new BitmapImage(new
            Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/tbox.jpg"));
            Player.ImageSource = new BitmapImage(new
            Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/head.png"));
            Enemies.ImageSource = new BitmapImage(new
            Uri(AppDomain.CurrentDomain.BaseDirectory + "Images/ennemie.jpg"));
            // assignement de skin du joueur au rectangle associé
            this.Background = background;

            CreationMatrice(matrice);
            CreationJoueur(matrice);
            CreationChemin(matrice);
            CreationCheminAlternatif(matrice);
            CreationEnnemy(matrice, 5);
            AugmenteDix(matrice);
            //CreateShapes();
            AffichageMatrice(matrice);
            //ChangeLaLuminosité();
            //Moveplayer();


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
                randY = 0;
                randX = 0;
                randX = rand.Next(mat.GetLength(0) / 4, mat.GetLength(0) / 4 * 3 - 1);
                randY = rand.Next(mat.GetLength(0) / 4, mat.GetLength(0) / 4 * 3 - 1);
                positionDepart.X = randX;
                positionDepart.Y = randY;
                Joueur.X = randX;
                Joueur.Y = randX;

            mat[randX, randY] = 4;

            Console.WriteLine($"Player x:{positionDepart.X} y:{positionDepart.Y}\n");
            


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

        private void AugmenteDix(int[,] mat)
        {
            for (int i = 0; i < mat.GetLength(0); i++)
            {
                for (int j = 0; j < mat.GetLength(1); j++)
                {
                    if (mat[i, j] != 4) // 4 représente le joueur
                    {
                        mat[i, j] += 10; // Sort de la boucle une fois que tous est noir
                    }
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

                if (mat[i, j] == 0 && mat[i, j] != 4 && mat[i, j] != 3)
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
                mat[x, y] = 1;
                chemins.X = x;
                chemins.Y = y;
                ListChemins.Add(chemins);

            }

            // Met à jour la position la plus éloignée pour la sortie
            if (distance > maxDistance)
            {
                maxDistance = distance;
                positionLaPlusEloignee = new Point(x, y);
            }

            // Directions possibles : haut, bas, gauche, droite
            int[] directionX = { -1, 1, 0, 0 };
            int[] directionY = { 0, 0, -1, 1 };
            DirectionsAleatoire(directionX, directionY);

            for (int i = 0; i < 4; i++)
            {
                int nouveauX = x + directionX[i] * 2;
                int nouveauxY = y + directionY[i] * 2;

                // Vérifiez si la nouvelle position est valide et non visitée
                if (EstValide(mat, nouveauX, nouveauxY))
                {
                    // Créez un chemin entre les cases, sauf si ça écrase le joueur
                    if (mat[x + directionX[i], y + directionY[i]] != 4)
                    {
                        mat[x + directionX[i], y + directionY[i]] = 1;
                        chemins.X = x + directionX[i];
                        chemins.Y = y + directionY[i];
                        ListChemins.Add(chemins);


                    }
                    DFS(mat, nouveauX, nouveauxY, distance + 1);
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
                    chemins.X = x + dx[i];
                    chemins.Y = y + dy[i];
                    ListChemins.Add(chemins);

                    mat[newX, newY] = 1;
                    chemins.X = newX;
                    chemins.Y = newY;
                    ListChemins.Add(chemins);
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
        private void CreationEnnemy(int[,] mat, int nbennemy)
        {
            for (int i = 0; i < nbennemy; i++)
            {
               // randIndex = rand.Next(0, ListChemins.Count - 1);
                //mat[(int)ListChemins[randIndex].X, (int)ListChemins[randIndex].Y]  = 6;

               // ListChemins.RemoveAt(randIndex);
              //  Console.WriteLine(ListChemins.Count);
               // ListEnnemies.Add(new Ennemies("fantomes", (int)ListChemins[randIndex].X, (int)ListChemins[randIndex].Y));
            }

        }


        private void Revelation(int[,] mat, int joueurX, int joueurY)
        {
            // Définit la portée de la révélation autour du joueur
            int portee = 1;

            // Parcour les cases autour de la position du joueur
            for (int i = joueurX - portee; i <= joueurX + portee; i++)
            {
                for (int j = joueurY - portee; j <= joueurY + portee; j++)
                {
                    // Vérifier si la case est à l'intérieur de la grille
                    if (i >= 0 && i < mat.GetLength(0) && j >= 0 && j < mat.GetLength(1))
                    {
                        // Révéle la case si elle est actuellement dans l'obscurité
                        if (mat[i, j] >= 10)
                        {
                            mat[i, j] -= 10;
                        }
                    }
                }
            }
        }

        private void CreateShapes()
        {
            // Efface tous les éléments existants dans le Canvas
           LabyrinthCanvas.Children.Clear();

            // Définit la largeur et la hauteur du Canvas en fonction de la taille de la matrice
            LabyrinthCanvas.Width = matrice.GetLength(1) * 20;
            LabyrinthCanvas.Height = matrice.GetLength(0) * 20;

            // Parcour la matrice et créer les formes en fonction des valeurs
            for (int i = 0; i < matrice.GetLength(0); i++)
            {
                for (int j = 0; j < matrice.GetLength(1); j++)
                {
                    Shape shape = null;
                    if (matrice[i, j] < 0)
                    {
                        matrice[i, j] += 10;
                    }

                    switch (matrice[i, j])
                    {
                        case 0: // Remplacez par un rectangle blanc
                            shape = new Rectangle
                            {
                                Tag = "murInterieur",
                                Width = 20,
                                Height = 20,
                                Fill = wall
                            };
                            break;
                        case 1: // Remplacez par un rectangle blanc
                            shape = new Rectangle
                            {
                                Tag = "CheminPrincipal",
                                Width = 20,
                                Height = 20,
                                Fill = Brushes.White
                            };
                            break;
                        case 2: // Remplacez par un rectangle noir
                            shape = new Rectangle
                            {
                                Tag = "MurExterieure",
                                Width = 20,
                                Height = 20,
                                Fill = murExterieur
                            };
                            break;
                        case 3: // Remplacez par un carré jaune
                            shape = new Rectangle
                            {
                                Tag = "tresore",
                                Width = 20,
                                Height = 20,
                                Fill = tbox
                            };
                            break;
                        case 4: // Remplacez par un rectangle rouge
                            shape = new Rectangle
                            {
                                Tag = "player",
                                Width = 20,
                                Height = 20,
                                Fill = Player
                            };
                            
                            break;
                        case 5: // Remplacez par un rectangle blanc
                            shape = new Rectangle
                            {
                                Tag = "cheminSecondaire",
                                Width = 20,
                                Height = 20,
                                Fill = Brushes.White
                            };
                            break;
                        case 6:
                            shape = new Rectangle()
                            {
                                Tag = "Enemy",
                                Width = 20,
                                Height = 20,
                                Fill = Enemies
                            };
                            break;
                        case 9: // Remplacez par un rectangle blanc
                            shape = new Rectangle
                            {
                                Tag = "cheminSecondaire",
                                Width = 20,
                                Height = 20,
                                Fill = Brushes.White
                            };
                            break;
                        default:
                            break;
                    }
                    if (matrice[i, j] > 9)
                    {
                        shape = new Rectangle()
                        {
                            Tag = "Tenebre",
                            Width = 20,
                            Height = 20,
                            Fill = Brushes.Black
                        };

                    }

                    if (shape != null)
                    {
                        // Positionne  la forme dans le Canvas
                        Canvas.SetLeft(shape, j * 20); // Ajuste la position X
                        Canvas.SetTop(shape, i * 20); // Ajuste la position Y

                        // Ajoute la forme au Canvas
                        LabyrinthCanvas.Children.Add(shape);

                    }
                }
            }
        }

        private void ShowSolution()
        {
            if (solutionAffiche)
            {
                for (int i = 0; i < matrice.GetLength(0); i++)
                {
                    for (int j = 0; j < matrice.GetLength(1); j++)
                    {

                        if (matrice[i, j] < 10 && matrice[i,j] != 4)
                        {
                            matrice[i, j] += 10;
                        }



                    }
                }
                solutionAffiche = false;
            }
            else
            {

                for (int i = 0; i < matrice.GetLength(0); i++)
                {
                    for (int j = 0; j < matrice.GetLength(1); j++)
                    {

                        if (matrice[i, j] >= 10 && matrice[i, j] != 4)
                        {
                            matrice[i, j] -= 10;
                        }



                    }
                }
                solutionAffiche = true;
            }


            CreateShapes();
        }





        private void MovePlayerLeftRight()
        {
            Rectangle player = LabyrinthCanvas.Children.OfType<Rectangle>().FirstOrDefault(r => r.Tag.ToString() == "player");
            double currentX = Canvas.GetLeft(player);
            double currentY = Canvas.GetTop(player);

            double newX = currentX;
            double newY = currentY;

            if (goLeft || goRight)
            {
                if (goLeft)
                {
                    newX -= 20; //
                }
                else if (goRight)
                {
                    newX += 20;
                }
            }
            else if (goUp || goDown)
            {
                if (goUp)
                {
                    newY -= 20; 
                }
                else if (goDown)
                {
                    newY += 20;
                }
            }

            
            int arrayX = (int)(newX / 20); 
            int arrayY = (int)(newY / 20); 

            if (arrayX >= 0 && arrayX < matrice.GetLength(1) && arrayY >= 0 && arrayY < matrice.GetLength(0))
            {
                if (matrice[arrayY, arrayX] == 13 || matrice[arrayY, arrayX] == 3)
                {
                    aGagne = true;
                }

                if (matrice[arrayY, arrayX] == 11 || matrice[arrayY, arrayX] == 1 || matrice[arrayY, arrayX] == 15 || matrice[arrayY, arrayX] == 5)
                {
                    if (goUp || goDown || goRight || goLeft)
                    {
                        matrice[(int)(currentY / 20), (int)(currentX / 20)] = 11;

                        Canvas.SetLeft(player, newX);
                        Canvas.SetTop(player, newY);

                        matrice[arrayY, arrayX] = 4;
                        Revelation(matrice, arrayY, arrayX);
                        AffichageMatrice(matrice);
                        CreateShapes();
                    }
                }
            }

          
        }

        private void LabyrinthCanvas_Loaded(object sender, RoutedEventArgs e)
        {
            CreateShapes();
        }


        private void LabyrinthCanvas_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = true;
            }
            if (e.Key == Key.Right)
            {
                goRight = true;
            }
            if(e.Key == Key.Up)
            {
                goUp = true;
            }
            if(e.Key==Key.Down) 
            { 
                goDown = true; 
            
            }
            if(e.Key == Key.F1)
            {
                ShowSolution();
            }

        }

        private void LabyrinthCanvas_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Left)
            {
                goLeft = false;
            }
            if (e.Key == Key.Right)
            {
                goRight = false;
            }
            if( e.Key == Key.Up)
            {
                goUp = false;
            }
            if(e.Key == Key.Down)
            {
                goDown = false;
            }
        }

        private void GameEngine(object sender, EventArgs e)
        {
            MovePlayerLeftRight();
            CheckWin();
        }
        /* private  void  ChangeLaLuminosité()
         {
             foreach(Rectangle x in LabyrinthCanvas.Children.OfType<Rectangle>())
             {
                 if( (string)x.Tag != "player")
                 {
                     x.Fill = Brushes.Black;
                 }

             }
         }
     */
        private void CheckWin()
        {

            if(aGagne)
            {
                MessageBox.Show("Gagné !!", "Fin de partie", MessageBoxButton.OK,MessageBoxImage.Exclamation);
                Application.Current.Shutdown();
            }
        }
    }
    
}
       
       
        
            
        
    

    

