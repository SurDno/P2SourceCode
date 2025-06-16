public class OverlayCamera : MonoBehaviour
{
  public static float Gamma = 2.2f;
  public Shader PostProcessShader;
  private Material material;
  private Camera camera;

  private void Awake() => CorrectPosition();

  private void CorrectPosition()
  {
    float x = 0.5f * (float) Screen.width;
    float y = 0.5f * (float) Screen.height;
    this.transform.localPosition = new Vector3(x, y, 0.0f);
    if ((Object) camera == (Object) null)
      camera = this.GetComponent<Camera>();
    camera.orthographicSize = y;
  }

  private void LateUpdate() => CorrectPosition();

  private void OnRenderImage(RenderTexture src, RenderTexture dest)
  {
    if ((Object) material == (Object) null)
    {
      if ((Object) PostProcessShader == (Object) null)
      {
        this.enabled = false;
        Graphics.Blit((Texture) src, dest);
        return;
      }
      material = new Material(PostProcessShader);
    }
    material.SetFloat("_Power", Gamma / 2.2f);
    Graphics.Blit((Texture) src, dest, material);
  }
}
