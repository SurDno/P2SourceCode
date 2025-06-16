using UnityEngine;

[ExecuteInEditMode]
public class Controls : MonoBehaviour
{
  public AmplifyOcclusionEffect occlusion;
  private const AmplifyOcclusionBase.ApplicationMethod POST = AmplifyOcclusionBase.ApplicationMethod.PostEffect;
  private const AmplifyOcclusionBase.ApplicationMethod DEFERRED = AmplifyOcclusionBase.ApplicationMethod.Deferred;
  private const AmplifyOcclusionBase.ApplicationMethod DEBUG = AmplifyOcclusionBase.ApplicationMethod.Debug;

  private void OnGUI()
  {
    GUILayout.BeginArea(new Rect(0.0f, 0.0f, (float) Screen.width, (float) Screen.height));
    GUILayout.BeginHorizontal();
    GUILayout.Space(5f);
    GUILayout.BeginVertical();
    this.occlusion.enabled = GUILayout.Toggle(this.occlusion.enabled, " Amplify Occlusion Enabled");
    GUILayout.Space(5f);
    this.occlusion.ApplyMethod = GUILayout.Toggle(this.occlusion.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.PostEffect, " Standard Post-effect") ? AmplifyOcclusionBase.ApplicationMethod.PostEffect : this.occlusion.ApplyMethod;
    this.occlusion.ApplyMethod = GUILayout.Toggle(this.occlusion.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Deferred, " Deferred Injection") ? AmplifyOcclusionBase.ApplicationMethod.Deferred : this.occlusion.ApplyMethod;
    this.occlusion.ApplyMethod = GUILayout.Toggle(this.occlusion.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Debug, " Debug Mode") ? AmplifyOcclusionBase.ApplicationMethod.Debug : this.occlusion.ApplyMethod;
    GUILayout.EndVertical();
    GUILayout.FlexibleSpace();
    GUILayout.BeginVertical();
    GUILayout.Space(5f);
    GUILayout.BeginHorizontal();
    GUILayout.BeginVertical();
    GUILayout.Space(-3f);
    GUILayout.Label("Intensity     ");
    GUILayout.EndVertical();
    this.occlusion.Intensity = GUILayout.HorizontalSlider(this.occlusion.Intensity, 0.0f, 1f, GUILayout.Width(100f));
    GUILayout.Space(5f);
    GUILayout.BeginVertical();
    GUILayout.Space(-3f);
    GUILayout.Label(" " + this.occlusion.Intensity.ToString("0.00"));
    GUILayout.EndVertical();
    GUILayout.Space(5f);
    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();
    GUILayout.BeginVertical();
    GUILayout.Space(-3f);
    GUILayout.Label("Power Exp. ");
    GUILayout.EndVertical();
    this.occlusion.PowerExponent = GUILayout.HorizontalSlider(this.occlusion.PowerExponent, 0.0001f, 6f, GUILayout.Width(100f));
    GUILayout.Space(5f);
    GUILayout.BeginVertical();
    GUILayout.Space(-3f);
    GUILayout.Label(" " + this.occlusion.PowerExponent.ToString("0.00"));
    GUILayout.EndVertical();
    GUILayout.Space(5f);
    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();
    GUILayout.BeginVertical();
    GUILayout.Space(-3f);
    GUILayout.Label("Radius        ");
    GUILayout.EndVertical();
    this.occlusion.Radius = GUILayout.HorizontalSlider(this.occlusion.Radius, 0.1f, 10f, GUILayout.Width(100f));
    GUILayout.Space(5f);
    GUILayout.BeginVertical();
    GUILayout.Space(-3f);
    GUILayout.Label(" " + this.occlusion.Radius.ToString("0.00"));
    GUILayout.EndVertical();
    GUILayout.Space(5f);
    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();
    GUILayout.BeginVertical();
    GUILayout.Space(-3f);
    GUILayout.Label("Quality        ");
    GUILayout.EndVertical();
    this.occlusion.SampleCount = (AmplifyOcclusionBase.SampleCountLevel) GUILayout.HorizontalSlider((float) this.occlusion.SampleCount, 0.0f, 3f, GUILayout.Width(100f));
    GUILayout.Space(5f);
    GUILayout.BeginVertical();
    GUILayout.Space(-3f);
    GUILayout.Label("        ");
    GUILayout.EndVertical();
    GUILayout.Space(5f);
    GUILayout.EndHorizontal();
    GUILayout.EndVertical();
    GUILayout.EndHorizontal();
    GUILayout.EndArea();
  }
}
