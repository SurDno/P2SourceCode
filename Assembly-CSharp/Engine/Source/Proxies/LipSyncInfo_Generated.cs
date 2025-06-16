using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Commons;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (LipSyncInfo))]
  public class LipSyncInfo_Generated : 
    LipSyncInfo,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      LipSyncInfo_Generated instance = Activator.CreateInstance<LipSyncInfo_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      LipSyncInfo_Generated syncInfoGenerated = (LipSyncInfo_Generated) target2;
      syncInfoGenerated.Clip = this.Clip;
      syncInfoGenerated.Data = this.Data;
      syncInfoGenerated.Tag = this.Tag;
    }

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write<AudioClip>(writer, "Clip", this.Clip);
      DefaultDataWriteUtility.Write(writer, "Data", this.Data);
      DefaultDataWriteUtility.Write(writer, "Tag", this.Tag);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.Clip = UnityDataReadUtility.Read<AudioClip>(reader, "Clip", this.Clip);
      this.Data = DefaultDataReadUtility.Read(reader, "Data", this.Data);
      this.Tag = DefaultDataReadUtility.Read(reader, "Tag", this.Tag);
    }
  }
}
