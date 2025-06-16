namespace UnityEngine.PostProcessing;

public class PostProcessingProfile : ScriptableObject {
	public BuiltinDebugViewsModel debugViews = new();
	public FogModel fog = new();
	public AntialiasingModel antialiasing = new();
	public AmbientOcclusionModel ambientOcclusion = new();
	public ScreenSpaceReflectionModel screenSpaceReflection = new();
	public DepthOfFieldModel depthOfField = new();
	public MotionBlurModel motionBlur = new();
	public EyeAdaptationModel eyeAdaptation = new();
	public BloomModel bloom = new();
	public ColorGradingModel colorGrading = new();
	public UserLutModel userLut = new();
	public ChromaticAberrationModel chromaticAberration = new();
	public GrainModel grain = new();
	public VignetteModel vignette = new();
	public DitheringModel dithering = new();
}