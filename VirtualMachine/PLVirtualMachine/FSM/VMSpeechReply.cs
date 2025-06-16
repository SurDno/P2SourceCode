// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.FSM.VMSpeechReply
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using PLVirtualMachine.GameLogic;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

#nullable disable
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
    [FieldData("OnlyOnce", DataFieldType.None)]
    private bool onlyOnce;
    [FieldData("OnlyOneReply", DataFieldType.None)]
    private bool onlyOneReply;
    [FieldData("Default", DataFieldType.None)]
    private bool isDefault;
    [FieldData("EnableCondition", DataFieldType.Reference)]
    private ICondition enableCondition;
    [FieldData("ActionLine", DataFieldType.Reference)]
    private IActionLine actionLine;
    [FieldData("OrderIndex", DataFieldType.None)]
    private int orderIndex;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "ActionLine":
              this.actionLine = EditorDataReadUtility.ReadReference<IActionLine>(xml, creator);
              continue;
            case "Default":
              this.isDefault = EditorDataReadUtility.ReadValue(xml, this.isDefault);
              continue;
            case "EnableCondition":
              this.enableCondition = EditorDataReadUtility.ReadReference<ICondition>(xml, creator);
              continue;
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
              continue;
            case "OnlyOnce":
              this.onlyOnce = EditorDataReadUtility.ReadValue(xml, this.onlyOnce);
              continue;
            case "OnlyOneReply":
              this.onlyOneReply = EditorDataReadUtility.ReadValue(xml, this.onlyOneReply);
              continue;
            case "OrderIndex":
              this.orderIndex = EditorDataReadUtility.ReadValue(xml, this.orderIndex);
              continue;
            case "Parent":
              this.parent = EditorDataReadUtility.ReadReference<IContainer>(xml, creator);
              continue;
            case "Text":
              this.text = EditorDataReadUtility.ReadReference<VMGameString>(xml, creator);
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

    public VMSpeechReply(ulong guid)
      : base(guid)
    {
    }

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_GRAPH_ELEMENT;

    public IGameString Text => (IGameString) this.text;

    public int Order => this.orderIndex;

    public bool OnlyOnce => this.onlyOnce;

    public bool OnlyOneReply => this.onlyOneReply;

    public bool IsDefault => this.isDefault;

    public ICondition EnableCondition => this.enableCondition;

    public IActionLine ActionLine => this.actionLine;

    public override void Clear()
    {
      base.Clear();
      if (this.enableCondition != null)
      {
        ((VMPartCondition) this.enableCondition).Clear();
        this.enableCondition = (ICondition) null;
      }
      if (this.actionLine != null)
      {
        ((VMActionLine) this.actionLine).Clear();
        this.actionLine = (IActionLine) null;
      }
      this.text = (VMGameString) null;
    }
  }
}
