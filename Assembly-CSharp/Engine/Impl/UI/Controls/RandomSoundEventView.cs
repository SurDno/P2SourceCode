// Decompiled with JetBrains decompiler
// Type: Engine.Impl.UI.Controls.RandomSoundEventView
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using UnityEngine;

#nullable disable
namespace Engine.Impl.UI.Controls
{
  public class RandomSoundEventView : EventView
  {
    [SerializeField]
    private SoundCollection soundCollection;

    public override void Invoke()
    {
      AudioClip clip = this.soundCollection?.GetClip();
      if ((Object) clip == (Object) null)
        return;
      MonoBehaviourInstance<UISounds>.Instance?.PlaySound(clip);
    }
  }
}
