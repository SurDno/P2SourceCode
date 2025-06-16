using UnityEngine;

public class GraphicSettingsData : ScriptableObjectInstance<GraphicSettingsData>
{
  [Space]
  [SerializeField]
  private global::ShadowSettings[] shadowSettings;
  [Space]
  [SerializeField]
  private global::TextureSettings[] textureSettings;
  [Space]
  [SerializeField]
  private int defaultPreset;
  [SerializeField]
  private GraphicSettingsPreset[] presets;

  public global::ShadowSettings[] ShadowSettings => this.shadowSettings;

  public global::TextureSettings[] TextureSettings => this.textureSettings;

  public GraphicSettingsPreset[] Presets => this.presets;

  public GraphicSettingsPreset DefaultPreset => this.presets[this.defaultPreset];

  public int MaxSupportedTextureSettings()
  {
    for (int index = 0; index < this.textureSettings.Length; ++index)
    {
      if (!this.textureSettings[index].CheckMemoryRequirement())
        return index == 0 ? 0 : index - 1;
    }
    return this.textureSettings.Length - 1;
  }
}
