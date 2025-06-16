using Engine.Common.Components;
using Engine.Common.Services.Mails;
using PLVirtualMachine.Common.EngineAPI.VMECS.VMAttributes;

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
