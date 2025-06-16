using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
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

namespace Engine.Source.Services;

[GameService(typeof(MapService))]
[GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
public class MapService : ISavesController {
	[Inspected] private HashSet<IMapItem> items = new();
	[Inspected] private HashSet<IMapItem> hudItems = new();

	[Inspected]
	public IEnumerable<IMapItem> VisibleItems {
		get {
			return items.Where(o =>
				(o.Resource != null && o.Text != LocalizedText.Empty) || o.Nodes.Any() || o.TooltipResource != null);
		}
	}

	[StateSaveProxy]
	[StateLoadProxy()]
	[Inspected(Mutable = true)]
	public bool BullModeAvailable { get; set; }

	[StateSaveProxy]
	[StateLoadProxy()]
	[Inspected(Mutable = true)]
	public bool BullModeForced { get; set; }

	[Inspected] public FastTravelComponent FastTravelOrigin { get; set; } = null;

	[Inspected] public IMapItem FocusedItem { get; set; } = null;

	[Inspected] public IMMNode FocusedNode { get; set; } = null;

	public event Action<IMapItem> HUDItemAddEvent;

	public event Action<IMapItem> HUDItemRemoveEvent;

	[Inspected] public IEntity CustomMarker { get; set; }

	public IEnumerable<IMapItem> Items => items;

	public IEnumerable<IMapItem> QuestItems => hudItems;

	public IEntity Current { get; set; }

	public void AddMapItem(IMapItem item) {
		items.Add(item);
	}

	public void RemoveMapItem(IMapItem item) {
		items.Remove(item);
	}

	public void AddHUDItem(IMapItem item) {
		if (hudItems.Contains(item))
			return;
		hudItems.Add(item);
		var hudItemAddEvent = HUDItemAddEvent;
		if (hudItemAddEvent == null)
			return;
		hudItemAddEvent(item);
	}

	public void RemoveHUDItem(IMapItem item) {
		if (!hudItems.Contains(item))
			return;
		hudItems.Remove(item);
		var hudItemRemoveEvent = HUDItemRemoveEvent;
		if (hudItemRemoveEvent == null)
			return;
		hudItemRemoveEvent(item);
	}

	public IEnumerator Load(IErrorLoadingHandler errorHandler) {
		yield break;
	}

	public IEnumerator Load(XmlElement element, string context, IErrorLoadingHandler errorHandler) {
		var node = element[TypeUtility.GetTypeName(GetType())];
		if (node == null)
			errorHandler.LogError(TypeUtility.GetTypeName(GetType()) + " node not found , context : " + context);
		else {
			var reader = new XmlNodeDataReader(node, context);
			((ISerializeStateLoad)this).StateLoad(reader, GetType());
			yield break;
		}
	}

	public void Unload() {
		BullModeAvailable = false;
		BullModeForced = false;
	}

	public void Save(IDataWriter writer, string context) {
		DefaultStateSaveUtility.SaveSerialize(writer, TypeUtility.GetTypeName(GetType()), this);
	}
}