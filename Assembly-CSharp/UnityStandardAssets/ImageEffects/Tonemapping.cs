using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ImageEffectAllowedInSceneView]
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Color Adjustments/Tonemapping")]
  public class Tonemapping : PostEffectsBase
  {
    public TonemapperType type = TonemapperType.Photographic;
    public AdaptiveTexSize adaptiveTextureSize = AdaptiveTexSize.Square256;
    public AnimationCurve remapCurve;
    private Texture2D curveTex;
    public float exposureAdjustment = 1.5f;
    public float middleGrey = 0.4f;
    public float white = 2f;
    public float adaptionSpeed = 1.5f;
    public Shader tonemapper;
    public bool validRenderTextureFormat = true;
    private Material tonemapMaterial;
    private RenderTexture rt;
    private RenderTextureFormat rtFormat = RenderTextureFormat.ARGBHalf;

    public override bool CheckResources()
    {
      CheckSupport(false, true);
      tonemapMaterial = CheckShaderAndCreateMaterial(tonemapper, tonemapMaterial);
      if (!(bool) (Object) curveTex && type == TonemapperType.UserCurve)
      {
        curveTex = new Texture2D(256, 1, TextureFormat.ARGB32, false, true);
        curveTex.filterMode = FilterMode.Bilinear;
        curveTex.wrapMode = TextureWrapMode.Clamp;
        curveTex.hideFlags = HideFlags.DontSave;
      }
      if (!isSupported)
        ReportAutoDisable();
      return isSupported;
    }

    public float UpdateCurve()
    {
      float num1 = 1f;
      if (remapCurve.keys.Length < 1)
        remapCurve = new AnimationCurve(new Keyframe(0.0f, 0.0f), new Keyframe(2f, 1f));
      if (remapCurve != null)
      {
        if (remapCurve.length > 0)
          num1 = remapCurve[remapCurve.length - 1].time;
        for (float num2 = 0.0f; num2 <= 1.0; num2 += 0.003921569f)
        {
          float num3 = remapCurve.Evaluate(num2 * 1f * num1);
          curveTex.SetPixel((int) Mathf.Floor(num2 * byte.MaxValue), 0, new Color(num3, num3, num3));
        }
        curveTex.Apply();
      }
      return 1f / num1;
    }

    private void OnDisable()
    {
      if ((bool) (Object) rt)
      {
        DestroyImmediate(rt);
        rt = null;
      }
      if ((bool) (Object) tonemapMaterial)
      {
        DestroyImmediate(tonemapMaterial);
        tonemapMaterial = null;
      }
      if (!(bool) (Object) curveTex)
        return;
      DestroyImmediate(curveTex);
      curveTex = null;
    }

    private bool CreateInternalRenderTexture()
    {
      if ((bool) (Object) rt)
        return false;
      rtFormat = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGHalf) ? RenderTextureFormat.RGHalf : RenderTextureFormat.ARGBHalf;
      rt = new RenderTexture(1, 1, 0, rtFormat);
      rt.hideFlags = HideFlags.DontSave;
      return true;
    }

    [ImageEffectTransformsToLDR]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!CheckResources())
      {
        Graphics.Blit(source, destination);
      }
      else
      {
        exposureAdjustment = exposureAdjustment < 1.0 / 1000.0 ? 1f / 1000f : exposureAdjustment;
        if (type == TonemapperType.UserCurve)
        {
          tonemapMaterial.SetFloat("_RangeScale", UpdateCurve());
          tonemapMaterial.SetTexture("_Curve", curveTex);
          Graphics.Blit(source, destination, tonemapMaterial, 4);
        }
        else if (type == TonemapperType.SimpleReinhard)
        {
          tonemapMaterial.SetFloat("_ExposureAdjustment", exposureAdjustment);
          Graphics.Blit(source, destination, tonemapMaterial, 6);
        }
        else if (type == TonemapperType.Hable)
        {
          tonemapMaterial.SetFloat("_ExposureAdjustment", exposureAdjustment);
          Graphics.Blit(source, destination, tonemapMaterial, 5);
        }
        else if (type == TonemapperType.Photographic)
        {
          tonemapMaterial.SetFloat("_ExposureAdjustment", exposureAdjustment);
          Graphics.Blit(source, destination, tonemapMaterial, 8);
        }
        else if (type == TonemapperType.OptimizedHejiDawson)
        {
          tonemapMaterial.SetFloat("_ExposureAdjustment", 0.5f * exposureAdjustment);
          Graphics.Blit(source, destination, tonemapMaterial, 7);
        }
        else
        {
          bool internalRenderTexture = CreateInternalRenderTexture();
          RenderTexture temporary = RenderTexture.GetTemporary((int) adaptiveTextureSize, (int) adaptiveTextureSize, 0, rtFormat);
          Graphics.Blit(source, temporary);
          int length = (int) Mathf.Log(temporary.width * 1f, 2f);
          int num = 2;
          RenderTexture[] renderTextureArray = new RenderTexture[length];
          for (int index = 0; index < length; ++index)
          {
            renderTextureArray[index] = RenderTexture.GetTemporary(temporary.width / num, temporary.width / num, 0, rtFormat);
            num *= 2;
          }
          RenderTexture source1 = renderTextureArray[length - 1];
          Graphics.Blit(temporary, renderTextureArray[0], tonemapMaterial, 1);
          if (type == TonemapperType.AdaptiveReinhardAutoWhite)
          {
            for (int index = 0; index < length - 1; ++index)
            {
              Graphics.Blit(renderTextureArray[index], renderTextureArray[index + 1], tonemapMaterial, 9);
              source1 = renderTextureArray[index + 1];
            }
          }
          else if (type == TonemapperType.AdaptiveReinhard)
          {
            for (int index = 0; index < length - 1; ++index)
            {
              Graphics.Blit(renderTextureArray[index], renderTextureArray[index + 1]);
              source1 = renderTextureArray[index + 1];
            }
          }
          adaptionSpeed = adaptionSpeed < 1.0 / 1000.0 ? 1f / 1000f : adaptionSpeed;
          tonemapMaterial.SetFloat("_AdaptionSpeed", adaptionSpeed);
          rt.MarkRestoreExpected();
          Graphics.Blit(source1, rt, tonemapMaterial, internalRenderTexture ? 3 : 2);
          middleGrey = middleGrey < 1.0 / 1000.0 ? 1f / 1000f : middleGrey;
          tonemapMaterial.SetVector("_HdrParams", new Vector4(middleGrey, middleGrey, middleGrey, white * white));
          tonemapMaterial.SetTexture("_SmallTex", rt);
          if (type == TonemapperType.AdaptiveReinhard)
            Graphics.Blit(source, destination, tonemapMaterial, 0);
          else if (type == TonemapperType.AdaptiveReinhardAutoWhite)
          {
            Graphics.Blit(source, destination, tonemapMaterial, 10);
          }
          else
          {
            Debug.LogError("No valid adaptive tonemapper type found!");
            Graphics.Blit(source, destination);
          }
          for (int index = 0; index < length; ++index)
            RenderTexture.ReleaseTemporary(renderTextureArray[index]);
          RenderTexture.ReleaseTemporary(temporary);
        }
      }
    }

    public enum TonemapperType
    {
      SimpleReinhard,
      UserCurve,
      Hable,
      Photographic,
      OptimizedHejiDawson,
      AdaptiveReinhard,
      AdaptiveReinhardAutoWhite,
    }

    public enum AdaptiveTexSize
    {
      Square16 = 16, // 0x00000010
      Square32 = 32, // 0x00000020
      Square64 = 64, // 0x00000040
      Square128 = 128, // 0x00000080
      Square256 = 256, // 0x00000100
      Square512 = 512, // 0x00000200
      Square1024 = 1024, // 0x00000400
    }
  }
}
