using LP.Plot.Primitives;
using LP.Plot.Signal;
using LP.Plot.Skia;
using LP.Plot.UI;
using SkiaSharp;

namespace LP.Plot;

public partial class PlotModel : IRenderable
{
    private ISignalPlot signalPlot;
    private DockerControl layout = new();
    private int leftAxisWidth = 80;
    private int topCellHeight = 20;
    private int rightAxisWidth = 80;
    private int bottomAxisHeight = 75;
    private LPSize canvasSize;
    private RenderInfo renderInfo = new();
    private AxisControl leftAxisControl;
    private AxisControl rightAxisControl;


    public PlotModel() : this(new ISignal[0], "") { }
    public PlotModel(string xAxisTitle) : this(new ISignal[0], xAxisTitle) { }
    public PlotModel(ISignal signal, string xAxisTitle) : this(new[] { signal }, xAxisTitle) { }
    public PlotModel(IEnumerable<ISignal> signals, string xAxisTitle)
    {
        signalPlot = new BufferedSignalPlot(signals);
        signalPlot.XAxis.Title = xAxisTitle ?? "";
        layout.Bottom = new AxisControl(signalPlot.XAxis, this) { Parent = layout, DesiredSize = new LPSize(0, bottomAxisHeight) };
        layout.Center = new SignalPlotControl(signalPlot, this) { Parent = layout };
        layout.Top = BorderControl.CreateBottom(layout, topCellHeight);

        leftAxisControl = new AxisControl(this) { Parent = layout, DesiredSize = new LPSize(leftAxisWidth, 0) };
        rightAxisControl = new AxisControl(this) { Parent = layout, DesiredSize = new LPSize(rightAxisWidth, 0) };
    }

    public void Add(ISignal signal)
    {
        signalPlot.Add(signal);
    }

    public void Add(IEnumerable<ISignal> signals)
    {
        foreach (var s in signals)
            signalPlot.Add(s);
    }

    public void Remove(ISignal signal)
    {
        signalPlot.Remove(signal);
    }

    public void Render(IRenderContext ctx)
    {
        UpdateAxesPosition();

        using (var m = renderInfo.Measure())
        {
            canvasSize = ctx.Size;
            ctx.Canvas.Clear(SKColors.Black);
            layout.SetRect(LPRect.Create(ctx.Size));
            layout.Render(ctx);
            DrawZoomRect(ctx);
        }
        renderInfo.Render(ctx);
    }

    private void UpdateAxesPosition()
    {
        // only show axes that are attached to a signal
        var leftAxis = signalPlot.YAxes.Where(x => x.Position == AxisPosition.Left).FirstOrDefault();
        var rightAxis = signalPlot.YAxes.Where(x => x.Position == AxisPosition.Right).FirstOrDefault();

        if (leftAxis is not null)
        {
            leftAxisControl.Content = leftAxis;
            layout.Left = leftAxisControl;
        }
        else
            layout.Left = BorderControl.CreateRight(layout, leftAxisWidth);

        if (rightAxis is not null)
        {
            rightAxisControl.Content = rightAxis;
            layout.Right = rightAxisControl;
        }
        else
            layout.Right = BorderControl.CreateLeft(layout, rightAxisWidth);
    }  

    public void ResetAxes()
    {
        if (signalPlot.Signals.Any())
        {
            var signals = signalPlot.Signals;
            this.signalPlot.XAxis.Range = new(signals.Min(x => x.XRange.Min), signals.Max(x => x.XRange.Max));
        }
    }
}