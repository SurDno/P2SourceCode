using System;
using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Cofe.Serializations.Data;
using Engine.Common.Comparers;
using Engine.Common.MindMap;
using Engine.Common.Services;
using Engine.Common.Types;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Common.EngineAPI;
using PLVirtualMachine.Common.Serialization;
using PLVirtualMachine.Data.SaveLoad;
using PLVirtualMachine.LogicMap;
using PLVirtualMachine.Objects;

namespace PLVirtualMachine.Dynamic
{
  public class DynamicMindMap : IDynamicMindMapObject, ISerializeStateSave, IDynamicLoadSerializable
  {
    private VMLogicMap staticMindMap;
    private Guid dynamicGuid;
    private IMMPage pageInstance;
    private bool archive;
    private List<DynamicMindMapNode> nodes = [];
    private static Dictionary<ulong, DynamicMindMap> mindMaps = new(UlongComparer.Instance);
    private static List<ulong> mindMapOrderList = [];
    private static Dictionary<ulong, IDynamicMindMapObject> mmObjectsDict = new(UlongComparer.Instance);

    public DynamicMindMap(VMLogicMap staticMindMap, IMMPage page)
    {
      archive = false;
      this.staticMindMap = staticMindMap;
      dynamicGuid = RegistrDynamicMMObject(this);
      if (staticMindMap.LogicMapType == ELogicMapType.LOGIC_MAP_TYPE_GLOBAL_MINDMAP)
        pageInstance = page;
      else if (staticMindMap.LogicMapType == ELogicMapType.LOGIC_MAP_TYPE_LOCAL_MINDMAP)
      {
        pageInstance = page;
      }
      else
      {
        Logger.AddError(string.Format("Invalid logic map {0} type {0} for mind map creation", staticMindMap.Name, staticMindMap.LogicMapType));
        return;
      }
      LoadNodes();
    }

    public string Name => staticMindMap.Name;

    public bool Global
    {
      get => PageInstance.Global;
      set => PageInstance.Global = value;
    }

    public ulong StaticGuid => staticMindMap == null ? 0UL : staticMindMap.BaseGuid;

    public Guid DynamicGuid => dynamicGuid;

    public void Think()
    {
      if (Archive)
        return;
      for (int index = 0; index < nodes.Count; ++index)
        nodes[index].Think();
    }

    public List<ILink> GetLinksByDestNode(DynamicMindMapNode node)
    {
      return staticMindMap.GetLinksByDestState(node.StaticNode);
    }

    public List<ILink> GetLinksBySourceNode(DynamicMindMapNode node)
    {
      return staticMindMap.GetLinksBySourceState(node.StaticNode);
    }

    public IMMPage PageInstance => pageInstance;

    public bool Archive
    {
      get => archive;
      set => archive = value;
    }

    public IGameMode GameTimeContext => staticMindMap.GameTimeContext;

