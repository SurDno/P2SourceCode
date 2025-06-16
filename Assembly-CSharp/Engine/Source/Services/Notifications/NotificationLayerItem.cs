// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.Notifications.NotificationLayerItem
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;
using System.Collections.Generic;

#nullable disable
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
