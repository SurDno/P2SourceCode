﻿using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Edge Detection/Edge Detection")]
  public class EdgeDetection : PostEffectsBase
  {
    public EdgeDetectMode mode = EdgeDetectMode.SobelDepthThin;
    public float sensitivityDepth = 1f;
    public float sensitivityNormals = 1f;
    public float lumThreshold = 0.2f;
    public float edgeExp = 1f;
    public float sampleDist = 1f;
    public float edgesOnly;
    public Color edgesOnlyBgColor = Color.white;
    public Shader edgeDetectShader;
    private Material edgeDetectMaterial;
    private EdgeDetectMode oldMode = EdgeDetectMode.SobelDepthThin;

    public override bool CheckResources()
    {
      CheckSupport(true);
      edgeDetectMaterial = CheckShaderAndCreateMaterial(edgeDetectShader, edgeDetectMaterial);
      if (mode != oldMode)
        SetCameraFlag();
      oldMode = mode;
      if (!isSupported)
        ReportAutoDisable();
      return isSupported;
    }

    private new void Start() => oldMode = mode;

    private void SetCameraFlag()
    {
      if (mode == EdgeDetectMode.SobelDepth || mode == EdgeDetectMode.SobelDepthThin)
      {
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
      }
      else
      {
        if (mode != EdgeDetectMode.TriangleDepthNormals && mode != EdgeDetectMode.RobertsCrossDepthNormals)
          return;
        GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
      }
    }

    private void OnEnable() => SetCameraFlag();

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!CheckResources())
      {
        Graphics.Blit(source, destination);
      }
      else
      {
        Vector2 vector2 = new Vector2(sensitivityDepth, sensitivityNormals);
        edgeDetectMaterial.SetVector("_Sensitivity", new Vector4(vector2.x, vector2.y, 1f, vector2.y));
        edgeDetectMaterial.SetFloat("_BgFade", edgesOnly);
        edgeDetectMaterial.SetFloat("_SampleDistance", sampleDist);
        edgeDetectMaterial.SetVector("_BgColor", edgesOnlyBgColor);
        edgeDetectMaterial.SetFloat("_Exponent", edgeExp);
        edgeDetectMaterial.SetFloat("_Threshold", lumThreshold);
        Graphics.Blit(source, destination, edgeDetectMaterial, (int) mode);
      }
    }

    public enum EdgeDetectMode
    {
      TriangleDepthNormals,
      RobertsCrossDepthNormals,
      SobelDepth,
      SobelDepthThin,
      TriangleLuminance,
    }
  }
}
