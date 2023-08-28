using LP.Plot.Core.Data;
using ScottPlot.Plottable;
using System.Windows;

namespace ScottTestv4;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();

        var data = new LocalDataService().LoadSignal_M();

        foreach (var c in data)
        {
            var signal = new SignalPlot();
            var plot = WpfPlot1.Plot.AddSignal(c.YValues, 50);
        }
        WpfPlot1.Refresh();
    }
}