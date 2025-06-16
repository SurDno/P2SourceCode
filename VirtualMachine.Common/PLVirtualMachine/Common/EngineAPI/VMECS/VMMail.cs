// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Common.EngineAPI.VMECS.VMMail
// Assembly: VirtualMachine.Common, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: FE3F54BA-1089-4F0E-B049-A4D27F3D2E73
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.Common.dll

using Engine.Common.Components;
using Engine.Common.Services.Mails;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

#nullable disable
namespace PLVirtualMachine.Common.EngineAPI.VMECS
{
  [Info("Mail", typeof (IMailComponent))]
  public class VMMail : VMEngineComponent<IMailComponent>
  {
    public const string ComponentName = "Mail";
    private ITextRef header;
    private ITextRef text;

    [Property("Header", "", true)]
    public ITextRef Header
    {
      get => this.header;
      set
      {
        this.header = value;
        this.Component.Header = EngineAPIManager.CreateEngineTextInstance(this.header);
      }
    }

    [Property("Text", "", true)]
    public ITextRef Text
    {
      get => this.text;
      set
      {
        this.text = value;
        this.Component.Text = EngineAPIManager.CreateEngineTextInstance(this.text);
      }
    }

    [Property("State", "", true)]
    public MailStateEnum State
    {
      get => this.Component.State;
      set => this.Component.State = value;
    }
  }
}
