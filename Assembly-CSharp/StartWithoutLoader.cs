using UnityEngine;
using UnityEngine.SceneManagement;

public class StartWithoutLoader : MonoBehaviour
{
  [SerializeField]
  private GameObject[] prefabs;

  private void Awake()
  {
    if (SceneManager.GetActiveScene() == this.gameObject.scene)
    {
      int siblingIndex = this.transform.GetSiblingIndex();
      foreach (GameObject prefab in this.prefabs)
      {
        if (!((Object) prefab == (Object) null))
        {
          GameObject gameObject = Object.Instantiate<GameObject>(prefab);
          gameObject.name = prefab.name;
          gameObject.transform.SetSiblingIndex(siblingIndex);
          ++siblingIndex;
        }
      }
    }
    Object.Destroy((Object) this.gameObject);
  }
}
