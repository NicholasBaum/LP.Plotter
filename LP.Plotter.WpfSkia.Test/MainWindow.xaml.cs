using LP.Plotter.Core.Models;
using LP.Plotter.Core.Services;
using OxyPlot;
using OxyPlot.SkiaSharp.Wpf;
using System.Windows;

namespace LP.Plotter.WpfSkia.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
            Loaded += MainWindow_Initialized;
        }

        private void MainWindow_Initialized(object? sender, EventArgs e)
        {
            Init();
        }

        public PlotModel OxyModel { get; set; } = new PlotModel();
        public ChannelPlotModel Model { get; set; } = new ChannelPlotModel();
        private LocalDataService dataService = new LocalDataService();
        private void Init()
        {
            var data = dataService.LoadTest();
            Model.Add(data);
            Model.Draw(OxyModel);
            OxyModel.InvalidatePlot(true);
        }

        private void ResetButton_Click(object sender, RoutedEventArgs e)
        {
            OxyModel.ResetAllAxes();
            OxyModel.InvalidatePlot(true);
        }
    }
}