using System.Xml;
using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using PLVirtualMachine.GameLogic;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

namespace PLVirtualMachine.LogicMap
{
  [TypeData(EDataType.TLogicMapNodeContent)]
  [DataFactory("MindMapNodeContent")]
  public class VMLogicMapNodeContent : VMBaseObject, IStub, IEditorDataReader, IOrderedChild
  {
    [FieldData("ContentType")]
    private EMMNodeContentType contentType;
    [FieldData("Number")]
    private int contentNumber;
    [FieldData("ContentDescriptionText", DataFieldType.Reference)]
    private VMGameString contentDescriptionText;
    [FieldData("ContentPicture", DataFieldType.Reference)]
    private ISample contentPicture;
    [FieldData("ContentCondition", DataFieldType.Reference)]
    private ICondition contentCondition;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "ContentCondition":
              contentCondition = EditorDataReadUtility.ReadReference<ICondition>(xml, creator);
              continue;
            case "ContentDescriptionText":
              contentDescriptionText = EditorDataReadUtility.ReadReference<VMGameString>(xml, creator);
              continue;
            case "ContentPicture":
              contentPicture = EditorDataReadUtility.ReadReference<ISample>(xml, creator);
              continue;
            case "ContentType":
              contentType = EditorDataReadUtility.ReadEnum<EMMNodeContentType>(xml);
              continue;
            case "Name":
              name = EditorDataReadUtility.ReadValue(xml, name);
              continue;
            case "Number":
              contentNumber = EditorDataReadUtility.ReadValue(xml, contentNumber);
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

    public VMLogicMapNodeContent(ulong guid)
      : base(guid)
    {
    }

    public EMMNodeContentType ContentType => contentType;

    public int ContentNumber => contentNumber;

    public IGameString DescriptionText => contentDescriptionText;

    public ICondition ContentCondition => contentCondition;

    public ISample Picture => contentPicture;

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_GRAPH_ELEMENT;

    public int Order => contentNumber;

    public override void Clear()
    {
      contentDescriptionText = null;
      contentPicture = null;
      if (contentCondition == null)
        return;
      ((VMPartCondition) contentCondition).Clear();
      contentCondition = null;
    }
  }
}
