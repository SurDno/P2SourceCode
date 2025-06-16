using System.Collections.Generic;
using Engine.Common;
using Engine.Common.Components.Movable;
using Engine.Common.Components.Parameters;
using Engine.Source.Commons;
using Inspectors;

namespace Engine.Source.Components.Crowds
{
  public class PointInfo
  {
    [Inspected]
    public Vector3 Position;
    [Inspected]
    public Quaternion Rotation;
    [Inspected]
    public IEntity Region;
    [Inspected]
    public AreaEnum Area;
    [Inspected]
    public IEntity Entity;
    [Inspected]
    public List<IParameter> States = new List<IParameter>();
    [Inspected]
    public bool OnNavMesh;
    [Inspected]
    public IEntity EntityPoint;

    public GameObject GameObject
    {
      get
      {
        return EntityPoint != null ? ((IEntityView) EntityPoint).GameObject : (GameObject) null;
      }
    }
  }
}
