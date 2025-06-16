// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.DifficultyPresetData_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Difficulties;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (DifficultyPresetData))]
  public class DifficultyPresetData_Generated : 
    DifficultyPresetData,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      DifficultyPresetData_Generated instance = Activator.CreateInstance<DifficultyPresetData_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      DifficultyPresetData_Generated presetDataGenerated = (DifficultyPresetData_Generated) target2;
      presetDataGenerated.Name = this.Name;
      CloneableObjectUtility.CopyListTo<DifficultyPresetItemData>(presetDataGenerated.Items, this.Items);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
      DefaultDataWriteUtility.WriteListSerialize<DifficultyPresetItemData>(writer, "Items", this.Items);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
      this.Items = DefaultDataReadUtility.ReadListSerialize<DifficultyPresetItemData>(reader, "Items", this.Items);
    }
  }
}
