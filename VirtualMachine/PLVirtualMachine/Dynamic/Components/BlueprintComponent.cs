// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.BlueprintComponent
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Proxies;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

#nullable disable
namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMBlueprintComponent))]
  public class BlueprintComponent : 
    VMBlueprintComponent,
    IInitialiseComponentFromHierarchy,
    IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (BlueprintComponent);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "Complete":
          this.Complete += (Action) (() => target.RaiseFromEngineImpl());
          break;
        case "Attach":
          this.Attach += (Action) (() => target.RaiseFromEngineImpl());
          break;
      }
    }
  }
}
