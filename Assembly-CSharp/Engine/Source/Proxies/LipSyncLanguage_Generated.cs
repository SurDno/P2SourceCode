using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Services;
using Engine.Source.Commons;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (LipSyncLanguage))]
  public class LipSyncLanguage_Generated : 
    LipSyncLanguage,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      LipSyncLanguage_Generated instance = Activator.CreateInstance<LipSyncLanguage_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      LipSyncLanguage_Generated languageGenerated = (LipSyncLanguage_Generated) target2;
      languageGenerated.Language = this.Language;
      CloneableObjectUtility.CopyListTo<LipSyncInfo>(languageGenerated.LipSyncs, this.LipSyncs);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<LanguageEnum>(writer, "Language", this.Language);
      DefaultDataWriteUtility.WriteListSerialize<LipSyncInfo>(writer, "LipSyncs", this.LipSyncs);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Language = DefaultDataReadUtility.ReadEnum<LanguageEnum>(reader, "Language");
      this.LipSyncs = DefaultDataReadUtility.ReadListSerialize<LipSyncInfo>(reader, "LipSyncs", this.LipSyncs);
    }
  }
}
