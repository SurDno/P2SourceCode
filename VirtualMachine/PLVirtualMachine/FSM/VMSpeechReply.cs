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

namespace PLVirtualMachine.FSM
{
  [TypeData(EDataType.TReply)]
  [DataFactory("Reply")]
  public class VMSpeechReply : 
    VMBaseObject,
    IStub,
    IEditorDataReader,
    ISpeechReply,
    IObject,
    IEditorBaseTemplate,
    IOrderedChild
  {
    [FieldData("Text", DataFieldType.Reference)]
    private VMGameString text;
    [FieldData("OnlyOnce")]
    private bool onlyOnce;
    [FieldData("OnlyOneReply")]
    private bool onlyOneReply;
    [FieldData("Default")]
    private bool isDefault;
    [FieldData("EnableCondition", DataFieldType.Reference)]
    private ICondition enableCondition;
    [FieldData("ActionLine", DataFieldType.Reference)]
    private IActionLine actionLine;
    [FieldData("OrderIndex")]
    private int orderIndex;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "ActionLine":
              actionLine = EditorDataReadUtility.ReadReference<IActionLine>(xml, creator);
              continue;
            case "Default":
              isDefault = EditorDataReadUtility.ReadValue(xml, isDefault);
              continue;
            case "EnableCondition":
              enableCondition = EditorDataReadUtility.ReadReference<ICondition>(xml, creator);
              continue;
            case "Name":
              name = EditorDataReadUtility.ReadValue(xml, name);
              continue;
            case "OnlyOnce":
              onlyOnce = EditorDataReadUtility.ReadValue(xml, onlyOnce);
              continue;
            case "OnlyOneReply":
              onlyOneReply = EditorDataReadUtility.ReadValue(xml, onlyOneReply);
              continue;
            case "OrderIndex":
              orderIndex = EditorDataReadUtility.ReadValue(xml, orderIndex);
              continue;
            case "Parent":
              parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "Text":
              text = EditorDataReadUtility.ReadReference<VMGameString>(xml, creator);
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

    public VMSpeechReply(ulong guid)
      : base(guid)
    {
    }

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_GRAPH_ELEMENT;

    public IGameString Text => text;

    public int Order => orderIndex;

    public bool OnlyOnce => onlyOnce;

    public bool OnlyOneReply => onlyOneReply;

    public bool IsDefault => isDefault;

    public ICondition EnableCondition => enableCondition;

    public IActionLine ActionLine => actionLine;

    public override void Clear()
    {
      base.Clear();
      if (enableCondition != null)
      {
        ((VMPartCondition) enableCondition).Clear();
        enableCondition = null;
      }
      if (actionLine != null)
      {
        ((VMActionLine) actionLine).Clear();
        actionLine = null;
      }
      text = null;
    }
  }
}
