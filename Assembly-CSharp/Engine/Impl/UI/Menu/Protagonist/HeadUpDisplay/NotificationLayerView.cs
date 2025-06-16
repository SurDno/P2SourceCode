using Engine.Common.Commons;
using Engine.Source.Services.Notifications;
using Engine.Source.UI.Menu.Protagonist.HeadUpDisplay;

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
          if ((Object) prefab2 == (Object) null)
            return null;
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
      return null;
    }
  }
}
