// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.Dynamic.DynamicMindMap
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

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
using System;
using System.Collections.Generic;
using System.Xml;

#nullable disable
namespace PLVirtualMachine.Dynamic
{
  public class DynamicMindMap : IDynamicMindMapObject, ISerializeStateSave, IDynamicLoadSerializable
  {
    private VMLogicMap staticMindMap;
    private Guid dynamicGuid;
    private IMMPage pageInstance;
    private bool archive;
    private List<DynamicMindMapNode> nodes = new List<DynamicMindMapNode>();
    private static Dictionary<ulong, DynamicMindMap> mindMaps = new Dictionary<ulong, DynamicMindMap>((IEqualityComparer<ulong>) UlongComparer.Instance);
    private static List<ulong> mindMapOrderList = new List<ulong>();
    private static Dictionary<ulong, IDynamicMindMapObject> mmObjectsDict = new Dictionary<ulong, IDynamicMindMapObject>((IEqualityComparer<ulong>) UlongComparer.Instance);

    public DynamicMindMap(VMLogicMap staticMindMap, IMMPage page)
    {
      this.archive = false;
      this.staticMindMap = staticMindMap;
      this.dynamicGuid = DynamicMindMap.RegistrDynamicMMObject((IDynamicMindMapObject) this);
      if (staticMindMap.LogicMapType == ELogicMapType.LOGIC_MAP_TYPE_GLOBAL_MINDMAP)
        this.pageInstance = page;
      else if (staticMindMap.LogicMapType == ELogicMapType.LOGIC_MAP_TYPE_LOCAL_MINDMAP)
      {
        this.pageInstance = page;
      }
      else
      {
        Logger.AddError(string.Format("Invalid logic map {0} type {0} for mind map creation", (object) staticMindMap.Name, (object) staticMindMap.LogicMapType));
        return;
      }
      this.LoadNodes();
    }

    public string Name => this.staticMindMap.Name;

    public bool Global
    {
      get => this.PageInstance.Global;
      set => this.PageInstance.Global = value;
    }

    public ulong StaticGuid => this.staticMindMap == null ? 0UL : this.staticMindMap.BaseGuid;

    public Guid DynamicGuid => this.dynamicGuid;

    public void Think()
    {
      if (this.Archive)
        return;
      for (int index = 0; index < this.nodes.Count; ++index)
        this.nodes[index].Think();
    }

    public List<ILink> GetLinksByDestNode(DynamicMindMapNode node)
    {
      return this.staticMindMap.GetLinksByDestState((IGraphObject) node.StaticNode);
    }

    public List<ILink> GetLinksBySourceNode(DynamicMindMapNode node)
    {
      return this.staticMindMap.GetLinksBySourceState((IGraphObject) node.StaticNode);
    }

    public IMMPage PageInstance => this.pageInstance;

    public bool Archive
    {
      get => this.archive;
      set => this.archive = value;
    }

    public IGameMode GameTimeContext => this.staticMindMap.GameTimeContext;

    public void Free()
    {
      for (int index = 0; index < this.nodes.Count; ++index)
        this.nodes[index].Free();
      IMMService mindMapService = ServiceCache.MindMapService;
      if (this.PageInstance.Global)
        mindMapService.CurrentGlobalPage = (IMMPage) null;
      else
        mindMapService.RemovePage(this.PageInstance);
    }

    public void StateSave(IDataWriter writer)
    {
      SaveManagerUtility.Save(writer, "StaticGuid", this.staticMindMap.BaseGuid);
      SaveManagerUtility.Save(writer, "DynamicGuid", this.dynamicGuid);
      SaveManagerUtility.Save(writer, "IsGlobal", this.Global);
      SaveManagerUtility.SaveDynamicSerializableList<DynamicMindMapNode>(writer, "MMNodeList", this.nodes);
    }

    public void LoadFromXML(XmlElement xmlNode)
    {
      for (int i = 0; i < xmlNode.ChildNodes.Count; ++i)
      {
        XmlElement childNode1 = (XmlElement) xmlNode.ChildNodes[i];
        if (childNode1.Name == "DynamicGuid")
          this.dynamicGuid = VMSaveLoadManager.ReadGuid((XmlNode) childNode1);
        else if (childNode1.Name == "MMNodeList")
        {
          foreach (XmlElement childNode2 in childNode1.ChildNodes)
          {
            ulong num = VMSaveLoadManager.ReadUlong(childNode2.FirstChild);
            if (this.staticMindMap.GetNodeByGuid(num) == null)
              return;
            if (DynamicMindMap.mmObjectsDict.ContainsKey(num))
              ((DynamicMindMapNode) DynamicMindMap.mmObjectsDict[num]).LoadFromXML(childNode2);
          }
        }
      }
    }

