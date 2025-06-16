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
      int siblingIndex = transform.GetSiblingIndex();
      foreach (GameObject prefab in prefabs)
      {
        if (!(prefab == null))
        {
          GameObject gameObject = Instantiate(prefab);
          gameObject.name = prefab.name;
          gameObject.transform.SetSiblingIndex(siblingIndex);
          ++siblingIndex;
        }
      }
    }
    Destroy(this.gameObject);
  }
}
