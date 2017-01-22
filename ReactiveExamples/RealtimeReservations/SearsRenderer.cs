using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace RealtimeReservations
{
    public class SearsRenderer : FrameworkElement
    {
        private readonly VisualCollection _visuals;
        private readonly Dictionary<int, bool> _states;

        private readonly int _maxX;
        private readonly int _maxY;
        private readonly int _maxId;
        
        public SearsRenderer()
        {
            _visuals = new VisualCollection(this);
            _states = new Dictionary<int, bool>();

            _maxX = 58;
            _maxY = 50;
            _maxId = _maxX * _maxY;

            InitStates();
        }

        private void InitStates()
        {
            int id = 0;
            for (int x = 0; x < _maxX; x++)
            {
                for (int y = 0; y < _maxY; y++)
                {
                    _states[id++] = false;
                }
            }
        }

        public void UpdateState(params SeatState[] reservationStates)
        {
            foreach (var newState in reservationStates)
                _states[newState.Id] = newState.IsReserved;

            Render();
        }

        public void Render()
        {
            _visuals.Clear();
            DrawingVisual visual = new DrawingVisual();
            using (DrawingContext dc = visual.RenderOpen())
            {
                int id = 0;
                for (int x = 0; x < _maxX; x++)
                {
                    for (int y = 0; y < _maxY; y++)
                    {
                        var b = _states[id++] ? Brushes.DeepSkyBlue : Brushes.White;

                        dc.DrawEllipse(b, new Pen(Brushes.Black, 1), new Point(x * 20 + 10, y * 20 + 10), 10, 10 );

                        if (id == _maxId)
                            id = 0;
                    }
                }
            }

            _visuals.Add(visual);
        }

        protected override Visual GetVisualChild(int index)
        {
            return _visuals[index];
        }

        protected override int VisualChildrenCount
        {
            get
            {
                return _visuals.Count;
            }
        }
    }
    public struct SeatState
    {
        public int Id { get; set; }
        public bool IsReserved { get; set; }
    }
}
