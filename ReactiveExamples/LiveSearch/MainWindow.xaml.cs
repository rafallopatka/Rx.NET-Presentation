using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Threading.Tasks;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace LiveSearch
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : MetroWindow
    {
        private readonly IDisposable _textChangedSubscription;
        private IDisposable _searchingSubscription;

        public MainWindow()
        {
            InitializeComponent();

            Closing += OnClosing;

            var textChangedStream = Observable
                .FromEventPattern<TextChangedEventArgs>(SearchBox, nameof(SearchBox.TextChanged))
                .Select(e => (e.Sender as TextBox)?.Text)
                .Where(searchPhrase => string.IsNullOrEmpty(searchPhrase) == false && searchPhrase.Length > 3);

            _textChangedSubscription = textChangedStream
                .Throttle(TimeSpan.FromSeconds(1))
                .Select(searchPhrase => SearchAsync(searchPhrase).ToObservable())
                .Switch()
                .ObserveOn(Dispatcher)
                .Subscribe(ShowSearchResults, ShowErrorMessage);

            _searchingSubscription = textChangedStream
                .ObserveOn(Dispatcher)
                .Subscribe(x =>
                {
                    ProgressIndicator.Visibility = Visibility.Visible;
                });
        }

        private void ShowErrorMessage(Exception e)
        {
            MessageBox.Show(this, $"Error occured: {e}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        private void ShowSearchResults(List<string> list)
        {
            ProgressIndicator.Visibility = Visibility.Collapsed;
            SearchResults.Items.Clear();
            list.ForEach(item => SearchResults.Items.Add(item));
        }

        private async Task<List<string>> SearchAsync(string searchPhrase)
        {
            var results = Enumerable.Range(0, 100)
                .Select(x => $"{x} {searchPhrase} {Thread.CurrentThread.ManagedThreadId}")
                .ToList();

            await Task.Delay(TimeSpan.FromSeconds(3));

            return results;
        }

        private void OnClosing(object sender, CancelEventArgs cancelEventArgs)
        {
            _textChangedSubscription.Dispose();
            _searchingSubscription.Dispose();
        }
    }
}
