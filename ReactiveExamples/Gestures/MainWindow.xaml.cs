using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Gestures
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDisposable _dragAndDropSubscription;
        private readonly IDisposable _shakeGestureSubscription;
        private readonly IDisposable _handleRotationSubscription;

        public MainWindow()
        {
            InitializeComponent();

            Closing += OnClosing;

            _handleRotationSubscription = HandleRotationGesture();
            _dragAndDropSubscription = HandleDragAndDrop();
            _shakeGestureSubscription = HandleShakeGesture();
        }

        private IDisposable HandleRotationGesture()
        {
            var rotation = new RotateTransform();
            Rectangle.RenderTransform = rotation;

            var mouseWheel = Observable
                .FromEventPattern<MouseWheelEventArgs>(Rectangle, nameof(Rectangle.MouseWheel))
                .Where(x => x.EventArgs.RightButton == MouseButtonState.Pressed)
                .Select(x => x.EventArgs.Delta);

            return mouseWheel
                .ObserveOn(Dispatcher)
                .Subscribe(degree =>
                {
                    rotation.CenterX = Rectangle.Width / 2;
                    rotation.CenterY = Rectangle.Height / 2;
                    rotation.Angle += degree;
                });
        }

        private IDisposable HandleDragAndDrop()
        {
            var mouseDown = Observable
                .FromEventPattern<MouseButtonEventArgs>(Rectangle, nameof(Rectangle.MouseLeftButtonDown))
                .Select(x => x.EventArgs.GetPosition(Rectangle));

            var mouseUp = Observable.FromEventPattern<MouseButtonEventArgs>(this, nameof(Rectangle.MouseLeftButtonUp));

            var mouseMove = Observable.FromEventPattern<MouseEventArgs>(this, nameof(Rectangle.MouseMove))
                .Select(x => x.EventArgs.GetPosition(this));

            var dragAndDrop = from startLocation in mouseDown
                              from endLocation in mouseMove.TakeUntil(mouseUp)
                              select new
                              {
                                  X = endLocation.X - startLocation.X,
                                  Y = endLocation.Y - startLocation.Y
                              };

            //var dragAndDropLambda = mouseDown.SelectMany(startLocation => mouseMove.TakeUntil(mouseUp),
            //    (startLocation, endLocation) => new
            //    {
            //        X = endLocation.X - startLocation.X,
            //        Y = endLocation.Y - startLocation.Y
            //    });

            return dragAndDrop
                .ObserveOn(Dispatcher)
                .Subscribe(position =>
                {
                    Canvas.SetTop(Rectangle, position.Y);
                    Canvas.SetLeft(Rectangle, position.X);
                });
        }

        private IDisposable HandleShakeGesture()
        {
            const int shakeTreshold = 3;
            var mouseMovements = Observable
                .FromEventPattern<MouseEventArgs>(Rectangle, nameof(Rectangle.MouseMove))
                .Where(e => e.EventArgs.LeftButton == MouseButtonState.Pressed)
                .Select(e => e.EventArgs.GetPosition(this))
                .Buffer(2, 1)
                .Select(ComputeDelta)
                .Buffer(TimeSpan.FromMilliseconds(150))
                .Select(CountDirectionChanges)
                .Where(directionChanges => IsShaking(directionChanges, shakeTreshold))
                .Throttle(TimeSpan.FromMilliseconds(500));

            return mouseMovements
                .ObserveOn(Dispatcher)
                .Subscribe(move =>
                {
                    Rectangle.Width += 10;
                    Rectangle.Height += 10;
                });
        }

        private static bool IsShaking(DirectionsCount changes, int shakeTreshold)
        {
            return changes.NegativeXChangesCount > shakeTreshold &&
                   changes.PositiveXChangesCount > shakeTreshold &&
                   changes.NegativeYChangesCount > shakeTreshold &&
                   changes.PositiveYChangesCount > shakeTreshold;
        }

        private static PositionChange ComputeDelta(IList<Point> positions)
        {
            return new PositionChange
            {
                DeltaX = positions.Last().X - positions.First().X,
                DeltaY = positions.Last().Y - positions.First().Y,
            };
        }

        private static DirectionsCount CountDirectionChanges(IList<PositionChange> moves)
        {
            return new DirectionsCount
            {
                NegativeXChangesCount = moves.Count(s => s.DeltaX < 0),
                PositiveXChangesCount = moves.Count(s => s.DeltaX > 0),
                NegativeYChangesCount = moves.Count(s => s.DeltaY < 0),
                PositiveYChangesCount = moves.Count(s => s.DeltaY > 0)
            };
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            _handleRotationSubscription.Dispose();
            _dragAndDropSubscription.Dispose();
            _shakeGestureSubscription.Dispose();
        }
    }
}
