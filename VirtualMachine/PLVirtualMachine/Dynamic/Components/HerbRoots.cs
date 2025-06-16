// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.HerbRoots
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Proxies;
using PLVirtualMachine.Objects;
using System;
using VirtualMachine.Common.EngineAPI.VMECS.VMComponents;

#nullable disable
namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMHerbRoots))]
  public class HerbRoots : VMHerbRoots, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => "HerbRootsComponent";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "TriggerEnterEvent":
          this.TriggerEnterEvent += (Action) (() => target.RaiseFromEngineImpl());
          break;
        case "TriggerLeaveEvent":
          this.TriggerLeaveEvent += (Action) (() => target.RaiseFromEngineImpl());
          break;
        case "ActivateStartEvent":
          this.ActivateStartEvent += (Action) (() => target.RaiseFromEngineImpl());
          break;
        case "ActivateEndEvent":
          this.ActivateEndEvent += (Action) (() => target.RaiseFromEngineImpl());
          break;
        case "HerbSpawnEvent":
          this.HerbSpawnEvent += (Action) (() => target.RaiseFromEngineImpl());
          break;
        case "LastHerbSpawnEvent":
          this.LastHerbSpawnEvent += (Action) (() => target.RaiseFromEngineImpl());
          break;
      }
    }
  }
}
