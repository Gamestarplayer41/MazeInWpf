using System;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfMaze.Mazegame;
using WpfMaze.MazeGame.Algorithms;

namespace WpfMaze
{
    public partial class MainWindow : Window
    {
        private Maze maze;
        private System.Windows.Point? MousePos;
        public int GameHeight = 1000;
        public int GameWidth = 1000;

        public MainWindow()
        {
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(this,BitmapScalingMode.NearestNeighbor);
            maze = new Maze(GameWidth, GameHeight, true);
            maze.paintBitmaps();
            this.injectMaze(maze);
            this.MouseLeftButtonDown += this.left_MouseDown;
            this.MouseLeftButtonUp += this.mouseUp;
            this.MouseMove += this.canvas_MouseMove;
            this.MouseWheel += transForm;
            this.MouseRightButtonDown += (sender,e)=>maze.paintBitmaps(true);
            maze.OnMazeSolved +=(maze)=> MessageBox.Show("Maze Solved", "Success", MessageBoxButton.OK);
            this.KeyUp += this.movePlayer;
        }
        private void movePlayer(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.W)
            {
                this.maze.MovePlayer(Direction.Up);
            }else if(e.Key == Key.D)
            {
                this.maze.MovePlayer(Direction.Right);
            }else if(e.Key == Key.S)
            {
                this.maze.MovePlayer(Direction.Down);
            }else if (e.Key == Key.A)
            {
                this.maze.MovePlayer(Direction.Left);
            }else if(e.Key == Key.X)
            {
                Task.Run(() =>
                {
                    Wallfollower w = new Wallfollower(maze);
                    w.SolveMaze();
                });
            }
        }

        private void injectMaze(Maze maze)
        {
            Bitmap.Source = maze.Bitmap;

        }

        private void transForm(object sender, MouseWheelEventArgs e)
        {
            var pos = e.GetPosition((UIElement)sender);
            var matrix = MatrixTransform.Matrix;
            var scale = e.Delta > 0 ? 1.1 : 1 / 1.1;
            matrix.ScaleAt(scale, scale, pos.X, pos.Y);
            MatrixTransform.Matrix = matrix;
        }

        private void left_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var viewport = (UIElement)sender;
            viewport.CaptureMouse();
            MousePos = e.GetPosition(viewport);
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (MousePos.HasValue)
            {
                var pos = e.GetPosition((UIElement)sender);
                var matrix = MatrixTransform.Matrix;
                matrix.Translate(pos.X - MousePos.Value.X, pos.Y - MousePos.Value.Y);
                MatrixTransform.Matrix = matrix;
                MousePos = pos;
            }
        }

        private void mouseUp(object sender, MouseButtonEventArgs e)
        {
            ((UIElement)sender).ReleaseMouseCapture();
            MousePos = null;
        }
    }
}