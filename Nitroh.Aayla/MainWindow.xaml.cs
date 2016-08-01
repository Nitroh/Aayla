using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using Nitroh.Mono;

namespace Nitroh.Aayla
{
    //TODO: Lots of test code
    public partial class MainWindow
    {
        private MonoExecutable _hearthstone;

        public MainWindow()
        {
            InitializeComponent();
            Task.Factory.StartNew(UpdateAsync);
        }

        public async Task UpdateAsync()
        {
            if(_hearthstone == null) _hearthstone = new MonoExecutable("Hearthstone");
            var counter = 0;
            while (true)
            {
                var test = _hearthstone.GetTest();
                await UpdateLabelOutputAsync($"ADDR: {test}. COUNTER: {counter}");
                await Task.Delay(100);
                counter++;
                if (counter == 1000) break;
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
