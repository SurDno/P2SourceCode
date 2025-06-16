using Inspectors;

namespace Engine.Source.Settings
{
  public class SubtitlesGameSettings : SettingsInstanceByRequest<SubtitlesGameSettings>
  {
    [Inspected]
    public IValue<bool> SubtitlesEnabled = (IValue<bool>) new BoolValue(nameof (SubtitlesEnabled));
    [Inspected]
    public IValue<bool> DialogSubtitlesEnabled = (IValue<bool>) new BoolValue(nameof (DialogSubtitlesEnabled), true);
  }
}
