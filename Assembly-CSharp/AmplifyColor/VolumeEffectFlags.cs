using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace AmplifyColor
{
  [Serializable]
  public class VolumeEffectFlags
  {
    public List<VolumeEffectComponentFlags> components;

    public VolumeEffectFlags() => components = new List<VolumeEffectComponentFlags>();

    public void AddComponent(Component c)
    {
      VolumeEffectComponentFlags effectComponentFlags;
      if ((effectComponentFlags = components.Find(s => s.componentName == string.Concat(c.GetType()))) != null)
        effectComponentFlags.UpdateComponentFlags(c);
      else
        components.Add(new VolumeEffectComponentFlags(c));
    }

    public void UpdateFlags(VolumeEffect effectVol)
    {
      foreach (VolumeEffectComponent component in effectVol.components)
      {
        VolumeEffectComponent comp = component;
        VolumeEffectComponentFlags effectComponentFlags;
        if ((effectComponentFlags = components.Find(s => s.componentName == comp.componentName)) == null)
          components.Add(new VolumeEffectComponentFlags(comp));
        else
          effectComponentFlags.UpdateComponentFlags(comp);
      }
    }

    public static void UpdateCamFlags(AmplifyColorBase[] effects, AmplifyColorVolumeBase[] volumes)
    {
      foreach (AmplifyColorBase effect in effects)
      {
        effect.EffectFlags = new VolumeEffectFlags();
        foreach (AmplifyColorVolumeBase volume in volumes)
        {
          VolumeEffect volumeEffect = volume.EffectContainer.FindVolumeEffect(effect);
          if (volumeEffect != null)
            effect.EffectFlags.UpdateFlags(volumeEffect);
        }
      }
    }

    public VolumeEffect GenerateEffectData(AmplifyColorBase go)
    {
      VolumeEffect effectData = new VolumeEffect(go);
      foreach (VolumeEffectComponentFlags component1 in components)
      {
        if (component1.blendFlag)
        {
          Component component2 = go.GetComponent(component1.componentName);
          if (component2 != null)
            effectData.AddComponent(component2, component1);
        }
      }
      return effectData;
    }

    public VolumeEffectComponentFlags FindComponentFlags(string compName)
    {
      for (int index = 0; index < components.Count; ++index)
      {
        if (components[index].componentName == compName)
          return components[index];
      }
      return null;
    }

    public string[] GetComponentNames()
    {
      return components.Where(r => r.blendFlag).Select(r => r.componentName).ToArray();
    }
  }
}
