using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Nitroh.Aayla.Hearthstone.Data;
using Nitroh.Mono.Hearthstone;

namespace Nitroh.Aayla
{
    //TODO: Lots of test code
    public partial class MainWindow
    {
        private HearthstoneExecutable _hearthstone;

        public MainWindow()
        {
            InitializeComponent();
            Task.Factory.StartNew(UpdateAsync);
        }

        public CollectionSummary CollectionSummary { get; } = new CollectionSummary();

        public async Task UpdateAsync()
        {
            if(_hearthstone == null) _hearthstone = new HearthstoneExecutable();
            var counter = 0;
            var sb = new StringBuilder();
            while (true)
            {
                _hearthstone.Update();
                await UpdateLabelAsync(LabelRunning, $"Running: {_hearthstone.Running}");
                sb.Clear();
                //sb.AppendLine($"RUNNING: {_hearthstone.Running}");
                if (_hearthstone.Running)
                {
                    sb.AppendLine($"GAME TYPE: {_hearthstone.GameManager.GameType}");
                    sb.AppendLine($"FORMAT TYPE: {_hearthstone.GameManager.FormatType}");
                    sb.AppendLine($"SPECTATING: {_hearthstone.GameManager.Spectating}");
                }
                await UpdateLabelAsync(LabelOutput, sb.ToString());
                await Task.Delay(100);
                counter++;
                if (counter == 9999) break;
            }
        }

        private async Task UpdateLabelAsync(ContentControl label, string text)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate {
                label.Content = text;
            });
        }

        private void ButtonNavHome_OnClick(object sender, RoutedEventArgs e)
        {
            StackPanelMain.Visibility = Visibility.Visible;
            StackPanelCollection.Visibility = Visibility.Hidden;
        }

        private void ButtonNavCollection_OnClick(object sender, RoutedEventArgs e)
        {
            StackPanelMain.Visibility = Visibility.Hidden;
            StackPanelCollection.Visibility = Visibility.Visible;
        }

        private void ButtonCollectionUpdate_OnClick(object sender, RoutedEventArgs e)
        {
            if (_hearthstone == null || !_hearthstone.Running) return;

            var cardRepository = new CardRepository();
            var cards = cardRepository.GetAllCards().ToList();
            var collection = new List<CollectionCard>();
            cards.ForEach(x => collection.Add(new CollectionCard { Card = x, Count = 0, GoldenCount = 0}));

            var cardStacks = _hearthstone.NetCache.Map.Collection.CardStacks;
            foreach (var stack in cardStacks)
            {
                var id = stack.CardDefinition.Name;
                var isGolden = stack.CardDefinition.Premium == 1;

                var collectionIndex = collection.FindIndex(x => x.Card.Id == id);
                var count = isGolden ? collection[collectionIndex].Count : stack.Count;
                var goldenCount = isGolden ? stack.Count : collection[collectionIndex].GoldenCount;
                collection[collectionIndex] = new CollectionCard { Card = collection[collectionIndex].Card, Count = count, GoldenCount = goldenCount };
            }

            CollectionSummary.Update(collection);
        }
    }
}
