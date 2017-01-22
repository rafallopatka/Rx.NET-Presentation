using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Shapes;

namespace _01_MouseMove
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var window = this;
            Observable
                .FromEventPattern<MouseEventArgs>(window, nameof(MouseMoveContainer.MouseMove))
                .Select(signal => new
                {
                    Position = signal.EventArgs.GetPosition(MouseMoveContainer),
                    IsPressed = signal.EventArgs.LeftButton == MouseButtonState.Pressed
                })
                .Buffer(count: 2, skip: 1)
                .Select(buffer => new
                {
                    Previous = buffer.First().Position,
                    Current = buffer.Last().Position,
                    IsPressed = buffer.Last().IsPressed
                })
                .Where(x => x.IsPressed)
                .Subscribe(points => 
                {
                    Draw(points.Previous, points.Current);
                });
        }

        private void Draw(Point previous, Point current)
        {
            Line line = new Line
            {
                Stroke = SystemColors.WindowFrameBrush,
                X1 = previous.X,
                Y1 = previous.Y,
                X2 = current.X,
                Y2 = current.Y
            };

            MouseMoveContainer.Children.Add(line);
        }
    }
}
