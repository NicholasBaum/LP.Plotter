using SkiaSharp.Views.Desktop;
using System.Windows;
using System.Windows.Input;
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
        private RenderInfo renderInfo = new();
        private Stopwatch frameTimer = new Stopwatch();


        public MainWindow()
        {
            InitializeComponent();

            MouseDown += OnMouseDown;
            MouseMove += OnMouseMove;
            MouseUp += OnMouseUp;
            MouseWheel += OnMouseWheel;

            var data = new LocalDataService().LoadSignal_M();
            this.signal = Core.Plot.CreateSignal(data);
        }

        private void OnPaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            frameTimer.Restart();
            this.signal.Render(new SkiaRenderContext(e.Surface.Canvas, e.Info.Width, e.Info.Height));
            renderInfo.PaintRenderInfo(e.Surface.Canvas, e.Info);
            Debug.WriteLine($"Rendertime {frameTimer.Elapsed.TotalSeconds}:0.00");
        }

        private Point lastMousePos;

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            renderInfo.Restart();
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
                signal.Pan(panx, pany);
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
            signal.ZoomAt(factor, xPos, yPos);
            skiaEl.InvalidateVisual();
        }
    }
}