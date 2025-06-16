using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Interactable;
using Engine.Source.Components.Interactable;
using Engine.Source.Services.Inputs;
using Scripts.Tools.Serializations.Converters;
using System;
using UnityEngine;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (InteractItem))]
  public class InteractItem_Generated : 
    InteractItem,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      InteractItem_Generated instance = Activator.CreateInstance<InteractItem_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      InteractItem_Generated interactItemGenerated = (InteractItem_Generated) target2;
      interactItemGenerated.type = this.type;
      interactItemGenerated.blueprint = this.blueprint;
      interactItemGenerated.action = this.action;
      interactItemGenerated.title = this.title;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<InteractType>(writer, "Type", this.type);
      UnityDataWriteUtility.Write<GameObject>(writer, "Blueprint", this.blueprint);
      DefaultDataWriteUtility.WriteEnum<GameActionType>(writer, "Action", this.action);
      DefaultDataWriteUtility.Write(writer, "Title", this.title);
    }

    public void DataRead(IDataReader reader, System.Type type)
    {
      this.type = DefaultDataReadUtility.ReadEnum<InteractType>(reader, "Type");
      this.blueprint = UnityDataReadUtility.Read<GameObject>(reader, "Blueprint", this.blueprint);
      this.action = DefaultDataReadUtility.ReadEnum<GameActionType>(reader, "Action");
      this.title = DefaultDataReadUtility.Read(reader, "Title", this.title);
    }
  }
}
