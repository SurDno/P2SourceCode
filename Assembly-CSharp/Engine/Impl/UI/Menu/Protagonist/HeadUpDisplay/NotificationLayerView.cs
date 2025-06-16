using Engine.Common.Commons;
using Engine.Source.Services.Notifications;
using Engine.Source.UI.Menu.Protagonist.HeadUpDisplay;
using UnityEngine;

namespace Engine.Impl.UI.Menu.Protagonist.HeadUpDisplay
{
  public class NotificationLayerView : MonoBehaviour
  {
    [SerializeField]
    private NotificationPair[] prefabs;

    public INotification Create(NotificationEnum type)
    {
      foreach (NotificationPair prefab1 in prefabs)
      {
        if (prefab1.Type == type)
        {
          GameObject prefab2 = prefab1.Prefab;
          if (prefab2 == null)
            return null;
          GameObject gameObject = Instantiate(prefab2);
          foreach (MonoBehaviour component in gameObject.GetComponents<MonoBehaviour>())
          {
            if (component is INotification notification)
            {
              gameObject.transform.SetParent(transform, false);
              gameObject.name = "[UI] " + prefab2.name;
              return notification;
            }
          }
          Destroy(gameObject);
        }
      }
      return null;
    }
  }
}
