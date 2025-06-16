public class PrefabAnchor : MonoBehaviour
{
  [SerializeField]
  private GameObject prefab;
  [SerializeField]
  private bool worldSpace = false;
  [SerializeField]
  private bool destroyOnDisable = false;
  private GameObject instance;

  private void OnEnable()
  {
    if ((Object) instance == (Object) null)
    {
      if (!((Object) prefab != (Object) null))
        return;
      instance = !worldSpace ? Object.Instantiate<GameObject>(prefab, this.transform, false) : Object.Instantiate<GameObject>(prefab);
      instance.name = prefab.name;
    }
    else
    {
      if (!worldSpace)
        return;
      instance.SetActive(true);
    }
  }

  private void OnDisable()
  {
    if ((Object) instance == (Object) null)
      return;
    if (destroyOnDisable)
    {
      Object.Destroy((Object) instance);
      instance = (GameObject) null;
    }
    else
    {
      if (!worldSpace)
        return;
      instance.SetActive(false);
    }
  }
}
