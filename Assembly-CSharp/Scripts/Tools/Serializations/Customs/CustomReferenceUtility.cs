using Cofe.Serializations.Converters;
using Cofe.Serializations.Data;
using Cofe.Utility;
using Engine.Common;
using Engine.Common.MindMap;
using Engine.Common.Services;
using Engine.Source.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Scripts.Tools.Serializations.Customs
{
  public static class CustomReferenceUtility
  {
    public static void SaveReference(IDataWriter writer, string name, object value)
    {
      switch (value)
      {
        case null:
          Debug.LogError((object) ("Value is null , name : " + name));
          break;
        case IEntity entity:
          if (entity.IsDisposed)
          {
            Debug.LogError((object) ("Entity is disposed : " + (object) entity.Id));
            break;
          }
          writer.Begin(name, (System.Type) null, true);
          writer.WriteSimple("Id", DefaultConverter.ToString(entity.Id));
          writer.WriteSimple("IsTemplate", DefaultConverter.ToString(entity.IsTemplate));
          writer.End(name, true);
          break;
        case IComponent component:
          if (component.IsDisposed)
          {
            Debug.LogError((object) ("Component is disposed , type : " + component.GetType().Name));
            break;
          }
          writer.Begin(name, (System.Type) null, true);
          writer.WriteSimple("Id", DefaultConverter.ToString(component.Owner.Id));
          writer.End(name, true);
          break;
        case IObject @object:
          writer.Begin(name, (System.Type) null, true);
          writer.WriteSimple("Id", DefaultConverter.ToString(@object.Id));
          writer.End(name, true);
          break;
        case IMMNode mmNode:
          writer.Begin(name, (System.Type) null, true);
          writer.WriteSimple("Id", DefaultConverter.ToString(mmNode.Id));
          writer.End(name, true);
          break;
        default:
          Debug.LogError((object) ("Type not supported , name : " + name + " , type : " + value.GetType().Name));
          break;
      }
    }

    public static object LoadReference(IDataReader reader, System.Type type)
    {
      string str = reader.ReadSimple("Id");
      Guid objectId = DefaultConverter.ParseGuid(str);
      if (objectId == Guid.Empty)
      {
        Debug.LogError((object) ("Error id , data : " + str + " , context : " + reader.GetContext()));
        return (object) null;
      }
      if (TypeUtility.IsAssignableFrom(typeof (IEntity), type))
      {
        if (DefaultConverter.ParseBool(reader.ReadSimple("IsTemplate")))
        {
          IObject template = ServiceLocator.GetService<ITemplateService>().GetTemplate(type, objectId);
          if (template != null)
            return (object) template;
          Debug.LogError((object) ("Template not found , id : " + (object) objectId + " , context : " + reader.GetContext()));
          return (object) null;
        }
        IEntity entity = ServiceLocator.GetService<ISimulation>().Get(objectId);
        if (entity != null)
          return (object) entity;
        Debug.LogError((object) ("Entity not found , id : " + (object) objectId + " , context : " + reader.GetContext()));
        return (object) null;
      }
      if (TypeUtility.IsAssignableFrom(typeof (IComponent), type))
      {
        IEntity entity = ServiceLocator.GetService<ISimulation>().Get(objectId);
        if (entity == null)
        {
          Debug.LogError((object) ("Entity not found , id : " + (object) objectId + " , context : " + reader.GetContext()));
          return (object) null;
        }
        IComponent component = entity.GetComponent(type);
        if (component != null)
          return (object) component;
        Debug.LogError((object) ("Component not found , id : " + (object) objectId + " , type : " + type.Name + " , context : " + reader.GetContext()));
        return (object) null;
      }
      if (TypeUtility.IsAssignableFrom(typeof (IObject), type))
      {
        IObject template = ServiceLocator.GetService<ITemplateService>().GetTemplate(type, objectId);
        if (template != null)
          return (object) template;
        Debug.LogError((object) ("Template not found , id : " + (object) objectId + " , context : " + reader.GetContext()));
        return (object) null;
      }
      if (TypeUtility.IsAssignableFrom(typeof (IMMNode), type))
      {
        IMMNode mmNode = ServiceLocator.GetService<MMService>().Pages.SelectMany<IMMPage, IMMNode>((Func<IMMPage, IEnumerable<IMMNode>>) (o => o.Nodes)).FirstOrDefault<IMMNode>((Func<IMMNode, bool>) (o => o.Id == objectId));
        if (mmNode != null)
          return (object) mmNode;
        Debug.LogError((object) ("Node not found , id : " + (object) objectId + " , context : " + reader.GetContext()));
        return (object) null;
      }
      Debug.LogError((object) ("Type not supported , id : " + (object) objectId + " , type : " + type.Name + " , context : " + reader.GetContext()));
      return (object) null;
    }

    public static object LoadReference(IDataReader reader, string name, System.Type type)
    {
      IDataReader child = reader.GetChild(name);
      return child == null ? (object) null : CustomReferenceUtility.LoadReference(child, type);
    }
  }
}
