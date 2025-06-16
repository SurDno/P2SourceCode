using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Otimizations;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (AllocMemoryStrategy))]
  public class AllocMemoryStrategy_Generated : 
    AllocMemoryStrategy,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      AllocMemoryStrategy_Generated instance = Activator.CreateInstance<AllocMemoryStrategy_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      AllocMemoryStrategy_Generated strategyGenerated = (AllocMemoryStrategy_Generated) target2;
      strategyGenerated.maxMemory = this.maxMemory;
      strategyGenerated.minMemory = this.minMemory;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "MaxMemory", this.maxMemory);
      DefaultDataWriteUtility.Write(writer, "MinMemory", this.minMemory);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.maxMemory = DefaultDataReadUtility.Read(reader, "MaxMemory", this.maxMemory);
      this.minMemory = DefaultDataReadUtility.Read(reader, "MinMemory", this.minMemory);
    }
  }
}
