using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
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

        public async Task UpdateAsync()
        {
            if(_hearthstone == null) _hearthstone = new HearthstoneExecutable();
            var counter = 0;
            var sb = new StringBuilder();
            while (true)
            {
                _hearthstone.Update();
                sb.Clear();
                sb.AppendLine($"RUNNING: {_hearthstone.Running}");
                if (_hearthstone.Running)
                {
                    sb.AppendLine($"GAME TYPE: {_hearthstone.GameManager.GameType}");
                    sb.AppendLine($"FORMAT TYPE: {_hearthstone.GameManager.FormatType}");
                    sb.AppendLine($"SPECTATING: {_hearthstone.GameManager.Spectating}");
                }
                //sb.AppendLine($"COUNTER: {counter}");
                await UpdateLabelOutputAsync(sb.ToString());
                await Task.Delay(100);
                counter++;
                if (counter == 9999) break;
            }
        }

        private async Task UpdateLabelOutputAsync(string text)
        {
            await Dispatcher.BeginInvoke(DispatcherPriority.Normal, (ThreadStart)delegate {
                LabelOutput.Content = text;
            });
        }
    }
}
