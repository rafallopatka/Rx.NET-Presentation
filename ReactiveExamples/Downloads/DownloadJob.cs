using PropertyChanged;

namespace Downloads
{
    [ImplementPropertyChanged]
    public class DownloadJob
    {
        public int Progress { get; set; }
        public string Name { get; set; }
    }
}