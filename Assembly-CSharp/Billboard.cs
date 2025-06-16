using UnityEngine;

public class Billboard : MonoBehaviour
{
  private Camera camera;

  private void Awake()
  {
    GameObject gameObjectWithTag = GameObject.FindGameObjectWithTag("MainCamera");
    if (!((Object) gameObjectWithTag != (Object) null))
      return;
    this.camera = gameObjectWithTag.GetComponent<Camera>();
  }

  private void Update()
  {
    if (!((Object) this.camera != (Object) null))
      return;
    this.transform.rotation = Quaternion.Euler(this.camera.transform.rotation.eulerAngles with
    {
      x = 0.0f,
      z = 0.0f
    });
  }
}
