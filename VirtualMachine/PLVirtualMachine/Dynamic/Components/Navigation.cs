// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.Navigation
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Components.Movable;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

#nullable disable
namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMNavigation))]
  public class Navigation : VMNavigation, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => "Position";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "LeaveRegionEvent":
          this.LeaveRegionEvent += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "ArrivedRegionEvent":
          this.ArrivedRegionEvent += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "LeaveAreaEvent":
          this.LeaveAreaEvent += (Action<AreaEnum>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "ArrivedAreaEvent":
          this.ArrivedAreaEvent += (Action<AreaEnum>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "LeaveBuildingEvent":
          this.LeaveBuildingEvent += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "ArrivedBuildingEvent":
          this.ArrivedBuildingEvent += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
      }
    }
  }
}
