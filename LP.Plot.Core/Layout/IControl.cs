using LP.Plot.Core.Primitives;

namespace LP.Plot.Core.Layout;

public interface IControl : IRenderable
{
    public IControl? Parent { get; }
    public LPSize DesiredSize { get; }
    public LPRect Rect { get; }
    public void SetRect(LPRect rect);
}