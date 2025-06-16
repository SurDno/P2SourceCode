using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Components.Storable;
using Engine.Source.Commons.Abilities.Controllers;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (ItemMountAbilityController))]
  public class ItemMountAbilityController_Generated : 
    ItemMountAbilityController,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead
  {
    public object Clone()
    {
      ItemMountAbilityController_Generated instance = Activator.CreateInstance<ItemMountAbilityController_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2) => ((ItemMountAbilityController) target2).group = this.group;

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.WriteEnum<InventoryGroup>(writer, "Group", this.group);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.group = DefaultDataReadUtility.ReadEnum<InventoryGroup>(reader, "Group");
    }
  }
}
