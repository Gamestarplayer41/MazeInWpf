using System;
using System.Linq;
using System.Windows;

namespace WpfMaze
{
    public partial class DevControlWindow : Window
    {
        public DevControlWindow()
        {
            InitializeComponent();
        }

        private void ZoomToPlayer_OnClick(object sender, RoutedEventArgs e)
        {
            foreach (Window window in Application.Current.Windows)
            {
                if (window.GetType() == typeof(MainWindow))
                {
                    (window as MainWindow).ZoomToPlayer();
                }
            }
        }
    }
}