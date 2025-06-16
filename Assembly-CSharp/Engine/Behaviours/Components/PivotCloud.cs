// Decompiled with JetBrains decompiler
// Type: Engine.Behaviours.Components.PivotCloud
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Engine.Source.Audio;
using UnityEngine;

#nullable disable
namespace Engine.Behaviours.Components
{
  [DisallowMultipleComponent]
  public class PivotCloud : MonoBehaviour
  {
    [Header("Audio")]
    public AudioSource AudioSource = (AudioSource) null;

    private void Awake() => this.EnableSound(false);

    public void EnableSound(bool enabled)
    {
      if (!((Object) this.AudioSource != (Object) null))
        return;
      if (enabled)
        this.AudioSource.PlayAndCheck();
      else
        this.AudioSource.Stop();
    }

    public void SetSoundMaxDistance(float distance)
    {
      if (!((Object) this.AudioSource != (Object) null))
        return;
      this.AudioSource.maxDistance = distance;
    }
  }
}
