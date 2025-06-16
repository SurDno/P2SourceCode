// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMCombination
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;
using PLVirtualMachine.Common.VMSpecialAttributes;

#nullable disable
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
