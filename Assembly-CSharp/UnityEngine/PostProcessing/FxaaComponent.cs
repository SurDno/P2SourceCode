namespace UnityEngine.PostProcessing;

public sealed class FxaaComponent : PostProcessingComponentRenderTexture<AntialiasingModel> {
	public override bool active =>
		model.enabled && model.settings.method == AntialiasingModel.Method.Fxaa && !context.interrupted;

	public void Render(RenderTexture source, RenderTexture destination) {
		var fxaaSettings = model.settings.fxaaSettings;
		var mat = context.materialFactory.Get("Hidden/Post FX/FXAA");
		var preset1 = AntialiasingModel.FxaaQualitySettings.presets[(int)fxaaSettings.preset];
		var preset2 = AntialiasingModel.FxaaConsoleSettings.presets[(int)fxaaSettings.preset];
		mat.SetVector(Uniforms._QualitySettings,
			new Vector3(preset1.subpixelAliasingRemovalAmount, preset1.edgeDetectionThreshold,
				preset1.minimumRequiredLuminance));
		mat.SetVector(Uniforms._ConsoleSettings,
			new Vector4(preset2.subpixelSpreadAmount, preset2.edgeSharpnessAmount, preset2.edgeDetectionThreshold,
				preset2.minimumRequiredLuminance));
		Graphics.Blit(source, destination, mat, 0);
	}

	private static class Uniforms {
		internal static readonly int _QualitySettings = Shader.PropertyToID(nameof(_QualitySettings));
		internal static readonly int _ConsoleSettings = Shader.PropertyToID(nameof(_ConsoleSettings));
	}
}