using Inspectors;
using System.Collections.Generic;

namespace Engine.Source.Services.Notifications
{
  public class NotificationLayerItem
  {
    [Inspected]
    public INotification Notifaction;
    [Inspected]
    public List<NotificationItem> Queue = new List<NotificationItem>();
  }
}
