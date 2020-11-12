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

namespace WpfMaze
{
    public partial class MainWindow : Window
    {
        private Maze maze;
        private System.Windows.Point StartCursorPosition;
        private System.Windows.Point PanelPosition;

        public MainWindow()
        {
            InitializeComponent();
            maze = new Maze(1000, 1000, true, 1);
            maze.paintBitmaps();
            this.injectMaze(maze);
            this.StackPanel.MouseLeftButtonDown += this.left_MouseDown;
            this.StackPanel.MouseLeftButtonUp += this.mouseUp;
            this.StackPanel.MouseMove += this.canvas_MouseMove;
            this.MouseWheel += transForm;
        }


        private void injectMaze(Maze maze)
        {
            Bitmap.Source = maze.Bitmaps[0].Bitmap;

        }

        private void transForm(object sender, MouseWheelEventArgs e)
        {
            System.Windows.Media.Matrix m = this.StackPanel.RenderTransform.Value;

            if (e.Delta > 0)
            {
                m.ScaleAt(
                    1.5,
                    1.5,
                    e.GetPosition(this).X,
                    e.GetPosition(this).Y);
            }
            else
            {
                m.ScaleAt(
                    1.0 / 1.5,
                    1.0 / 1.5,
                    e.GetPosition(this).X,
                    e.GetPosition(this).Y);
            }
            this.StackPanel.RenderTransform = new MatrixTransform(m);
        }

        private void left_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var tt = this.StackPanel.RenderTransform.Value;
            StartCursorPosition = e.GetPosition(this);
            PanelPosition = new System.Windows.Point(tt.OffsetX, tt.OffsetY);
            this.StackPanel.CaptureMouse();
        }

        private void canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (this.StackPanel.IsMouseCaptured)
            {
                var matrix = this.StackPanel.RenderTransform.Value;
                Vector strecke = StartCursorPosition - e.GetPosition(this);
                double OffsetX = PanelPosition.X - strecke.X;
                double OffsetY = PanelPosition.Y - strecke.Y;
                matrix.Translate(OffsetX, OffsetY);
                this.StackPanel.RenderTransform = new MatrixTransform(matrix);
            }
        }

        private void mouseUp(object sender, MouseButtonEventArgs e)
        {
            this.StackPanel.ReleaseMouseCapture();
        }
    }
}