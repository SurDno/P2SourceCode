using Engine.Common.Components.Movable;

namespace Engine.Common.Components
{
  public delegate void AreaHandler(
    ref EventArgument<IEntity, AreaEnum> eventArguments);
}
