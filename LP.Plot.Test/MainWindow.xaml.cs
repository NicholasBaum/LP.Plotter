﻿using LP.Plot.Core.Data;
using System.Windows;

namespace LP.Plot.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private WpfPlot signal;

        public MainWindow()
        {
            InitializeComponent();

            var data = new LocalDataService().LoadSignal_M();
            this.signal = new WpfPlot(data, this, skiaEl);
        }
    }
}