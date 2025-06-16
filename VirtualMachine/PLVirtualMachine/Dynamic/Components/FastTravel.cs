// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.FastTravel
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Proxies;
using Engine.Common.Components.Regions;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

#nullable disable
namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMFastTravel))]
  public class FastTravel : VMFastTravel, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => "FastTravelComponent";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      if (!(target.Name == "TravelToPoint"))
        return;
      this.TravelToPoint += (Action<FastTravelPointEnum, GameTime>) ((p1, p2) => target.RaiseFromEngineImpl((object) p1, (object) p2));
    }
  }
}
