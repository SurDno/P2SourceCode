// Decompiled with JetBrains decompiler
// Type: Engine.Source.Proxies.DynamicModelComponent_Generated
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Source.Components;
using Engine.Source.Connections;
using Scripts.Tools.Serializations.Converters;
using System;

#nullable disable
namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (DynamicModelComponent))]
  public class DynamicModelComponent_Generated : 
    DynamicModelComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      DynamicModelComponent_Generated instance = Activator.CreateInstance<DynamicModelComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      DynamicModelComponent_Generated componentGenerated = (DynamicModelComponent_Generated) target2;
      componentGenerated.isEnabled = this.isEnabled;
      componentGenerated.model = this.model;
      CloneableObjectUtility.FillListTo<Typed<IModel>>(componentGenerated.models, this.models);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
      UnityDataWriteUtility.Write<IModel>(writer, "Model", this.model);
      UnityDataWriteUtility.WriteList<IModel>(writer, "Models", this.models);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
      this.model = UnityDataReadUtility.Read<IModel>(reader, "Model", this.model);
      this.models = UnityDataReadUtility.ReadList<IModel>(reader, "Models", this.models);
    }

    public void StateSave(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
      UnityDataWriteUtility.Write<IModel>(writer, "Model", this.model);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
      this.model = UnityDataReadUtility.Read<IModel>(reader, "Model", this.model);
    }
  }
}
