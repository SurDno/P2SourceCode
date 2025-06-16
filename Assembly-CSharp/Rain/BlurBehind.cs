// Decompiled with JetBrains decompiler
// Type: Rain.BlurBehind
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Rain
{
  [AddComponentMenu("Image Effects/Blur Behind")]
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  public class BlurBehind : MonoBehaviour
  {
    private static RenderTexture storedTexture = (RenderTexture) null;
    private static int count = 0;
    private static Rect storedRect = new Rect(0.0f, 0.0f, 1f, 1f);
    public static Shader blurShader = (Shader) null;
    private Material blurMaterial = (Material) null;
    public BlurBehind.Mode mode = BlurBehind.Mode.Relative;
    public float radius = 15f;
    public BlurBehind.Settings settings = BlurBehind.Settings.Manual;
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
      Shader.SetGlobalVector("_BlurBehindRect", new Vector4((BlurBehind.storedRect.x - rect.x) / rect.width, BlurBehind.storedRect.y / rect.height + rect.y, BlurBehind.storedRect.width / rect.width, BlurBehind.storedRect.height / rect.height));
    }

    public static void ResetViewport()
    {
      Shader.SetGlobalVector("_BlurBehindRect", new Vector4(BlurBehind.storedRect.x, BlurBehind.storedRect.y, BlurBehind.storedRect.width, BlurBehind.storedRect.height));
    }

    private void CheckSettings(int sourceSize)
    {
      if ((double) this.radius < 0.0)
        this.radius = 0.0f;
      if ((double) this.downsample < 1.0)
        this.downsample = 1f;
      if (this.iterations < 0)
        this.iterations = 0;
      if (this.settings == BlurBehind.Settings.Manual)
        return;
      float p = this.settings == BlurBehind.Settings.Standard ? 36f : 6f;
      if (this.mode == BlurBehind.Mode.Absolute)
      {
        if ((double) this.radius > 0.0)
        {
          this.iterations = (double) this.radius >= (double) p ? Mathf.FloorToInt(Mathf.Log(this.radius, p)) + 1 : 1;
          this.downsample = this.radius / Mathf.Pow(3f, (float) this.iterations);
          if ((double) this.downsample < 1.0)
            this.downsample = 1f;
        }
        else
        {
          this.downsample = 1f;
          this.iterations = 0;
        }
      }
      else if ((double) this.radius > 0.0)
      {
        float f = this.radius / 100f * (float) sourceSize;
        this.iterations = (double) f >= (double) p ? Mathf.FloorToInt(Mathf.Log(f, p)) + 1 : 1;
        this.downsample = (float) sourceSize / (f / Mathf.Pow(3f, (float) this.iterations));
      }
      else
      {
        this.downsample = float.PositiveInfinity;
        this.iterations = 0;
      }
    }

    private void CheckOutput(int rtW, int rtH, RenderTextureFormat format)
    {
      if ((Object) BlurBehind.storedTexture == (Object) null)
        this.CreateOutput(rtW, rtH, format);
      else if (BlurBehind.storedTexture.width != rtW || BlurBehind.storedTexture.height != rtH || BlurBehind.storedTexture.format != format)
      {
        BlurBehind.storedTexture.Release();
        Object.DestroyImmediate((Object) BlurBehind.storedTexture);
        this.CreateOutput(rtW, rtH, format);
      }
      else
        BlurBehind.storedTexture.DiscardContents();
    }

    private bool CheckResources()
    {
      if ((Object) this.blurMaterial == (Object) null)
      {
        if ((Object) BlurBehind.blurShader == (Object) null)
          BlurBehind.blurShader = Shader.Find("Hidden/Blur Behind/Blur");
        if ((Object) BlurBehind.blurShader != (Object) null)
        {
          if (BlurBehind.blurShader.isSupported)
          {
            this.blurMaterial = new Material(BlurBehind.blurShader);
            this.blurMaterial.hideFlags = HideFlags.DontSave;
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
      BlurBehind.storedTexture = new RenderTexture(width, height, 0, format);
      BlurBehind.storedTexture.filterMode = FilterMode.Bilinear;
      BlurBehind.storedTexture.hideFlags = HideFlags.DontSave;
      Shader.SetGlobalTexture("_BlurBehindTex", (Texture) BlurBehind.storedTexture);
      Shader.EnableKeyword("BLUR_BEHIND_SET");
    }

    private RenderTexture CropSource(RenderTexture source)
    {
      Rect rect1 = new Rect(this.cropRect.x * (float) source.width + this.pixelOffset.x, this.cropRect.y * (float) source.height + this.pixelOffset.y, this.cropRect.width * (float) source.width + this.pixelOffset.width, this.cropRect.height * (float) source.height + this.pixelOffset.height);
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
        this.blurMaterial.SetVector("_Parameter", new Vector4(rect1.x, rect1.y, rect1.width, rect1.height));
        Graphics.Blit((Texture) source, dest, this.blurMaterial, 2);
        BlurBehind.storedRect = rect1;
      }
      else
      {
        dest = source;
        BlurBehind.storedRect = new Rect(0.0f, 0.0f, 1f, 1f);
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
        BlurBehind.storedRect = new Rect(rect2.x + BlurBehind.storedRect.x * rect2.width, rect2.y + BlurBehind.storedRect.y * rect2.height, rect2.width * BlurBehind.storedRect.width, rect2.height * BlurBehind.storedRect.height);
      }
      Shader.SetGlobalVector("_BlurBehindRect", new Vector4(BlurBehind.storedRect.x, BlurBehind.storedRect.y, BlurBehind.storedRect.width, BlurBehind.storedRect.height));
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
          this.blurMaterial.SetVector("_Parameter", new Vector4(renderTexture.texelSize.x, renderTexture.texelSize.y, -renderTexture.texelSize.x, -renderTexture.texelSize.y));
          Graphics.Blit((Texture) renderTexture, temporary, this.blurMaterial, 1);
          if ((Object) renderTexture != (Object) source)
            RenderTexture.ReleaseTemporary(renderTexture);
          renderTexture = temporary;
        }
        if (num > 1)
        {
          this.blurMaterial.SetVector("_Parameter", new Vector4(renderTexture.texelSize.x, renderTexture.texelSize.y, -renderTexture.texelSize.x, -renderTexture.texelSize.y));
          Graphics.Blit((Texture) renderTexture, dest, this.blurMaterial, 1);
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
      if ((bool) (Object) this.blurMaterial)
      {
        Object.Destroy((Object) this.blurMaterial);
        this.blurMaterial = (Material) null;
      }
      --BlurBehind.count;
      if (BlurBehind.count != 0 || !(bool) (Object) BlurBehind.storedTexture)
        return;
      BlurBehind.storedTexture.Release();
      Object.Destroy((Object) BlurBehind.storedTexture);
      BlurBehind.storedTexture = (RenderTexture) null;
      Shader.SetGlobalTexture("_BlurBehindTex", (Texture) null);
      Shader.DisableKeyword("BLUR_BEHIND_SET");
    }

    private void OnEnable() => ++BlurBehind.count;

    private void OnPreRender() => BlurBehind.SetViewport();

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!this.CheckSupport() || !this.CheckResources())
      {
        this.enabled = false;
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        RenderTexture renderTexture = this.CropSource(source);
        int sourceSize = renderTexture.width > renderTexture.height ? renderTexture.width : renderTexture.height;
        this.CheckSettings(sourceSize);
        int width;
        int height;
        this.SetOutputSize(source, renderTexture, out width, out height);
        this.CheckOutput(width, height, renderTexture.format);
        this.Downsample(renderTexture, BlurBehind.storedTexture);
        if ((Object) renderTexture != (Object) source)
          RenderTexture.ReleaseTemporary(renderTexture);
        if (this.iterations > 0 && (double) this.radius > 0.0)
        {
          RenderTexture temporary = RenderTexture.GetTemporary(width, height, 0, renderTexture.format);
          temporary.filterMode = FilterMode.Bilinear;
          for (int p = 0; p < this.iterations; ++p)
          {
            float num = this.radius / 300f * Mathf.Pow(3f, (float) p) / Mathf.Pow(3f, (float) (this.iterations - 1));
            if (this.mode == BlurBehind.Mode.Absolute)
              num *= 100f / (float) sourceSize;
            else if ((Object) renderTexture != (Object) source)
              num *= (source.width > source.height ? (float) source.width : (float) source.height) / (float) sourceSize;
            float f = (float) p * 0.7853982f / (float) this.iterations;
            Vector2 vector2_1 = new Vector2(Mathf.Sin(f), Mathf.Cos(f)) * num;
            Vector2 vector2_2 = width > height ? new Vector2(1f, 1f / (float) height * (float) width) : new Vector2(1f / (float) width * (float) height, 1f);
            Vector4 vector4 = new Vector4(vector2_1.x * vector2_2.x, vector2_1.y * vector2_2.y, 0.0f, 0.0f);
            vector4.z = -vector4.x;
            vector4.w = -vector4.y;
            this.blurMaterial.SetVector("_Parameter", vector4);
            Graphics.Blit((Texture) BlurBehind.storedTexture, temporary, this.blurMaterial, 0);
            BlurBehind.storedTexture.DiscardContents();
            vector4 = new Vector4(vector2_1.y * vector2_2.x, -vector2_1.x * vector2_2.y, 0.0f, 0.0f);
            vector4.z = -vector4.x;
            vector4.w = -vector4.y;
            this.blurMaterial.SetVector("_Parameter", vector4);
            Graphics.Blit((Texture) temporary, BlurBehind.storedTexture, this.blurMaterial, 0);
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
      float num = this.mode != BlurBehind.Mode.Absolute ? (source.width <= source.height ? ((double) source.height <= (double) this.downsample ? 1f : this.downsample / (float) source.height) : ((double) source.width <= (double) this.downsample ? 1f : this.downsample / (float) source.width)) : 1f / this.downsample;
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
