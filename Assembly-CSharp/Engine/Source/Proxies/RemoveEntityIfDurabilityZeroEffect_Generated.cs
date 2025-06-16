// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.RemoveEntityIfDurabilityZeroEffect_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;
using UnityEngine.Audio;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (RemoveEntityIfDurabilityZeroEffect))]
  public class RemoveEntityIfDurabilityZeroEffect_Generated : 
    RemoveEntityIfDurabilityZeroEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      RemoveEntityIfDurabilityZeroEffect_Generated instance = Activator.CreateInstance<RemoveEntityIfDurabilityZeroEffect_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      RemoveEntityIfDurabilityZeroEffect_Generated zeroEffectGenerated = (RemoveEntityIfDurabilityZeroEffect_Generated) target2;
      zeroEffectGenerated.queue = this.queue;
      zeroEffectGenerated.removeSound = this.removeSound;
      zeroEffectGenerated.removeSoundMixer = this.removeSoundMixer;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      UnityDataWriteUtility.Write<AudioClip>(writer, "RemoveSound", this.removeSound);
      UnityDataWriteUtility.Write<AudioMixerGroup>(writer, "RemoveSoundMixer", this.removeSoundMixer);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.removeSound = UnityDataReadUtility.Read<AudioClip>(reader, "RemoveSound", this.removeSound);
      this.removeSoundMixer = UnityDataReadUtility.Read<AudioMixerGroup>(reader, "RemoveSoundMixer", this.removeSoundMixer);
    }
  }
}
