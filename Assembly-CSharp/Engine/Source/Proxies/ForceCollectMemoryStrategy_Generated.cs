using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Otimizations;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ForceCollectMemoryStrategy))]
  public class ForceCollectMemoryStrategy_Generated : 
    ForceCollectMemoryStrategy,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ForceCollectMemoryStrategy_Generated instance = Activator.CreateInstance<ForceCollectMemoryStrategy_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ((ForceCollectMemoryStrategy) target2).disableGC = this.disableGC;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "DisableGC", this.disableGC);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.disableGC = DefaultDataReadUtility.Read(reader, "DisableGC", this.disableGC);
    }
  }
}
