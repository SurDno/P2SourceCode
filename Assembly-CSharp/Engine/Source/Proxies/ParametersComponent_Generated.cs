// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.ParametersComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Parameters;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ParametersComponent))]
  public class ParametersComponent_Generated : 
    ParametersComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      ParametersComponent_Generated instance = Activator.CreateInstance<ParametersComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      CloneableObjectUtility.CopyListTo<IParameter>(((ParametersComponent) target2).parameters, this.parameters);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteListSerialize<IParameter>(writer, "Parameters", this.parameters);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.parameters = DefaultDataReadUtility.ReadListSerialize<IParameter>(reader, "Parameters", this.parameters);
    }

    public void StateSave(IDataWriter writer)
    {
      CustomStateSaveUtility.SaveListParameters(writer, "Parameters", this.parameters);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.parameters = CustomStateLoadUtility.LoadListParameters(reader, "Parameters", this.parameters);
    }
  }
}
