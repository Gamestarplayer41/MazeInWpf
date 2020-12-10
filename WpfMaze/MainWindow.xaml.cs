using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using WpfMaze.Mazegame;
using WpfMaze.MazeGame.Algorithms;
using WpfMaze.Mazegame.MazeGenerationAlgorithms;
using WpfMaze.MazeGame.Space;
using WpfMaze.Utils;
using Point = System.Windows.Point;

namespace WpfMaze
{
    public partial class MainWindow
    {
        public MainWindow()
        {
            ConsoleAllocator.ShowConsoleWindow();
            InitializeComponent();
            RenderOptions.SetBitmapScalingMode(this, BitmapScalingMode.NearestNeighbor);
            AlgorithmButtons();
            StackPanelEvents();
            OnCreateNewGame(null, null);
            WindowEvents();
            MessageBox.Show("Player: WASD \nZoom to Finish: F \nReset: R", "Steuerung", MessageBoxButton.OK);
        }

        private int GameHeight { get; set; } = 10;
        private int GameWidth { get; set; } = 10;
        private MazeRewrite MazeRewrite { get; set; }
        private Point? MousePos { get; set; }

        private void WindowEvents()
        {
            CreateNewGame.Click += OnCreateNewGame;
            SizeChanged += (_, _) => ZoomCanvas();
            KeyDown += MovePlayer;
        }

        private void StackPanelEvents()
        {
            MouseEventRectangle.MouseLeftButtonDown += OnBitmapLeftMouseDown;
            MouseEventRectangle.MouseLeftButtonUp += OnBitmapLeftMouseUp;
            MouseEventRectangle.MouseMove += OnBitmapMouseMove;
            MouseEventRectangle.MouseWheel += OnBitapMouseWheel;
        }

        private void AlgorithmButtons()
        {
            WallfollowerAlgorithm.Click += (_, _) =>
            {
                AAlgorithm algo = new Wallfollower(MazeRewrite);
                var t = new AlgorithmThread(algo);
                t.StartThread();
            };
            RandomBacktrackerAlgorithm.Click += (_, _) =>
            {
                AAlgorithm algo = new RandomBacktracker(MazeRewrite);
                var t = new AlgorithmThread(algo);
                t.StartThread();
            };
            WavePropagationAlgorithm.Click += (_, _) =>
            {
                AAlgorithm algo = new WavePropagation(MazeRewrite);
                var t = new AlgorithmThread(algo);
                t.StartThread();
            };
        }

        private void OnCreateNewGame(object sender, EventArgs e)
        {
            GameSizeInputs();
            MazeRewrite = new MazeRewrite(GameWidth, GameHeight, typeof(BinaryTree));
            Bitmap.Source = MazeRewrite.Bitmap;
            MazeRewrite.OnSolved += (_, _) => { MessageBox.Show("Labyrinth Gelöst!", "Erfolg", MessageBoxButton.OK); };
        }

        private void GameSizeInputs()
        {
            GameWidth = int.TryParse(GameWidthInput.Text, out var value) ? value : GameWidth;
            GameHeight = int.TryParse(GameHeightInput.Text, out value) ? value : GameHeight;
            GameWidthInput.Text = GameWidth.ToString();
            GameHeightInput.Text = GameHeight.ToString();
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
                case Key.R:
                    OnCreateNewGame(null, null);
                    ZoomCanvas();
                    return;
                case Key.F:
                    ZoomCanvas(MazeRewrite.Finish.X, MazeRewrite.Finish.Y);
                    return;
                default:
                    return;
            }

            ZoomToPlayer();
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
            ZoomCanvas();
        }

        private void OpenDevToolWindowButtonClick(object sender, RoutedEventArgs routedEventArgs)
        {
            Window devtool = new DevControlWindow();
            devtool.Show();
        }

        public void ZoomToPlayer()
        {
            ZoomCanvas(MazeRewrite.Player.X, MazeRewrite.Player.Y);
        }

        private void ZoomCanvas(double? x = null, double? y = null)
        {
            Matrix matrix = MatrixTransform.Matrix;
            matrix.M11 = 1;
            matrix.M22 = 1;
            var scale = Math.Min(BitmapCanvas.ActualWidth / MazeRewrite.Width,
                BitmapCanvas.ActualHeight / MazeRewrite.Height);
            if (x != null && y != null)
            {
                scale *= MazeRewrite.Width / 10;
                matrix.Scale(scale, scale);
                matrix.OffsetX = (double) (BitmapCanvas.ActualWidth / 2 - scale / 2 - x * scale);
                matrix.OffsetY = (double) (BitmapCanvas.ActualHeight / 2 - scale / 2 - y * scale);
            }
            else
            {
                matrix.Scale(scale, scale);
                matrix.OffsetX = 0;
                matrix.OffsetY = 0;
            }


            MatrixTransform.Matrix = matrix;
        }
    }
}