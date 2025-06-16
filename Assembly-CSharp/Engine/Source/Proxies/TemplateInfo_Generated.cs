// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.TemplateInfo_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (TemplateInfo))]
  public class TemplateInfo_Generated : 
    TemplateInfo,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      TemplateInfo_Generated instance = Activator.CreateInstance<TemplateInfo_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      TemplateInfo_Generated templateInfoGenerated = (TemplateInfo_Generated) target2;
      templateInfoGenerated.Id = this.Id;
      templateInfoGenerated.InventoryTemplate = this.InventoryTemplate;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Id", this.Id);
      UnityDataWriteUtility.Write<IEntity>(writer, "InventoryTemplate", this.InventoryTemplate);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Id = DefaultDataReadUtility.Read(reader, "Id", this.Id);
      this.InventoryTemplate = UnityDataReadUtility.Read<IEntity>(reader, "InventoryTemplate", this.InventoryTemplate);
    }
  }
}
