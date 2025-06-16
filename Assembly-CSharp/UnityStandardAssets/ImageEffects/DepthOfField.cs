using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Camera/Depth of Field (Lens Blur, Scatter, DX11)")]
  public class DepthOfField : PostEffectsBase
  {
    public bool visualizeFocus;
    public float focalLength = 10f;
    public float focalSize = 0.05f;
    public float aperture = 0.5f;
    public Transform focalTransform;
    public float maxBlurSize = 2f;
    public bool highResolution;
    public BlurType blurType = BlurType.DiscBlur;
    public BlurSampleCount blurSampleCount = BlurSampleCount.High;
    public bool nearBlur;
    public float foregroundOverlap = 1f;
    public Shader dofHdrShader;
    private Material dofHdrMaterial;
    public Shader dx11BokehShader;
    private Material dx11bokehMaterial;
    public float dx11BokehThreshold = 0.5f;
    public float dx11SpawnHeuristic = 0.0875f;
    public Texture2D dx11BokehTexture;
    public float dx11BokehScale = 1.2f;
    public float dx11BokehIntensity = 2.5f;
    private float focalDistance01 = 10f;
    private ComputeBuffer cbDrawArgs;
    private ComputeBuffer cbPoints;
    private float internalBlurWidth = 1f;
    private Camera cachedCamera;

    public override bool CheckResources()
    {
      CheckSupport(true);
      if (dofHdrShader == null)
        dofHdrShader = Shader.Find("Hidden/Dof/DepthOfFieldHdr");
      dofHdrMaterial = CheckShaderAndCreateMaterial(dofHdrShader, dofHdrMaterial);
      if (supportDX11 && blurType == BlurType.DX11)
      {
        if (dofHdrShader == null)
          dofHdrShader = Shader.Find("Hidden/Dof/DX11Dof");
        dx11bokehMaterial = CheckShaderAndCreateMaterial(dx11BokehShader, dx11bokehMaterial);
        CreateComputeResources();
      }
      if (!isSupported)
        ReportAutoDisable();
      return isSupported;
    }

    private void OnEnable()
    {
      cachedCamera = GetComponent<Camera>();
      cachedCamera.depthTextureMode |= DepthTextureMode.Depth;
    }

    private void OnDisable()
    {
      ReleaseComputeResources();
      if ((bool) (Object) dofHdrMaterial)
        DestroyImmediate(dofHdrMaterial);
      dofHdrMaterial = null;
      if ((bool) (Object) dx11bokehMaterial)
        DestroyImmediate(dx11bokehMaterial);
      dx11bokehMaterial = null;
    }

    private void ReleaseComputeResources()
    {
      if (cbDrawArgs != null)
        cbDrawArgs.Release();
      cbDrawArgs = null;
      if (cbPoints != null)
        cbPoints.Release();
      cbPoints = null;
    }

    private void CreateComputeResources()
    {
      if (cbDrawArgs == null)
      {
        cbDrawArgs = new ComputeBuffer(1, 16, ComputeBufferType.DrawIndirect);
        cbDrawArgs.SetData(new int[4]
        {
          0,
          1,
          0,
          0
        });
      }
      if (cbPoints != null)
        return;
      cbPoints = new ComputeBuffer(90000, 28, ComputeBufferType.Append);
    }

    private float FocalDistance01(float worldDist)
    {
      return cachedCamera.WorldToViewportPoint((worldDist - cachedCamera.nearClipPlane) * cachedCamera.transform.forward + cachedCamera.transform.position).z / (cachedCamera.farClipPlane - cachedCamera.nearClipPlane);
    }

    private void WriteCoc(RenderTexture fromTo, bool fgDilate)
    {
      dofHdrMaterial.SetTexture("_FgOverlap", null);
      if (nearBlur & fgDilate)
      {
        int width = fromTo.width / 2;
        int height = fromTo.height / 2;
        RenderTexture temporary1 = RenderTexture.GetTemporary(width, height, 0, fromTo.format);
        Graphics.Blit(fromTo, temporary1, dofHdrMaterial, 4);
        float num = internalBlurWidth * foregroundOverlap;
        dofHdrMaterial.SetVector("_Offsets", new Vector4(0.0f, num, 0.0f, num));
        RenderTexture temporary2 = RenderTexture.GetTemporary(width, height, 0, fromTo.format);
        Graphics.Blit(temporary1, temporary2, dofHdrMaterial, 2);
        RenderTexture.ReleaseTemporary(temporary1);
        dofHdrMaterial.SetVector("_Offsets", new Vector4(num, 0.0f, 0.0f, num));
        RenderTexture temporary3 = RenderTexture.GetTemporary(width, height, 0, fromTo.format);
        Graphics.Blit(temporary2, temporary3, dofHdrMaterial, 2);
        RenderTexture.ReleaseTemporary(temporary2);
        dofHdrMaterial.SetTexture("_FgOverlap", temporary3);
        fromTo.MarkRestoreExpected();
        Graphics.Blit(fromTo, fromTo, dofHdrMaterial, 13);
        RenderTexture.ReleaseTemporary(temporary3);
      }
      else
      {
        fromTo.MarkRestoreExpected();
        Graphics.Blit(fromTo, fromTo, dofHdrMaterial, 0);
      }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!CheckResources())
      {
        Graphics.Blit(source, destination);
      }
      else
      {
        if (aperture < 0.0)
          aperture = 0.0f;
        if (maxBlurSize < 0.10000000149011612)
          maxBlurSize = 0.1f;
        focalSize = Mathf.Clamp(focalSize, 0.0f, 2f);
        internalBlurWidth = Mathf.Max(maxBlurSize, 0.0f);
        focalDistance01 = (bool) (Object) focalTransform ? cachedCamera.WorldToViewportPoint(focalTransform.position).z / cachedCamera.farClipPlane : FocalDistance01(focalLength);
        dofHdrMaterial.SetVector("_CurveParams", new Vector4(1f, focalSize, (float) (1.0 / (1.0 - aperture) - 1.0), focalDistance01));
        RenderTexture renderTexture1 = null;
        RenderTexture renderTexture2 = null;
        float num1 = internalBlurWidth * foregroundOverlap;
        if (visualizeFocus)
        {
          WriteCoc(source, true);
          Graphics.Blit(source, destination, dofHdrMaterial, 16);
        }
        else if (blurType == BlurType.DX11 && (bool) (Object) dx11bokehMaterial)
        {
          if (highResolution)
          {
            internalBlurWidth = internalBlurWidth < 0.10000000149011612 ? 0.1f : internalBlurWidth;
            float num2 = internalBlurWidth * foregroundOverlap;
            renderTexture1 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            RenderTexture temporary1 = RenderTexture.GetTemporary(source.width, source.height, 0, source.format);
            WriteCoc(source, false);
            RenderTexture temporary2 = RenderTexture.GetTemporary(source.width >> 1, source.height >> 1, 0, source.format);
            RenderTexture temporary3 = RenderTexture.GetTemporary(source.width >> 1, source.height >> 1, 0, source.format);
            Graphics.Blit(source, temporary2, dofHdrMaterial, 15);
            dofHdrMaterial.SetVector("_Offsets", new Vector4(0.0f, 1.5f, 0.0f, 1.5f));
            Graphics.Blit(temporary2, temporary3, dofHdrMaterial, 19);
            dofHdrMaterial.SetVector("_Offsets", new Vector4(1.5f, 0.0f, 0.0f, 1.5f));
            Graphics.Blit(temporary3, temporary2, dofHdrMaterial, 19);
            if (nearBlur)
              Graphics.Blit(source, temporary3, dofHdrMaterial, 4);
            dx11bokehMaterial.SetTexture("_BlurredColor", temporary2);
            dx11bokehMaterial.SetFloat("_SpawnHeuristic", dx11SpawnHeuristic);
            dx11bokehMaterial.SetVector("_BokehParams", new Vector4(dx11BokehScale, dx11BokehIntensity, Mathf.Clamp(dx11BokehThreshold, 0.005f, 4f), internalBlurWidth));
            dx11bokehMaterial.SetTexture("_FgCocMask", nearBlur ? temporary3 : (Texture) null);
            Graphics.SetRandomWriteTarget(1, cbPoints);
            Graphics.Blit(source, renderTexture1, dx11bokehMaterial, 0);
            Graphics.ClearRandomWriteTargets();
            if (nearBlur)
            {
              dofHdrMaterial.SetVector("_Offsets", new Vector4(0.0f, num2, 0.0f, num2));
              Graphics.Blit(temporary3, temporary2, dofHdrMaterial, 2);
              dofHdrMaterial.SetVector("_Offsets", new Vector4(num2, 0.0f, 0.0f, num2));
              Graphics.Blit(temporary2, temporary3, dofHdrMaterial, 2);
              Graphics.Blit(temporary3, renderTexture1, dofHdrMaterial, 3);
            }
            Graphics.Blit(renderTexture1, temporary1, dofHdrMaterial, 20);
            dofHdrMaterial.SetVector("_Offsets", new Vector4(internalBlurWidth, 0.0f, 0.0f, internalBlurWidth));
            Graphics.Blit(renderTexture1, source, dofHdrMaterial, 5);
            dofHdrMaterial.SetVector("_Offsets", new Vector4(0.0f, internalBlurWidth, 0.0f, internalBlurWidth));
            Graphics.Blit(source, temporary1, dofHdrMaterial, 21);
            Graphics.SetRenderTarget(temporary1);
            ComputeBuffer.CopyCount(cbPoints, cbDrawArgs, 0);
            dx11bokehMaterial.SetBuffer("pointBuffer", cbPoints);
            dx11bokehMaterial.SetTexture("_MainTex", dx11BokehTexture);
            dx11bokehMaterial.SetVector("_Screen", new Vector3((float) (1.0 / (1.0 * source.width)), (float) (1.0 / (1.0 * source.height)), internalBlurWidth));
            dx11bokehMaterial.SetPass(2);
            Graphics.DrawProceduralIndirect(MeshTopology.Points, cbDrawArgs, 0);
            Graphics.Blit(temporary1, destination);
            RenderTexture.ReleaseTemporary(temporary1);
            RenderTexture.ReleaseTemporary(temporary2);
            RenderTexture.ReleaseTemporary(temporary3);
          }
          else
          {
            renderTexture1 = RenderTexture.GetTemporary(source.width >> 1, source.height >> 1, 0, source.format);
            renderTexture2 = RenderTexture.GetTemporary(source.width >> 1, source.height >> 1, 0, source.format);
            float num3 = internalBlurWidth * foregroundOverlap;
            WriteCoc(source, false);
            source.filterMode = FilterMode.Bilinear;
            Graphics.Blit(source, renderTexture1, dofHdrMaterial, 6);
            RenderTexture temporary4 = RenderTexture.GetTemporary(renderTexture1.width >> 1, renderTexture1.height >> 1, 0, renderTexture1.format);
            RenderTexture temporary5 = RenderTexture.GetTemporary(renderTexture1.width >> 1, renderTexture1.height >> 1, 0, renderTexture1.format);
            Graphics.Blit(renderTexture1, temporary4, dofHdrMaterial, 15);
            dofHdrMaterial.SetVector("_Offsets", new Vector4(0.0f, 1.5f, 0.0f, 1.5f));
            Graphics.Blit(temporary4, temporary5, dofHdrMaterial, 19);
            dofHdrMaterial.SetVector("_Offsets", new Vector4(1.5f, 0.0f, 0.0f, 1.5f));
            Graphics.Blit(temporary5, temporary4, dofHdrMaterial, 19);
            RenderTexture renderTexture3 = null;
            if (nearBlur)
            {
              renderTexture3 = RenderTexture.GetTemporary(source.width >> 1, source.height >> 1, 0, source.format);
              Graphics.Blit(source, renderTexture3, dofHdrMaterial, 4);
            }
            dx11bokehMaterial.SetTexture("_BlurredColor", temporary4);
            dx11bokehMaterial.SetFloat("_SpawnHeuristic", dx11SpawnHeuristic);
            dx11bokehMaterial.SetVector("_BokehParams", new Vector4(dx11BokehScale, dx11BokehIntensity, Mathf.Clamp(dx11BokehThreshold, 0.005f, 4f), internalBlurWidth));
            dx11bokehMaterial.SetTexture("_FgCocMask", renderTexture3);
            Graphics.SetRandomWriteTarget(1, cbPoints);
            Graphics.Blit(renderTexture1, renderTexture2, dx11bokehMaterial, 0);
            Graphics.ClearRandomWriteTargets();
            RenderTexture.ReleaseTemporary(temporary4);
            RenderTexture.ReleaseTemporary(temporary5);
            if (nearBlur)
            {
              dofHdrMaterial.SetVector("_Offsets", new Vector4(0.0f, num3, 0.0f, num3));
              Graphics.Blit(renderTexture3, renderTexture1, dofHdrMaterial, 2);
              dofHdrMaterial.SetVector("_Offsets", new Vector4(num3, 0.0f, 0.0f, num3));
              Graphics.Blit(renderTexture1, renderTexture3, dofHdrMaterial, 2);
              Graphics.Blit(renderTexture3, renderTexture2, dofHdrMaterial, 3);
            }
            dofHdrMaterial.SetVector("_Offsets", new Vector4(internalBlurWidth, 0.0f, 0.0f, internalBlurWidth));
            Graphics.Blit(renderTexture2, renderTexture1, dofHdrMaterial, 5);
            dofHdrMaterial.SetVector("_Offsets", new Vector4(0.0f, internalBlurWidth, 0.0f, internalBlurWidth));
            Graphics.Blit(renderTexture1, renderTexture2, dofHdrMaterial, 5);
            Graphics.SetRenderTarget(renderTexture2);
            ComputeBuffer.CopyCount(cbPoints, cbDrawArgs, 0);
            dx11bokehMaterial.SetBuffer("pointBuffer", cbPoints);
            dx11bokehMaterial.SetTexture("_MainTex", dx11BokehTexture);
            dx11bokehMaterial.SetVector("_Screen", new Vector3((float) (1.0 / (1.0 * renderTexture2.width)), (float) (1.0 / (1.0 * renderTexture2.height)), internalBlurWidth));
            dx11bokehMaterial.SetPass(1);
            Graphics.DrawProceduralIndirect(MeshTopology.Points, cbDrawArgs, 0);
            dofHdrMaterial.SetTexture("_LowRez", renderTexture2);
            dofHdrMaterial.SetTexture("_FgOverlap", renderTexture3);
            dofHdrMaterial.SetVector("_Offsets", (float) (1.0 * source.width / (1.0 * renderTexture2.width)) * internalBlurWidth * Vector4.one);
            Graphics.Blit(source, destination, dofHdrMaterial, 9);
            if ((bool) (Object) renderTexture3)
              RenderTexture.ReleaseTemporary(renderTexture3);
          }
        }
        else
        {
          source.filterMode = FilterMode.Bilinear;
          if (highResolution)
            internalBlurWidth *= 2f;
          WriteCoc(source, true);
          renderTexture1 = RenderTexture.GetTemporary(source.width >> 1, source.height >> 1, 0, source.format);
          renderTexture2 = RenderTexture.GetTemporary(source.width >> 1, source.height >> 1, 0, source.format);
          int pass = blurSampleCount == BlurSampleCount.High || blurSampleCount == BlurSampleCount.Medium ? 17 : 11;
          if (highResolution)
          {
            dofHdrMaterial.SetVector("_Offsets", new Vector4(0.0f, internalBlurWidth, 0.025f, internalBlurWidth));
            Graphics.Blit(source, destination, dofHdrMaterial, pass);
          }
          else
          {
            dofHdrMaterial.SetVector("_Offsets", new Vector4(0.0f, internalBlurWidth, 0.1f, internalBlurWidth));
            Graphics.Blit(source, renderTexture1, dofHdrMaterial, 6);
            Graphics.Blit(renderTexture1, renderTexture2, dofHdrMaterial, pass);
            dofHdrMaterial.SetTexture("_LowRez", renderTexture2);
            dofHdrMaterial.SetTexture("_FgOverlap", null);
            dofHdrMaterial.SetVector("_Offsets", Vector4.one * (float) (1.0 * source.width / (1.0 * renderTexture2.width)) * internalBlurWidth);
            Graphics.Blit(source, destination, dofHdrMaterial, blurSampleCount == BlurSampleCount.High ? 18 : 12);
          }
        }
        if ((bool) (Object) renderTexture1)
          RenderTexture.ReleaseTemporary(renderTexture1);
        if (!(bool) (Object) renderTexture2)
          return;
        RenderTexture.ReleaseTemporary(renderTexture2);
      }
    }

    public enum BlurType
    {
      DiscBlur,
      DX11,
    }

    public enum BlurSampleCount
    {
      Low,
      Medium,
      High,
    }
  }
}
