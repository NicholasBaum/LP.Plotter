using LP.Plot.Core.Primitives;
using LP.Plot.Core.Signal;
using LP.Plot.Core.Skia;
using LP.Plot.Core.UI;
using SkiaSharp;

namespace LP.Plot.Core;

public partial class Plot : IRenderable
{
    private ISignalPlot signalPlot = null!;
    private DockerControl layout = null!;
    private int leftAxisWidth = 75;
    private int topCellHeight = 20;
    private int rightAxisWidth = 75;
    private int bottomAxisHeight = 75;
    private LPSize canvasSize;
    private RenderInfo renderInfo = new();

    public Plot(ISignal signal, string xAxisTitle = "") : this(new[] { signal }, xAxisTitle) { }

    public Plot(IEnumerable<ISignal> signals, string xAxisTitle = "")
    {
        signalPlot = new BufferedSignalPlot(signals);
        signalPlot.XAxis.Title = xAxisTitle;
        var leftAxis = signalPlot.YAxes.Where(x => x.Position == AxisPosition.Left).FirstOrDefault();
        var rightAxis = signalPlot.YAxes.Where(x => x.Position == AxisPosition.Right).FirstOrDefault();
        layout = new DockerControl();
        if (leftAxis is not null)
            layout.Left = new AxisControl(leftAxis, this) { Parent = layout, DesiredSize = new LPSize(leftAxisWidth, 0) };
        layout.Top = new BorderControl() { Parent = layout, DesiredSize = new LPSize(0, topCellHeight), ShowLeft = false, ShowTop = false, ShowRight = false };
        if (rightAxis is not null)
            layout.Right = new AxisControl(rightAxis, this) { Parent = layout, DesiredSize = new LPSize(rightAxisWidth, 0) };
        layout.Bottom = new AxisControl(signalPlot.XAxis, this) { Parent = layout, DesiredSize = new LPSize(0, bottomAxisHeight) };
        layout.Center = new SignalPlotControl(signalPlot, this) { Parent = layout };
    }

    public void Render(IRenderContext ctx)
    {
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

    public void SetLeftAxis(Axis axis)
    {
        if ((this.layout.Right as AxisControl)?.Content == axis)
            throw new InvalidOperationException("Axis object is already used on the right side.");
        axis.Position = AxisPosition.Left;
        this.layout.Left = new AxisControl(axis, this) { Parent = layout, DesiredSize = new LPSize(leftAxisWidth, 0) };
    }

    public void SetRightAxis(Axis axis)
    {
        if ((this.layout.Left as AxisControl)?.Content == axis)
            throw new InvalidOperationException("Axis object is already used on the left side.");
        axis.Position = AxisPosition.Right;
        this.layout.Right = new AxisControl(axis, this) { Parent = layout, DesiredSize = new LPSize(rightAxisWidth, 0) };
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

    public void ResetAxes()
    {
        if (signalPlot.Signals.Any())
        {
            var signals = signalPlot.Signals;
            this.signalPlot.XAxis.Range = new(signals.Min(x => x.XRange.Min), signals.Max(x => x.XRange.Max));
        }
    }
}