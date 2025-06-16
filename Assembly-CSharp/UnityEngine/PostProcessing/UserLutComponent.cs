namespace UnityEngine.PostProcessing
{
  public sealed class UserLutComponent : PostProcessingComponentRenderTexture<UserLutModel>
  {
    public override bool active
    {
      get
      {
        UserLutModel.Settings settings = model.settings;
        return model.enabled && (Object) settings.lut != (Object) null && settings.contribution > 0.0 && settings.lut.height == (int) Mathf.Sqrt((float) settings.lut.width) && !context.interrupted;
      }
    }

    public override void Prepare(Material uberMaterial)
    {
      UserLutModel.Settings settings = model.settings;
      uberMaterial.EnableKeyword("USER_LUT");
      uberMaterial.SetTexture(Uniforms._UserLut, (Texture) settings.lut);
      uberMaterial.SetVector(Uniforms._UserLut_Params, new Vector4(1f / (float) settings.lut.width, 1f / (float) settings.lut.height, (float) settings.lut.height - 1f, settings.contribution));
    }

    public void OnGUI()
    {
      UserLutModel.Settings settings = model.settings;
      GUI.DrawTexture(new Rect((float) ((double) context.viewport.x * (double) Screen.width + 8.0), 8f, (float) settings.lut.width, (float) settings.lut.height), (Texture) settings.lut);
    }

    private static class Uniforms
    {
      internal static readonly int _UserLut = Shader.PropertyToID(nameof (_UserLut));
      internal static readonly int _UserLut_Params = Shader.PropertyToID(nameof (_UserLut_Params));
    }
  }
}
