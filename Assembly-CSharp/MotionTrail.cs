using UnityEngine;

[ExecuteInEditMode]
public class MotionTrail : MonoBehaviour
{
  [SerializeField]
  private Shader blendShader;
  [SerializeField]
  [Range(0.0f, 1f)]
  [Tooltip("Approximate opacity of the previous frame at 30 fps")]
  private float strength;
  private Material material;
  private int propertyId;
  private RenderTexture historyBuffer;

  public float Strength
  {
    get => strength;
    set => strength = Mathf.Clamp01(value);
  }

  private void OnDisable()
  {
    if (!(historyBuffer != null))
      return;
    DestroyBuffer(ref historyBuffer);
  }

  private void OnRenderImage(RenderTexture source, RenderTexture destination)
  {
    if (strength == 0.0 || !MaterialValid())
    {
      if (historyBuffer != null)
        DestroyBuffer(ref historyBuffer);
      Graphics.Blit(source, destination);
    }
    else if (this.historyBuffer == null)
    {
      CreateHistoryBuffer(source);
      Graphics.Blit(source, historyBuffer);
      Graphics.Blit(source, destination);
    }
    else
    {
      if (FramePropertiesChanged(source))
      {
        RenderTexture historyBuffer = this.historyBuffer;
        CreateHistoryBuffer(source);
        Graphics.Blit(historyBuffer, this.historyBuffer);
        DestroyBuffer(ref historyBuffer);
      }
      material.SetFloat(propertyId, 1f - Mathf.Pow(strength, Time.deltaTime * 30f));
      Graphics.Blit(source, this.historyBuffer, material);
      Graphics.Blit(this.historyBuffer, destination);
    }
  }

  private bool FramePropertiesChanged(RenderTexture source)
  {
    return historyBuffer.width != source.width || historyBuffer.height != source.height || historyBuffer.format != source.format;
  }

  private bool MaterialValid()
  {
    if (material != null)
      return true;
    if (blendShader == null || !blendShader.isSupported)
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
    Destroy(buffer);
    buffer = null;
  }
}
