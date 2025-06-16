// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.BoundCharacterComponent
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Proxies;
using Engine.Common.Commons;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

#nullable disable
namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMBoundCharacterComponent))]
  public class BoundCharacterComponent : 
    VMBoundCharacterComponent,
    IInitialiseComponentFromHierarchy,
    IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (BoundCharacterComponent);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      if (!(target.Name == "OnChangeBoundHealthState"))
        return;
      this.OnChangeBoundHealthState += (Action<BoundHealthStateEnum>) (p1 => target.RaiseFromEngineImpl((object) p1));
    }
  }
}
