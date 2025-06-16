using Engine.Common.Components;
using Engine.Source.Components;
using Inspectors;

namespace Engine.Source.Services.Detectablies
{
  public struct DetectableCandidatInfo
  {
    [Inspected]
    public DetectableComponent Detectable;
    [Inspected]
    public ILocationItemComponent LocationItem;
    [Inspected]
    public GameObject GameObject;
    [Inspected]
    public Vector3 Offset;
  }
}
