using Engine.Source.Commons;
using Engine.Source.Difficulties;
using UnityEngine;

namespace Engine.Impl.UI.Controls
{
  public class SetDifficultyPresetEventView : EventView
  {
    [SerializeField]
    private string difficultyPresetName;

    public override void Invoke()
    {
      DifficultyUtility.SetPresetValues(this.difficultyPresetName);
      DifficultySettings instance = InstanceByRequest<DifficultySettings>.Instance;
      instance.OriginalExperience.Value = this.difficultyPresetName == "Default";
      instance.Apply();
    }
  }
}
