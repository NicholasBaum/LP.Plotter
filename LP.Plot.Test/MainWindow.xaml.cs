using SkiaSharp.Views.Desktop;
using System.Windows;
using System.Diagnostics;
using LP.Plotter.Core.Services;
using LP.Plot.Core;

namespace LP.Plot.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Core.Plot signal;

        public MainWindow()
        {
            InitializeComponent();

            var data = new LocalDataService().LoadSignal_M();
            this.signal = WpfPlot.CreateSignal(data, this, skiaEl);
        }
    }
}