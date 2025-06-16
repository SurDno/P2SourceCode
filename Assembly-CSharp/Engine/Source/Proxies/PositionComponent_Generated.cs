// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.PositionComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (PositionComponent))]
  public class PositionComponent_Generated : 
    PositionComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      PositionComponent_Generated instance = Activator.CreateInstance<PositionComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      PositionComponent_Generated componentGenerated = (PositionComponent_Generated) target2;
      componentGenerated.position = this.position;
      componentGenerated.rotation = this.rotation;
    }

    public void DataWrite(IDataWriter writer)
    {
      UnityDataWriteUtility.Write(writer, "Position", this.position);
      UnityDataWriteUtility.Write(writer, "Rotation", this.rotation);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.position = UnityDataReadUtility.Read(reader, "Position", this.position);
      this.rotation = UnityDataReadUtility.Read(reader, "Rotation", this.rotation);
    }
  }
}
