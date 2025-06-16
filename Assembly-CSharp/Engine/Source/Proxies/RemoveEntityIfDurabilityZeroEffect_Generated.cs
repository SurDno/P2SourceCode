using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Effects;
using Scripts.Tools.Serializations.Converters;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      RemoveEntityIfDurabilityZeroEffect_Generated zeroEffectGenerated = (RemoveEntityIfDurabilityZeroEffect_Generated) target2;
      zeroEffectGenerated.queue = queue;
      zeroEffectGenerated.removeSound = removeSound;
      zeroEffectGenerated.removeSoundMixer = removeSoundMixer;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
      UnityDataWriteUtility.Write(writer, "RemoveSound", removeSound);
      UnityDataWriteUtility.Write(writer, "RemoveSoundMixer", removeSoundMixer);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      removeSound = UnityDataReadUtility.Read(reader, "RemoveSound", removeSound);
      removeSoundMixer = UnityDataReadUtility.Read(reader, "RemoveSoundMixer", removeSoundMixer);
    }
  }
}
