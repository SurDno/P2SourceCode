using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using System.Collections.Generic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.LogicMap
{
  [TypeData(EDataType.TLogicMapNode)]
  [DataFactory("MindMapNode")]
  public class VMLogicMapNode : 
    VMBaseObject,
    IStub,
    IEditorDataReader,
    IGraphObject,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable,
    ILocalContext
  {
    [FieldData("InputLinks", DataFieldType.Reference)]
    private List<ILink> inputLinks = new List<ILink>();
    [FieldData("OutputLinks", DataFieldType.Reference)]
    private List<ILink> outputLinks = new List<ILink>();
    [FieldData("LogicMapNodeType", DataFieldType.None)]
    private ELogicMapNodeType logicMapNodeType = ELogicMapNodeType.LM_NODE_TYPE_COMMON;
    [FieldData("NodeContent", DataFieldType.Reference)]
    private List<VMLogicMapNodeContent> nodeContentList = new List<VMLogicMapNodeContent>();
    [FieldData("GameScreenPosX", DataFieldType.None)]
    private float gameScreenPosX;
    [FieldData("GameScreenPosY", DataFieldType.None)]
    private float gameScreenPosY;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "GameScreenPosX":
              this.gameScreenPosX = EditorDataReadUtility.ReadValue(xml, this.gameScreenPosX);
              continue;
            case "GameScreenPosY":
              this.gameScreenPosY = EditorDataReadUtility.ReadValue(xml, this.gameScreenPosY);
              continue;
            case "InputLinks":
              this.inputLinks = EditorDataReadUtility.ReadReferenceList<ILink>(xml, creator, this.inputLinks);
              continue;
            case "LogicMapNodeType":
              this.logicMapNodeType = EditorDataReadUtility.ReadEnum<ELogicMapNodeType>(xml);
              continue;
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
              continue;
            case "NodeContent":
              this.nodeContentList = EditorDataReadUtility.ReadReferenceList<VMLogicMapNodeContent>(xml, creator, this.nodeContentList);
              continue;
            case "OutputLinks":
              this.outputLinks = EditorDataReadUtility.ReadReferenceList<ILink>(xml, creator, this.outputLinks);
              continue;
            case "Parent":
              this.parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
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

    public VMLogicMapNode(ulong guid)
      : base(guid)
    {
    }

    public ELogicMapNodeType NodeType => this.logicMapNodeType;

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_NONE;

    public float GameScreenPositionX => this.gameScreenPosX;

    public float GameScreenPositionY => this.gameScreenPosY;

    public List<VMLogicMapNodeContent> Contents => this.nodeContentList;

    public VMLogicMapNodeContent GetContentByGuid(ulong contentId)
    {
      foreach (VMLogicMapNodeContent nodeContent in this.nodeContentList)
      {
        if ((long) nodeContent.BaseGuid == (long) contentId)
          return nodeContent;
      }
      Logger.AddError(string.Format("Logic map node content with id {0} not found in mindmap node {1}", (object) contentId, (object) this.Name));
      return (VMLogicMapNodeContent) null;
    }

    public void OnAfterLoad() => this.UpdateLinks();

    public List<ILink> OutputLinks => this.outputLinks;

    public virtual List<IVariable> GetLocalContextVariables(
      EContextVariableCategory eContextVarCategory,
      IContextElement currentElement,
      int iCounter = 0)
    {
      return new List<IVariable>();
    }

    public virtual IVariable GetLocalContextVariable(
      string variableUniName,
      IContextElement currentElement = null)
    {
      return (IVariable) null;
    }

    public override void Clear()
    {
      if (this.nodeContentList != null)
      {
        foreach (VMBaseObject nodeContent in this.nodeContentList)
          nodeContent.Clear();
        this.nodeContentList.Clear();
        this.nodeContentList = (List<VMLogicMapNodeContent>) null;
      }
      if (this.inputLinks != null)
      {
        this.inputLinks.Clear();
        this.inputLinks = (List<ILink>) null;
      }
      if (this.outputLinks == null)
        return;
      this.outputLinks.Clear();
      this.outputLinks = (List<ILink>) null;
    }

    protected void UpdateLinks()
    {
      for (int index = 0; index < this.inputLinks.Count; ++index)
        this.inputLinks[index].Update();
      for (int index = 0; index < this.outputLinks.Count; ++index)
        this.outputLinks[index].Update();
    }
  }
}
