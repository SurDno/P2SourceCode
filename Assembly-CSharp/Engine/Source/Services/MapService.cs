// Decompiled with JetBrains decompiler
// Type: Engine.Source.Services.MapService
// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4BDBC255-6935-43E6-AE4B-B6BF8667EAAF
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\Assembly-CSharp.dll

using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.MindMap;
using Engine.Common.Types;
using Engine.Source.Components;
using Engine.Source.Saves;
using Engine.Source.Services.Saves;
using Inspectors;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

#nullable disable
namespace Engine.Source.Services
{
  [GameService(new Type[] {typeof (MapService)})]
  [GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
  public class MapService : ISavesController
  {
    [Inspected]
    private HashSet<IMapItem> items = new HashSet<IMapItem>();
    [Inspected]
    private HashSet<IMapItem> hudItems = new HashSet<IMapItem>();

    [Inspected]
    public IEnumerable<IMapItem> VisibleItems
    {
      get
      {
        return this.items.Where<IMapItem>((Func<IMapItem, bool>) (o => o.Resource != null && o.Text != LocalizedText.Empty || o.Nodes.Any<IMMNode>() || o.TooltipResource != null));
      }
    }

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected(Mutable = true)]
    public bool BullModeAvailable { get; set; } = false;

    [StateSaveProxy(MemberEnum.None)]
    [StateLoadProxy(MemberEnum.None)]
    [Inspected(Mutable = true)]
    public bool BullModeForced { get; set; } = false;

    [Inspected]
    public FastTravelComponent FastTravelOrigin { get; set; } = (FastTravelComponent) null;

    [Inspected]
    public IMapItem FocusedItem { get; set; } = (IMapItem) null;

    [Inspected]
    public IMMNode FocusedNode { get; set; } = (IMMNode) null;

    public event Action<IMapItem> HUDItemAddEvent;

    public event Action<IMapItem> HUDItemRemoveEvent;

    [Inspected]
    public IEntity CustomMarker { get; set; }

    public IEnumerable<IMapItem> Items => (IEnumerable<IMapItem>) this.items;

    public IEnumerable<IMapItem> QuestItems => (IEnumerable<IMapItem>) this.hudItems;

    public IEntity Current { get; set; }

    public void AddMapItem(IMapItem item) => this.items.Add(item);

    public void RemoveMapItem(IMapItem item) => this.items.Remove(item);

    public void AddHUDItem(IMapItem item)
    {
      if (this.hudItems.Contains(item))
        return;
      this.hudItems.Add(item);
      Action<IMapItem> hudItemAddEvent = this.HUDItemAddEvent;
      if (hudItemAddEvent == null)
        return;
      hudItemAddEvent(item);
    }

    public void RemoveHUDItem(IMapItem item)
    {
      if (!this.hudItems.Contains(item))
        return;
      this.hudItems.Remove(item);
      Action<IMapItem> hudItemRemoveEvent = this.HUDItemRemoveEvent;
      if (hudItemRemoveEvent == null)
        return;
      hudItemRemoveEvent(item);
    }

    public IEnumerator Load(IErrorLoadingHandler errorHandler)
    {
      yield break;
    }

    public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler)
    {
      XmlElement node = element[TypeUtility.GetTypeName(this.GetType())];
      if (node == null)
      {
        errorHandler.LogError(TypeUtility.GetTypeName(this.GetType()) + " node not found , context : " + context);
      }
      else
      {
        XmlNodeDataReader reader = new XmlNodeDataReader((XmlNode) node, context);
        ((ISerializeStateLoad) this).StateLoad((IDataReader) reader, this.GetType());
        yield break;
      }
    }

    public void Unload()
    {
      this.BullModeAvailable = false;
      this.BullModeForced = false;
    }

    public void Save(IDataWriter writer, string context)
    {
      DefaultStateSaveUtility.SaveSerialize<MapService>(writer, TypeUtility.GetTypeName(this.GetType()), this);
    }
  }
}
