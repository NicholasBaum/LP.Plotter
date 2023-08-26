using SkiaSharp.Views.Desktop;
using SkiaSharp;
using System.Windows;
using System.Windows.Input;
using System.Diagnostics;
using LP.Plot.Core.Signal;
using LP.Plotter.Core.Services;
using LP.Plotter.Core;

namespace LP.Plot.Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private SignalRenderer signal;

        public MainWindow()
        {
            InitializeComponent();

            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            MouseWheel += OnMouseWheel;

            var data = new LocalDataService().LoadSignal_M();
            this.signal = new SignalRenderer(data);
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            if (!sw.IsRunning)
                sw.Start();
            sw2.Restart();
            this.signal.Render(new SkiaRenderContext(e.Surface.Canvas, e.Info.Width, e.Info.Height));
            this.DrawInfo(e.Surface.Canvas, e.Info);
            Debug.WriteLine($"Rendertime {sw2.Elapsed.TotalSeconds}:0.00");
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
                var panx = -deltaX / skiaEl.ActualWidth;
                var pany = deltaY / skiaEl.ActualHeight;
                Debug.WriteLine(panx);
                signal.XAxis.Pan(panx);
                signal.YAxis.Pan(pany);
                skiaEl.InvalidateVisual();
                lastMousePos = newMousePos;
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (IsMouseCaptured)
                ReleaseMouseCapture();
            var diff = lastMousePos - e.GetPosition(this);
        }

        private void OnMouseWheel(object sender, MouseWheelEventArgs e)
        {
            var factor = Math.Sign(e.Delta) > 0 ? 0.9 : 1.1;
            var pos = e.GetPosition(skiaEl);
            var xPos = pos.X / skiaEl.ActualWidth;
            var yPos = 1 - pos.Y / skiaEl.ActualHeight;
            signal.XAxis.ZoomAt(factor, xPos);
            signal.YAxis.ZoomAt(factor, yPos);
            //signal.XAxis.Scale(factor);
            //signal.YAxis.Scale(factor);
            skiaEl.InvalidateVisual();
        }
    }
}