// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.Region
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Proxies;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

#nullable disable
namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMRegion))]
  public class Region : VMRegion, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Region);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
      IParam obj;
      if (!((IBlueprint) templateObject).TryGetProperty("Region.RegionIndex", out obj))
        return;
      this.RegionIndex = (int) obj.Value;
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "ReputationChanged":
          this.ReputationChanged += (Action<float>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "DiseaseLevelChanged":
          this.DiseaseLevelChanged += (Action<int>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
      }
    }
  }
}
