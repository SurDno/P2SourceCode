// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.Detector
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Proxies;
using Engine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

#nullable disable
namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMDetector))]
  public class Detector : VMDetector, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Detector);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "OnSee":
          this.OnSee += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "OnStopSee":
          this.OnStopSee += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "OnHear":
          this.OnHear += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
      }
    }
  }
}
