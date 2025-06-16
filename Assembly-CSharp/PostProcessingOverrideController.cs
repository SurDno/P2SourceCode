// Decompiled with JetBrains decompiler
// Type: PostProcessingOverrideController
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;
using UnityEngine.PostProcessing;

#nullable disable
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
    get => this.stackOverride;
    set => this.stackOverride = value;
  }

  private void ApplyTo(PostProcessingProfile source, PostProcessingProfile target)
  {
    PostProcessingOverrideController.ResetTargetProfile(source, target);
    if (!((Object) this.stackOverride != (Object) null))
      return;
    this.stackOverride.ApplyTo(source, target);
  }

  private void LateUpdate()
  {
    if ((Object) this.overrideProfile == (Object) null)
      return;
    this.ApplyTo(this.baseProfile, this.overrideProfile);
  }

  private void OnDisable()
  {
    if ((Object) this.overrideProfile == (Object) null)
      return;
    this.postProcessingBehaviour.profile = this.baseProfile;
    if (Application.isPlaying)
      Object.Destroy((Object) this.overrideProfile);
    else
      Object.DestroyImmediate((Object) this.overrideProfile);
    this.overrideProfile = (PostProcessingProfile) null;
  }

  private void OnEnable()
  {
    this.postProcessingBehaviour = this.GetComponent<PostProcessingBehaviour>();
    if ((Object) this.postProcessingBehaviour == (Object) null || (Object) this.baseProfile == (Object) null)
      return;
    this.overrideProfile = Object.Instantiate<PostProcessingProfile>(this.baseProfile);
    this.overrideProfile.name = this.baseProfile.name + "_Override";
    this.overrideProfile.hideFlags = HideFlags.HideAndDontSave;
    this.postProcessingBehaviour.profile = this.overrideProfile;
    this.ApplyTo(this.baseProfile, this.overrideProfile);
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
