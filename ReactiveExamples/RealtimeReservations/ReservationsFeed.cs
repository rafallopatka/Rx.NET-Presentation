using System;
using System.Threading.Tasks;

namespace RealtimeReservations
{
    public class ReservationsFeed
    {
        private bool _isRunning;

        public void StartAsync()
        {
            Task.Run(() => Start());
        }

        private void Start()
        {
            _isRunning = true;
            ulong counter = 0;

            Random rnd = new Random();
            while (_isRunning)
            {
                ReservationChanged?.Invoke(this, new ReservationChangedEventArgs
                {
                    Counter = counter++,
                    ProductId = rnd.Next(3000),
                    ChangeType = rnd.Next(1000) % 2 == 0 ? ReservationChangeType.Released : ReservationChangeType.Reserved
                });

                Task.Delay(rnd.Next(20)).Wait();
            }
        }

        public void Stop()
        {
            _isRunning = false;
        }

        public event EventHandler<ReservationChangedEventArgs> ReservationChanged;
    }
}