using Cofe.Proxies;
using Cofe.Serializations.Data;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Cloneable;
using Engine.Common.Commons.Converters;
using Engine.Common.Maps;
using Engine.Common.MindMap;
using Engine.Source.Components.Maps;
using Scripts.Tools.Serializations.Converters;
using System;

namespace Engine.Source.Proxies
{
  [FactoryProxy(typeof (MapItemComponent))]
  public class MapItemComponent_Generated : 
    MapItemComponent,
    ICloneable,
    ICopyable,
    ISerializeDataWrite,
    ISerializeDataRead,
    ISerializeStateSave,
    ISerializeStateLoad
  {
    public object Clone()
    {
      MapItemComponent_Generated instance = Activator.CreateInstance<MapItemComponent_Generated>();
      this.CopyTo((object) instance);
      return (object) instance;
    }

    public void CopyTo(object target2)
    {
      MapItemComponent_Generated componentGenerated = (MapItemComponent_Generated) target2;
      componentGenerated.isEnabled = this.isEnabled;
      componentGenerated.placeholder = this.placeholder;
    }

    public void DataWrite(IDataWriter writer)
    {
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
      UnityDataWriteUtility.Write<IMapPlaceholder>(writer, "Placeholder", this.placeholder);
    }

    public void DataRead(IDataReader reader, Type type)
    {
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
      this.placeholder = UnityDataReadUtility.Read<IMapPlaceholder>(reader, "Placeholder", this.placeholder);
    }

    public void StateSave(IDataWriter writer)
    {
      CustomStateSaveUtility.SaveReference(writer, "BoundCharacter", (object) this.BoundCharacter);
      EngineDataWriteUtility.Write(writer, "Text", this.Text);
      EngineDataWriteUtility.Write(writer, "TooltipText", this.TooltipText);
      DefaultDataWriteUtility.Write(writer, "IsEnabled", this.isEnabled);
      CustomStateSaveUtility.SaveListReferences<IMMNode>(writer, "Nodes", this.nodes);
      CustomStateSaveUtility.SaveReference(writer, "TooltipResource", (object) this.tooltipResource);
      DefaultDataWriteUtility.Write(writer, "Discovered", this.discovered);
      EngineDataWriteUtility.Write(writer, "Title", this.title);
    }

    public void StateLoad(IDataReader reader, Type type)
    {
      this.BoundCharacter = CustomStateLoadUtility.LoadReference<IEntity>(reader, "BoundCharacter", this.BoundCharacter);
      this.Text = EngineDataReadUtility.Read(reader, "Text", this.Text);
      this.TooltipText = EngineDataReadUtility.Read(reader, "TooltipText", this.TooltipText);
      this.isEnabled = DefaultDataReadUtility.Read(reader, "IsEnabled", this.isEnabled);
      this.nodes = CustomStateLoadUtility.LoadListReferences<IMMNode>(reader, "Nodes", this.nodes);
      this.tooltipResource = CustomStateLoadUtility.LoadReference<IMapTooltipResource>(reader, "TooltipResource", this.tooltipResource);
      this.discovered = DefaultDataReadUtility.Read(reader, "Discovered", this.discovered);
      this.title = EngineDataReadUtility.Read(reader, "Title", this.title);
    }
  }
}
