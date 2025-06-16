// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.CrowdTemplateInfo_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components.Crowds;
using Engine.Source.Connections;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (CrowdTemplateInfo))]
  public class CrowdTemplateInfo_Generated : 
    CrowdTemplateInfo,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      CrowdTemplateInfo_Generated instance = Activator.CreateInstance<CrowdTemplateInfo_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CrowdTemplateInfo_Generated templateInfoGenerated = (CrowdTemplateInfo_Generated) target2;
      CloneableObjectUtility.FillListTo<Typed<IEntity>>(templateInfoGenerated.Templates, this.Templates);
      templateInfoGenerated.Min = this.Min;
      templateInfoGenerated.Max = this.Max;
      templateInfoGenerated.Chance = this.Chance;
    }

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.WriteList<IEntity>(writer, "Templates", this.Templates);
      DefaultDataWriteUtility.Write(writer, "Min", this.Min);
      DefaultDataWriteUtility.Write(writer, "Max", this.Max);
      DefaultDataWriteUtility.Write(writer, "Chance", this.Chance);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Templates = UnityDataReadUtility.ReadList<IEntity>(reader, "Templates", this.Templates);
      this.Min = DefaultDataReadUtility.Read(reader, "Min", this.Min);
      this.Max = DefaultDataReadUtility.Read(reader, "Max", this.Max);
      this.Chance = DefaultDataReadUtility.Read(reader, "Chance", this.Chance);
    }
  }
}
