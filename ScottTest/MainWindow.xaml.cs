using LP.Plotter.Core.Services;
using OxyPlot.Axes;
using SkiaSharp;
using System.Diagnostics;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ScottTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private List<(double[] yvalues, Axis YAxis)> channels = new();
        private Axis XAxis = new();

        public MainWindow()
        {
            InitializeComponent();


            double[] dataX = new double[] { 1, 2, 3, 4, 5 };
            double[] dataY = new double[] { 1, 4, 9, 16, 25 };

            var data = new LocalDataService().LoadTest();

            foreach (var c in data.Channels.Where(x => x.Name.Contains("Speed") || x.Name.Contains("TT")))
            {
                var yValues = c.Points.Select(x => x.Y).ToArray();
                var yaxis = new Axis() { Min = (float)yValues.Min(), Max = (float)yValues.Max() }.Scale(1.1f);
                this.channels.Add((yValues, yaxis));
                Debug.WriteLine($"{c.Name} {c.Points.Count} {c.Points.Count / c.Points.Last().X}");
            }
            var min = data.SpeedChannel.Points.First().X;
            var max = data.SpeedChannel.Points.Last().X;
            this.XAxis = new Axis() { Min = (float)min, Max = (float)max };

            foreach (var c in channels.Skip(0))
            {
                var plot = WpfPlot1.Plot.Add.Signal(c.yvalues, 50);
            }
            WpfPlot1.Refresh();


        }

        private class Axis
        {
            public float Min { get; set; }
            public float Max { get; set; }
            public float Length => Max - Min;
            public Axis Scale(float s) => new Axis() { Min = Min - (s - 1) * Length, Max = Max + (s - 1) * Length };
        }
    }
}