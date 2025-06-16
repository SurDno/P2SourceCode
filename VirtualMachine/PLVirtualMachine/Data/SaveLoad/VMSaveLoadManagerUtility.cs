// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Data.SaveLoad.VMSaveLoadManagerUtility
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI.VMECS;
using PLVirtualMachine.Dynamic;
using System;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace PLVirtualMachine.Data.SaveLoad
{
  public static class VMSaveLoadManagerUtility
  {
    private static Dictionary<string, XmlElement> preloadedNodes = new Dictionary<string, XmlElement>();
    private static XmlElement preloadedNode = (XmlElement) null;

    public static void LoadEntities(XmlElement rootNode)
    {
      VMSaveLoadManagerUtility.PreLoadObjectsFromXml(rootNode);
      foreach (KeyValuePair<string, XmlElement> preloadedNode in VMSaveLoadManagerUtility.preloadedNodes)
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
            childEntity.Initialize((ILogicObject) objectByGuid, guid);
            childEntity.OnCreate(true);
            childEntity.LoadFromXML(xmlNode);
            VirtualMachine.Instance.GameRootEntity.AddChildEntity((VMBaseEntity) childEntity);
          }
          else
            Logger.AddError(string.Format("Saveload error: static template with id = {0} not found", (object) uint64));
        }
      }
    }

    private static void PreLoadObjectsFromXml(XmlElement xmlNode)
    {
      if (VMSaveLoadManagerUtility.preloadedNode == xmlNode)
        return;
      VMSaveLoadManagerUtility.preloadedNode = xmlNode;
      VMSaveLoadManagerUtility.preloadedNodes = new Dictionary<string, XmlElement>(xmlNode.ChildNodes.Count);
      foreach (XmlNode childNode in xmlNode.ChildNodes)
      {
        XmlNode firstChild = childNode.FirstChild;
        if (firstChild == null)
        {
          Logger.AddError(string.Format("Saveload error: wrong saved entity in node {0} !", (object) childNode.ToString()));
        }
        else
        {
          string innerText = firstChild.InnerText;
          VMSaveLoadManagerUtility.preloadedNodes[innerText] = (XmlElement) childNode;
        }
      }
    }

    public static XmlElement GetPreloadedObjectNodeByKey(string sEntityKeyGuid)
    {
      XmlElement preloadedObjectNodeByKey;
      VMSaveLoadManagerUtility.preloadedNodes.TryGetValue(sEntityKeyGuid, out preloadedObjectNodeByKey);
      return preloadedObjectNodeByKey;
    }

    public static void Clear()
    {
      VMSaveLoadManagerUtility.preloadedNodes.Clear();
      VMSaveLoadManagerUtility.preloadedNode = (XmlElement) null;
    }

    public static bool ObjectPreloaded(string saveLoadKey)
    {
      return VMSaveLoadManagerUtility.preloadedNode != null && VMSaveLoadManagerUtility.preloadedNodes.ContainsKey(saveLoadKey);
    }
  }
}
