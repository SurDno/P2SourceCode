// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.LogicMap.VMLogicMap
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using PLVirtualMachine.Objects;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

#nullable disable
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
    [FieldData("LogicMapType", DataFieldType.None)]
    private ELogicMapType logicMapType;
    [FieldData("GameTimeContext", DataFieldType.Reference)]
    private IGameMode gameTimeContext;
    [FieldData("Title", DataFieldType.Reference)]
    private VMGameString titleText;
    private bool isAfterLoaded;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "GameTimeContext":
              this.gameTimeContext = EditorDataReadUtility.ReadReference<IGameMode>(xml, creator);
              continue;
            case "Links":
              this.mapLinks = EditorDataReadUtility.ReadReferenceList<ILink>(xml, creator, this.mapLinks);
              continue;
            case "LogicMapType":
              this.logicMapType = EditorDataReadUtility.ReadEnum<ELogicMapType>(xml);
              continue;
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
              continue;
            case "Nodes":
              this.mapNodes = EditorDataReadUtility.ReadReferenceList<IGraphObject>(xml, creator, this.mapNodes);
              continue;
            case "Parent":
              this.parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "Title":
              this.titleText = EditorDataReadUtility.ReadReference<VMGameString>(xml, creator);
              continue;
            default:
              if (XMLDataLoader.Logs.Add(typeContext + " : " + xml.Name))
                Logger.AddError(typeContext + " : " + xml.Name);
              XmlReaderUtility.SkipNode(xml);
              continue;
          }
        }
        else if (xml.NodeType == XmlNodeType.EndElement)
          break;
      }
    }

    public VMLogicMap(ulong guid)
      : base(guid)
    {
    }

    public ELogicMapType LogicMapType => this.logicMapType;

    public List<ILink> Links => this.mapLinks;

    public List<IGraphObject> Nodes => this.mapNodes;

    public VMLogicMapNode GetNodeByGuid(ulong nodeId)
    {
      foreach (IGraphObject mapNode in this.mapNodes)
      {
        if ((long) mapNode.BaseGuid == (long) nodeId)
          return (VMLogicMapNode) mapNode;
      }
      Logger.AddError(string.Format("Logic map node with id {0} not found in mindmap {1}", (object) nodeId, (object) this.Name));
      return (VMLogicMapNode) null;
    }

    public List<ILink> GetLinksByDestState(IGraphObject state)
    {
      List<ILink> linksByDestState = new List<ILink>();
      for (int index = 0; index < this.mapLinks.Count; ++index)
      {
        if (this.mapLinks[index].Destination != null && (long) this.mapLinks[index].Destination.BaseGuid == (long) state.BaseGuid)
          linksByDestState.Add(this.mapLinks[index]);
      }
      return linksByDestState;
    }

    public List<ILink> GetLinksBySourceState(IGraphObject state)
    {
      List<ILink> linksBySourceState = new List<ILink>();
      for (int index = 0; index < this.mapLinks.Count; ++index)
      {
        if (this.mapLinks[index].Source != null && (long) this.mapLinks[index].Source.BaseGuid == (long) state.BaseGuid)
          linksBySourceState.Add(this.mapLinks[index]);
      }
      return linksBySourceState;
    }

    public virtual void OnAfterLoad()
    {
      for (int index = 0; index < this.mapNodes.Count; ++index)
        ((VMLogicMapNode) this.mapNodes[index]).OnAfterLoad();
      this.UpdateGraph();
      this.isAfterLoaded = true;
    }

    public bool IsAfterLoaded => this.isAfterLoaded;

    public void UpdateGraph()
    {
      for (int index = 0; index < this.mapNodes.Count; ++index)
        this.mapNodes[index].Update();
      for (int index = 0; index < this.mapLinks.Count; ++index)
        this.mapLinks[index].Update();
    }

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_LOGIC_MAP;

    public IGameMode GameTimeContext
    {
      get
      {
        return this.gameTimeContext == null ? ((VMGameRoot) IStaticDataContainer.StaticDataContainer.GameRoot).MainGameMode : this.gameTimeContext;
      }
    }

    public IGameString Title => (IGameString) this.titleText;

    public override void Clear()
    {
      base.Clear();
      if (this.mapNodes != null)
      {
        foreach (IContainer mapNode in this.mapNodes)
          mapNode.Clear();
        this.mapNodes.Clear();
        this.mapNodes = (List<IGraphObject>) null;
      }
      if (this.mapLinks != null)
      {
        foreach (IContainer mapLink in this.mapLinks)
          mapLink.Clear();
        this.mapLinks.Clear();
        this.mapLinks = (List<ILink>) null;
      }
      this.gameTimeContext = (IGameMode) null;
      this.titleText = (VMGameString) null;
    }
  }
}
