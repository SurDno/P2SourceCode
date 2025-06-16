using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Cofe.Serializations.Data;
using Cofe.Serializations.Data.Xml;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.Commons;
using Engine.Common.Commons.Converters;
using Engine.Common.Generator;
using Engine.Common.Services;
using Engine.Impl.Services;
using Engine.Source.Commons;
using Engine.Source.Saves;
using Engine.Source.Services.Notifications;
using Engine.Source.Services.Saves;
using Engine.Source.UI;
using Inspectors;
using UnityEngine;

namespace Engine.Source.Services;

[GameService(typeof(INotificationService), typeof(NotificationService))]
[GenerateProxy(TypeEnum.StateSave | TypeEnum.StateLoad)]
public class NotificationService :
	INotificationService,
	IUpdatable,
	IInitialisable,
	ISavesController {
	[Inspected] private Dictionary<NotificationEnum, NotificationLayerItem> layers = new();

	[StateSaveProxy] [StateLoadProxy()] [Inspected]
	protected List<NotificationEnum> blockedTypes = new();

	public void AddNotify(NotificationEnum type, params object[] values) {
		if (TypeOrLayerBlocked(type))
			return;
		var layer = GetLayer(type);
		if (IgnoreSimilar(type) && ((layer.Notifaction != null && layer.Notifaction.Type == type) ||
		                            layer.Queue.Any(o => o.Type == type)))
			return;
		layer.Queue.Add(new NotificationItem {
			Type = type,
			Values = values
		});
	}

	public void BlockType(NotificationEnum type) {
		var notificationEnum = type;
		if (blockedTypes.Contains(notificationEnum))
			return;
		blockedTypes.Add(notificationEnum);
	}

	public void UnblockType(NotificationEnum type) {
		blockedTypes.Remove(type);
	}

	private bool TypeBlocked(NotificationEnum type) {
		return blockedTypes.Contains(type);
	}

	public bool TypeOrLayerBlocked(NotificationEnum type) {
		return TypeBlocked(type) || TypeBlocked(GetLayerEnum(type));
	}

	private bool IgnoreSimilar(NotificationEnum type) {
		switch (GetLayerEnum(type)) {
			case NotificationEnum.Tooltip_Layer:
			case NotificationEnum.Item_Layer:
			case NotificationEnum.MindMap_Layer:
				return false;
			default:
				return true;
		}
	}

	private bool UpdateLayerOnPause(NotificationEnum layer) {
		return layer == NotificationEnum.Tooltip_Layer;
	}

	public void RemoveNotify(NotificationEnum type) {
		var layer = GetLayer(type);
		if (layer.Notifaction != null && layer.Notifaction.Type == type) {
			layer.Notifaction.Shutdown();
			layer.Notifaction = null;
		}

		for (var index = layer.Queue.Count - 1; index >= 0; --index)
			if (layer.Queue[index].Type == type)
				layer.Queue.RemoveAt(index);
	}

	private NotificationEnum GetLayerEnum(NotificationEnum type) {
		return (NotificationEnum)((int)type / 1024 * 1024);
	}

	private NotificationLayerItem GetLayer(NotificationEnum type) {
		var layerEnum = GetLayerEnum(type);
		NotificationLayerItem layer;
		if (!layers.TryGetValue(layerEnum, out layer)) {
			layer = new NotificationLayerItem();
			layers.Add(layerEnum, layer);
		}

		return layer;
	}

	public void Initialise() {
		InstanceByRequest<UpdateService>.Instance.Updater.AddUpdatable(this);
	}

	public void Terminate() {
		InstanceByRequest<UpdateService>.Instance.Updater.RemoveUpdatable(this);
	}

	public void ComputeUpdate() {
		foreach (var layer in layers)
			if (!InstanceByRequest<EngineApplication>.Instance.IsPaused || UpdateLayerOnPause(layer.Key)) {
				var notificationLayerItem = layer.Value;
				if ((Object)notificationLayerItem.Notifaction != null) {
					if (notificationLayerItem.Notifaction.Complete) {
						notificationLayerItem.Notifaction.Shutdown();
						notificationLayerItem.Notifaction = null;
					}
				} else if (notificationLayerItem.Queue.Count != 0) {
					var notificationItem = notificationLayerItem.Queue[0];
					notificationLayerItem.Queue.RemoveAt(0);
					var service = ServiceLocator.GetService<UIService>();
					if (service != null) {
						var hudWindow = service.Get<IHudWindow>();
						if (hudWindow != null) {
							var notification = hudWindow.Create(notificationItem.Type);
							if (notification != null) {
								notificationLayerItem.Notifaction = notification;
								notification.Initialise(notificationItem.Type, notificationItem.Values);
							}
						}
					}
				}
			}
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
		foreach (var layer in layers) {
			var notificationLayerItem = layer.Value;
			notificationLayerItem.Notifaction?.Shutdown();
			notificationLayerItem.Queue.Clear();
		}

		layers.Clear();
		blockedTypes.Clear();
	}

	public void Save(IDataWriter writer, string context) {
		DefaultStateSaveUtility.SaveSerialize(writer, TypeUtility.GetTypeName(GetType()), this);
	}
}