    public void AfterSaveLoading()
    {
      for (int index = 0; index < this.nodes.Count; ++index)
        this.nodes[index].AfterSaveLoading();
    }

    public static DynamicMindMap AddGlobalMindMap(ILogicMap mindMap)
    {
      if (DynamicMindMap.mindMaps.ContainsKey(mindMap.BaseGuid))
      {
        foreach (KeyValuePair<ulong, DynamicMindMap> mindMap1 in DynamicMindMap.mindMaps)
        {
          if ((long) mindMap1.Value.StaticGuid != (long) mindMap.BaseGuid)
            mindMap1.Value.Global = false;
        }
        DynamicMindMap mindMap2 = DynamicMindMap.mindMaps[mindMap.BaseGuid];
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
        foreach (KeyValuePair<ulong, DynamicMindMap> mindMap3 in DynamicMindMap.mindMaps)
          mindMap3.Value.Global = false;
      }
      DynamicMindMap.mindMaps.Add(mindMap.BaseGuid, dynamicMindMap);
      DynamicMindMap.mindMapOrderList.Add(mindMap.BaseGuid);
      return dynamicMindMap;
    }

    public static DynamicMindMap AddMindMap(ILogicMap mindMap)
    {
      if (DynamicMindMap.mindMaps.ContainsKey(mindMap.BaseGuid))
      {
        Logger.AddError(string.Format("Cannot add mind map {0}: this mind map already added", (object) mindMap.Name));
        return (DynamicMindMap) null;
      }
      IMMPage page = ServiceCache.Factory.Create<IMMPage>();
      page.Global = false;
      page.Title = EngineAPIManager.CreateEngineTextInstance(mindMap.Title);
      ServiceCache.MindMapService.AddPage(page);
      DynamicMindMap dynamicMindMap = new DynamicMindMap((VMLogicMap) mindMap, page);
      DynamicMindMap.mindMaps.Add(mindMap.BaseGuid, dynamicMindMap);
      DynamicMindMap.mindMapOrderList.Add(mindMap.BaseGuid);
      return dynamicMindMap;
    }

    public static void RemoveMindMap(ILogicMap mindMap)
    {
      if (!DynamicMindMap.mindMaps.ContainsKey(mindMap.BaseGuid))
      {
        Logger.AddError(string.Format("Cannot remove mind map {0}: this mind map not added previously, or has been removed", (object) mindMap.Name));
      }
      else
      {
        DynamicMindMap.mindMaps[mindMap.BaseGuid].Free();
        DynamicMindMap.mindMaps.Remove(mindMap.BaseGuid);
        DynamicMindMap.mindMapOrderList.Remove(mindMap.BaseGuid);
      }
    }

    public static void Clear()
    {
      foreach (KeyValuePair<ulong, DynamicMindMap> mindMap in DynamicMindMap.mindMaps)
        mindMap.Value.Free();
      DynamicMindMap.mindMaps.Clear();
      DynamicMindMap.mindMapOrderList.Clear();
      DynamicMindMap.mmObjectsDict.Clear();
    }

    public static void Update()
    {
      foreach (KeyValuePair<ulong, DynamicMindMap> mindMap in DynamicMindMap.mindMaps)
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
        DynamicMindMap.mmObjectsDict.Add(mmObj.StaticGuid, mmObj);
        return guid;
      }
      catch (Exception ex)
      {
        Logger.AddError(string.Format("SaveLoad error: cannot register mindmap object guid={0}, error: {1}", (object) mmObj.StaticGuid, (object) ex.ToString()));
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
      IDynamicMindMapObject objectByStaticguid = DynamicMindMap.GetDynamicMMObjectByStaticguid(stGuid);
      if (objectByStaticguid != null && typeof (DynamicMindMapNode) == objectByStaticguid.GetType())
        return ((DynamicMindMapNode) objectByStaticguid).EngineNode;
      Logger.AddError(string.Format("Cannot find mind map node {0} with static guid {1}", (object) "", (object) stGuid));
      return (IMMNode) null;
    }

