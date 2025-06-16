using Engine.Common.Commons;

namespace Engine.Source.Services.Notifications
{
  public interface INotification
  {
    bool Complete { get; }

    NotificationEnum Type { get; }

    void Initialise(NotificationEnum type, object[] values);

    void Shutdown();
  }
}
