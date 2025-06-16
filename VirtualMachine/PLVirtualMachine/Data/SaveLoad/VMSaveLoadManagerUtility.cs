using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Dynamic;

namespace PLVirtualMachine.Data.SaveLoad
{
  public static class VMSaveLoadManagerUtility
  {
    private static Dictionary<string, XmlElement> preloadedNodes = new Dictionary<string, XmlElement>();
    private static XmlElement preloadedNode;

    public static void LoadEntities(XmlElement rootNode)
    {
      PreLoadObjectsFromXml(rootNode);
      foreach (KeyValuePair<string, XmlElement> preloadedNode in preloadedNodes)
      {
        string key = preloadedNode.Key;
        XmlElement xmlNode = preloadedNode.Value;
        VMEntity objectEntityByUniName = WorldEntityUtility.GetDynamicObjectEntityByUniName(key);
        if (objectEntityByUniName != null)
          objectEntityByUniName.LoadFromXML(xmlNode);
        else if (VirtualMachine.Instance.GameRootEntity != null)
        {
          XmlNode firstChild = xmlNode.FirstChild;
          Guid guid = StringUtility.ToGuid(firstChild.InnerText);
          ulong uint64 = StringUtility.ToUInt64(firstChild.NextSibling.InnerText);
          IBlueprint objectByGuid = (IBlueprint) IStaticDataContainer.StaticDataContainer.GetObjectByGuid(uint64);
          if (objectByGuid != null)
          {
            VMEntity childEntity = new VMEntity();
            childEntity.Initialize(objectByGuid, guid);
            childEntity.OnCreate(true);
            childEntity.LoadFromXML(xmlNode);
            VirtualMachine.Instance.GameRootEntity.AddChildEntity(childEntity);
          }
          else
            Logger.AddError(string.Format("Saveload error: static template with id = {0} not found", uint64));
        }
      }
    }

    private static void PreLoadObjectsFromXml(XmlElement xmlNode)
    {
      if (preloadedNode == xmlNode)
        return;
      preloadedNode = xmlNode;
      preloadedNodes = new Dictionary<string, XmlElement>(xmlNode.ChildNodes.Count);
      foreach (XmlNode childNode in xmlNode.ChildNodes)
      {
        XmlNode firstChild = childNode.FirstChild;
        if (firstChild == null)
        {
          Logger.AddError(string.Format("Saveload error: wrong saved entity in node {0} !", childNode));
        }
        else
        {
          string innerText = firstChild.InnerText;
          preloadedNodes[innerText] = (XmlElement) childNode;
        }
      }
    }

    public static XmlElement GetPreloadedObjectNodeByKey(string sEntityKeyGuid)
    {
      XmlElement preloadedObjectNodeByKey;
      preloadedNodes.TryGetValue(sEntityKeyGuid, out preloadedObjectNodeByKey);
      return preloadedObjectNodeByKey;
    }

    public static void Clear()
    {
      preloadedNodes.Clear();
      preloadedNode = null;
    }

    public static bool ObjectPreloaded(string saveLoadKey)
    {
      return preloadedNode != null && preloadedNodes.ContainsKey(saveLoadKey);
    }
  }
}
