using LP.Plot.Primitives;
using LP.Plot.Signal;
using LP.Plot.Skia;
using LP.Plot.UI;
using SkiaSharp;

namespace LP.Plot;

public partial class Plot : IRenderable
{
    private ISignalPlot signalPlot;
    private DockerControl layout = new();
    private int leftAxisWidth = 80;
    private int topCellHeight = 20;
    private int rightAxisWidth = 80;
    private int bottomAxisHeight = 75;
    private LPSize canvasSize;
    private RenderInfo renderInfo = new();

    public Plot() : this(new ISignal[0], "") { }
    public Plot(string xAxisTitle) : this(new ISignal[0], xAxisTitle) { }
    public Plot(ISignal signal, string xAxisTitle) : this(new[] { signal }, xAxisTitle) { }
    public Plot(IEnumerable<ISignal> signals, string xAxisTitle)
    {
        signalPlot = new BufferedSignalPlot(signals);
        signalPlot.XAxis.Title = xAxisTitle ?? "";
        SetDefaultYAxes();
        layout.Bottom = new AxisControl(signalPlot.XAxis, this) { Parent = layout, DesiredSize = new LPSize(0, bottomAxisHeight) };
        layout.Center = new SignalPlotControl(signalPlot, this) { Parent = layout };
        layout.Top = BorderControl.CreateBottom(layout, topCellHeight);
    }

    public void SetDefaultYAxes()
    {
        var leftAxis = signalPlot.YAxes.Where(x => x.Position == AxisPosition.Left).FirstOrDefault();
        var rightAxis = signalPlot.YAxes.Where(x => x.Position == AxisPosition.Right).FirstOrDefault();

        if (leftAxis is not null)
            layout.Left = new AxisControl(leftAxis, this) { Parent = layout, DesiredSize = new LPSize(leftAxisWidth, 0) };
        else
            layout.Left = BorderControl.CreateRight(layout, leftAxisWidth);

        if (rightAxis is not null)
            layout.Right = new AxisControl(rightAxis, this) { Parent = layout, DesiredSize = new LPSize(rightAxisWidth, 0) };
        else
            layout.Right = BorderControl.CreateLeft(layout, rightAxisWidth);
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

    public void SetLeftAxis(Axis? axis)
    {
        if (axis is null)
        {
            layout.Left = BorderControl.CreateRight(layout, leftAxisWidth);
            return;
        }
        if ((this.layout.Right as AxisControl)?.Content == axis)
            throw new InvalidOperationException("Axis object is already used on the right side.");
        axis.Position = AxisPosition.Left;
        this.layout.Left = new AxisControl(axis, this) { Parent = layout, DesiredSize = new LPSize(leftAxisWidth, 0) };
    }

    public void SetRightAxis(Axis? axis)
    {
        if (axis is null)
        {
            layout.Right = BorderControl.CreateLeft(layout, rightAxisWidth);
            return;
        }
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
        RemoveUnassigendAxisControls();
    }

    private void RemoveUnassigendAxisControls()
    {
        var axes = signalPlot.YAxes;
        if (layout.Left is AxisControl lac && !axes.Contains(lac.Content))
            SetLeftAxis(null);
        if (layout.Right is AxisControl rac && !axes.Contains(rac.Content))
            SetRightAxis(null);
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