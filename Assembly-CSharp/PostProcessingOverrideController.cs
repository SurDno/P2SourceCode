using UnityEngine.PostProcessing;

[ExecuteInEditMode]
[RequireComponent(typeof (PostProcessingBehaviour))]
public class PostProcessingOverrideController : MonoBehaviour
{
  [SerializeField]
  private PostProcessingProfile baseProfile;
  [SerializeField]
  private PostProcessingStackOverride stackOverride;
  private PostProcessingBehaviour postProcessingBehaviour;
  private PostProcessingProfile overrideProfile;

  public PostProcessingStackOverride StackOverride
  {
    get => stackOverride;
    set => stackOverride = value;
  }

  private void ApplyTo(PostProcessingProfile source, PostProcessingProfile target)
  {
    ResetTargetProfile(source, target);
    if (!((Object) stackOverride != (Object) null))
      return;
    stackOverride.ApplyTo(source, target);
  }

  private void LateUpdate()
  {
    if ((Object) overrideProfile == (Object) null)
      return;
    ApplyTo(baseProfile, overrideProfile);
  }

  private void OnDisable()
  {
    if ((Object) overrideProfile == (Object) null)
      return;
    postProcessingBehaviour.profile = baseProfile;
    if (Application.isPlaying)
      Object.Destroy((Object) overrideProfile);
    else
      Object.DestroyImmediate((Object) overrideProfile);
    overrideProfile = null;
  }

  private void OnEnable()
  {
    postProcessingBehaviour = this.GetComponent<PostProcessingBehaviour>();
    if ((Object) postProcessingBehaviour == (Object) null || (Object) baseProfile == (Object) null)
      return;
    overrideProfile = Object.Instantiate<PostProcessingProfile>(baseProfile);
    overrideProfile.name = baseProfile.name + "_Override";
    overrideProfile.hideFlags = HideFlags.HideAndDontSave;
    postProcessingBehaviour.profile = overrideProfile;
    ApplyTo(baseProfile, overrideProfile);
  }

  private static void ResetTargetProfile(PostProcessingProfile source, PostProcessingProfile target)
  {
    target.debugViews = source.debugViews;
    target.fog = source.fog;
    target.antialiasing = source.antialiasing;
    target.ambientOcclusion = source.ambientOcclusion;
    target.screenSpaceReflection = source.screenSpaceReflection;
    target.depthOfField = source.depthOfField;
    target.motionBlur = source.motionBlur;
    target.eyeAdaptation = source.eyeAdaptation;
    target.bloom = source.bloom;
    target.colorGrading = source.colorGrading;
    target.userLut = source.userLut;
    target.chromaticAberration = source.chromaticAberration;
    target.grain = source.grain;
    target.vignette = source.vignette;
    target.dithering = source.dithering;
  }
}
