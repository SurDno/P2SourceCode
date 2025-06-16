// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.SetDifficultyPresetEventView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Commons;
using Engine.Source.Difficulties;
using UnityEngine;

#nullable disable
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
