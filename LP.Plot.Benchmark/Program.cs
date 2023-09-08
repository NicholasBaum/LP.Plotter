using LP.Plot.Data;
using LP.Plot.Primitives;
using LP.Plot.Signal;
using SkiaSharp;
using System.Diagnostics;

Console.WriteLine("Hello, World!");

var signals = new LocalDataService().LoadSignal_XL();
var w = 1920;
var h = 1080;
var surface = SKSurface.Create(new SKImageInfo(w, h));
var canvas = surface.Canvas;
var runs = 100;
var xAxisRange = new Span(signals.Min(x => x.XRange.Min), signals.Max(x => x.XRange.Max));

//var zoom = 0.1;
//xAxisRange.ScaleAtCenter(zoom);
//foreach (var s in signals)
//    s.YAxis.ZoomAtRelative(zoom, 0.5);


var path = new SKPath();
var sw = new Stopwatch();
sw.Restart();
for (int i = 0; i < runs; i++)
{
    canvas.Clear(SKColors.Black);
    foreach (var s in signals)
    {
        SignalRenderer.FillDecimatedPath(s.YValues, s.XRange, xAxisRange, s.YAxis.Range, new LPSize(w, h), path);
        canvas.DrawPath(path, s.Paint);
    }
}

sw.Stop();
var fullTime = sw.Elapsed;
Console.WriteLine("Full Time");
Console.WriteLine($"TotalTime: {fullTime}\t\tFrameTime: {fullTime.TotalMilliseconds / runs}");

SaveImage(surface);
Process.Start(new ProcessStartInfo("output.png") { UseShellExecute = true });

sw.Restart();
for (int i = 0; i < runs; i++)
{
    canvas.Clear(SKColors.Black);
    foreach (var s in signals)
    {
        SignalRenderer.FillDecimatedPath(s.YValues, s.XRange, xAxisRange, s.YAxis.Range, new LPSize(w, h), path);
    }
}
sw.Stop();
var pathTime = sw.Elapsed;
var DrawTime = fullTime - pathTime;
Console.WriteLine("Path Time");
Console.WriteLine($"TotalTime: {pathTime}\t\tFrameTime: {pathTime.TotalMilliseconds / runs}");
Console.WriteLine("Draw Time");
Console.WriteLine($"TotalTime: {DrawTime}\t\tFrameTime: {DrawTime.TotalMilliseconds / runs}");


void SaveImage(SKSurface surface)
{
    using (SKImage image = surface.Snapshot())
    {
        // Convert the SKImage to a bitmap (if needed)
        using (SKBitmap bitmap = SKBitmap.FromImage(image))
        {
            // Now you can use the SKBitmap as needed, such as displaying it in a UI
        }

        // You can also save the SKImage to a file
        using (var fileStream = new FileStream("output.png", FileMode.Create))
        {
            image.Encode(SKEncodedImageFormat.Png, 100).SaveTo(fileStream);
        }
    }
}