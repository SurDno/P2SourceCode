// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.OutdoorCrowdTemplates_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.OutdoorCrowds;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (OutdoorCrowdTemplates))]
  public class OutdoorCrowdTemplates_Generated : 
    OutdoorCrowdTemplates,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      OutdoorCrowdTemplates_Generated instance = Activator.CreateInstance<OutdoorCrowdTemplates_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      OutdoorCrowdTemplates_Generated templatesGenerated = (OutdoorCrowdTemplates_Generated) target2;
      templatesGenerated.Name = this.Name;
      CloneableObjectUtility.CopyListTo<OutdoorCrowdTemplate>(templatesGenerated.Templates, this.Templates);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
      DefaultDataWriteUtility.WriteListSerialize<OutdoorCrowdTemplate>(writer, "Templates", this.Templates);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
      this.Templates = DefaultDataReadUtility.ReadListSerialize<OutdoorCrowdTemplate>(reader, "Templates", this.Templates);
    }
  }
}
