using Engine.Common;
using Inspectors;

namespace Engine.Source.Components
{
  public struct DebugTeleportData
  {
    [Inspected]
    public IEntity Entity;
    [Inspected]
    public DebugTeleportContext TeleportContext;
    [Inspected]
    public Vector3 Position;
    [Inspected]
    public string Context;
    [Inspected]
    public int Frame;
  }
}
