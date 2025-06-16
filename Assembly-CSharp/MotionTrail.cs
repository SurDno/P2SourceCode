using UnityEngine;

[ExecuteInEditMode]
public class MotionTrail : MonoBehaviour
{
  [SerializeField]
  private Shader blendShader = (Shader) null;
  [SerializeField]
  [Range(0.0f, 1f)]
  [Tooltip("Approximate opacity of the previous frame at 30 fps")]
  private float strength = 0.0f;
  private Material material = (Material) null;
  private int propertyId = 0;
  private RenderTexture historyBuffer = (RenderTexture) null;

  public float Strength
  {
    get => this.strength;
    set => this.strength = Mathf.Clamp01(value);
  }

  private void OnDisable()
  {
    if (!((Object) this.historyBuffer != (Object) null))
      return;
    this.DestroyBuffer(ref this.historyBuffer);
  }

  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if ((double) this.strength == 0.0 || !this.MaterialValid())
    {
      if ((Object) this.historyBuffer != (Object) null)
        this.DestroyBuffer(ref this.historyBuffer);
      Graphics.Blit((Texture) source, destination);
    }
    else if ((Object) this.historyBuffer == (Object) null)
    {
      this.CreateHistoryBuffer(source);
      Graphics.Blit((Texture) source, this.historyBuffer);
      Graphics.Blit((Texture) source, destination);
    }
    else
    {
      if (this.FramePropertiesChanged(source))
      {
        RenderTexture historyBuffer = this.historyBuffer;
        this.CreateHistoryBuffer(source);
        Graphics.Blit((Texture) historyBuffer, this.historyBuffer);
        this.DestroyBuffer(ref historyBuffer);
      }
      this.material.SetFloat(this.propertyId, 1f - Mathf.Pow(this.strength, Time.deltaTime * 30f));
      Graphics.Blit((Texture) source, this.historyBuffer, this.material);
      Graphics.Blit((Texture) this.historyBuffer, destination);
    }
  }

  private bool FramePropertiesChanged(RenderTexture source)
  {
    return this.historyBuffer.width != source.width || this.historyBuffer.height != source.height || this.historyBuffer.format != source.format;
  }

  private bool MaterialValid()
  {
    if ((Object) this.material != (Object) null)
      return true;
    if ((Object) this.blendShader == (Object) null || !this.blendShader.isSupported)
      return false;
    this.material = new Material(this.blendShader);
    this.propertyId = Shader.PropertyToID("_Opacity");
    return true;
  }

  private void CreateHistoryBuffer(RenderTexture source)
  {
    this.historyBuffer = new RenderTexture(source.width, source.height, 0, source.format, source.sRGB ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear);
  }

  private void DestroyBuffer(ref RenderTexture buffer)
  {
    buffer.Release();
    Object.Destroy((Object) buffer);
    buffer = (RenderTexture) null;
  }
}
