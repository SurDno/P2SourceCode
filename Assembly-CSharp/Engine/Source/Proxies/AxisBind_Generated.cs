// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.AxisBind_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Services.Inputs;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (AxisBind))]
  public class AxisBind_Generated : 
    AxisBind,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      AxisBind_Generated instance = Activator.CreateInstance<AxisBind_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      AxisBind_Generated axisBindGenerated = (AxisBind_Generated) target2;
      axisBindGenerated.Name = this.Name;
      axisBindGenerated.Axis = this.Axis;
      axisBindGenerated.Dead = this.Dead;
      axisBindGenerated.Normalize = this.Normalize;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Name", this.Name);
      DefaultDataWriteUtility.Write(writer, "Axis", this.Axis);
      DefaultDataWriteUtility.Write(writer, "Dead", this.Dead);
      DefaultDataWriteUtility.Write(writer, "Normalize", this.Normalize);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.Name = DefaultDataReadUtility.Read(reader, "Name", this.Name);
      this.Axis = DefaultDataReadUtility.Read(reader, "Axis", this.Axis);
      this.Dead = DefaultDataReadUtility.Read(reader, "Dead", this.Dead);
      this.Normalize = DefaultDataReadUtility.Read(reader, "Normalize", this.Normalize);
    }
  }
}
