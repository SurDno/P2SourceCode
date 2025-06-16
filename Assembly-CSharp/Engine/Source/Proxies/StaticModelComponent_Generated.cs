// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.StaticModelComponent_Generated
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
  [FactoryProxy(typeof (StaticModelComponent))]
  public class StaticModelComponent_Generated : 
    StaticModelComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      StaticModelComponent_Generated instance = Activator.CreateInstance<StaticModelComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      StaticModelComponent_Generated componentGenerated = (StaticModelComponent_Generated) target2;
      componentGenerated.relativePosition = this.relativePosition;
      componentGenerated.connection = this.connection;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "RelativePosition", this.relativePosition);
      UnityDataWriteUtility.Write<IScene>(writer, "Connection", this.connection);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.relativePosition = DefaultDataReadUtility.Read(reader, "RelativePosition", this.relativePosition);
      this.connection = UnityDataReadUtility.Read<IScene>(reader, "Connection", this.connection);
    }
  }
}
