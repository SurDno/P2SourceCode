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
  [FactoryProxy(typeof (OpenRandomSymptomEffect))]
  public class OpenRandomSymptomEffect_Generated : 
    OpenRandomSymptomEffect,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      OpenRandomSymptomEffect_Generated instance = Activator.CreateInstance<OpenRandomSymptomEffect_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      OpenRandomSymptomEffect_Generated symptomEffectGenerated = (OpenRandomSymptomEffect_Generated) target2;
      symptomEffectGenerated.queue = queue;
      symptomEffectGenerated.count = count;
      CloneableObjectUtility.FillListTo(symptomEffectGenerated.targetSymptoms, targetSymptoms);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Queue", queue);
      DefaultDataWriteUtility.Write(writer, "Count", count);
      UnityDataWriteUtility.WriteList(writer, "TargetSymptoms", targetSymptoms);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      count = DefaultDataReadUtility.Read(reader, "Count", count);
      targetSymptoms = UnityDataReadUtility.ReadList(reader, "TargetSymptoms", targetSymptoms);
    }
  }
}
