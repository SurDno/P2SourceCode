// Decompiled with JetBrains decompiler
// Type: AmplifyColor.VolumeEffectContainer
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using System;
using System.Collections.Generic;
using System.Linq;

#nullable disable
namespace AmplifyColor
{
  [Serializable]
  public class VolumeEffectContainer
  {
    public List<VolumeEffect> volumes;

    public VolumeEffectContainer() => this.volumes = new List<VolumeEffect>();

    public void AddColorEffect(AmplifyColorBase colorEffect)
    {
      VolumeEffect volumeEffect1;
      if ((volumeEffect1 = this.FindVolumeEffect(colorEffect)) != null)
      {
        volumeEffect1.UpdateVolume();
      }
      else
      {
        VolumeEffect volumeEffect2 = new VolumeEffect(colorEffect);
        this.volumes.Add(volumeEffect2);
        volumeEffect2.UpdateVolume();
      }
    }

    public VolumeEffect AddJustColorEffect(AmplifyColorBase colorEffect)
    {
      VolumeEffect volumeEffect = new VolumeEffect(colorEffect);
      this.volumes.Add(volumeEffect);
      return volumeEffect;
    }

    public VolumeEffect FindVolumeEffect(AmplifyColorBase colorEffect)
    {
      for (int index = 0; index < this.volumes.Count; ++index)
      {
        if ((UnityEngine.Object) this.volumes[index].gameObject == (UnityEngine.Object) colorEffect)
          return this.volumes[index];
      }
      for (int index = 0; index < this.volumes.Count; ++index)
      {
        if ((UnityEngine.Object) this.volumes[index].gameObject != (UnityEngine.Object) null && this.volumes[index].gameObject.SharedInstanceID == colorEffect.SharedInstanceID)
          return this.volumes[index];
      }
      return (VolumeEffect) null;
    }

    public void RemoveVolumeEffect(VolumeEffect volume) => this.volumes.Remove(volume);

    public AmplifyColorBase[] GetStoredEffects()
    {
      return this.volumes.Select<VolumeEffect, AmplifyColorBase>((Func<VolumeEffect, AmplifyColorBase>) (r => r.gameObject)).ToArray<AmplifyColorBase>();
    }
  }
}
