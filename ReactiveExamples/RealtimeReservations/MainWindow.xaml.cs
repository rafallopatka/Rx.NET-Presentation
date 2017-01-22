using System;
using System.Linq;
using System.Reactive.Linq;
using System.Windows;

namespace RealtimeReservations
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();

            Renderer.Render();
            var feed = new ReservationsFeed();
            feed.StartAsync();

            var feedObservable = Observable
                .FromEventPattern<ReservationChangedEventArgs>(feed, nameof(feed.ReservationChanged))
                .Select(x => new SeatState
                {
                    Id = x.EventArgs.ProductId,
                    IsReserved = x.EventArgs.ChangeType == ReservationChangeType.Reserved
                });

            //var basicSubscription = feedObservable
            //    .ObserveOn(Dispatcher)
            //    .Subscribe(seats =>
            //    {
            //        Renderer.UpdateState(seats);
            //    });


            // buforowanie sygnałów
            var bufferedObservable = feedObservable
                .Buffer(TimeSpan.FromMilliseconds(1000))
                .Where(x => x.Any());


            // aktualizacja stanu krzesełek i rendering
            bufferedObservable
                .ObserveOn(Dispatcher)
                .Subscribe(seats =>
                {
                    Renderer.UpdateState(seats.ToArray());
                });


            // statystyki per bufor
            bufferedObservable
                .Select(x => new
                {
                    Reservations = x.Count(s => s.IsReserved),
                    Releases = x.Count(s => s.IsReserved == false),
                    Total = x.Count
                })
                .ObserveOn(Dispatcher)
                .Subscribe(statistics =>
                {
                    TxtReservationsCount.Text = statistics.Reservations.ToString();
                    TxtReleaseCount.Text = statistics.Releases.ToString();
                    TxtSignalsCount.Text = statistics.Total.ToString();
                });


            // statystyki w czasie rzeczywistym
            var realTime = feedObservable
                .Select((x, index) => new
                {
                    Reserved = x.IsReserved ? 1 : 0,
                    Released = x.IsReserved ? 0 : 1,
                })
                .Scan(new
                {
                    Reserved = 0.0,
                    Released = 0.0,
                    Count = 1,
                }, (prev, current) => new
                {
                    Reserved = prev.Reserved + current.Reserved,
                    Released = prev.Released + current.Released,
                    Count = prev.Count + 1
                })
                .ObserveOnDispatcher()
                .Subscribe(x =>
                {
                    TxtRealTimeCount.Text = x.Count.ToString();
                    TxtRealTimeReleasedCount.Text = x.Released.ToString();
                    TxtRealTimeReservedCount.Text = x.Reserved.ToString();
                    TxtRealTimeReservedAverage.Text = (x.Reserved / x.Count * 100).ToString("##");
                    TxtRealTimeResleasedAverage.Text = (x.Released / x.Count * 100).ToString("##");
                });

        }
    }
}
