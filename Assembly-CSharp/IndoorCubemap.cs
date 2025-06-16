using UnityEngine;

[ExecuteInEditMode]
public class IndoorCubemap : MonoBehaviour
{
  [SerializeField]
  private Cubemap cubemap;

  private void OnDisable()
  {
    Shader.SetGlobalInt("Pathologic_IndoorCubemapLod", 0);
    Shader.SetGlobalTexture("Pathologic_IndoorCubemap", null);
  }

  private void OnEnable()
  {
    if (!(cubemap != null))
      return;
    Shader.SetGlobalInt("Pathologic_IndoorCubemapLod", (int) Mathf.Log(cubemap.width, 2f));
    Shader.SetGlobalTexture("Pathologic_IndoorCubemap", cubemap);
  }
}
