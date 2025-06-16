// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.OutdoorCrowdTemplate_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Loggers;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.OutdoorCrowds;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (OutdoorCrowdTemplate))]
  public class OutdoorCrowdTemplate_Generated : 
    OutdoorCrowdTemplate,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      OutdoorCrowdTemplate_Generated instance = Activator.CreateInstance<OutdoorCrowdTemplate_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      OutdoorCrowdTemplate_Generated templateGenerated = (OutdoorCrowdTemplate_Generated) target2;
      templateGenerated.Name = this.Name;
      templateGenerated.Template = this.Template;
      ((ICopyable) this.Day).CopyTo((object) templateGenerated.Day);
      ((ICopyable) this.Night).CopyTo((object) templateGenerated.Night);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
      UnityDataWriteUtility.Write<IEntity>(writer, "Template", this.Template);
      DefaultDataWriteUtility.WriteSerialize<OutdoorCrowdTemplateCount>(writer, "Day", this.Day);
      DefaultDataWriteUtility.WriteSerialize<OutdoorCrowdTemplateCount>(writer, "Night", this.Night);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
      this.Template = UnityDataReadUtility.Read<IEntity>(reader, "Template", this.Template);
      IDataReader child1 = reader.GetChild("Day");
      OutdoorCrowdTemplateCount day = this.Day;
      if (day is ISerializeDataRead serializeDataRead1)
        serializeDataRead1.DataRead(child1, typeof (OutdoorCrowdTemplateCount));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(day.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
      IDataReader child2 = reader.GetChild("Night");
      OutdoorCrowdTemplateCount night = this.Night;
      if (night is ISerializeDataRead serializeDataRead2)
        serializeDataRead2.DataRead(child2, typeof (OutdoorCrowdTemplateCount));
      else
        Logger.AddError("Type : " + TypeUtility.GetTypeName(night.GetType()) + " is not " + typeof (ISerializeDataRead).Name);
    }
  }
}
