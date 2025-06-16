using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
  public Camera lookAtCamera;
  public bool lookOnlyOnAwake;

  public void Start()
  {
    if ((Object) this.lookAtCamera == (Object) null)
      this.lookAtCamera = Camera.main;
    if (!this.lookOnlyOnAwake)
      return;
    this.LookCam();
  }

  public void Update()
  {
    if (this.lookOnlyOnAwake)
      return;
    this.LookCam();
  }

  public void LookCam() => this.transform.LookAt(this.lookAtCamera.transform);
}
