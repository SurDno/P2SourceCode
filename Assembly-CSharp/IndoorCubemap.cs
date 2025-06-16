[ExecuteInEditMode]
public class IndoorCubemap : MonoBehaviour
{
  [SerializeField]
  private Cubemap cubemap;

  private void OnDisable()
  {
    Shader.SetGlobalInt("Pathologic_IndoorCubemapLod", 0);
    Shader.SetGlobalTexture("Pathologic_IndoorCubemap", (Texture) null);
  }

  private void OnEnable()
  {
    if (!((Object) cubemap != (Object) null))
      return;
    Shader.SetGlobalInt("Pathologic_IndoorCubemapLod", (int) Mathf.Log((float) cubemap.width, 2f));
    Shader.SetGlobalTexture("Pathologic_IndoorCubemap", (Texture) cubemap);
  }
}
