public class LookAtCamera : MonoBehaviour
{
  public Camera lookAtCamera;
  public bool lookOnlyOnAwake;

  public void Start()
  {
    if ((Object) lookAtCamera == (Object) null)
      lookAtCamera = Camera.main;
    if (!lookOnlyOnAwake)
      return;
    LookCam();
  }

  public void Update()
  {
    if (lookOnlyOnAwake)
      return;
    LookCam();
  }

  public void LookCam() => this.transform.LookAt(lookAtCamera.transform);
}
