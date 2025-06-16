[ExecuteInEditMode]
public class MotionTrail : MonoBehaviour
{
  [SerializeField]
  private Shader blendShader = (Shader) null;
  [SerializeField]
  [Range(0.0f, 1f)]
  [Tooltip("Approximate opacity of the previous frame at 30 fps")]
  private float strength;
  private Material material = (Material) null;
  private int propertyId;
  private RenderTexture historyBuffer = (RenderTexture) null;

  public float Strength
  {
    get => strength;
    set => strength = Mathf.Clamp01(value);
  }

  private void OnDisable()
  {
    if (!((Object) historyBuffer != (Object) null))
      return;
    DestroyBuffer(ref historyBuffer);
  }

  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if (strength == 0.0 || !MaterialValid())
    {
      if ((Object) historyBuffer != (Object) null)
        DestroyBuffer(ref historyBuffer);
      Graphics.Blit((Texture) source, destination);
    }
    else if ((Object) this.historyBuffer == (Object) null)
    {
      CreateHistoryBuffer(source);
      Graphics.Blit((Texture) source, historyBuffer);
      Graphics.Blit((Texture) source, destination);
    }
    else
    {
      if (FramePropertiesChanged(source))
      {
        RenderTexture historyBuffer = this.historyBuffer;
        CreateHistoryBuffer(source);
        Graphics.Blit((Texture) historyBuffer, this.historyBuffer);
        DestroyBuffer(ref historyBuffer);
      }
      material.SetFloat(propertyId, 1f - Mathf.Pow(strength, Time.deltaTime * 30f));
      Graphics.Blit((Texture) source, this.historyBuffer, material);
      Graphics.Blit((Texture) this.historyBuffer, destination);
    }
  }

  private bool FramePropertiesChanged(RenderTexture source)
  {
    return historyBuffer.width != source.width || historyBuffer.height != source.height || historyBuffer.format != source.format;
  }

  private bool MaterialValid()
  {
    if ((Object) material != (Object) null)
      return true;
    if ((Object) blendShader == (Object) null || !blendShader.isSupported)
      return false;
    material = new Material(blendShader);
    propertyId = Shader.PropertyToID("_Opacity");
    return true;
  }

  private void CreateHistoryBuffer(RenderTexture source)
  {
    historyBuffer = new RenderTexture(source.width, source.height, 0, source.format, source.sRGB ? RenderTextureReadWrite.sRGB : RenderTextureReadWrite.Linear);
  }

  private void DestroyBuffer(ref RenderTexture buffer)
  {
    buffer.Release();
    Object.Destroy((Object) buffer);
    buffer = (RenderTexture) null;
  }
}
