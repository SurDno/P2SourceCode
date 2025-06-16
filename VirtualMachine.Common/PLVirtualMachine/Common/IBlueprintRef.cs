// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.IBlueprintRef
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using PLVirtualMachine.Common.Data;

#nullable disable
namespace PLVirtualMachine.Common
{
  [VMType("IBlueprintRef")]
  public interface IBlueprintRef : IRef, IVariable, INamed, IVMStringSerializable, IEngineTemplated
  {
    IBlueprint Blueprint { get; }
  }
}
