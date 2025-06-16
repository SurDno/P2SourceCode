using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;

namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Combination", null)]
  public class VMCombination : VMComponent
  {
    public const string ComponentName = "Combination";
    private ObjectCombinationDataStruct combinationData;

    public override void Initialize(VMBaseEntity parent) => base.Initialize(parent);

    [Property("Storable spawn combination", "", true)]
    [SpecialProperty(ESpecialPropertyName.SPN_COMBINATION_DATA)]
    public ObjectCombinationDataStruct CombinationData
    {
      get => this.combinationData;
      set => this.combinationData = value;
    }
  }
}
