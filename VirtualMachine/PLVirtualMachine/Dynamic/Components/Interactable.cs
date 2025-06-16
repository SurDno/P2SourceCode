// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.Interactable
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Components.Interactable;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

#nullable disable
namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMInteractable))]
  public class Interactable : VMInteractable, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => "Interactive";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
      IParam obj;
      if (!((IBlueprint) templateObject).TryGetProperty("Interactive.ObjectName", out obj))
        return;
      this.ObjectName = (ITextRef) obj.Value;
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "BeginIteractEvent":
          this.BeginIteractEvent += (Action<IEntity, IEntity, InteractType>) ((p1, p2, p3) => target.RaiseFromEngineImpl((object) p1, (object) p2, (object) p3));
          break;
        case "EndIteractEvent":
          this.EndIteractEvent += (Action<IEntity, IEntity, InteractType>) ((p1, p2, p3) => target.RaiseFromEngineImpl((object) p1, (object) p2, (object) p3));
          break;
      }
    }
  }
}
