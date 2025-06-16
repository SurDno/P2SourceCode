using UnityEngine;

public class EnableOnAwake : MonoBehaviour
{
  [SerializeField]
  private GameObject[] objects;

  private void Awake()
  {
    foreach (GameObject gameObject in objects)
    {
      if (gameObject != null)
        gameObject.SetActive(true);
    }
  }
}
