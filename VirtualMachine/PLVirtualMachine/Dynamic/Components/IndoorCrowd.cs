// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.IndoorCrowd
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Proxies;
using Engine.Common;
using PLVirtualMachine.Objects;
using System;
using VirtualMachine.Common.EngineAPI.VMECS.VMComponents;

#nullable disable
namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMIndoorCrowd))]
  public class IndoorCrowd : VMIndoorCrowd, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => "IndoorCrowdComponent";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "NeedCreateObjectEvent":
          this.NeedCreateObjectEvent += (VMIndoorCrowd.NeedCreateObjectEventType) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "NeedDeleteObjectEvent":
          this.NeedDeleteObjectEvent += (Action<IEntity>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
      }
    }
  }
}