    public static void SaveMindMapsToXML(IDataWriter writer)
    {
      writer.Begin("MindMapSystem", (Type) null, true);
      writer.Begin("MindMapList", (Type) null, true);
      for (int index = 0; index < DynamicMindMap.mindMapOrderList.Count; ++index)
      {
        ulong mindMapOrder = DynamicMindMap.mindMapOrderList[index];
        if (DynamicMindMap.mindMaps.ContainsKey(mindMapOrder))
          SaveManagerUtility.SaveDynamicSerializable(writer, "Element", (ISerializeStateSave) DynamicMindMap.mindMaps[mindMapOrder]);
        else
          Logger.AddError(string.Format("SaveLoad error: invalid mind map guid in indexed list: {0}", (object) mindMapOrder));
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
        (num == 0 ? DynamicMindMap.AddMindMap(logicMap) : DynamicMindMap.AddGlobalMindMap(logicMap)).LoadFromXML(childNode);
      }
      foreach (KeyValuePair<ulong, DynamicMindMap> mindMap in DynamicMindMap.mindMaps)
        mindMap.Value.AfterSaveLoading();
    }

    public bool Modified => true;

    public void OnModify()
    {
    }

    public static IDynamicMindMapObject GetDynamicMMObjectByStaticguid(ulong stGuid)
    {
      IDynamicMindMapObject objectByStaticguid;
      if (DynamicMindMap.mmObjectsDict.TryGetValue(stGuid, out objectByStaticguid))
        return objectByStaticguid;
      Logger.AddError(string.Format("Cannot find mind map object {0} with static guid {1}", (object) "", (object) stGuid));
      return (IDynamicMindMapObject) null;
    }

    private void LoadNodes()
    {
      for (int index = 0; index < this.staticMindMap.Nodes.Count; ++index)
        this.AddMindMapNode((VMLogicMapNode) this.staticMindMap.Nodes[index]);
      this.LoadLinks();
    }

    private DynamicMindMapNode AddMindMapNode(VMLogicMapNode staticNode)
    {
      DynamicMindMapNode dynamicMindMapNode = new DynamicMindMapNode(this, staticNode);
      MMNodeKind nodeKindByNodeType = DynamicMindMap.GetEngineNodeKindByNodeType(dynamicMindMapNode.StaticNode.NodeType);
      Position position = new Position(dynamicMindMapNode.StaticNode.GameScreenPositionX, dynamicMindMapNode.StaticNode.GameScreenPositionY);
      IMMNode node = ServiceCache.Factory.Create<IMMNode>(dynamicMindMapNode.DynamicGuid);
      node.Position = position;
      node.NodeKind = nodeKindByNodeType;
      this.pageInstance.AddNode(node);
      dynamicMindMapNode.EngineNode = node;
      this.nodes.Add(dynamicMindMapNode);
      return dynamicMindMapNode;
    }

    private void LoadLinks()
    {
      for (int index = 0; index < this.staticMindMap.Links.Count; ++index)
      {
        ILink link1 = this.staticMindMap.Links[index];
        if (link1.Source == null)
          Logger.AddError(string.Format("Mind map link {0} source not defined at mind map {1}", (object) link1.BaseGuid, (object) this.Name));
        else if (!DynamicMindMap.mmObjectsDict.ContainsKey(link1.Source.BaseGuid))
          Logger.AddError(string.Format("invalid mm link source id={0} at mind map {1}", (object) link1.Source.BaseGuid, (object) this.Name));
        else if (link1.Destination == null)
          Logger.AddError(string.Format("Mind map link {0} destination not defined at mind map {1}", (object) link1.BaseGuid, (object) this.Name));
        else if (!DynamicMindMap.mmObjectsDict.ContainsKey(link1.Destination.BaseGuid))
        {
          Logger.AddError(string.Format("invalid mm link destination id={0} at mind map {1}", (object) link1.Destination.BaseGuid, (object) this.Name));
        }
        else
        {
          DynamicMindMapNode dynamicMindMapNode1 = (DynamicMindMapNode) DynamicMindMap.mmObjectsDict[link1.Source.BaseGuid];
          DynamicMindMapNode dynamicMindMapNode2 = (DynamicMindMapNode) DynamicMindMap.mmObjectsDict[link1.Destination.BaseGuid];
          IMMLink link2 = ServiceCache.Factory.Create<IMMLink>();
          link2.Origin = dynamicMindMapNode1.EngineNode;
          link2.Target = dynamicMindMapNode2.EngineNode;
          link2.Kind = MMLinkKind.DirectHidden;
          this.pageInstance.AddLink(link2);
        }
      }
    }
  }
}
