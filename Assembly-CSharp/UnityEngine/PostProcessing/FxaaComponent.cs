// Decompiled with JetBrains decompiler
// Type: UnityEngine.PostProcessing.FxaaComponent
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

#nullable disable
namespace UnityEngine.PostProcessing
{
  public sealed class FxaaComponent : PostProcessingComponentRenderTexture<AntialiasingModel>
  {
    public override bool active
    {
      get
      {
        return this.model.enabled && this.model.settings.method == AntialiasingModel.Method.Fxaa && !this.context.interrupted;
      }
    }

    public void Render(RenderTexture source, RenderTexture destination)
    {
      AntialiasingModel.FxaaSettings fxaaSettings = this.model.settings.fxaaSettings;
      Material mat = this.context.materialFactory.Get("Hidden/Post FX/FXAA");
      AntialiasingModel.FxaaQualitySettings preset1 = AntialiasingModel.FxaaQualitySettings.presets[(int) fxaaSettings.preset];
      AntialiasingModel.FxaaConsoleSettings preset2 = AntialiasingModel.FxaaConsoleSettings.presets[(int) fxaaSettings.preset];
      mat.SetVector(FxaaComponent.Uniforms._QualitySettings, (Vector4) new Vector3(preset1.subpixelAliasingRemovalAmount, preset1.edgeDetectionThreshold, preset1.minimumRequiredLuminance));
      mat.SetVector(FxaaComponent.Uniforms._ConsoleSettings, new Vector4(preset2.subpixelSpreadAmount, preset2.edgeSharpnessAmount, preset2.edgeDetectionThreshold, preset2.minimumRequiredLuminance));
      Graphics.Blit((Texture) source, destination, mat, 0);
    }

    private static class Uniforms
    {
      internal static readonly int _QualitySettings = Shader.PropertyToID(nameof (_QualitySettings));
      internal static readonly int _ConsoleSettings = Shader.PropertyToID(nameof (_ConsoleSettings));
    }
  }
}
