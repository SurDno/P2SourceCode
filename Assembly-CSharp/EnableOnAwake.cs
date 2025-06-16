public class EnableOnAwake : MonoBehaviour
{
  [SerializeField]
  private GameObject[] objects;

  private void Awake()
  {
    foreach (GameObject gameObject in objects)
    {
      if ((Object) gameObject != (Object) null)
        gameObject.SetActive(true);
    }
  }
}
