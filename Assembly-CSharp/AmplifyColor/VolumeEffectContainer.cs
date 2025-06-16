using System;
using System.Collections.Generic;
using System.Linq;

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
