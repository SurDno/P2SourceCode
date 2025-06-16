using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components;
using Engine.Source.Components;
using Scripts.Tools.Serializations.Converters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (StorageComponent))]
  public class StorageComponent_Generated : 
    StorageComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      StorageComponent_Generated instance = Activator.CreateInstance<StorageComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      StorageComponent_Generated componentGenerated = (StorageComponent_Generated) target2;
      componentGenerated.tag = this.tag;
      CloneableObjectUtility.CopyListTo<TemplateInfo>(componentGenerated.inventoryTemplates, this.inventoryTemplates);
      CloneableObjectUtility.CopyListTo<IStorableComponent>(componentGenerated.items, this.items);
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "Tag", this.tag);
      DefaultDataWriteUtility.WriteListSerialize<TemplateInfo>(writer, "InventoryTemplates", this.inventoryTemplates);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.tag = DefaultDataReadUtility.Read(reader, "Tag", this.tag);
      this.inventoryTemplates = DefaultDataReadUtility.ReadListSerialize<TemplateInfo>(reader, "InventoryTemplates", this.inventoryTemplates);
    }

    public void StateSave(IDataWriter writer)
    {
      CustomStateSaveUtility.SaveListReferences<IStorableComponent>(writer, "Items", this.items);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.items = CustomStateLoadUtility.LoadListReferences<IStorableComponent>(reader, "Items", this.items);
    }
  }
}
