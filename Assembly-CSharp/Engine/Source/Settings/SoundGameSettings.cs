// Decompiled with JetBrains decompiler
// Type: Engine.Source.Settings.SoundGameSettings
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Inspectors;

#nullable disable
namespace Engine.Source.Settings
{
  public class SoundGameSettings : SettingsInstanceByRequest<SoundGameSettings>
  {
    [Inspected]
    public IValue<float> MasterVolume = (IValue<float>) new FloatValue(nameof (MasterVolume), 1f, 0.0f, 1f);
    [Inspected]
    public IValue<float> MusicVolume = (IValue<float>) new FloatValue(nameof (MusicVolume), 0.8f, 0.0f, 1f);
    [Inspected]
    public IValue<float> EffectsVolume = (IValue<float>) new FloatValue(nameof (EffectsVolume), 0.8f, 0.0f, 1f);
    [Inspected]
    public IValue<float> VoiceVolume = (IValue<float>) new FloatValue(nameof (VoiceVolume), 0.8f, 0.0f, 1f);
  }
}
