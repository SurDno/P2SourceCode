// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.PostProcessingProfile
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace UnityEngine.PostProcessing
{
  public class PostProcessingProfile : ScriptableObject
  {
    public BuiltinDebugViewsModel debugViews = new BuiltinDebugViewsModel();
    public FogModel fog = new FogModel();
    public AntialiasingModel antialiasing = new AntialiasingModel();
    public AmbientOcclusionModel ambientOcclusion = new AmbientOcclusionModel();
    public ScreenSpaceReflectionModel screenSpaceReflection = new ScreenSpaceReflectionModel();
    public DepthOfFieldModel depthOfField = new DepthOfFieldModel();
    public MotionBlurModel motionBlur = new MotionBlurModel();
    public EyeAdaptationModel eyeAdaptation = new EyeAdaptationModel();
    public BloomModel bloom = new BloomModel();
    public ColorGradingModel colorGrading = new ColorGradingModel();
    public UserLutModel userLut = new UserLutModel();
    public ChromaticAberrationModel chromaticAberration = new ChromaticAberrationModel();
    public GrainModel grain = new GrainModel();
    public VignetteModel vignette = new VignetteModel();
    public DitheringModel dithering = new DitheringModel();
  }
}
