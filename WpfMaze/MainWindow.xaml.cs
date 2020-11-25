using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfMaze.Mazegame;
using WpfMaze.MazeGame.Algorithms;
using WpfMaze.MazeGame.Space;
using WpfMaze.Utils;
using Point = System.Windows.Point;

namespace WpfMaze
{
    public partial class MainWindow
    {
        private int GameHeight = 10;
        private int GameWidth = 10;

        // private Maze Maze;

        private MazeRewrite MazeRewrite;
        private Point? MousePos;


        public MainWindow()
        {
            ConsoleAllocator.ShowConsoleWindow();
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            CreateNewGame.Click += OnCreateNewGame;
            SizeChanged += (sender, e) => ZoomToSize();
            KeyDown += MovePlayer;
            GameHeightInput.Text = Convert.ToString(GameHeight);
            GameWidthInput.Text = Convert.ToString(GameWidth);
            AlgorithmButtons();
            StackPanelEvents();
            OnCreateNewGame(null, null);
        }

        private void StackPanelEvents()
        {
            StackPanelBorder.MouseLeftButtonDown += OnBitmapLeftMouseDown;
            StackPanelBorder.MouseLeftButtonUp += OnBitmapLeftMouseUp;
            StackPanelBorder.MouseMove += OnBitmapMouseMove;
            StackPanelBorder.MouseWheel += OnBitapMouseWheel;
        }

        private void AlgorithmButtons()
        {
            WallfollowerAlgorithm.Click += (sender, e) =>
            {
                IAlgorithm algo = new Wallfollower();
                var t = new AlgorithmThread(algo);
                t.injectMaze(MazeRewrite);
                t.startThread();
            };
            RecursiveAlogrithm.Click += (sender, e) =>
            {
                IAlgorithm algo = new Recursive();
                var t = new AlgorithmThread(algo);
                t.injectMaze(MazeRewrite);
                t.startThread();
            };
        }

        private void OnCreateNewGame(object sender, EventArgs e)
        {
            GameWidth = Convert.ToInt32(GameWidthInput.Text);
            GameHeight = Convert.ToInt32(GameHeightInput.Text);
            MazeRewrite = new MazeRewrite(GameWidth, GameHeight);
            Bitmap.Source = MazeRewrite.Bitmap;
            MazeRewrite.OnSolved += (maze, objects) =>
            {
                MessageBox.Show("Labyrinth Gelöst!", "Erfolg", MessageBoxButton.OK);
            };
        }

        private void ZoomToSize()
        {
            Matrix matrix = MatrixTransform.Matrix;
            var widthOrHeight = Math.Min(StackPanelBorder.ActualWidth, StackPanelBorder.ActualHeight);
            var zoomLevel = widthOrHeight / (matrix.M11 * GameWidth);
            matrix.ScaleAtPrepend(zoomLevel, zoomLevel, 0, 0);
            matrix.OffsetX = 0;
            matrix.OffsetY = 0;
            MatrixTransform.Matrix = matrix;
        }

        private void MovePlayer(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.W:
                    MazeRewrite.MovePlayer(Direction.Up);
                    break;
                case Key.D:
                    MazeRewrite.MovePlayer(Direction.Right);
                    break;
                case Key.S:
                    MazeRewrite.MovePlayer(Direction.Down);
                    break;
                case Key.A:
                    MazeRewrite.MovePlayer(Direction.Left);
                    break;
            }
        }

        private void OnBitapMouseWheel(object sender, MouseWheelEventArgs e)
        {
            Point pos = e.GetPosition((UIElement) sender);
            Matrix matrix = MatrixTransform.Matrix;
            var scale = e.Delta > 0 ? 1.1 : 1 / 1.1;
            matrix.ScaleAt(scale, scale, pos.X, pos.Y);
            MatrixTransform.Matrix = matrix;
        }

        private void OnBitmapLeftMouseDown(object sender, MouseButtonEventArgs e)
        {
            var viewport = (UIElement) sender;
            viewport.CaptureMouse();
            MousePos = e.GetPosition(viewport);
        }

        private void OnBitmapMouseMove(object sender, MouseEventArgs e)
        {
            if (!MousePos.HasValue) return;
            Point pos = e.GetPosition((UIElement) sender);
            Matrix matrix = MatrixTransform.Matrix;
            matrix.Translate(pos.X - MousePos.Value.X, pos.Y - MousePos.Value.Y);
            MatrixTransform.Matrix = matrix;
            MousePos = pos;
        }

        private void OnBitmapLeftMouseUp(object sender, MouseButtonEventArgs e)
        {
            ((UIElement) sender).ReleaseMouseCapture();
            MousePos = null;
        }

        private void OnResizeButtonClick(object sender, RoutedEventArgs e)
        {
            ZoomToSize();
        }
    }
}