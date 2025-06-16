// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.NpcController
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Proxies;
using Engine.Common;
using Engine.Common.Commons;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;
using System;

#nullable disable
namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMNpcController))]
  public class NpcController : VMNpcController, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => "NpcControllerComponent";

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      switch (target.Name)
      {
        case "ActionEvent":
          this.ActionEvent += (Action<ActionEnum>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "CombatActionEvent":
          this.CombatActionEvent += (Action<CombatActionEnum, IEntity>) ((p1, p2) => target.RaiseFromEngineImpl((object) p1, (object) p2));
          break;
        case "OnChangeHealth":
          this.OnChangeHealth += (Action<float>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "OnChangePain":
          this.OnChangePain += (Action<float>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
        case "ChangeAwayEvent":
          this.ChangeAwayEvent += (Action<bool>) (p1 => target.RaiseFromEngineImpl((object) p1));
          break;
      }
    }
  }
}
