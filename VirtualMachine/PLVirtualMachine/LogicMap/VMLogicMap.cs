using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using PLVirtualMachine.Objects;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.LogicMap
{
  [TypeData(EDataType.TLogicMap)]
  [DataFactory("MindMap")]
  public class VMLogicMap : 
    VMBaseObject,
    IStub,
    IEditorDataReader,
    ILogicMap,
    IGraph,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable,
    IOnAfterLoaded
  {
    [FieldData("Nodes", DataFieldType.Reference)]
    private List<IGraphObject> mapNodes = new List<IGraphObject>();
    [FieldData("Links", DataFieldType.Reference)]
    private List<ILink> mapLinks = new List<ILink>();
    [FieldData("LogicMapType")]
    private ELogicMapType logicMapType;
    [FieldData("GameTimeContext", DataFieldType.Reference)]
    private IGameMode gameTimeContext;
    [FieldData("Title", DataFieldType.Reference)]
    private VMGameString titleText;
    private bool isAfterLoaded;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "GameTimeContext":
              gameTimeContext = EditorDataReadUtility.ReadReference<IGameMode>(xml, creator);
              continue;
            case "Links":
              mapLinks = EditorDataReadUtility.ReadReferenceList(xml, creator, mapLinks);
              continue;
            case "LogicMapType":
              logicMapType = EditorDataReadUtility.ReadEnum<ELogicMapType>(xml);
              continue;
            case "Name":
              name = EditorDataReadUtility.ReadValue(xml, name);
              continue;
            case "Nodes":
              mapNodes = EditorDataReadUtility.ReadReferenceList(xml, creator, mapNodes);
              continue;
            case "Parent":
              parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "Title":
              titleText = EditorDataReadUtility.ReadReference<VMGameString>(xml, creator);
              continue;
            default:
              if (XMLDataLoader.Logs.Add(typeContext + " : " + xml.Name))
                Logger.AddError(typeContext + " : " + xml.Name);
              XmlReaderUtility.SkipNode(xml);
              continue;
          }
        }

        if (xml.NodeType == XmlNodeType.EndElement)
          break;
      }
    }

    public VMLogicMap(ulong guid)
      : base(guid)
    {
    }

    public ELogicMapType LogicMapType => logicMapType;

    public List<ILink> Links => mapLinks;

    public List<IGraphObject> Nodes => mapNodes;

    public VMLogicMapNode GetNodeByGuid(ulong nodeId)
    {
      foreach (IGraphObject mapNode in mapNodes)
      {
        if ((long) mapNode.BaseGuid == (long) nodeId)
          return (VMLogicMapNode) mapNode;
      }
      Logger.AddError(string.Format("Logic map node with id {0} not found in mindmap {1}", nodeId, Name));
      return null;
    }

    public List<ILink> GetLinksByDestState(IGraphObject state)
    {
      List<ILink> linksByDestState = new List<ILink>();
      for (int index = 0; index < mapLinks.Count; ++index)
      {
        if (mapLinks[index].Destination != null && (long) mapLinks[index].Destination.BaseGuid == (long) state.BaseGuid)
          linksByDestState.Add(mapLinks[index]);
      }
      return linksByDestState;
    }

    public List<ILink> GetLinksBySourceState(IGraphObject state)
    {
      List<ILink> linksBySourceState = new List<ILink>();
      for (int index = 0; index < mapLinks.Count; ++index)
      {
        if (mapLinks[index].Source != null && (long) mapLinks[index].Source.BaseGuid == (long) state.BaseGuid)
          linksBySourceState.Add(mapLinks[index]);
      }
      return linksBySourceState;
    }

    public virtual void OnAfterLoad()
    {
      for (int index = 0; index < mapNodes.Count; ++index)
        ((VMLogicMapNode) mapNodes[index]).OnAfterLoad();
      UpdateGraph();
      isAfterLoaded = true;
    }

    public bool IsAfterLoaded => isAfterLoaded;

    public void UpdateGraph()
    {
      for (int index = 0; index < mapNodes.Count; ++index)
        mapNodes[index].Update();
      for (int index = 0; index < mapLinks.Count; ++index)
        mapLinks[index].Update();
    }

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_LOGIC_MAP;

    public IGameMode GameTimeContext
    {
      get
      {
        return gameTimeContext == null ? ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).MainGameMode : gameTimeContext;
      }
    }

    public IGameString Title => titleText;

    public override void Clear()
    {
      base.Clear();
      if (mapNodes != null)
      {
        foreach (IContainer mapNode in mapNodes)
          mapNode.Clear();
        mapNodes.Clear();
        mapNodes = null;
      }
      if (mapLinks != null)
      {
        foreach (IContainer mapLink in mapLinks)
          mapLink.Clear();
        mapLinks.Clear();
        mapLinks = null;
      }
      gameTimeContext = null;
      titleText = null;
    }
  }
}
