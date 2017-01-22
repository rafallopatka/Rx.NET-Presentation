using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reactive.Disposables;
using System.Reactive.Linq;

namespace Downloads
{
    public static class ObservableDownload
    {
        public static IObservable<int> Create(string fileUrl, string filePath)
        {
            return Observable.Create<int>(observer =>
            {
                var webClient = new WebClient();

                DownloadProgressChangedEventHandler downloadProgressChangedHandler = (sender, args) =>
                {
                    observer.OnNext(args.ProgressPercentage);
                };
                
                DownloadDataCompletedEventHandler downloadCompletedHandler = (sender, args) =>
                {
                    File.WriteAllBytes(filePath, args.Result);

                    observer.OnCompleted();
                };
                try
                {
                    webClient.DownloadProgressChanged += downloadProgressChangedHandler;
                    webClient.DownloadDataCompleted += downloadCompletedHandler;
                    webClient.DownloadDataAsync(new Uri(fileUrl));
                }
                catch (Exception ex)
                {
                    observer.OnError(ex);
                }

                return Disposable.Create(() =>
                {
                    if (webClient.IsBusy)
                        webClient.CancelAsync();

                    webClient.Dispose();

                    webClient.DownloadProgressChanged -= downloadProgressChangedHandler;
                    webClient.DownloadDataCompleted -= downloadCompletedHandler;
                });
            });
        }
    }
}