using Inspectors;

namespace Engine.Source.Settings
{
  public class SoundGameSettings : SettingsInstanceByRequest<SoundGameSettings>
  {
    [Inspected]
    public IValue<float> MasterVolume = new FloatValue(nameof (MasterVolume), 1f, 0.0f, 1f);
    [Inspected]
    public IValue<float> MusicVolume = new FloatValue(nameof (MusicVolume), 0.8f, 0.0f, 1f);
    [Inspected]
    public IValue<float> EffectsVolume = new FloatValue(nameof (EffectsVolume), 0.8f, 0.0f, 1f);
    [Inspected]
    public IValue<float> VoiceVolume = new FloatValue(nameof (VoiceVolume), 0.8f, 0.0f, 1f);
  }
}
