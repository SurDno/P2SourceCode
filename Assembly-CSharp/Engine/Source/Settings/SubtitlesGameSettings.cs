using Inspectors;

namespace Engine.Source.Settings
{
  public class SubtitlesGameSettings : SettingsInstanceByRequest<SubtitlesGameSettings>
  {
    [Inspected]
    public IValue<bool> SubtitlesEnabled = new BoolValue(nameof (SubtitlesEnabled));
    [Inspected]
    public IValue<bool> DialogSubtitlesEnabled = new BoolValue(nameof (DialogSubtitlesEnabled), true);
  }
}
