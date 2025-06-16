using Engine.Common.Services.Mails;
using Engine.Common.Types;

namespace Engine.Common.Components
{
  public interface IMailComponent : IComponent
  {
    MailStateEnum State { get; set; }

    LocalizedText Header { get; set; }

    LocalizedText Text { get; set; }
  }
}
