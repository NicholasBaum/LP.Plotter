using LP.Plot.Core.Data;
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

            var data = new LocalDataService().LoadSignal_XL();
            data = SciChartDemo.LoadPoints();
            this.plotView = new WpfPlotView(data, this, skiaEl);
        }
    }
}