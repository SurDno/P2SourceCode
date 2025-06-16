// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Menu.Protagonist.HeadUpDisplay.NotificationLayerView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Common.Commons;
using Engine.Source.Services.Notifications;
using Engine.Source.UI.Menu.Protagonist.HeadUpDisplay;
using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Menu.Protagonist.HeadUpDisplay
{
  public class NotificationLayerView : MonoBehaviour
  {
    [SerializeField]
    private NotificationPair[] prefabs;

    public INotification Create(NotificationEnum type)
    {
      foreach (NotificationPair prefab1 in this.prefabs)
      {
        if (prefab1.Type == type)
        {
          GameObject prefab2 = prefab1.Prefab;
          if ((Object) prefab2 == (Object) null)
            return (INotification) null;
          GameObject gameObject = Object.Instantiate<GameObject>(prefab2);
          foreach (MonoBehaviour component in gameObject.GetComponents<MonoBehaviour>())
          {
            if (component is INotification notification)
            {
              gameObject.transform.SetParent(this.transform, false);
              gameObject.name = "[UI] " + prefab2.name;
              return notification;
            }
          }
          Object.Destroy((Object) gameObject);
        }
      }
      return (INotification) null;
    }
  }
}
