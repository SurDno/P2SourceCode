using Engine.Common.Binders;

namespace Engine.Common.Components.Storable
{
  [EnumType("ContainerOpenState")]
  public enum ContainerOpenStateEnum
  {
    None,
    Open,
    Closed,
    Locked,
  }
}
