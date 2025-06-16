// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.LogicMap.VMLogicLink
// Assembly: VirtualMachine, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4141C12C-9CB3-4BEE-B86E-276A0762C9CD
// Assembly location: C:\Program Files (x86)\Steam\steamapps\common\Pathologic\Pathologic_Data\Managed\VirtualMachine.dll

using Cofe.Loggers;
using Engine.Common.Commons;
using PLVirtualMachine.Base;
using PLVirtualMachine.Common;
using PLVirtualMachine.Common.Data;
using PLVirtualMachine.Data;
using System.Xml;
using VirtualMachine.Common;
using VirtualMachine.Common.Data;
using VirtualMachine.Data;

#nullable disable
namespace PLVirtualMachine.LogicMap
{
  [TypeData(EDataType.TLogicMapLink)]
  [DataFactory("MindMapLink")]
  public class VMLogicLink : 
    VMBaseObject,
    IStub,
    IEditorDataReader,
    ILink,
    IContainer,
    IObject,
    IEditorBaseTemplate,
    INamedElement,
    INamed,
    IStaticUpdateable
  {
    [FieldData("Source", DataFieldType.Reference)]
    private VMLogicMapNode source;
    [FieldData("Destination", DataFieldType.Reference)]
    private VMLogicMapNode destination;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "Source":
              this.source = EditorDataReadUtility.ReadReference<VMLogicMapNode>(xml, creator);
              continue;
            case "Destination":
              this.destination = EditorDataReadUtility.ReadReference<VMLogicMapNode>(xml, creator);
              continue;
            case "Name":
              this.name = EditorDataReadUtility.ReadValue(xml, this.name);
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

    public VMLogicLink(ulong guid)
      : base(guid)
    {
    }

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_GRAPH_ELEMENT;

    public IGraphObject Source => (IGraphObject) this.source;

    public IGraphObject Destination => (IGraphObject) this.destination;

    public override void Clear()
    {
      base.Clear();
      this.source = (VMLogicMapNode) null;
      this.destination = (VMLogicMapNode) null;
    }
  }
}
