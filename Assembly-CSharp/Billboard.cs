public class Billboard : MonoBehaviour
{
  private Camera camera;

  private void Awake()
  {
    GameObject gameObjectWithTag = GameObject.FindGameObjectWithTag("MainCamera");
    if (!((Object) gameObjectWithTag != (Object) null))
      return;
    camera = gameObjectWithTag.GetComponent<Camera>();
  }

  private void Update()
  {
    if (!((Object) camera != (Object) null))
      return;
    this.transform.rotation = Quaternion.Euler(camera.transform.rotation.eulerAngles with
    {
      x = 0.0f,
      z = 0.0f
    });
  }
}
