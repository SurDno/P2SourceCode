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
  [TypeData(EDataType.TEntryPoint)]
  [DataFactory("EntryPoint")]
  public class VMEntryPoint(ulong guid) :
    VMBaseObject(guid),
    IStub,
    IEditorDataReader,
    IEntryPoint,
    IObject,
    IEditorBaseTemplate {
    [FieldData("AssociatedEntryPoint", DataFieldType.Reference)]
    private VMEntryPoint assocEntryPoint;
    [FieldData("ActionLine", DataFieldType.Reference)]
    private IActionLine actionLine;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read()) {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "AssociatedEntryPoint":
              assocEntryPoint = EditorDataReadUtility.ReadReference<VMEntryPoint>(xml, creator);
              continue;
            case "ActionLine":
              actionLine = EditorDataReadUtility.ReadReference<IActionLine>(xml, creator);
              continue;
            case "Name":
              name = EditorDataReadUtility.ReadValue(xml, name);
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

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_NONE;

    public IActionLine ActionLine => assocEntryPoint != null && assocEntryPoint.ActionLine != null ? assocEntryPoint.ActionLine : actionLine;

    public override void Clear()
    {
      base.Clear();
      assocEntryPoint = null;
      if (actionLine == null)
        return;
      ((VMActionLine) actionLine).Clear();
      actionLine = null;
    }
  }
}
