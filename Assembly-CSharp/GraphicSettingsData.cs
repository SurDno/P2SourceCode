using UnityEngine;

public class GraphicSettingsData : ScriptableObjectInstance<GraphicSettingsData>
{
  [Space]
  [SerializeField]
  private ShadowSettings[] shadowSettings;
  [Space]
  [SerializeField]
  private TextureSettings[] textureSettings;
  [Space]
  [SerializeField]
  private int defaultPreset;
  [SerializeField]
  private GraphicSettingsPreset[] presets;

  public ShadowSettings[] ShadowSettings => shadowSettings;

  public TextureSettings[] TextureSettings => textureSettings;

  public GraphicSettingsPreset[] Presets => presets;

  public GraphicSettingsPreset DefaultPreset => presets[defaultPreset];

  public int MaxSupportedTextureSettings()
  {
    for (int index = 0; index < textureSettings.Length; ++index)
    {
      if (!textureSettings[index].CheckMemoryRequirement())
        return index == 0 ? 0 : index - 1;
    }
    return textureSettings.Length - 1;
  }
}
