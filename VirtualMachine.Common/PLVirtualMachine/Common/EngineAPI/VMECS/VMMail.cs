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
      get => header;
      set
      {
        header = value;
        Component.Header = EngineAPIManager.CreateEngineTextInstance(header);
      }
    }

    [Property("Text", "", true)]
    public ITextRef Text
    {
      get => text;
      set
      {
        text = value;
        Component.Text = EngineAPIManager.CreateEngineTextInstance(text);
      }
    }

    [Property("State", "", true)]
    public MailStateEnum State
    {
      get => Component.State;
      set => Component.State = value;
    }
  }
}
