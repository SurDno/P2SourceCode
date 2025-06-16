using System;
using System.Linq;
using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.MindMap;
using Engine.Common.Services;
using Engine.Source.Services;
using UnityEngine;

namespace Scripts.Tools.Serializations.Customs;

public static class CustomReferenceUtility {
	public static void SaveReference(IDataWriter writer, string name, object value) {
		switch (value) {
			case null:
				Debug.LogError("Value is null , name : " + name);
				break;
			case IEntity entity:
				if (entity.IsDisposed) {
					Debug.LogError("Entity is disposed : " + entity.Id);
					break;
				}

				writer.Begin(name, null, true);
				writer.WriteSimple("Id", DefaultConverter.ToString(entity.Id));
				writer.WriteSimple("IsTemplate", DefaultConverter.ToString(entity.IsTemplate));
				writer.End(name, true);
				break;
			case IComponent component:
				if (component.IsDisposed) {
					Debug.LogError("Component is disposed , type : " + component.GetType().Name);
					break;
				}

				writer.Begin(name, null, true);
				writer.WriteSimple("Id", DefaultConverter.ToString(component.Owner.Id));
				writer.End(name, true);
				break;
			case IObject @object:
				writer.Begin(name, null, true);
				writer.WriteSimple("Id", DefaultConverter.ToString(@object.Id));
				writer.End(name, true);
				break;
			case IMMNode mmNode:
				writer.Begin(name, null, true);
				writer.WriteSimple("Id", DefaultConverter.ToString(mmNode.Id));
				writer.End(name, true);
				break;
			default:
				Debug.LogError("Type not supported , name : " + name + " , type : " + value.GetType().Name);
				break;
		}
	}

	public static object LoadReference(IDataReader reader, Type type) {
		var str = reader.ReadSimple("Id");
		var objectId = DefaultConverter.ParseGuid(str);
		if (objectId == Guid.Empty) {
			Debug.LogError("Error id , data : " + str + " , context : " + reader.GetContext());
			return null;
		}

		if (TypeUtility.IsAssignableFrom(typeof(IEntity), type)) {
			if (DefaultConverter.ParseBool(reader.ReadSimple("IsTemplate"))) {
				var template = ServiceLocator.GetService<ITemplateService>().GetTemplate(type, objectId);
				if (template != null)
					return template;
				Debug.LogError("Template not found , id : " + objectId + " , context : " + reader.GetContext());
				return null;
			}

			var entity = ServiceLocator.GetService<ISimulation>().Get(objectId);
			if (entity != null)
				return entity;
			Debug.LogError("Entity not found , id : " + objectId + " , context : " + reader.GetContext());
			return null;
		}

		if (TypeUtility.IsAssignableFrom(typeof(IComponent), type)) {
			var entity = ServiceLocator.GetService<ISimulation>().Get(objectId);
			if (entity == null) {
				Debug.LogError("Entity not found , id : " + objectId + " , context : " + reader.GetContext());
				return null;
			}

			var component = entity.GetComponent(type);
			if (component != null)
				return component;
			Debug.LogError("Component not found , id : " + objectId + " , type : " + type.Name + " , context : " +
			               reader.GetContext());
			return null;
		}

		if (TypeUtility.IsAssignableFrom(typeof(IObject), type)) {
			var template = ServiceLocator.GetService<ITemplateService>().GetTemplate(type, objectId);
			if (template != null)
				return template;
			Debug.LogError("Template not found , id : " + objectId + " , context : " + reader.GetContext());
			return null;
		}

		if (TypeUtility.IsAssignableFrom(typeof(IMMNode), type)) {
			var mmNode = ServiceLocator.GetService<MMService>().Pages.SelectMany(o => o.Nodes)
				.FirstOrDefault(o => o.Id == objectId);
			if (mmNode != null)
				return mmNode;
			Debug.LogError("Node not found , id : " + objectId + " , context : " + reader.GetContext());
			return null;
		}

		Debug.LogError("Type not supported , id : " + objectId + " , type : " + type.Name + " , context : " +
		               reader.GetContext());
		return null;
	}

	public static object LoadReference(IDataReader reader, string name, Type type) {
		var child = reader.GetChild(name);
		return child == null ? null : LoadReference(child, type);
	}
}