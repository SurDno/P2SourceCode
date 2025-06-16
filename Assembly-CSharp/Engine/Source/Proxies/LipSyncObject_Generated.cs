using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Commons;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (LipSyncObject))]
  public class LipSyncObject_Generated : 
    LipSyncObject,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      return (object) ServiceCache.Factory.Instantiate<LipSyncObject_Generated>(this);
    }

    public void CopyTo(object target2)
    {
      LipSyncObject_Generated syncObjectGenerated = (LipSyncObject_Generated) target2;
      syncObjectGenerated.name = this.name;
      CloneableObjectUtility.CopyListTo<LipSyncLanguage>(syncObjectGenerated.languages, this.languages);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.id);
      DefaultDataWriteUtility.WriteListSerialize<LipSyncLanguage>(writer, "Languages", this.languages);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.id = DefaultDataReadUtility.Read(reader, "Id", this.id);
      this.languages = DefaultDataReadUtility.ReadListSerialize<LipSyncLanguage>(reader, "Languages", this.languages);
    }
  }
}
