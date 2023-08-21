using SkiaSharp.Views.Desktop;
using SkiaSharp;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using LP.Plotter.Core.Drawing;
using LP.Plotter.Core.Services;

namespace LP.Plot.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            Loaded += MainWindow_Loaded;

            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            var data = new LocalDataService().LoadTest();
            Debug.WriteLine($"SampleCount {data.SpeedChannel.Points.Count}");
            drawer = new SignalPlotter(data);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private SignalPlotter drawer;

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (!sw.IsRunning)
                sw.Start();
            sw2.Restart();
            this.drawer.Draw(e.Surface.Canvas, e.Info);
            this.DrawInfo(e.Surface.Canvas, e.Info);
            System.Diagnostics.Debug.WriteLine($"Rendertime {sw2.Elapsed.TotalSeconds}:0.00");
        }

        private void DrawInfo(SKCanvas canvas, SKImageInfo imageInfo)
        {
            using var black = new SKPaint
            {
                Color = SKColors.Black,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Left,
            };
            using var white = new SKPaint
            {
                Color = SKColors.White,
                IsAntialias = true,
                Style = SKPaintStyle.Fill,
                TextAlign = SKTextAlign.Left,
                TextSize = 20,
            };
            using var font = new SKFont
            {
                Size = 24
            };

            var coord = new SKPoint(imageInfo.Width + 200, imageInfo.Height + 200);
            canvas.DrawRect(0, 0, 100, 100, black);

            var text = $"Frames {frameCount++}";
            SKRect bounds = new SKRect();
            white.MeasureText(text, ref bounds);
            canvas.DrawText(text, 0, 1.5f * bounds.Height, font, white);
            text = $"Fps {(frameCount / (double)sw.Elapsed.TotalSeconds):0.00}";

            white.MeasureText(text, ref bounds);
            canvas.DrawText(text, 0, bounds.Height * 2.5f, font, white);
        }

        private int frameCount = 0;
        private Stopwatch sw = new Stopwatch();
        private Stopwatch sw2 = new Stopwatch();


        private Point lastMousePos;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            frameCount = 0;
            sw.Restart();
            lastMousePos = e.GetPosition(this);
            CaptureMouse(); // Capture mouse events to track movement outside the control
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (IsMouseCaptured)
            {
                Point newMousePos = e.GetPosition(this);
                double deltaX = newMousePos.X - lastMousePos.X;
                double deltaY = newMousePos.Y - lastMousePos.Y;

                // Update the content's position (e.g., Canvas' Left and Top properties)
                // based on deltaX and deltaY

                lastMousePos = newMousePos;
                skiaEl.InvalidateVisual();

            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
                ReleaseMouseCapture();
            var diff = lastMousePos - e.GetPosition(this);
        }
    }
}