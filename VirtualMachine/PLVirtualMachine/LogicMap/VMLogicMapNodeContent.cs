// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.LogicMap.VMLogicMapNodeContent
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
namespace PLVirtualMachine.LogicMap
{
  [TypeData(EDataType.TLogicMapNodeContent)]
  [DataFactory("MindMapNodeContent")]
  public class VMLogicMapNodeContent : VMBaseObject, IStub, IEditorDataReader, IOrderedChild
  {
    [FieldData("ContentType", DataFieldType.None)]
    private EMMNodeContentType contentType;
    [FieldData("Number", DataFieldType.None)]
    private int contentNumber;
    [FieldData("ContentDescriptionText", DataFieldType.Reference)]
    private VMGameString contentDescriptionText;
    [FieldData("ContentPicture", DataFieldType.Reference)]
    private ISample contentPicture;
    [FieldData("ContentCondition", DataFieldType.Reference)]
    private ICondition contentCondition;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "ContentCondition":
              this.contentCondition = EditorDataReadUtility.ReadReference<ICondition>(xml, creator);
              continue;
            case "ContentDescriptionText":
              this.contentDescriptionText = EditorDataReadUtility.ReadReference<VMGameString>(xml, creator);
              continue;
            case "ContentPicture":
              this.contentPicture = EditorDataReadUtility.ReadReference<ISample>(xml, creator);
              continue;
            case "ContentType":
              this.contentType = EditorDataReadUtility.ReadEnum<EMMNodeContentType>(xml);
              continue;
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
              continue;
            case "Number":
              this.contentNumber = EditorDataReadUtility.ReadValue(xml, this.contentNumber);
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

    public VMLogicMapNodeContent(ulong guid)
      : base(guid)
    {
    }

    public EMMNodeContentType ContentType => this.contentType;

    public int ContentNumber => this.contentNumber;

    public IGameString DescriptionText => (IGameString) this.contentDescriptionText;

    public ICondition ContentCondition => this.contentCondition;

    public ISample Picture => this.contentPicture;

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_GRAPH_ELEMENT;

    public int Order => this.contentNumber;

    public override void Clear()
    {
      this.contentDescriptionText = (VMGameString) null;
      this.contentPicture = (ISample) null;
      if (this.contentCondition == null)
        return;
      ((VMPartCondition) this.contentCondition).Clear();
      this.contentCondition = (ICondition) null;
    }
  }
}
