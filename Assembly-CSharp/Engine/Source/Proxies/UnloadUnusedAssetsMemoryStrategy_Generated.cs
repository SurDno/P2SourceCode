using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Otimizations;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (UnloadUnusedAssetsMemoryStrategy))]
  public class UnloadUnusedAssetsMemoryStrategy_Generated : 
    UnloadUnusedAssetsMemoryStrategy,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      UnloadUnusedAssetsMemoryStrategy_Generated instance = Activator.CreateInstance<UnloadUnusedAssetsMemoryStrategy_Generated>();
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
    }

    public void DataWrite(IDataWriter writer)
    {
    }

    public void DataRead(IDataReader reader, Type type)
    {
    }
  }
}
