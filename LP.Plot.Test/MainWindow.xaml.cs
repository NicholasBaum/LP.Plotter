using LP.Plot.Data;
using System.Windows;

namespace LP.Plot.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WpfPlotView plotView;

        public MainWindow()
        {
            InitializeComponent();

            var data = new LocalDataService().LoadSignal_L();
            this.plotView = new WpfPlotView(data, this, skiaEl);
        }
    }
}