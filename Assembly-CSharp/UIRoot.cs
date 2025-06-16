using Engine.Source.UI.Menu.Main;

public class UIRoot : MonoBehaviour
{
  [SerializeField]
  private GameObject root;
  [SerializeField]
  private GameObject[] prefabs;
  [Space]
  [SerializeField]
  private SmallLoading smallLoading;
  [SerializeField]
  private GameObject virtualCursor;

  public SmallLoading SmallLoading => smallLoading;

  public GameObject VirtualCursor => virtualCursor;

  public GameObject Root => root;

  public void Initialize()
  {
    Transform transform = root?.transform;
    if ((Object) transform == (Object) null)
      transform = this.transform;
    foreach (GameObject prefab in prefabs)
    {
      if (!((Object) prefab == (Object) null))
      {
        GameObject gameObject = Object.Instantiate<GameObject>(prefab, transform, false);
        gameObject.SetActive(false);
        gameObject.name = prefab.name;
      }
    }
  }
}
