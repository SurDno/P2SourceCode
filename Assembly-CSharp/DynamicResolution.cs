using System.Collections.Generic;
using UnityEngine;

public class DynamicResolution
{
  private static DynamicResolution instance;
  private List<Camera> cameras = new List<Camera>();
  private int lastFrame = 0;
  private float scale = 1f;
  private RenderTexture targetTexture = (RenderTexture) null;

  public static DynamicResolution Instance
  {
    get
    {
      if (DynamicResolution.instance == null)
        DynamicResolution.instance = new DynamicResolution();
      return DynamicResolution.instance;
    }
  }

  public void AddCamera(Camera camera) => this.cameras.Add(camera);

  private void DestroyTexture()
  {
    for (int index = 0; index < this.cameras.Count; ++index)
      this.cameras[index].targetTexture = (RenderTexture) null;
    this.targetTexture.Release();
    Object.Destroy((Object) this.targetTexture);
    this.targetTexture = (RenderTexture) null;
  }

  public RenderTexture GetTargetTexture()
  {
    if (this.lastFrame == Time.frameCount)
      return this.targetTexture;
    this.lastFrame = Time.frameCount;
    if ((double) this.scale == 1.0)
    {
      if ((Object) this.targetTexture != (Object) null)
        this.DestroyTexture();
      return this.targetTexture;
    }
    if (Screen.width < 1 || Screen.height < 1)
      return this.targetTexture;
    int width = Mathf.RoundToInt((float) ((double) Screen.width * (double) this.scale / 2.0)) * 2;
    if (width < 1)
      width = 1;
    int height = Mathf.RoundToInt((float) ((double) Screen.height * (double) this.scale / 2.0)) * 2;
    if (height < 1)
      height = 1;
    if ((Object) this.targetTexture != (Object) null && (this.targetTexture.width != width || this.targetTexture.height != height))
      this.DestroyTexture();
    if ((Object) this.targetTexture == (Object) null)
      this.targetTexture = new RenderTexture(width, height, 32, RenderTextureFormat.DefaultHDR);
    return this.targetTexture;
  }

  public void RemoveCamera(Camera camera)
  {
    this.cameras.Remove(camera);
    if (this.cameras.Count != 0 || !((Object) this.targetTexture != (Object) null))
      return;
    this.DestroyTexture();
  }

  public void SetScale(float value) => this.scale = value;
}
