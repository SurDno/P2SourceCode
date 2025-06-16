using Engine.Source.Commons;
using Engine.Source.Difficulties;

namespace Engine.Impl.UI.Controls
{
  public class SetDifficultyPresetEventView : EventView
  {
    [SerializeField]
    private string difficultyPresetName;

    public override void Invoke()
    {
      DifficultyUtility.SetPresetValues(difficultyPresetName);
      DifficultySettings instance = InstanceByRequest<DifficultySettings>.Instance;
      instance.OriginalExperience.Value = difficultyPresetName == "Default";
      instance.Apply();
    }
  }
}
