using System.Collections.Generic;
using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
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
    [FieldData("LogicMapNodeType")]
    private ELogicMapNodeType logicMapNodeType = ELogicMapNodeType.LM_NODE_TYPE_COMMON;
    [FieldData("NodeContent", DataFieldType.Reference)]
    private List<VMLogicMapNodeContent> nodeContentList = new List<VMLogicMapNodeContent>();
    [FieldData("GameScreenPosX")]
    private float gameScreenPosX;
    [FieldData("GameScreenPosY")]
    private float gameScreenPosY;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "GameScreenPosX":
              gameScreenPosX = EditorDataReadUtility.ReadValue(xml, gameScreenPosX);
              continue;
            case "GameScreenPosY":
              gameScreenPosY = EditorDataReadUtility.ReadValue(xml, gameScreenPosY);
              continue;
            case "InputLinks":
              inputLinks = EditorDataReadUtility.ReadReferenceList(xml, creator, inputLinks);
              continue;
            case "LogicMapNodeType":
              logicMapNodeType = EditorDataReadUtility.ReadEnum<ELogicMapNodeType>(xml);
              continue;
            case "Name":
              name = EditorDataReadUtility.ReadValue(xml, name);
              continue;
            case "NodeContent":
              nodeContentList = EditorDataReadUtility.ReadReferenceList(xml, creator, nodeContentList);
              continue;
            case "OutputLinks":
              outputLinks = EditorDataReadUtility.ReadReferenceList(xml, creator, outputLinks);
              continue;
            case "Parent":
              parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
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

    public VMLogicMapNode(ulong guid)
      : base(guid)
    {
    }

    public ELogicMapNodeType NodeType => logicMapNodeType;

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_NONE;

    public float GameScreenPositionX => gameScreenPosX;

    public float GameScreenPositionY => gameScreenPosY;

    public List<VMLogicMapNodeContent> Contents => nodeContentList;

    public VMLogicMapNodeContent GetContentByGuid(ulong contentId)
    {
      foreach (VMLogicMapNodeContent nodeContent in nodeContentList)
      {
        if ((long) nodeContent.BaseGuid == (long) contentId)
          return nodeContent;
      }
      Logger.AddError(string.Format("Logic map node content with id {0} not found in mindmap node {1}", contentId, Name));
      return null;
    }

    public void OnAfterLoad() => UpdateLinks();

    public List<ILink> OutputLinks => outputLinks;

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
      return null;
    }

    public override void Clear()
    {
      if (nodeContentList != null)
      {
        foreach (VMBaseObject nodeContent in nodeContentList)
          nodeContent.Clear();
        nodeContentList.Clear();
        nodeContentList = null;
      }
      if (inputLinks != null)
      {
        inputLinks.Clear();
        inputLinks = null;
      }
      if (outputLinks == null)
        return;
      outputLinks.Clear();
      outputLinks = null;
    }

    protected void UpdateLinks()
    {
      for (int index = 0; index < inputLinks.Count; ++index)
        inputLinks[index].Update();
      for (int index = 0; index < outputLinks.Count; ++index)
        outputLinks[index].Update();
    }
  }
}
