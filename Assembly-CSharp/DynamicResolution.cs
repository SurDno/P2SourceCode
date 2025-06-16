using System.Collections.Generic;

public class DynamicResolution
{
  private static DynamicResolution instance;
  private List<Camera> cameras = new List<Camera>();
  private int lastFrame;
  private float scale = 1f;
  private RenderTexture targetTexture = (RenderTexture) null;

  public static DynamicResolution Instance
  {
    get
    {
      if (instance == null)
        instance = new DynamicResolution();
      return instance;
    }
  }

  public void AddCamera(Camera camera) => cameras.Add(camera);

  private void DestroyTexture()
  {
    for (int index = 0; index < cameras.Count; ++index)
      cameras[index].targetTexture = (RenderTexture) null;
    targetTexture.Release();
    Object.Destroy((Object) targetTexture);
    targetTexture = (RenderTexture) null;
  }

  public RenderTexture GetTargetTexture()
  {
    if (lastFrame == Time.frameCount)
      return targetTexture;
    lastFrame = Time.frameCount;
    if (scale == 1.0)
    {
      if ((Object) targetTexture != (Object) null)
        DestroyTexture();
      return targetTexture;
    }
    if (Screen.width < 1 || Screen.height < 1)
      return targetTexture;
    int width = Mathf.RoundToInt((float) ((double) Screen.width * scale / 2.0)) * 2;
    if (width < 1)
      width = 1;
    int height = Mathf.RoundToInt((float) ((double) Screen.height * scale / 2.0)) * 2;
    if (height < 1)
      height = 1;
    if ((Object) targetTexture != (Object) null && (targetTexture.width != width || targetTexture.height != height))
      DestroyTexture();
    if ((Object) targetTexture == (Object) null)
      targetTexture = new RenderTexture(width, height, 32, RenderTextureFormat.DefaultHDR);
    return targetTexture;
  }

  public void RemoveCamera(Camera camera)
  {
    cameras.Remove(camera);
    if (cameras.Count != 0 || !((Object) targetTexture != (Object) null))
      return;
    DestroyTexture();
  }

  public void SetScale(float value) => scale = value;
}
