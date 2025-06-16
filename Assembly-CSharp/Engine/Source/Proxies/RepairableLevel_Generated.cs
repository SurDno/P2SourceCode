// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.RepairableLevel_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components.Repairing;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (RepairableLevel))]
  public class RepairableLevel_Generated : 
    RepairableLevel,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      RepairableLevel_Generated instance = Activator.CreateInstance<RepairableLevel_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      RepairableLevel_Generated repairableLevelGenerated = (RepairableLevel_Generated) target2;
      repairableLevelGenerated.maxDurability = this.maxDurability;
      CloneableObjectUtility.CopyListTo<RepairableCostItem>(repairableLevelGenerated.cost, this.cost);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "MaxDurability", this.maxDurability);
      DefaultDataWriteUtility.WriteListSerialize<RepairableCostItem>(writer, "Cost", this.cost);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.maxDurability = DefaultDataReadUtility.Read(reader, "MaxDurability", this.maxDurability);
      this.cost = DefaultDataReadUtility.ReadListSerialize<RepairableCostItem>(reader, "Cost", this.cost);
    }
  }
}
