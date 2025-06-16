namespace UnityEngine.PostProcessing
{
  public sealed class FxaaComponent : PostProcessingComponentRenderTexture<AntialiasingModel>
  {
    public override bool active
    {
      get
      {
        return model.enabled && model.settings.method == AntialiasingModel.Method.Fxaa && !context.interrupted;
      }
    }

    public void Render(RenderTexture source, RenderTexture destination)
    {
      AntialiasingModel.FxaaSettings fxaaSettings = model.settings.fxaaSettings;
      Material mat = context.materialFactory.Get("Hidden/Post FX/FXAA");
      AntialiasingModel.FxaaQualitySettings preset1 = AntialiasingModel.FxaaQualitySettings.presets[(int) fxaaSettings.preset];
      AntialiasingModel.FxaaConsoleSettings preset2 = AntialiasingModel.FxaaConsoleSettings.presets[(int) fxaaSettings.preset];
      mat.SetVector(Uniforms._QualitySettings, (Vector4) new Vector3(preset1.subpixelAliasingRemovalAmount, preset1.edgeDetectionThreshold, preset1.minimumRequiredLuminance));
      mat.SetVector(Uniforms._ConsoleSettings, new Vector4(preset2.subpixelSpreadAmount, preset2.edgeSharpnessAmount, preset2.edgeDetectionThreshold, preset2.minimumRequiredLuminance));
      Graphics.Blit((Texture) source, destination, mat, 0);
    }

    private static class Uniforms
    {
      internal static readonly int _QualitySettings = Shader.PropertyToID(nameof (_QualitySettings));
      internal static readonly int _ConsoleSettings = Shader.PropertyToID(nameof (_ConsoleSettings));
    }
  }
}
