using Engine.Common.Components;
using Engine.Common.Components.Regions;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Building", typeof (IBuildingComponent))]
  public class VMBuilding : VMEngineComponent<IBuildingComponent>
  {
    public const string ComponentName = "Building";

    [Property("Building", "", true)]
    public BuildingEnum Building => Component.Building;
  }
}
