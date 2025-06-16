// Decompiled with JetBrains decompiler
// Type: GraphicSettingsData
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
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
