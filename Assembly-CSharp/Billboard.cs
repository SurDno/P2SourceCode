using UnityEngine;

public class Billboard : MonoBehaviour
{
  private Camera camera;

  private void Awake()
  {
    GameObject gameObjectWithTag = GameObject.FindGameObjectWithTag("MainCamera");
    if (!(gameObjectWithTag != null))
      return;
    camera = gameObjectWithTag.GetComponent<Camera>();
  }

  private void Update()
  {
    if (!(camera != null))
      return;
    transform.rotation = Quaternion.Euler(camera.transform.rotation.eulerAngles with
    {
      x = 0.0f,
      z = 0.0f
    });
  }
}
