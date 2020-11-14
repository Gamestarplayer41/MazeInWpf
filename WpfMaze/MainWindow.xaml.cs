using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Threading;
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
        private Maze Maze;
        private System.Windows.Point? MousePos;
        public int GameHeight = 10;
        public int GameWidth = 10;
        public AlgorithmManager AlgorithmManager = new AlgorithmManager();
        
        public MainWindow()
        {
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            this.StackPanelBorder.MouseLeftButtonDown += this.onBitmapLeftMouseDown;
            this.StackPanelBorder.MouseLeftButtonUp += this.onBitmapLeftMouseUp;
            this.StackPanelBorder.MouseMove += this.onBitmapMouseMove;
            this.StackPanelBorder.MouseWheel += onBitapMouseWheel;
            this.CreateNewGame.Click += this.onCreateNewGame;
            this.SizeChanged += (sender, e) => zoomToSize();
            this.StackPanel.MouseRightButtonDown += (sender, e) => { this.Maze.paintBitmaps(); };
            this.KeyUp += this.movePlayer;
            this.GameHeightInput.Text = Convert.ToString(this.GameHeight);
            this.GameWidthInput.Text = Convert.ToString(this.GameWidth);
            this.onCreateNewGame(null, null);

            foreach (var algorithm in AlgorithmManager.Alorithms)
            {
                Button b = new Button();
                b.Content = algorithm;
                b.Click += (sender, e) =>
                {
                    AlgorithmManager.injectMaze(algorithm,this.Maze);
                    AlgorithmManager.assignThread(algorithm);
                    AlgorithmManager.startThread(algorithm);
                };
                b.Margin = new Thickness(){Top = 20,Left = 20,Right = 20,Bottom = 20};
                this.SolvingButtons.Children.Add(b);
            }
            
            Button stopAllThreads = new Button();
            stopAllThreads.Content = "STOP";
            stopAllThreads.Click += (sender, e) =>
            {
                AlgorithmManager
            };
        }

        private void onCreateNewGame(object sender, EventArgs e)
        {
            this.GameWidth = Convert.ToInt32(this.GameWidthInput.Text);
            this.GameHeight = Convert.ToInt32(this.GameHeightInput.Text);
            this.Maze = new Maze(GameWidth, GameHeight, true);
            Maze.paintBitmaps();
            this.injectMaze(this.Maze);
            Maze.OnMazeSolved += (maze) => MessageBox.Show("Labyrinth Gelöst", "Erfolg", MessageBoxButton.OK);
        }

        private void zoomToSize()
        {
            var matrix = MatrixTransform.Matrix;
            double widthOrHeight = Math.Min(this.StackPanelBorder.ActualWidth, this.StackPanelBorder.ActualHeight);
            double zoomLevel = widthOrHeight / (matrix.M11 * this.GameWidth);
            matrix.ScaleAtPrepend(zoomLevel, zoomLevel, 0, 0);
            matrix.OffsetX = 0;
            matrix.OffsetY = 0;
            MatrixTransform.Matrix = matrix;
        }

        private void movePlayer(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.W)
            {
                this.Maze.MovePlayer(Direction.Up);
            }
            else if (e.Key == Key.D)
            {
                this.Maze.MovePlayer(Direction.Right);
            }
            else if (e.Key == Key.S)
            {
                this.Maze.MovePlayer(Direction.Down);
            }
            else if (e.Key == Key.A)
            {
                this.Maze.MovePlayer(Direction.Left);
            }
        }

        private void injectMaze(Maze maze)
        {
            Bitmap.Source = maze.Bitmap;
        }

        private void onBitapMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var pos = e.GetPosition((UIElement) sender);
            var matrix = MatrixTransform.Matrix;
            var scale = e.Delta > 0 ? 1.1 : 1 / 1.1;
            matrix.ScaleAt(scale, scale, pos.X, pos.Y);
            MatrixTransform.Matrix = matrix;
        }

        private void onBitmapLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            var viewport = (UIElement) sender;
            viewport.CaptureMouse();
            MousePos = e.GetPosition(viewport);
        }

        private void onBitmapMouseMove(object sender, MouseEventArgs e)
        {
            if (MousePos.HasValue)
            {
                var pos = e.GetPosition((UIElement) sender);
                var matrix = MatrixTransform.Matrix;
                matrix.Translate(pos.X - MousePos.Value.X, pos.Y - MousePos.Value.Y);
                MatrixTransform.Matrix = matrix;
                MousePos = pos;
            }
        }

        private void onBitmapLeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            ((UIElement) sender).ReleaseMouseCapture();
            MousePos = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.zoomToSize();
        }
    }
}