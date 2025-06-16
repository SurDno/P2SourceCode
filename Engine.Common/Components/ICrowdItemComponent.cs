using Engine.Common.Components.Movable;

namespace Engine.Common.Components
{
  public interface ICrowdItemComponent : IComponent
  {
    AreaEnum Area { get; }
  }
}
