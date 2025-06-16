using UnityEngine;

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
    if ((Object) this.instance == (Object) null)
    {
      if (!((Object) this.prefab != (Object) null))
        return;
      this.instance = !this.worldSpace ? Object.Instantiate<GameObject>(this.prefab, this.transform, false) : Object.Instantiate<GameObject>(this.prefab);
      this.instance.name = this.prefab.name;
    }
    else
    {
      if (!this.worldSpace)
        return;
      this.instance.SetActive(true);
    }
  }

  private void OnDisable()
  {
    if ((Object) this.instance == (Object) null)
      return;
    if (this.destroyOnDisable)
    {
      Object.Destroy((Object) this.instance);
      this.instance = (GameObject) null;
    }
    else
    {
      if (!this.worldSpace)
        return;
      this.instance.SetActive(false);
    }
  }
}
