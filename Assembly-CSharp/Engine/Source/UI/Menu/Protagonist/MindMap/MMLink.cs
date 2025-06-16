using Engine.Common.MindMap;
using Engine.Impl.Services.Factories;
using Inspectors;

namespace Engine.Source.UI.Menu.Protagonist.MindMap
{
  [Factory(typeof (IMMLink))]
  public class MMLink : IMMLink
  {
    [Inspected]
    public IMMNode Origin { get; set; }

    [Inspected]
    public IMMNode Target { get; set; }

    [Inspected]
    public MMLinkKind Kind { get; set; }
  }
}
