namespace RealtimeReservations
{
    public class ReservationChangedEventArgs
    {
        public ulong Counter { get; set; }
        public int ProductId { get; set; }
        public ReservationChangeType ChangeType { get; set; }
    }
}