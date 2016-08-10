using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Nitroh.Aayla.Dtos;
using Type = Nitroh.Aayla.Dtos.Type;

namespace Nitroh.Aayla
{
    public class CollectionCard
    {
        public Card Card { get; set; }
        public int Count { get; set; }
        public int GoldenCount { get; set; }
    }

    public class CollectionSummary : INotifyPropertyChanged
    {
        private IEnumerable<CollectionCard> _collection;

        public CollectionSummaryLine TotalLine { get; private set; }

        private readonly List<CollectionSummaryLine> _classLines;
        public IEnumerable<CollectionSummaryLine> ClassLines => _classLines;

        private readonly List<CollectionSummaryLine> _rarityLines;
        public IEnumerable<CollectionSummaryLine> RarityLines => _rarityLines;

        private readonly List<CollectionSummaryLine> _typeLines;
        public IEnumerable<CollectionSummaryLine> TypeLines => _typeLines;

        private readonly List<CollectionSummaryLine> _gameModeLines;
        public IEnumerable<CollectionSummaryLine> GameModeLines => _gameModeLines;

        private readonly List<CollectionSummaryLine> _setLines;
        public IEnumerable<CollectionSummaryLine> SetLines => _setLines;

        public CollectionSummary()
        {
            _classLines = new List<CollectionSummaryLine>();
            _rarityLines = new List<CollectionSummaryLine>();
            _typeLines = new List<CollectionSummaryLine>();
            _gameModeLines = new List<CollectionSummaryLine>();
            _setLines = new List<CollectionSummaryLine>();
        }

        public void Update(IEnumerable<CollectionCard> collection)
        {
            _collection = collection;
            Summarize();
        }

        private void Summarize()
        {
            TotalLine = new CollectionSummaryLine("All", _collection);
            OnPropertyChanged(new PropertyChangedEventArgs("TotalLine"));

            foreach (Class item in Enum.GetValues(typeof(Class)))
            {
                var cards = _collection.Where(x => x.Card.Class == item).ToList();
                var line = new CollectionSummaryLine(item.ToString(), cards);
                _classLines.Add(line);
            }
            OnPropertyChanged(new PropertyChangedEventArgs("ClassLines"));

            foreach (Rarity item in Enum.GetValues(typeof(Rarity)))
            {
                var cards = _collection.Where(x => x.Card.Rarity == item).ToList();
                var line = new CollectionSummaryLine(item.ToString(), cards);
                _rarityLines.Add(line);
            }
            OnPropertyChanged(new PropertyChangedEventArgs("RarityLines"));

            foreach (Type item in Enum.GetValues(typeof(Type)))
            {
                var cards = _collection.Where(x => x.Card.Type == item).ToList();
                var line = new CollectionSummaryLine(item.ToString(), cards);
                _typeLines.Add(line);
            }
            OnPropertyChanged(new PropertyChangedEventArgs("TypeLines"));

            foreach (GameMode item in Enum.GetValues(typeof(GameMode)))
            {
                var cards = _collection.Where(x => x.Card.GameMode == item).ToList();
                var line = new CollectionSummaryLine(item.ToString(), cards);
                _gameModeLines.Add(line);
            }
            OnPropertyChanged(new PropertyChangedEventArgs("GameModeLine"));

            foreach (Set item in Enum.GetValues(typeof(Set)))
            {
                var cards = _collection.Where(x => x.Card.Set == item).ToList();
                var line = new CollectionSummaryLine(item.ToString(), cards);
                _setLines.Add(line);
            }
            OnPropertyChanged(new PropertyChangedEventArgs("SetLines"));
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, e);
        }
    }

    public class CollectionSummaryLine
    {
        public string Name { get; set; }
        public int Total { get; set; }
        public int Count { get; set; }
        public int GoldenCount { get; set; }

        public CollectionSummaryLine(string name, IEnumerable<CollectionCard> cards)
        {
            var cardsList = cards as IList<CollectionCard> ?? cards.ToList();
            Name = name;
            Total = cardsList.Sum(x => x.Card.MaxCount);
            Count = cardsList.Sum(x => x.Count);
            GoldenCount = cardsList.Sum(x => x.GoldenCount);
        }

        public string CountPercentage => $"{(decimal)Count / Total:P2}";
        public string GoldenCountPercentage => $"{(decimal)GoldenCount / Total:P2}";
    }
}