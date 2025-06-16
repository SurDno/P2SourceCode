namespace UnityEngine.PostProcessing
{
  public sealed class VignetteComponent : PostProcessingComponentRenderTexture<VignetteModel>
  {
    public override bool active => model.enabled && !context.interrupted;

    public override void Prepare(Material uberMaterial)
    {
      VignetteModel.Settings settings = model.settings;
      uberMaterial.SetColor(Uniforms._Vignette_Color, settings.color);
      if (settings.mode == VignetteModel.Mode.Classic)
      {
        uberMaterial.SetVector(Uniforms._Vignette_Center, settings.center);
        uberMaterial.EnableKeyword("VIGNETTE_CLASSIC");
        float z = (float) ((1.0 - settings.roundness) * 6.0) + settings.roundness;
        uberMaterial.SetVector(Uniforms._Vignette_Settings, new Vector4(settings.intensity * 3f, settings.smoothness * 5f, z, settings.rounded ? 1f : 0.0f));
      }
      else
      {
        if (settings.mode != VignetteModel.Mode.Masked || !(settings.mask != null) || settings.opacity <= 0.0)
          return;
        uberMaterial.EnableKeyword("VIGNETTE_MASKED");
        uberMaterial.SetTexture(Uniforms._Vignette_Mask, settings.mask);
        uberMaterial.SetFloat(Uniforms._Vignette_Opacity, settings.opacity);
      }
    }

    private static class Uniforms
    {
      internal static readonly int _Vignette_Color = Shader.PropertyToID(nameof (_Vignette_Color));
      internal static readonly int _Vignette_Center = Shader.PropertyToID(nameof (_Vignette_Center));
      internal static readonly int _Vignette_Settings = Shader.PropertyToID(nameof (_Vignette_Settings));
      internal static readonly int _Vignette_Mask = Shader.PropertyToID(nameof (_Vignette_Mask));
      internal static readonly int _Vignette_Opacity = Shader.PropertyToID(nameof (_Vignette_Opacity));
    }
  }
}
