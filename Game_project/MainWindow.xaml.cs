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

namespace Game_project
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int[,] labyrinthe;
        private int largeur, hauteur;
        private Random alea = new Random();
        public MainWindow()
        {
           InitializeComponent();
            GeneationLabyrinthe(300, 300);
            DrawMaze();

        }
  
        public void GeneationLabyrinthe(int largeur, int hauteur)
        {
            labyrinthe = new int[largeur, hauteur];
            for (int y = 0; y < hauteur; y++)
            {
                for (int x = 0; x < largeur; x++)
                {
                    labyrinthe[y, x] = 0; // Initialise tous les murs
                }
            }

            // Commence à partir d'une position aléatoire
            DFS(alea.Next(hauteur), alea.Next(largeur));
        }
        private void DFS(int y, int x)
        {
            // Directions possibles à déplacer: haut, bas, gauche, droite
            int[] dx = { 0, 0, -1, 1 };
            int[] dy = { -1, 1, 0, 0 };
            int[] directions = { 0, 1, 2, 3 };
            Shuffle(directions); // Mélange les directions

            // Marque la cellule actuelle comme un chemin
            labyrinthe[y, x] = 1;

            // Explore les voisins
            for (int i = 0; i < directions.Length; i++)
            {
                int nx = x + dx[directions[i]] * 2;
                int ny = y + dy[directions[i]] * 2;

                if (IsInBounds(ny, nx) && labyrinthe[ny, nx] == 0)
                {
                    // Enlève le mur entre la cellule actuelle et la cellule voisine
                    labyrinthe[ny - dy[directions[i]], nx - dx[directions[i]]] = 1;
                    DFS(ny, nx); // Récursivement visite la cellule voisine
                }
            }
        }
            private bool IsInBounds(int y, int x)
            {
                return x >= 0 && y >= 0 && x < largeur && y < hauteur;
            }

            private void Shuffle(int[] array)
            {
                for (int i = array.Length - 1; i > 0; i--)
                {
                    int j = alea.Next(i + 1);
                    int temp = array[i];
                    array[i] = array[j];
                    array[j] = temp;
                }
            }
        public void DrawMaze()
        {
            LabyrinthCanvas.Children.Clear();

            for (int y = 0; y < 30; y++)
            {
                for (int x = 0; x < 30; x++)
                {
                    if (labyrinthe[y, x] == 0) // Si c'est un mur
                    {
                        Rectangle rect = new Rectangle
                        {
                            Width = 30,
                            Height = 30,
                            Fill = Brushes.Black
                        };
                        Canvas.SetLeft(rect, x * 30);
                        Canvas.SetTop(rect, y * 30);
                        LabyrinthCanvas.Children.Add(rect);
                    }
                }
            }
        }

        }

}

