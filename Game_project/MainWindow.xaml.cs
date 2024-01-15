using System;
using System.Collections.Generic;
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
        private bool goLeft, goRight = false;
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
            dispatcherTimer.Interval = TimeSpan.FromMilliseconds(16);
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

            while (!sortieEstPlace && !playerEstPlace)
            {
                CreationMatrice(matrice);
                CreationJoueur(matrice);
                CreationChemin(matrice);
            }

            CreationCheminAlternatif(matrice);
            AffichageMatrice(matrice);
            CreationEnnemy(matrice, 5);
            CreateShapes();
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

            if (!playerEstPlace)
            {

                randY = 0;
                randX = 0;
                randX = rand.Next(mat.GetLength(0) / 4, mat.GetLength(0) / 4 * 3 - 1);
                randY = rand.Next(mat.GetLength(0) / 4, mat.GetLength(0) / 4 * 3 - 1);
                positionDepart.X = randX;
                positionDepart.Y = randY;

                mat[randX, randY] = 4;
               Joueur.X = positionDepart.X;
               Joueur.Y = positionDepart.Y;

                Console.WriteLine($"Player x:{positionDepart.X} y:{positionDepart.Y}\n");
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
                randIndex = rand.Next(0, ListChemins.Count - 1);
                mat[(int)ListChemins[randIndex].X, (int)ListChemins[randIndex].Y]  = 6;

                ListChemins.RemoveAt(randIndex);
                Console.WriteLine(ListChemins.Count);
                ListEnnemies.Add(new Ennemies("fantomes", (int)ListChemins[randIndex].X, (int)ListChemins[randIndex].Y));
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


        private void MovePlayerLeftRight()
        {
            int newX = (int)Joueur.X;

            if (goLeft || goRight)
            {
                if (goLeft)
                {
                    newX -= 1;
                }
                else if (goRight)
                {
                    newX += 1;
                }

                // Ensure newX is within the horizontal bounds of the matrix
                if (newX >= 0 && newX < matrice.GetLength(1) &&
                    (matrice[(int)Joueur.Y, newX] == 0 || matrice[(int)Joueur.Y, newX] == 1 || matrice[(int)Joueur.Y, newX] == 5))
                {
                    // Reset the original position in the matrix
                    matrice[(int)Joueur.Y, (int)Joueur.X] = 1; // Assuming 1 is the original value

                    // Set the new position in the matrix
                    Joueur.X = newX;
                    matrice[(int)Joueur.Y, newX] = 4; // 4 represents the player

                    // Update the player's position on the canvas
                    Rectangle player = LabyrinthCanvas.Children.OfType<Rectangle>().FirstOrDefault(r => r.Tag.ToString() == "player");
                    if (player != null)
                    {
                        Canvas.SetLeft(player, newX * 20); // Adjusting position based on tile size
                    }

                    Console.WriteLine($"Joueur x : {Joueur.X}  y: {Joueur.Y}");

                    Console.WriteLine("---------------------------------");
                    AffichageMatrice(matrice);
                    Console.WriteLine("---------------------------------");
                }
            }
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
        }

        private void GameEngine(object sender, EventArgs e)
        {
            MovePlayerLeftRight();

         

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

    }
}
       
       
        
            
        
    

    

