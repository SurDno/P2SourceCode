using System;
using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Interactable;
using Engine.Source.Components.Interactable;
using Engine.Source.Services.Inputs;
using Scripts.Tools.Serializations.Converters;

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
      CopyTo(instance);
      return instance;
    }

    public void CopyTo(object target2)
    {
      InteractItem_Generated interactItemGenerated = (InteractItem_Generated) target2;
      interactItemGenerated.type = type;
      interactItemGenerated.blueprint = blueprint;
      interactItemGenerated.action = action;
      interactItemGenerated.title = title;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum(writer, "Type", type);
      UnityDataWriteUtility.Write(writer, "Blueprint", blueprint);
      DefaultDataWriteUtility.WriteEnum(writer, "Action", action);
      DefaultDataWriteUtility.Write(writer, "Title", title);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.type = DefaultDataReadUtility.ReadEnum<InteractType>(reader, "Type");
      blueprint = UnityDataReadUtility.Read(reader, "Blueprint", blueprint);
      action = DefaultDataReadUtility.ReadEnum<GameActionType>(reader, "Action");
      title = DefaultDataReadUtility.Read(reader, "Title", title);
    }
  }
}
