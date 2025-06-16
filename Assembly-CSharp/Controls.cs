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
    occlusion.enabled = GUILayout.Toggle(occlusion.enabled, " Amplify Occlusion Enabled");
    GUILayout.Space(5f);
    occlusion.ApplyMethod = GUILayout.Toggle(occlusion.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.PostEffect, " Standard Post-effect") ? AmplifyOcclusionBase.ApplicationMethod.PostEffect : occlusion.ApplyMethod;
    occlusion.ApplyMethod = GUILayout.Toggle(occlusion.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Deferred, " Deferred Injection") ? AmplifyOcclusionBase.ApplicationMethod.Deferred : occlusion.ApplyMethod;
    occlusion.ApplyMethod = GUILayout.Toggle(occlusion.ApplyMethod == AmplifyOcclusionBase.ApplicationMethod.Debug, " Debug Mode") ? AmplifyOcclusionBase.ApplicationMethod.Debug : occlusion.ApplyMethod;
    GUILayout.EndVertical();
    GUILayout.FlexibleSpace();
    GUILayout.BeginVertical();
    GUILayout.Space(5f);
    GUILayout.BeginHorizontal();
    GUILayout.BeginVertical();
    GUILayout.Space(-3f);
    GUILayout.Label("Intensity     ");
    GUILayout.EndVertical();
    occlusion.Intensity = GUILayout.HorizontalSlider(occlusion.Intensity, 0.0f, 1f, GUILayout.Width(100f));
    GUILayout.Space(5f);
    GUILayout.BeginVertical();
    GUILayout.Space(-3f);
    GUILayout.Label(" " + occlusion.Intensity.ToString("0.00"));
    GUILayout.EndVertical();
    GUILayout.Space(5f);
    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();
    GUILayout.BeginVertical();
    GUILayout.Space(-3f);
    GUILayout.Label("Power Exp. ");
    GUILayout.EndVertical();
    occlusion.PowerExponent = GUILayout.HorizontalSlider(occlusion.PowerExponent, 0.0001f, 6f, GUILayout.Width(100f));
    GUILayout.Space(5f);
    GUILayout.BeginVertical();
    GUILayout.Space(-3f);
    GUILayout.Label(" " + occlusion.PowerExponent.ToString("0.00"));
    GUILayout.EndVertical();
    GUILayout.Space(5f);
    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();
    GUILayout.BeginVertical();
    GUILayout.Space(-3f);
    GUILayout.Label("Radius        ");
    GUILayout.EndVertical();
    occlusion.Radius = GUILayout.HorizontalSlider(occlusion.Radius, 0.1f, 10f, GUILayout.Width(100f));
    GUILayout.Space(5f);
    GUILayout.BeginVertical();
    GUILayout.Space(-3f);
    GUILayout.Label(" " + occlusion.Radius.ToString("0.00"));
    GUILayout.EndVertical();
    GUILayout.Space(5f);
    GUILayout.EndHorizontal();
    GUILayout.BeginHorizontal();
    GUILayout.BeginVertical();
    GUILayout.Space(-3f);
    GUILayout.Label("Quality        ");
    GUILayout.EndVertical();
    occlusion.SampleCount = (AmplifyOcclusionBase.SampleCountLevel) GUILayout.HorizontalSlider((float) occlusion.SampleCount, 0.0f, 3f, GUILayout.Width(100f));
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
