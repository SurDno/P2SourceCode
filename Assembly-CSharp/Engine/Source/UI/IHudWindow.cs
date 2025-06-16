using Engine.Common.Commons;
using Engine.Source.Services.Notifications;

namespace Engine.Source.UI
{
  public interface IHudWindow : IWindow
  {
    InteractableWindow InteractableInterface { get; }

    INotification Create(NotificationEnum type);

    void SetVisibility(bool visible, bool ignoreTextNotifications = false);
  }
}