    public void Free()
    {
      for (int index = 0; index < nodes.Count; ++index)
        nodes[index].Free();
      IMMService mindMapService = ServiceCache.MindMapService;
      if (PageInstance.Global)
        mindMapService.CurrentGlobalPage = null;
      else
        mindMapService.RemovePage(PageInstance);
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "StaticGuid", staticMindMap.BaseGuid);
      SaveManagerUtility.Save(writer, "DynamicGuid", dynamicGuid);
      SaveManagerUtility.Save(writer, "IsGlobal", Global);
      SaveManagerUtility.SaveDynamicSerializableList(writer, "MMNodeList", nodes);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode1 = (XmlElement) xmlNode.ChildNodes[i];
        if (childNode1.Name == "DynamicGuid")
          dynamicGuid = VMSaveLoadManager.ReadGuid(childNode1);
        else if (childNode1.Name == "MMNodeList")
        {
          foreach (XmlElement childNode2 in childNode1.ChildNodes)
          {
            ulong num = VMSaveLoadManager.ReadUlong(childNode2.FirstChild);
            if (staticMindMap.GetNodeByGuid(num) == null)
              return;
            if (mmObjectsDict.ContainsKey(num))
              ((DynamicMindMapNode) mmObjectsDict[num]).LoadFromXML(childNode2);
          }
        }
      }
    }

    public void AfterSaveLoading()
    {
      for (int index = 0; index < nodes.Count; ++index)
        nodes[index].AfterSaveLoading();
    }

    public static DynamicMindMap AddGlobalMindMap(ILogicMap mindMap)
    {
      if (mindMaps.ContainsKey(mindMap.BaseGuid))
      {
        foreach (KeyValuePair<ulong, DynamicMindMap> mindMap1 in mindMaps)
        {
          if ((long) mindMap1.Value.StaticGuid != (long) mindMap.BaseGuid)
            mindMap1.Value.Global = false;
        }
        DynamicMindMap mindMap2 = mindMaps[mindMap.BaseGuid];
        mindMap2.Global = true;
        ServiceCache.MindMapService.CurrentGlobalPage = mindMap2.PageInstance;
        return mindMap2;
      }
      IMMPage page = ServiceCache.Factory.Create<IMMPage>();
      page.Global = true;
      page.Title = EngineAPIManager.CreateEngineTextInstance(mindMap.Title);
      ServiceCache.MindMapService.CurrentGlobalPage = page;
      DynamicMindMap dynamicMindMap = new DynamicMindMap((VMLogicMap) mindMap, page);
      if (page.Global)
      {
        foreach (KeyValuePair<ulong, DynamicMindMap> mindMap3 in mindMaps)
          mindMap3.Value.Global = false;
      }
      mindMaps.Add(mindMap.BaseGuid, dynamicMindMap);
      mindMapOrderList.Add(mindMap.BaseGuid);
      return dynamicMindMap;
    }

    public static DynamicMindMap AddMindMap(ILogicMap mindMap)
    {
      if (mindMaps.ContainsKey(mindMap.BaseGuid))
      {
        Logger.AddError(string.Format("Cannot add mind map {0}: this mind map already added", mindMap.Name));
        return null;
      }
      IMMPage page = ServiceCache.Factory.Create<IMMPage>();
      page.Global = false;
      page.Title = EngineAPIManager.CreateEngineTextInstance(mindMap.Title);
      ServiceCache.MindMapService.AddPage(page);
      DynamicMindMap dynamicMindMap = new DynamicMindMap((VMLogicMap) mindMap, page);
      mindMaps.Add(mindMap.BaseGuid, dynamicMindMap);
      mindMapOrderList.Add(mindMap.BaseGuid);
      return dynamicMindMap;
    }

    public static void RemoveMindMap(ILogicMap mindMap)
    {
      if (!mindMaps.ContainsKey(mindMap.BaseGuid))
      {
        Logger.AddError(string.Format("Cannot remove mind map {0}: this mind map not added previously, or has been removed", mindMap.Name));
      }
      else
      {
        mindMaps[mindMap.BaseGuid].Free();
        mindMaps.Remove(mindMap.BaseGuid);
        mindMapOrderList.Remove(mindMap.BaseGuid);
      }
    }

    public static void Clear()
    {
      foreach (KeyValuePair<ulong, DynamicMindMap> mindMap in mindMaps)
        mindMap.Value.Free();
      mindMaps.Clear();
      mindMapOrderList.Clear();
      mmObjectsDict.Clear();
    }

    public static void Update()
    {
      foreach (KeyValuePair<ulong, DynamicMindMap> mindMap in mindMaps)
      {
        DynamicMindMap dynamicMindMap = mindMap.Value;
        if (!dynamicMindMap.Archive)
          dynamicMindMap.Think();
      }
    }

    public static Guid RegistrDynamicMMObject(IDynamicMindMapObject mmObj)
    {
      try
      {
        Guid guid = Guid.NewGuid();
        mmObjectsDict.Add(mmObj.StaticGuid, mmObj);
        return guid;
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("SaveLoad error: cannot register mindmap object guid={0}, error: {1}", mmObj.StaticGuid, ex));
        return new Guid();
      }
    }

    public static MMNodeKind GetEngineNodeKindByNodeType(ELogicMapNodeType nodeType)
    {
      if (nodeType == ELogicMapNodeType.LM_NODE_TYPE_COMMON)
        return MMNodeKind.Normal;
      return nodeType == ELogicMapNodeType.LM_NODE_TYPE_CONCLUSION ? MMNodeKind.Сonclusion : MMNodeKind.Mission;
    }

    public static MMContentKind GetEngineNodeContentKindByNodeContentType(
      EMMNodeContentType contentType)
    {
      switch (contentType)
      {
        case EMMNodeContentType.NODE_CONTENT_TYPE_FAILURE:
          return MMContentKind.Fail;
        case EMMNodeContentType.NODE_CONTENT_TYPE_SUCCESS:
          return MMContentKind.Success;
        case EMMNodeContentType.NODE_CONTENT_TYPE_KNOWLEDGE:
          return MMContentKind.Knowledge;
        case EMMNodeContentType.NODE_CONTENT_TYPE_ISOLATEDFACT:
          return MMContentKind.IsolatedFact;
        default:
          return MMContentKind.Normal;
      }
    }

    public static IMMNode GetEngineNodeByStaticGuid(ulong stGuid)
    {
      IDynamicMindMapObject objectByStaticguid = GetDynamicMMObjectByStaticguid(stGuid);
      if (objectByStaticguid != null && typeof (DynamicMindMapNode) == objectByStaticguid.GetType())
        return ((DynamicMindMapNode) objectByStaticguid).EngineNode;
      Logger.AddError(string.Format("Cannot find mind map node {0} with static guid {1}", "", stGuid));
      return null;
    }

    public static void SaveMindMapsToXML(IDataWriter writer)
    {
      writer.Begin("MindMapSystem", null, true);
      writer.Begin("MindMapList", null, true);
      for (int index = 0; index < mindMapOrderList.Count; ++index)
      {
        ulong mindMapOrder = mindMapOrderList[index];
        if (mindMaps.ContainsKey(mindMapOrder))
          SaveManagerUtility.SaveDynamicSerializable(writer, "Element", mindMaps[mindMapOrder]);
        else
          Logger.AddError(string.Format("SaveLoad error: invalid mind map guid in indexed list: {0}", mindMapOrder));
      }
      writer.End("MindMapList", true);
      writer.End("MindMapSystem", true);
    }

    public static void LoadMindMapsFromXML(XmlElement xmlNode)
    {
      XmlElement firstChild1 = (XmlElement) xmlNode.FirstChild;
      for (int i = 0; i < firstChild1.ChildNodes.Count; ++i)
      {
        XmlElement childNode = (XmlElement) firstChild1.ChildNodes[i];
        XmlNode firstChild2 = childNode.FirstChild;
        XmlNode nextSibling = firstChild2.NextSibling.NextSibling;
        ulong key = VMSaveLoadManager.ReadUlong(firstChild2);
        int num = VMSaveLoadManager.ReadBool(nextSibling) ? 1 : 0;
        ILogicMap logicMap = ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).LogicMaps[key];
        (num == 0 ? AddMindMap(logicMap) : AddGlobalMindMap(logicMap)).LoadFromXML(childNode);
      }
      foreach (KeyValuePair<ulong, DynamicMindMap> mindMap in mindMaps)
        mindMap.Value.AfterSaveLoading();
    }

    public bool Modified => true;

    public void OnModify()
    {
    }

    public static IDynamicMindMapObject GetDynamicMMObjectByStaticguid(ulong stGuid)
    {
      if (mmObjectsDict.TryGetValue(stGuid, out IDynamicMindMapObject objectByStaticguid))
        return objectByStaticguid;
      Logger.AddError(string.Format("Cannot find mind map object {0} with static guid {1}", "", stGuid));
      return null;
    }

    private void LoadNodes()
    {
      for (int index = 0; index < staticMindMap.Nodes.Count; ++index)
        AddMindMapNode((VMLogicMapNode) staticMindMap.Nodes[index]);
      LoadLinks();
    }

    private DynamicMindMapNode AddMindMapNode(VMLogicMapNode staticNode)
    {
      DynamicMindMapNode dynamicMindMapNode = new DynamicMindMapNode(this, staticNode);
      MMNodeKind nodeKindByNodeType = GetEngineNodeKindByNodeType(dynamicMindMapNode.StaticNode.NodeType);
      Position position = new Position(dynamicMindMapNode.StaticNode.GameScreenPositionX, dynamicMindMapNode.StaticNode.GameScreenPositionY);
      IMMNode node = ServiceCache.Factory.Create<IMMNode>(dynamicMindMapNode.DynamicGuid);
      node.Position = position;
      node.NodeKind = nodeKindByNodeType;
      pageInstance.AddNode(node);
      dynamicMindMapNode.EngineNode = node;
      nodes.Add(dynamicMindMapNode);
      return dynamicMindMapNode;
    }

    private void LoadLinks()
    {
      for (int index = 0; index < staticMindMap.Links.Count; ++index)
      {
        ILink link1 = staticMindMap.Links[index];
        if (link1.Source == null)
          Logger.AddError(string.Format("Mind map link {0} source not defined at mind map {1}", link1.BaseGuid, Name));
        else if (!mmObjectsDict.ContainsKey(link1.Source.BaseGuid))
          Logger.AddError(string.Format("invalid mm link source id={0} at mind map {1}", link1.Source.BaseGuid, Name));
        else if (link1.Destination == null)
          Logger.AddError(string.Format("Mind map link {0} destination not defined at mind map {1}", link1.BaseGuid, Name));
        else if (!mmObjectsDict.ContainsKey(link1.Destination.BaseGuid))
        {
          Logger.AddError(string.Format("invalid mm link destination id={0} at mind map {1}", link1.Destination.BaseGuid, Name));
        }
        else
        {
          DynamicMindMapNode dynamicMindMapNode1 = (DynamicMindMapNode) mmObjectsDict[link1.Source.BaseGuid];
          DynamicMindMapNode dynamicMindMapNode2 = (DynamicMindMapNode) mmObjectsDict[link1.Destination.BaseGuid];
          IMMLink link2 = ServiceCache.Factory.Create<IMMLink>();
          link2.Origin = dynamicMindMapNode1.EngineNode;
          link2.Target = dynamicMindMapNode2.EngineNode;
          link2.Kind = MMLinkKind.DirectHidden;
          pageInstance.AddLink(link2);
        }
      }
    }
  }
}
