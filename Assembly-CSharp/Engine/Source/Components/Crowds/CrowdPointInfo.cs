using Engine.Common;
using Engine.Common.Components.Movable;
using Inspectors;

namespace Engine.Source.Components.Crowds
{
  public struct CrowdPointInfo
  {
    [Inspected]
    public GameObject GameObject;
    [Inspected]
    public int Radius;
    [Inspected]
    public Vector3 CenterPoint;
    [Inspected]
    public Vector3 Position;
    [Inspected]
    public Quaternion Rotation;
    [Inspected]
    public AreaEnum Area;
    [Inspected]
    public IEntity EntityPoint;
    [Inspected]
    public bool OnNavMesh;
    [Inspected]
    public bool NotReady;
  }
}
