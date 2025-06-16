// Decompiled with JetBrains decompiler
// Type: PLVirtualMachine.FSM.VMEntryPoint
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
  [TypeData(EDataType.TEntryPoint)]
  [DataFactory("EntryPoint")]
  public class VMEntryPoint : 
    VMBaseObject,
    IStub,
    IEditorDataReader,
    IEntryPoint,
    IObject,
    IEditorBaseTemplate
  {
    [FieldData("AssociatedEntryPoint", DataFieldType.Reference)]
    private VMEntryPoint assocEntryPoint;
    [FieldData("ActionLine", DataFieldType.Reference)]
    private IActionLine actionLine;

    public virtual void EditorDataRead(XmlReader xml, IDataCreator creator, string typeContext)
    {
      while (xml.Read())
      {
        if (xml.NodeType == XmlNodeType.Element)
        {
          switch (xml.Name)
          {
            case "AssociatedEntryPoint":
              this.assocEntryPoint = EditorDataReadUtility.ReadReference<VMEntryPoint>(xml, creator);
              continue;
            case "ActionLine":
              this.actionLine = EditorDataReadUtility.ReadReference<IActionLine>(xml, creator);
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

    public VMEntryPoint(ulong guid)
      : base(guid)
    {
    }

    public override EObjectCategory GetCategory() => EObjectCategory.OBJECT_CATEGORY_NONE;

    public IActionLine ActionLine
    {
      get
      {
        return this.assocEntryPoint != null && this.assocEntryPoint.ActionLine != null ? this.assocEntryPoint.ActionLine : this.actionLine;
      }
    }

    public override void Clear()
    {
      base.Clear();
      this.assocEntryPoint = (VMEntryPoint) null;
      if (this.actionLine == null)
        return;
      ((VMActionLine) this.actionLine).Clear();
      this.actionLine = (IActionLine) null;
    }
  }
}
