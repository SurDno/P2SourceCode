using System;
using System.Collections.Generic;
using System.Linq;

namespace AmplifyColor
{
  [Serializable]
  public class VolumeEffectContainer
  {
    public List<VolumeEffect> volumes;

    public VolumeEffectContainer() => volumes = new List<VolumeEffect>();

    public void AddColorEffect(AmplifyColorBase colorEffect)
    {
      VolumeEffect volumeEffect1;
      if ((volumeEffect1 = FindVolumeEffect(colorEffect)) != null)
      {
        volumeEffect1.UpdateVolume();
      }
      else
      {
        VolumeEffect volumeEffect2 = new VolumeEffect(colorEffect);
        volumes.Add(volumeEffect2);
        volumeEffect2.UpdateVolume();
      }
    }

    public VolumeEffect AddJustColorEffect(AmplifyColorBase colorEffect)
    {
      VolumeEffect volumeEffect = new VolumeEffect(colorEffect);
      volumes.Add(volumeEffect);
      return volumeEffect;
    }

    public VolumeEffect FindVolumeEffect(AmplifyColorBase colorEffect)
    {
      for (int index = 0; index < volumes.Count; ++index)
      {
        if (volumes[index].gameObject == colorEffect)
          return volumes[index];
      }
      for (int index = 0; index < volumes.Count; ++index)
      {
        if (volumes[index].gameObject != null && volumes[index].gameObject.SharedInstanceID == colorEffect.SharedInstanceID)
          return volumes[index];
      }
      return null;
    }

    public void RemoveVolumeEffect(VolumeEffect volume) => volumes.Remove(volume);

    public AmplifyColorBase[] GetStoredEffects()
    {
      return volumes.Select(r => r.gameObject).ToArray();
    }
  }
}
