using Engine.Common.Commons;

namespace Engine.Common.Services
{
  public interface INotificationService
  {
    void AddNotify(NotificationEnum type, params object[] values);

    void BlockType(NotificationEnum type);

    void UnblockType(NotificationEnum type);
  }
}
