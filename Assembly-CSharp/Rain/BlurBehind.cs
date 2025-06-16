namespace Rain
{
  [AddComponentMenu("Image Effects/Blur Behind")]
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  public class BlurBehind : MonoBehaviour
  {
    private static RenderTexture storedTexture = (RenderTexture) null;
    private static int count;
    private static Rect storedRect = new Rect(0.0f, 0.0f, 1f, 1f);
    public static Shader blurShader = (Shader) null;
    private Material blurMaterial = (Material) null;
    public Mode mode = Mode.Relative;
    public float radius = 15f;
    public Settings settings = Settings.Manual;
    public float downsample = 64f;
    public int iterations = 2;
    public Rect cropRect = new Rect(0.0f, 0.0f, 1f, 1f);
    public Rect pixelOffset = new Rect(0.0f, 0.0f, 0.0f, 0.0f);

    public static void SetViewport()
    {
      Rect rect = Camera.current.rect;
      if (!(rect != new Rect(0.0f, 0.0f, 1f, 1f)))
        return;
      Vector2 vector2 = !((Object) Camera.current.targetTexture == (Object) null) ? new Vector2((float) Camera.current.targetTexture.width, (float) Camera.current.targetTexture.height) : new Vector2((float) Screen.width, (float) Screen.height);
      rect.width = Mathf.Round(Mathf.Clamp01(rect.width + rect.x) * vector2.x) / vector2.x;
      rect.height = Mathf.Round(Mathf.Clamp01(rect.height + rect.y) * vector2.y) / vector2.y;
      rect.x = Mathf.Round(Mathf.Clamp01(rect.x) * vector2.x) / vector2.x;
      rect.y = Mathf.Round(Mathf.Clamp01(rect.y) * vector2.y) / vector2.y;
      rect.width -= rect.x;
      rect.height -= rect.y;
      Shader.SetGlobalVector("_BlurBehindRect", new Vector4((storedRect.x - rect.x) / rect.width, storedRect.y / rect.height + rect.y, storedRect.width / rect.width, storedRect.height / rect.height));
    }

    public static void ResetViewport()
    {
      Shader.SetGlobalVector("_BlurBehindRect", new Vector4(storedRect.x, storedRect.y, storedRect.width, storedRect.height));
    }

    private void CheckSettings(int sourceSize)
    {
      if (radius < 0.0)
        radius = 0.0f;
      if (downsample < 1.0)
        downsample = 1f;
      if (iterations < 0)
        iterations = 0;
      if (settings == Settings.Manual)
        return;
      float p = settings == Settings.Standard ? 36f : 6f;
      if (mode == Mode.Absolute)
      {
        if (radius > 0.0)
        {
          iterations = radius >= (double) p ? Mathf.FloorToInt(Mathf.Log(radius, p)) + 1 : 1;
          downsample = radius / Mathf.Pow(3f, (float) iterations);
          if (downsample < 1.0)
            downsample = 1f;
        }
        else
        {
          downsample = 1f;
          iterations = 0;
        }
      }
      else if (radius > 0.0)
      {
        float f = radius / 100f * sourceSize;
        iterations = f >= (double) p ? Mathf.FloorToInt(Mathf.Log(f, p)) + 1 : 1;
        downsample = sourceSize / (f / Mathf.Pow(3f, (float) iterations));
      }
      else
      {
        downsample = float.PositiveInfinity;
        iterations = 0;
      }
    }

    private void CheckOutput(int rtW, int rtH, RenderTextureFormat format)
    {
      if ((Object) storedTexture == (Object) null)
        CreateOutput(rtW, rtH, format);
      else if (storedTexture.width != rtW || storedTexture.height != rtH || storedTexture.format != format)
      {
        storedTexture.Release();
        Object.DestroyImmediate((Object) storedTexture);
        CreateOutput(rtW, rtH, format);
      }
      else
        storedTexture.DiscardContents();
    }

    private bool CheckResources()
    {
      if ((Object) blurMaterial == (Object) null)
      {
        if ((Object) blurShader == (Object) null)
          blurShader = Shader.Find("Hidden/Blur Behind/Blur");
        if ((Object) blurShader != (Object) null)
        {
          if (blurShader.isSupported)
          {
            blurMaterial = new Material(blurShader);
            blurMaterial.hideFlags = HideFlags.DontSave;
          }
          else
          {
            Debug.Log((object) "Blur Behind UI: Shader not supported");
            return false;
          }
        }
        else
        {
          Debug.Log((object) "Blur Behind UI: Shader reference missing");
          return false;
        }
      }
      return true;
    }

    private bool CheckSupport()
    {
      if (SystemInfo.supportsImageEffects)
        return true;
      Debug.Log((object) "Blur Behind UI: Image effects not supported");
      return false;
    }

    private void CreateOutput(int width, int height, RenderTextureFormat format)
    {
      storedTexture = new RenderTexture(width, height, 0, format);
      storedTexture.filterMode = FilterMode.Bilinear;
      storedTexture.hideFlags = HideFlags.DontSave;
      Shader.SetGlobalTexture("_BlurBehindTex", (Texture) storedTexture);
      Shader.EnableKeyword("BLUR_BEHIND_SET");
    }

    private RenderTexture CropSource(RenderTexture source)
    {
      Rect rect1 = new Rect(cropRect.x * (float) source.width + pixelOffset.x, cropRect.y * (float) source.height + pixelOffset.y, cropRect.width * (float) source.width + pixelOffset.width, cropRect.height * (float) source.height + pixelOffset.height);
      rect1.width = Mathf.Clamp01(Mathf.Round(rect1.width + rect1.x) / (float) source.width);
      rect1.height = Mathf.Clamp01(Mathf.Round(rect1.height + rect1.y) / (float) source.height);
      rect1.x = Mathf.Clamp01(Mathf.Round(rect1.x) / (float) source.width);
      rect1.y = Mathf.Clamp01(Mathf.Round(rect1.y) / (float) source.height);
      rect1.width -= rect1.x;
      rect1.height -= rect1.y;
      RenderTexture dest;
      if (rect1 != new Rect(0.0f, 0.0f, 1f, 1f))
      {
        dest = RenderTexture.GetTemporary(Mathf.RoundToInt(rect1.width * (float) source.width), Mathf.RoundToInt(rect1.height * (float) source.height), 0, source.format);
        blurMaterial.SetVector("_Parameter", new Vector4(rect1.x, rect1.y, rect1.width, rect1.height));
        Graphics.Blit((Texture) source, dest, blurMaterial, 2);
        storedRect = rect1;
      }
      else
      {
        dest = source;
        storedRect = new Rect(0.0f, 0.0f, 1f, 1f);
      }
      Rect rect2 = Camera.current.rect;
      if (rect2 != new Rect(0.0f, 0.0f, 1f, 1f))
      {
        Vector2 vector2 = !((Object) Camera.current.targetTexture == (Object) null) ? new Vector2((float) Camera.current.targetTexture.width, (float) Camera.current.targetTexture.height) : new Vector2((float) Screen.width, (float) Screen.height);
        rect2.width = Mathf.Round(Mathf.Clamp01(rect2.width + rect2.x) * vector2.x) / vector2.x;
        rect2.height = Mathf.Round(Mathf.Clamp01(rect2.height + rect2.y) * vector2.y) / vector2.y;
        rect2.x = Mathf.Round(Mathf.Clamp01(rect2.x) * vector2.x) / vector2.x;
        rect2.y = Mathf.Round(Mathf.Clamp01(rect2.y) * vector2.y) / vector2.y;
        rect2.width -= rect2.x;
        rect2.height -= rect2.y;
        storedRect = new Rect(rect2.x + storedRect.x * rect2.width, rect2.y + storedRect.y * rect2.height, rect2.width * storedRect.width, rect2.height * storedRect.height);
      }
      Shader.SetGlobalVector("_BlurBehindRect", new Vector4(storedRect.x, storedRect.y, storedRect.width, storedRect.height));
      return dest;
    }

    private void Downsample(RenderTexture source, RenderTexture dest)
    {
      int num = 0;
      if (source.width > source.height)
      {
        for (int width = source.width; width > dest.width; width >>= 1)
          ++num;
      }
      else
      {
        for (int height = source.height; height > dest.height; height >>= 1)
          ++num;
      }
      if (num > 1)
      {
        RenderTexture renderTexture = source;
        for (; num > 2; num -= 2)
        {
          int width = renderTexture.width >> 2;
          if (width < 1)
            width = 1;
          int height = renderTexture.height >> 2;
          if (height < 1)
            height = 1;
          renderTexture.filterMode = FilterMode.Bilinear;
          RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, renderTexture.format);
          blurMaterial.SetVector("_Parameter", new Vector4(renderTexture.texelSize.x, renderTexture.texelSize.y, -renderTexture.texelSize.x, -renderTexture.texelSize.y));
          Graphics.Blit((Texture) renderTexture, temporary, blurMaterial, 1);
          if ((Object) renderTexture != (Object) source)
            RenderTexture.ReleaseTemporary(renderTexture);
          renderTexture = temporary;
        }
        if (num > 1)
        {
          blurMaterial.SetVector("_Parameter", new Vector4(renderTexture.texelSize.x, renderTexture.texelSize.y, -renderTexture.texelSize.x, -renderTexture.texelSize.y));
          Graphics.Blit((Texture) renderTexture, dest, blurMaterial, 1);
        }
        else
          Graphics.Blit((Texture) renderTexture, dest);
        if (!((Object) renderTexture != (Object) source))
          return;
        RenderTexture.ReleaseTemporary(renderTexture);
      }
      else
        Graphics.Blit((Texture) source, dest);
    }

    private void OnDisable()
    {
      if ((bool) (Object) blurMaterial)
      {
        Object.Destroy((Object) blurMaterial);
        blurMaterial = (Material) null;
      }
      --count;
      if (count != 0 || !(bool) (Object) storedTexture)
        return;
      storedTexture.Release();
      Object.Destroy((Object) storedTexture);
      storedTexture = (RenderTexture) null;
      Shader.SetGlobalTexture("_BlurBehindTex", (Texture) null);
      Shader.DisableKeyword("BLUR_BEHIND_SET");
    }

    private void OnEnable() => ++count;

    private void OnPreRender() => SetViewport();

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!CheckSupport() || !CheckResources())
      {
        this.enabled = false;
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        RenderTexture renderTexture = CropSource(source);
        int sourceSize = renderTexture.width > renderTexture.height ? renderTexture.width : renderTexture.height;
        CheckSettings(sourceSize);
        int width;
        int height;
        SetOutputSize(source, renderTexture, out width, out height);
        CheckOutput(width, height, renderTexture.format);
        Downsample(renderTexture, storedTexture);
        if ((Object) renderTexture != (Object) source)
          RenderTexture.ReleaseTemporary(renderTexture);
        if (iterations > 0 && radius > 0.0)
        {
          RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, renderTexture.format);
          temporary.filterMode = FilterMode.Bilinear;
          for (int p = 0; p < iterations; ++p)
          {
            float num = radius / 300f * Mathf.Pow(3f, (float) p) / Mathf.Pow(3f, (float) (iterations - 1));
            if (mode == Mode.Absolute)
              num *= 100f / sourceSize;
            else if ((Object) renderTexture != (Object) source)
              num *= (source.width > source.height ? (float) source.width : (float) source.height) / sourceSize;
            float f = p * 0.7853982f / iterations;
            Vector2 vector2_1 = new Vector2(Mathf.Sin(f), Mathf.Cos(f)) * num;
            Vector2 vector2_2 = width > height ? new Vector2(1f, 1f / height * width) : new Vector2(1f / width * height, 1f);
            Vector4 vector4 = new Vector4(vector2_1.x * vector2_2.x, vector2_1.y * vector2_2.y, 0.0f, 0.0f);
            vector4.z = -vector4.x;
            vector4.w = -vector4.y;
            blurMaterial.SetVector("_Parameter", vector4);
            Graphics.Blit((Texture) storedTexture, temporary, blurMaterial, 0);
            storedTexture.DiscardContents();
            vector4 = new Vector4(vector2_1.y * vector2_2.x, -vector2_1.x * vector2_2.y, 0.0f, 0.0f);
            vector4.z = -vector4.x;
            vector4.w = -vector4.y;
            blurMaterial.SetVector("_Parameter", vector4);
            Graphics.Blit((Texture) temporary, storedTexture, blurMaterial, 0);
            temporary.DiscardContents();
          }
          RenderTexture.ReleaseTemporary(temporary);
        }
        Graphics.Blit((Texture) source, destination);
      }
    }

    private void SetOutputSize(
      RenderTexture source,
      RenderTexture croppedSource,
      out int width,
      out int height)
    {
      float num = mode != Mode.Absolute ? (source.width <= source.height ? ((double) source.height <= downsample ? 1f : downsample / (float) source.height) : ((double) source.width <= downsample ? 1f : downsample / (float) source.width)) : 1f / downsample;
      width = Mathf.RoundToInt((float) croppedSource.width * num);
      height = Mathf.RoundToInt((float) croppedSource.height * num);
      if (width < 1)
        width = 1;
      else if (width > croppedSource.width)
        width = croppedSource.width;
      if (height < 1)
      {
        height = 1;
      }
      else
      {
        if (height <= croppedSource.height)
          return;
        height = croppedSource.height;
      }
    }

    public enum Mode
    {
      Absolute,
      Relative,
    }

    public enum Settings
    {
      Standard,
      Smooth,
      Manual,
    }
  }
}
