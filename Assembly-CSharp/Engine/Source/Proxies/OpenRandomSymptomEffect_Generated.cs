using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons.Effects;
using Engine.Source.Connections;
using Engine.Source.Effects;
using Scripts.Tools.Serializations.Converters;
using System;

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
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      OpenRandomSymptomEffect_Generated symptomEffectGenerated = (OpenRandomSymptomEffect_Generated) target2;
      symptomEffectGenerated.queue = this.queue;
      symptomEffectGenerated.count = this.count;
      CloneableObjectUtility.FillListTo<Typed<IEntity>>(symptomEffectGenerated.targetSymptoms, this.targetSymptoms);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<ParameterEffectQueueEnum>(writer, "Queue", this.queue);
      DefaultDataWriteUtility.Write(writer, "Count", this.count);
      UnityDataWriteUtility.WriteList<IEntity>(writer, "TargetSymptoms", this.targetSymptoms);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.queue = DefaultDataReadUtility.ReadEnum<ParameterEffectQueueEnum>(reader, "Queue");
      this.count = DefaultDataReadUtility.Read(reader, "Count", this.count);
      this.targetSymptoms = UnityDataReadUtility.ReadList<IEntity>(reader, "TargetSymptoms", this.targetSymptoms);
    }
  }
}
