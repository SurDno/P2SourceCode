public class DeferredShadingSetup : MonoBehaviour
{
  [SerializeField]
  private Shader deferredShader;

  private void OnEnable()
  {
    GraphicsSettings.SetCustomShader(BuiltinShaderType.DeferredShading, deferredShader);
    GraphicsSettings.SetShaderMode(BuiltinShaderType.DeferredShading, BuiltinShaderMode.UseCustom);
  }

  private void OnDisable()
  {
    GraphicsSettings.SetShaderMode(BuiltinShaderType.DeferredShading, BuiltinShaderMode.UseBuiltin);
    GraphicsSettings.SetCustomShader(BuiltinShaderType.DeferredShading, (Shader) null);
  }
}
