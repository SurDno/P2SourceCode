using Inspectors;

namespace Engine.Source.Components
{
  public struct EffectInfo
  {
    [Inspected(Header = true)]
    public string Type;
    [Inspected(Header = true)]
    public string Name;
    [Inspected(Header = true)]
    public int Count;
    [Inspected(Header = true)]
    public int Compute;
  }
}
