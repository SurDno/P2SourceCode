using Inspectors;

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
