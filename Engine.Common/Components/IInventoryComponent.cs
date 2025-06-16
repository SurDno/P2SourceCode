using Engine.Common.Components.Parameters;
using Engine.Common.Components.Storable;

namespace Engine.Common.Components
{
  public interface IInventoryComponent : IComponent
  {
    IParameter<bool> Enabled { get; }

    IParameter<bool> Available { get; }

    IParameter<float> Disease { get; }

    IParameter<ContainerOpenStateEnum> OpenState { get; }
  }
}
