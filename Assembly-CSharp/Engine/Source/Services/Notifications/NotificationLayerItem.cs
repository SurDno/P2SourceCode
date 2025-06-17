using System.Collections.Generic;
using Inspectors;

namespace Engine.Source.Services.Notifications
{
  public class NotificationLayerItem
  {
    [Inspected]
    public INotification Notifaction;
    [Inspected]
    public List<NotificationItem> Queue = [];
  }
}
