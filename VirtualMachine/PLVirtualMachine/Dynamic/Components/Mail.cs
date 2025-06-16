// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.Components.Mail
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Proxies;
using Engine.Common.Services.Mails;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Objects;

#nullable disable
namespace PLVirtualMachine.Dynamic.Components
{
  [FactoryProxy(typeof (VMMail))]
  public class Mail : VMMail, IInitialiseComponentFromHierarchy, IInitialiseEvents
  {
    public override string GetComponentTypeName() => nameof (Mail);

    public void InitiliseComponentFromHierarchy(VMEntity entity, VMLogicObject templateObject)
    {
      IParam obj1;
      if (((IBlueprint) templateObject).TryGetProperty("Mail.Header", out obj1))
        this.Header = (ITextRef) obj1.Value;
      IParam obj2;
      if (((IBlueprint) templateObject).TryGetProperty("Mail.Text", out obj2))
        this.Text = (ITextRef) obj2.Value;
      IParam obj3;
      if (!((IBlueprint) templateObject).TryGetProperty("Mail.State", out obj3))
        return;
      this.State = (MailStateEnum) obj3.Value;
    }

    public void InitialiseEvent(DynamicEvent target)
    {
      string name = target.Name;
    }
  }
}
