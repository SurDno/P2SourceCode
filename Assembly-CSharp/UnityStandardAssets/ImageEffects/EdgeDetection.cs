using UnityEngine;

namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Edge Detection/Edge Detection")]
  public class EdgeDetection : PostEffectsBase
  {
    public EdgeDetection.EdgeDetectMode mode = EdgeDetection.EdgeDetectMode.SobelDepthThin;
    public float sensitivityDepth = 1f;
    public float sensitivityNormals = 1f;
    public float lumThreshold = 0.2f;
    public float edgeExp = 1f;
    public float sampleDist = 1f;
    public float edgesOnly = 0.0f;
    public Color edgesOnlyBgColor = Color.white;
    public Shader edgeDetectShader;
    private Material edgeDetectMaterial = (Material) null;
    private EdgeDetection.EdgeDetectMode oldMode = EdgeDetection.EdgeDetectMode.SobelDepthThin;

    public override bool CheckResources()
    {
      this.CheckSupport(true);
      this.edgeDetectMaterial = this.CheckShaderAndCreateMaterial(this.edgeDetectShader, this.edgeDetectMaterial);
      if (this.mode != this.oldMode)
        this.SetCameraFlag();
      this.oldMode = this.mode;
      if (!this.isSupported)
        this.ReportAutoDisable();
      return this.isSupported;
    }

    private new void Start() => this.oldMode = this.mode;

    private void SetCameraFlag()
    {
      if (this.mode == EdgeDetection.EdgeDetectMode.SobelDepth || this.mode == EdgeDetection.EdgeDetectMode.SobelDepthThin)
      {
        this.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.Depth;
      }
      else
      {
        if (this.mode != EdgeDetection.EdgeDetectMode.TriangleDepthNormals && this.mode != EdgeDetection.EdgeDetectMode.RobertsCrossDepthNormals)
          return;
        this.GetComponent<Camera>().depthTextureMode |= DepthTextureMode.DepthNormals;
      }
    }

    private void OnEnable() => this.SetCameraFlag();

    [ImageEffectOpaque]
    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!this.CheckResources())
      {
        Graphics.Blit((Texture) source, destination);
      }
      else
      {
        Vector2 vector2 = new Vector2(this.sensitivityDepth, this.sensitivityNormals);
        this.edgeDetectMaterial.SetVector("_Sensitivity", new Vector4(vector2.x, vector2.y, 1f, vector2.y));
        this.edgeDetectMaterial.SetFloat("_BgFade", this.edgesOnly);
        this.edgeDetectMaterial.SetFloat("_SampleDistance", this.sampleDist);
        this.edgeDetectMaterial.SetVector("_BgColor", (Vector4) this.edgesOnlyBgColor);
        this.edgeDetectMaterial.SetFloat("_Exponent", this.edgeExp);
        this.edgeDetectMaterial.SetFloat("_Threshold", this.lumThreshold);
        Graphics.Blit((Texture) source, destination, this.edgeDetectMaterial, (int) this.mode);
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
