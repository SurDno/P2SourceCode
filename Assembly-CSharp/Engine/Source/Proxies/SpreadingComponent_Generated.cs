// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.SpreadingComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (SpreadingComponent))]
  public class SpreadingComponent_Generated : 
    SpreadingComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      SpreadingComponent_Generated instance = Activator.CreateInstance<SpreadingComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      ((SpreadingComponent) target2).diseasedState = this.diseasedState;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<DiseasedStateEnum>(writer, "DiseasedState", this.diseasedState);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.diseasedState = DefaultDataReadUtility.ReadEnum<DiseasedStateEnum>(reader, "DiseasedState");
    }
  }
}
