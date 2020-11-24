using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
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
        private MazeRewrite MazeRewrite;
        private System.Windows.Point? MousePos;
        public int GameHeight = 10;
        public int GameWidth = 10;

        private static class ConsoleAllocator
        {
            [DllImport(@"kernel32.dll", SetLastError = true)]
            static extern bool AllocConsole();

            [DllImport(@"kernel32.dll")]
            static extern IntPtr GetConsoleWindow();

            [DllImport(@"user32.dll")]
            static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

            const int SwHide = 0;
            const int SwShow = 5;


            public static void ShowConsoleWindow()
            {
                var handle = GetConsoleWindow();

                if (handle == IntPtr.Zero)
                {
                    AllocConsole();
                }
                else
                {
                    ShowWindow(handle, SwShow);
                }
            }

            public static void HideConsoleWindow()
            {
                var handle = GetConsoleWindow();
                ShowWindow(handle, SwHide);
            }
        }


        public MainWindow()
        {
            ConsoleAllocator.ShowConsoleWindow();
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            this.StackPanelBorder.MouseLeftButtonDown += this.onBitmapLeftMouseDown;
            this.StackPanelBorder.MouseLeftButtonUp += this.onBitmapLeftMouseUp;
            this.StackPanelBorder.MouseMove += this.onBitmapMouseMove;
            this.StackPanelBorder.MouseWheel += onBitapMouseWheel;
            this.CreateNewGame.Click += this.onCreateNewGame;
            this.SizeChanged += (sender, e) => zoomToSize();
            this.StackPanel.MouseRightButtonDown += (sender, e) => { this.Maze.paintBitmaps(); };
            this.KeyDown += this.movePlayer;
            this.GameHeightInput.Text = Convert.ToString(this.GameHeight);
            this.GameWidthInput.Text = Convert.ToString(this.GameWidth);
            this.WallfollowerAlgorithm.Click += (sender, e) =>
            {
                IAlgorithm algo = new Wallfollower();
                AlgorithmThread t = new AlgorithmThread(algo);
                t.injectMaze(this.MazeRewrite);
                t.startThread();
            };
            this.RecursiveAlogrithm.Click += (sender, e) =>
            {
                IAlgorithm algo = new Recursive();
                AlgorithmThread t = new AlgorithmThread(algo);
                t.injectMaze(this.MazeRewrite);
                t.startThread();
            };
            this.onCreateNewGame(null, null);
        }

        private void onCreateNewGame(object sender, EventArgs e)
        {
            this.GameWidth = Convert.ToInt32(this.GameWidthInput.Text);
            this.GameHeight = Convert.ToInt32(this.GameHeightInput.Text);
            this.MazeRewrite = new MazeRewrite(GameWidth, GameHeight);
            Bitmap.Source = this.MazeRewrite.Bitmap;
            this.MazeRewrite.onSolved += (maze, objects) =>
            {
                MessageBox.Show("Labyrinth Gelöst!", "Erfolg", MessageBoxButton.OK);
            };
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
            switch (e.Key)
            {
                case Key.W:
                    this.MazeRewrite.MovePlayer(Direction.Up);
                    break;
                case Key.D:
                    this.MazeRewrite.MovePlayer(Direction.Right);
                    break;
                case Key.S:
                    this.MazeRewrite.MovePlayer(Direction.Down);
                    break;
                case Key.A:
                    this.MazeRewrite.MovePlayer(Direction.Left);
                    break;
            }
        }

        private void injectMaze(Maze maze)
        {
            Bitmap.Source = maze.Bitmap;
        }

        private void onBitapMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var pos = e.GetPosition((UIElement)sender);
            var matrix = MatrixTransform.Matrix;
            var scale = e.Delta > 0 ? 1.1 : 1 / 1.1;
            matrix.ScaleAt(scale, scale, pos.X, pos.Y);
            MatrixTransform.Matrix = matrix;
        }

        private void onBitmapLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            var viewport = (UIElement)sender;
            viewport.CaptureMouse();
            MousePos = e.GetPosition(viewport);
        }

        private void onBitmapMouseMove(object sender, MouseEventArgs e)
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

        private void onBitmapLeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            ((UIElement)sender).ReleaseMouseCapture();
            MousePos = null;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.zoomToSize();
        }
    }
}