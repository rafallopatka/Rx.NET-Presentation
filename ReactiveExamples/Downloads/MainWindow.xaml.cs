using System;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows;
using Downloads.Annotations;
using PropertyChanged;

namespace Downloads
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            var downloadRequests = Observable
                .FromEventPattern(BtnDownload, nameof(BtnDownload.Click))
                .Select(click => TxtFileUrl.Text)
                .Select(fileUrl =>
                {
                    return new
                    {
                        FileUrl = fileUrl,
                        FileName = GetFileName(fileUrl),
                        Progress = ObservableDownload.Create(fileUrl, GetFileName(fileUrl))
                    };
                });

            var subscription = downloadRequests
                .ObserveOn(Dispatcher)
                .Subscribe(job =>
                {
                    var downloadJob = new DownloadJob { Name = job.FileUrl };

                    job.Progress.Subscribe(progress =>
                        {
                            var scopeJob = downloadJob;
                            scopeJob.Progress = progress;
                        });

                    Downloads.Items.Add(downloadJob);
                });
        }

        private static string GetFileName(string filePath)
        {
            return System.IO.Path.GetFileName(new Uri(filePath).LocalPath);
        }
    }
}


