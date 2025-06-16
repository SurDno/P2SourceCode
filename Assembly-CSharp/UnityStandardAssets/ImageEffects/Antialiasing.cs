namespace UnityStandardAssets.ImageEffects
{
  [ExecuteInEditMode]
  [RequireComponent(typeof (Camera))]
  [AddComponentMenu("Image Effects/Other/Antialiasing")]
  public class Antialiasing : PostEffectsBase
  {
    public AAMode mode = AAMode.FXAA3Console;
    public bool showGeneratedNormals = false;
    public float offsetScale = 0.2f;
    public float blurRadius = 18f;
    public float edgeThresholdMin = 0.05f;
    public float edgeThreshold = 0.2f;
    public float edgeSharpness = 4f;
    public bool dlaaSharp = false;
    public Shader ssaaShader;
    private Material ssaa;
    public Shader dlaaShader;
    private Material dlaa;
    public Shader nfaaShader;
    private Material nfaa;
    public Shader shaderFXAAPreset2;
    private Material materialFXAAPreset2;
    public Shader shaderFXAAPreset3;
    private Material materialFXAAPreset3;
    public Shader shaderFXAAII;
    private Material materialFXAAII;
    public Shader shaderFXAAIII;
    private Material materialFXAAIII;

    public Material CurrentAAMaterial()
    {
      Material material;
      switch (mode)
      {
        case AAMode.FXAA2:
          material = materialFXAAII;
          break;
        case AAMode.FXAA3Console:
          material = materialFXAAIII;
          break;
        case AAMode.FXAA1PresetA:
          material = materialFXAAPreset2;
          break;
        case AAMode.FXAA1PresetB:
          material = materialFXAAPreset3;
          break;
        case AAMode.NFAA:
          material = nfaa;
          break;
        case AAMode.SSAA:
          material = ssaa;
          break;
        case AAMode.DLAA:
          material = dlaa;
          break;
        default:
          material = (Material) null;
          break;
      }
      return material;
    }

    public override bool CheckResources()
    {
      CheckSupport(false);
      materialFXAAPreset2 = CreateMaterial(shaderFXAAPreset2, materialFXAAPreset2);
      materialFXAAPreset3 = CreateMaterial(shaderFXAAPreset3, materialFXAAPreset3);
      materialFXAAII = CreateMaterial(shaderFXAAII, materialFXAAII);
      materialFXAAIII = CreateMaterial(shaderFXAAIII, materialFXAAIII);
      nfaa = CreateMaterial(nfaaShader, nfaa);
      ssaa = CreateMaterial(ssaaShader, ssaa);
      dlaa = CreateMaterial(dlaaShader, dlaa);
      if (!ssaaShader.isSupported)
      {
        NotSupported();
        ReportAutoDisable();
      }
      return isSupported;
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
      if (!CheckResources())
        Graphics.Blit((Texture) source, destination);
      else if (mode == AAMode.FXAA3Console && (Object) materialFXAAIII != (Object) null)
      {
        materialFXAAIII.SetFloat("_EdgeThresholdMin", edgeThresholdMin);
        materialFXAAIII.SetFloat("_EdgeThreshold", edgeThreshold);
        materialFXAAIII.SetFloat("_EdgeSharpness", edgeSharpness);
        Graphics.Blit((Texture) source, destination, materialFXAAIII);
      }
      else if (mode == AAMode.FXAA1PresetB && (Object) materialFXAAPreset3 != (Object) null)
        Graphics.Blit((Texture) source, destination, materialFXAAPreset3);
      else if (mode == AAMode.FXAA1PresetA && (Object) materialFXAAPreset2 != (Object) null)
      {
        source.anisoLevel = 4;
        Graphics.Blit((Texture) source, destination, materialFXAAPreset2);
        source.anisoLevel = 0;
      }
      else if (mode == AAMode.FXAA2 && (Object) materialFXAAII != (Object) null)
        Graphics.Blit((Texture) source, destination, materialFXAAII);
      else if (mode == AAMode.SSAA && (Object) ssaa != (Object) null)
        Graphics.Blit((Texture) source, destination, ssaa);
      else if (mode == AAMode.DLAA && (Object) dlaa != (Object) null)
      {
        source.anisoLevel = 0;
        RenderTexture temporary = RenderTexture.GetTemporary(source.width, source.height);
        Graphics.Blit((Texture) source, temporary, dlaa, 0);
        Graphics.Blit((Texture) temporary, destination, dlaa, dlaaSharp ? 2 : 1);
        RenderTexture.ReleaseTemporary(temporary);
      }
      else if (mode == AAMode.NFAA && (Object) nfaa != (Object) null)
      {
        source.anisoLevel = 0;
        nfaa.SetFloat("_OffsetScale", offsetScale);
        nfaa.SetFloat("_BlurRadius", blurRadius);
        Graphics.Blit((Texture) source, destination, nfaa, showGeneratedNormals ? 1 : 0);
      }
      else
        Graphics.Blit((Texture) source, destination);
    }
  }
}
