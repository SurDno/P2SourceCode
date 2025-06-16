using UnityEngine;

public class OverlayCamera : MonoBehaviour
{
  public static float Gamma = 2.2f;
  public Shader PostProcessShader;
  private Material material;
  private Camera camera;

  private void Awake() => this.CorrectPosition();

  private void CorrectPosition()
  {
    float x = 0.5f * (float) Screen.width;
    float y = 0.5f * (float) Screen.height;
    this.transform.localPosition = new Vector3(x, y, 0.0f);
    if ((Object) this.camera == (Object) null)
      this.camera = this.GetComponent<Camera>();
    this.camera.orthographicSize = y;
  }

  private void LateUpdate() => this.CorrectPosition();

  private void OnRenderImage(RenderTexture src, RenderTexture dest)
  {
    if ((Object) this.material == (Object) null)
    {
      if ((Object) this.PostProcessShader == (Object) null)
      {
        this.enabled = false;
        Graphics.Blit((Texture) src, dest);
        return;
      }
      this.material = new Material(this.PostProcessShader);
    }
    this.material.SetFloat("_Power", OverlayCamera.Gamma / 2.2f);
    Graphics.Blit((Texture) src, dest, this.material);
  }
}
