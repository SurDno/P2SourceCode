namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [AddComponentMenu("Image Effects/Color Adjustments/Color Correction (3D Lookup Texture)")]
  public class ColorCorrectionLookup : PostEffectsBase
  {
    public Shader shader;
    private Material material;
    public Texture3D converted3DLut = (Texture3D) null;
    public string basedOnTempTex = "";

    public override bool CheckResources()
    {
      CheckSupport(false);
      material = CheckShaderAndCreateMaterial(shader, material);
      if (!isSupported || !SystemInfo.supports3DTextures)
        ReportAutoDisable();
      return isSupported;
    }

    private void OnDisable()
    {
      if (!(bool) (Object) material)
        return;
      Object.DestroyImmediate((Object) material);
      material = (Material) null;
    }

    private void OnDestroy()
    {
      if ((bool) (Object) converted3DLut)
        Object.DestroyImmediate((Object) converted3DLut);
      converted3DLut = (Texture3D) null;
    }

    public void SetIdentityLut()
    {
      int num1 = 16;
      Color[] colors = new Color[num1 * num1 * num1];
      float num2 = (float) (1.0 / (1.0 * num1 - 1.0));
      for (int index1 = 0; index1 < num1; ++index1)
      {
        for (int index2 = 0; index2 < num1; ++index2)
        {
          for (int index3 = 0; index3 < num1; ++index3)
            colors[index1 + index2 * num1 + index3 * num1 * num1] = new Color(index1 * 1f * num2, index2 * 1f * num2, index3 * 1f * num2, 1f);
        }
      }
      if ((bool) (Object) converted3DLut)
        Object.DestroyImmediate((Object) converted3DLut);
      converted3DLut = new Texture3D(num1, num1, num1, TextureFormat.ARGB32, false);
      converted3DLut.SetPixels(colors);
      converted3DLut.Apply();
      basedOnTempTex = "";
    }

    public bool ValidDimensions(Texture2D tex2d)
    {
      return (bool) (Object) tex2d && tex2d.height == Mathf.FloorToInt(Mathf.Sqrt((float) tex2d.width));
    }

    public void Convert(Texture2D temp2DTex, string path)
    {
      if ((bool) (Object) temp2DTex)
      {
        int num1 = temp2DTex.width * temp2DTex.height;
        int height = temp2DTex.height;
        if (!ValidDimensions(temp2DTex))
        {
          Debug.LogWarning((object) ("The given 2D texture " + temp2DTex.name + " cannot be used as a 3D LUT."));
          basedOnTempTex = "";
        }
        else
        {
          Color[] pixels = temp2DTex.GetPixels();
          Color[] colors = new Color[pixels.Length];
          for (int index1 = 0; index1 < height; ++index1)
          {
            for (int index2 = 0; index2 < height; ++index2)
            {
              for (int index3 = 0; index3 < height; ++index3)
              {
                int num2 = height - index2 - 1;
                colors[index1 + index2 * height + index3 * height * height] = pixels[index3 * height + index1 + num2 * height * height];
              }
            }
          }
          if ((bool) (Object) converted3DLut)
            Object.DestroyImmediate((Object) converted3DLut);
          converted3DLut = new Texture3D(height, height, height, TextureFormat.ARGB32, false);
          converted3DLut.SetPixels(colors);
          converted3DLut.Apply();
          basedOnTempTex = path;
        }
      }
      else
        Debug.LogError((object) "Couldn't color correct with 3D LUT texture. Image Effect will be disabled.");
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!CheckResources() || !SystemInfo.supports3DTextures)
      {
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        if ((Object) converted3DLut == (Object) null)
          SetIdentityLut();
        int width = converted3DLut.width;
        converted3DLut.wrapMode = TextureWrapMode.Clamp;
        material.SetFloat("_Scale", (width - 1) / (1f * width));
        material.SetFloat("_Offset", (float) (1.0 / (2.0 * width)));
        material.SetTexture("_ClutTex", (Texture) converted3DLut);
        Graphics.Blit((Texture) source, destination, material, QualitySettings.activeColorSpace == ColorSpace.Linear ? 1 : 0);
      }
    }
  }
}
