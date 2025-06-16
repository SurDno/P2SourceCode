// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.Controller
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Components.Interactable;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

#nullable disable
namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMController))]
  public class Controller : VMController, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Controller);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "BeginControllIteractEvent":
          this.BeginControllIteractEvent += (Action<IEntity, IEntity, InteractType>) ((p1, p2, p3) => target.RaiseFromEngineImpl((object) p1, (object) p2, (object) p3));
          break;
        case "EndControllIteractEvent":
          this.EndControllIteractEvent += (Action<IEntity, IEntity, InteractType>) ((p1, p2, p3) => target.RaiseFromEngineImpl((object) p1, (object) p2, (object) p3));
          break;
      }
    }
  }
}
